using Telegram.Bot;
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
using EgeBot.Bot.Services.Handlers;
using EgeBot.Bot.Models.db;
using Microsoft.EntityFrameworkCore;
using EgeBot.Bot.Services.Responses;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot
{
    public class Bot
    {
        private string token;
        private IS3Storage Storage { get; }
        private MessageHandler MessageHandler { get; }
        private UpdateType[] validTypes = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery };

        public Bot(string token, IS3Storage storage, BotDbContext connectionDbString)
        {
            this.token = token;
            Storage = storage;
            MessageHandler = new MessageHandler(connectionDbString);
        }

        public async void Run()
        {
            var botClient = new TelegramBotClient(this.token);

            using CancellationTokenSource cts = new();
            // StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
            ReceiverOptions receiverOptions = new()
            {
                //AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
                AllowedUpdates = validTypes
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

        private bool isValidUpdateType(Update update)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.CallbackQuery != null)
                return true;
            if (update.Message != null)
                return true;

            return false;
        }

        async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            //загрузка файла. Нужно переместить туда, где ей место. А ещё она файлы на диске создает, но я не знаю как это поправить
            /*var file = botClient.GetFileAsync(update.Message.Document.FileId);
            var fileName = update.Message.Document.FileName;
            var taskNumber = 15;
            var subject = "informatics";
            using (var saveImageStream = System.IO.File.Open(fileName, FileMode.Create))
            {
                await botClient.DownloadFileAsync(file.Result.FilePath, saveImageStream);
                await Storage.PostFile(fileName, saveImageStream, taskNumber, subject);
                Console.WriteLine("file uploaded");
            }*/

            //Пример отправки изображений из облака ботом
            var filename = "informatics/15/74bbdf28-51b3-4606-bd62-e3faa88a9c1e.png";//это брать из бд
            using (var responce = await Storage.GetFile(filename))
            {
                if (filename.Split('.').Last() == "png")
                {
                    var msg1 = await botClient.SendPhotoAsync(update.Message.Chat.Id, new InputFileStream(responce.ResponseStream, filename.Split('/').Last()));
                }
                else
                {
                    var msg2 = await botClient.SendDocumentAsync(update.Message.Chat.Id, new InputFileStream(responce.ResponseStream, filename.Split('/').Last()));
                }                    
            }
            

            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (!isValidUpdateType(update))
                return;

            Response response = null;

            if (update.Type == UpdateType.CallbackQuery)              
                response = await MessageHandler.HandleCallbackUpdate(update);
            
            if (update.Type == UpdateType.Message)
                response = await MessageHandler.HandleTextUpdate(update); 
            
            if (response == null)
                return; //No message meant to be sent

            if(response.Answer == "do_not_send")
                return;

            // Echo received message text
            Message sentMessage = await botClient.SendTextMessageAsync(
                chatId: response.ChatId,
                text: response.Answer,
                replyMarkup: response.Markup,
                cancellationToken: cancellationToken);
        }
    }
}
