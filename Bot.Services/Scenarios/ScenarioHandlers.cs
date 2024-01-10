using Amazon.Runtime.Internal.Transform;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Services.Attributes;
using EgeBot.Bot.Services.Interfaces;
using EgeBot.Bot.Services.Responses;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace EgeBot.Bot.Services.Scenarios
{
    /// <summary>
    /// MessageHandler is a class responsible for handling incoming messages.
    /// </summary>
    public partial class MessageHandler
    {
        private static Dictionary<string, MethodInfo> commands = new Dictionary<string, MethodInfo> { };

        private static Dictionary<MethodInfo, IReplyMarkup> buttonResponses = new Dictionary<MethodInfo, IReplyMarkup> { };

        private static Dictionary<long, string> lastChatIDMessage = new Dictionary<long, string>();

        private static Dictionary<MethodInfo, MethodInfo> callbackMap = new Dictionary<MethodInfo, MethodInfo> { };

        private ScenarioHandler ScenarioHandler { get; }


        public MessageHandler(BotDbContext connectionDbString)
        {
            ScenarioHandler = new ScenarioHandler(connectionDbString);
            InitializeCommandWheel(FindAllMethodsWithAttribute(typeof(MessageHandlerAttribute)));
            InitializeButtonsResponse();
        }

        private IEnumerable<MethodInfo> FindAllMethodsWithAttribute(Type attribute)
        {
            var methods = ScenarioHandler.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(attribute, false).Length > 0)
                .ToArray();
            return methods;
        }

        private void InitializeCommandWheel(IEnumerable<MethodInfo> methods)
        {
            foreach (var method in methods)
            {
                var myCommand = (MessageHandlerAttribute)method.GetCustomAttribute(typeof(MessageHandlerAttribute));
                if (myCommand == null)
                    continue;
                commands.Add(myCommand.MyMessageToHandle, method);
            }
            return;
        }

        private void InitializeButtonsResponse()
        {
            var methods = ScenarioHandler.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods)
            {
                var myCommand = (ButtonResponseAttribute)method.GetCustomAttribute(typeof(ButtonResponseAttribute));
                if (myCommand == null)
                    buttonResponses.Add(method, new ReplyKeyboardRemove());
                else
                    buttonResponses.Add(method, myCommand.ButtonResponse.Markup);
            }
            return;
        }

        private static ResponseCodes IsValidUpdate(Update update)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return ResponseCodes.InvalidOperation;
            // Only process text messages
            if (message.Text is not { } messageText)
                return ResponseCodes.InvalidOperation;

            if (update == null)
                return ResponseCodes.InvalidOperation;

            if (update.Message == null)
                return ResponseCodes.InvalidOperation;

            if (update.Message.Text == null)
                return ResponseCodes.InvalidOperation;

            if (update.Message.Text.Length == 0)
                return ResponseCodes.InvalidOperation;

            return ResponseCodes.OK;
        }

        public async Task<Response> HandleCallbackUpdate(Update update)
        {
            var chatID = update.CallbackQuery.Message.Chat.Id; //Chat ID where callback was originated from
            var replyMessageText = update.CallbackQuery.Message?.Text;  //Original response text
            var originalData = update.CallbackQuery.Data; //Data that was passed to callback

            var cmdArgs = originalData.Trim().Split(" ", 2);
            if (!commands.ContainsKey(cmdArgs[0]))
                return ErrorHandler(chatID, ResponseCodes.NotImplemented);

            var argument = cmdArgs.Length > 1 ? cmdArgs[1] : "";

            var cmd = cmdArgs[0];

            Response? responsePayload = null;         

            var fullparams = new object[] { argument, chatID };
            responsePayload = await (Task<Response>)commands[cmd].Invoke(ScenarioHandler, fullparams);
            
            responsePayload.Markup = responsePayload.Markup == null ? buttonResponses[commands[cmd]] : responsePayload.Markup;

            return responsePayload;
        }

        public async Task<Response> HandleTextUpdate(Update update)
        {
            var chatID = update.Message.Chat.Id;

            if(IsValidUpdate(update) != ResponseCodes.OK)
                return ErrorHandler(chatID, ResponseCodes.InvalidOperation);

            string messageText = update.Message.Text;

            //Parse message accordingly

            //It's like "/cmd Ah yes this is command" -> ["/cmd", "Ah yes this is command"]
            var cmdArgs = messageText.Trim().Split(" ", 2);
            if (!commands.ContainsKey(cmdArgs[0]))
                return ErrorHandler(chatID, ResponseCodes.NotImplemented);

            var cmd = cmdArgs[0];
            var argument = cmdArgs.Length > 1 ? cmdArgs[1] : "";

            Response? responsePayload = null;         

            var fullparams = new object[] { argument, chatID };
            responsePayload = await (Task<Response>)commands[cmd].Invoke(ScenarioHandler, fullparams);
            
            responsePayload.Markup = responsePayload.Markup == null ? buttonResponses[commands[cmd]] : responsePayload.Markup;
            
            lastChatIDMessage[chatID] = responsePayload.Answer;
            
            return responsePayload;
        }

        private static Response ErrorHandler(long ChatId, ResponseCodes code)
        {
            return new Response(code.ToString(), ChatId);
        }
    }
}