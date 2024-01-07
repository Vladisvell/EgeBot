using EgeBot.Bot.Services.Attributes;
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

        private static Dictionary<MethodInfo, string?[]> buttonResponses = new Dictionary<MethodInfo, string?[]> { };

        private ScenarioHandler ScenarioHandler { get; }


        public MessageHandler()
        {
            ScenarioHandler = new ScenarioHandler();
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
                    buttonResponses.Add(method, null);
                else
                    buttonResponses.Add(method, myCommand.Buttons);
            }
            return;
        }

        private ReplyKeyboardMarkup ReplyKeyboardWrapper(string[] answers)
        {
            if(answers == null)
                return null;

            var keyboardButtons = answers.Select(x => new KeyboardButton(x));
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        public async Task<Response> HandleUpdate(Update update)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return await ErrorHandler(update.Message.Chat.Id, ErrorCodes.InvalidOperation);
            // Only process text messages
            if (message.Text is not { } messageText)
                return await ErrorHandler(update.Message.Chat.Id, ErrorCodes.InvalidOperation);

            var chatID = update.Message.Chat.Id;

            if (update == null)
                return await ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if (update.Message == null)
                return await ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if (update.Message.Text == null)
                return await ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if (update.Message.Text.Length == 0)
                return await ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            string textRef = update.Message.Text;
            bool isCommand = textRef[0] == '/' ? true : false;

            //Parse message accordingly


            //It's like "/cmd Ah yes this is command" -> ["/cmd", "Ah yes this is command"]
            var cmdArgs = textRef.Split(' ', 1);
            if (!commands.ContainsKey(cmdArgs[0]))
                return await ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            Response? responsePayload = null;
           

            if (cmdArgs.Length == 1)
            {
                var parameters = new object[] { "", chatID };
                responsePayload = await (Task<Response>)commands[cmdArgs[0]].Invoke(ScenarioHandler, parameters);
                responsePayload.Payload = new Payload(ReplyKeyboardWrapper(buttonResponses[commands[cmdArgs[0]]]));
                return responsePayload;
            }

            var fullparams = new object[] { cmdArgs[1], chatID };
            responsePayload = await (Task<Response>)commands[cmdArgs[0]].Invoke(cmdArgs[1], fullparams);
            responsePayload.Payload = new Payload(ReplyKeyboardWrapper(buttonResponses[commands[cmdArgs[0]]]));
            return responsePayload;
        }

        private static async Task<Response> ErrorHandler(long ChatId, ErrorCodes code)
        {
            return new Response(code.ToString(), ChatId);
        }
    }

    public enum ErrorCodes
    {
        NotFound,
        InvalidOperation,
        NotImplemented
    }

    public struct Payload
    {
        public object load { get; set; }

        public Payload(object load)
        {
            this.load = load;
        }
    }

    public class Response
    {
        public string Answer { get; set; }
        public long ChatId { get; set; }
        public Payload Payload { get; set; }

        public Response(string answer, long chatId, Payload payload)
        {
            Answer = answer;
            ChatId = ChatId;
            Payload = payload;
        }

        public Response(string answer, long chatId)
        {
            Answer = answer;
            ChatId = ChatId;
        }

        public Response(long chatId)
        {
            Answer = "Err505";
            ChatId = chatId;
        }
    }
}