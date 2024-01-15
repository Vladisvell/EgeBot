using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Castle.Core.Configuration;
using Microsoft.Extensions.Configuration;
using static System.Net.Mime.MediaTypeNames;
using EgeBot.Bot.Infrastructure;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot
{
    internal class Bot : IBot
    {
        private readonly string _token;
        private readonly Services.Interfaces.IUpdateHandler _updateHelper;
        private readonly IS3Storage _storage;

        public Bot(IConfigurationRoot config, Services.Interfaces.IUpdateHandler updateHelper, IS3Storage storage)
        {
            _token = config.GetConnectionString("BotToken");
            _updateHelper = updateHelper;
            _storage = storage;
        }

        public async void Run()
        {
            var botClient = new TelegramBotClient(_token);

            using CancellationTokenSource cts = new();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                //AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
                AllowedUpdates = _updateHelper.validTypes
            };

            botClient.StartReceiving(
                updateHandler: HandleUpdateAsync,
                pollingErrorHandler: HandlePollingErrorAsync,
                receiverOptions: receiverOptions,
                cancellationToken: cts.Token
            );

            var me = await botClient.GetMeAsync();

            Console.WriteLine($"Start listening for @{me.Username}");
            Console.ReadLine();
            // Send cancellation request to stop bot
            cts.Cancel();
        }

        Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMessage = exception switch
            {
                ApiRequestException apiRequestException
                    => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
                _ => exception.ToString()
            };

            Console.WriteLine(ErrorMessage);
            return Task.CompletedTask;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            var chatId = _updateHelper.GetChatId(update);
            var data = await _updateHelper.Parse(update);

            if (data.Document != null)
            {
                var file = botClient.GetFileAsync(data.Document.FileId);
                var fileName = data.Document.FileName;
                var subject = "informatics";
                using (var saveImageStream = System.IO.File.Open(fileName, FileMode.Create))
                {
                    await botClient.DownloadFileAsync(file.Result.FilePath, saveImageStream);
                    var filePath = await _storage.PostFile(fileName, saveImageStream, subject);
                    if (filePath != null)
                        data.Argument = $"{data.Argument} {filePath}";
                    else
                        data = Data.Empty;
                    Console.WriteLine("file uploaded");
                }
            }

            var response = await _updateHelper.Generate(data, chatId);

            if (response.PathToDownload != null)
            {
                using (var resp = await _storage.GetFile(response.PathToDownload))
                {
                    if (response.PathToDownload.Split('.').Last() == "png")
                    {
                        var msg1 = await botClient.SendPhotoAsync(chatId, new InputFileStream(resp.ResponseStream, response.PathToDownload.Split('/').Last()));
                    }
                    else
                    {
                        var msg2 = await botClient.SendDocumentAsync(chatId, new InputFileStream(resp.ResponseStream, response.PathToDownload.Split('/').Last()));
                    }
                }
            }

            //// Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: response.Text,
                replyMarkup: response.Markup,
                cancellationToken: cancellationToken);
        }
    }
}
