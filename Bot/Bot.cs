﻿using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using Telegram.Bot.Types.ReplyMarkups;
using System.Collections.Generic;
using System.Reflection;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;
using EgeBot.Bot.Services.Scenarios;

namespace EgeBot.Bot
{
    public class Bot
    {
        private string token;
        private s3Storage Storage { get; }
        private MessageHandler MessageHandler { get; }

        public Bot(string token, s3Storage storage)
        {
            this.token = token;
            Storage = storage;
            MessageHandler = new MessageHandler();
        }

        public async void Run()
        {
            var botClient = new TelegramBotClient(this.token);

            using CancellationTokenSource cts = new();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
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

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //загрузка файла. Нужно переместить туда, где ей место. А ещё она файлы на диске создает, но я не знаю как это поправить
            /*var updateHandler = update;
            var file = botClient.GetFileAsync(updateHandler.Message.Document.FileId);
            var fileName = update.Message.Document.FileName;
            var taskNumber = 15;
            var subject = "informatics";
            using (var saveImageStream = System.IO.File.Open(fileName, FileMode.Create))
            {
                await botClient.DownloadFileAsync(file.Result.FilePath, saveImageStream);
                await Storage.PostFile(fileName, saveImageStream, taskNumber, subject);
                Console.WriteLine("file uploaded");
            }*/


            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return;

            var chatId = message.Chat.Id;

            Console.WriteLine($"Received message in chat {chatId}.");

            Response response = await MessageHandler.HandleUpdate(update);

            var replyKeyboardMarkup = response.Payload.load == null ? null : (ReplyKeyboardMarkup)response.Payload.load;

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: chatId,
                text: response.Answer,
                replyMarkup: replyKeyboardMarkup == null ? new ReplyKeyboardRemove() : replyKeyboardMarkup,
                cancellationToken: cancellationToken);
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
    }
}
