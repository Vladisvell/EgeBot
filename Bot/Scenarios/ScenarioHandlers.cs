using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EgeBot.Bot
{
    /// <summary>
    /// MessageHandler is a class responsible for handling incoming messages.
    /// </summary>
    public static partial class MessageHandler
    {
        public static Response HandleUpdate(Update update)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return ErrorHandler(update.Message.Chat.Id, ErrorCodes.InvalidOperation);
            // Only process text messages
            if (message.Text is not { } messageText)
                return ErrorHandler(update.Message.Chat.Id, ErrorCodes.InvalidOperation);

            var chatID = update.Message.Chat.Id;

            if(update == null)
                return ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if(update.Message == null)
                return ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if(update.Message.Text == null)
                return ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            if (update.Message.Text.Length == 0)
                return ErrorHandler(chatID, ErrorCodes.InvalidOperation);

            string textRef = update.Message.Text;
            bool isCommand = textRef[0] == '/' ? true : false;

            //Parse message accordingly

            if (isCommand)
            {   
                //It's like "/cmd Ah yes this is command" -> ["/cmd", "Ah yes this is command"]
                var cmdArgs = textRef.Split(' ', 1);
                if (!CommandDictionary.ContainsKey(cmdArgs[0]))
                    return ErrorHandler(chatID, ErrorCodes.InvalidOperation);
                
                if(cmdArgs.Length == 1)
                    return CommandDictionary[cmdArgs[0]].Invoke("", chatID);
                
                return CommandDictionary[cmdArgs[0]].Invoke(cmdArgs[1], chatID);
            }

            //if this is not a command, then it may be an answer to a question
            //Todo: need to handle persistance data about client & question
            return new Response("Answer handler message", chatID);

        }

        private static Response ErrorHandler(long ChatId, ErrorCodes code)
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
        string Info { get; set; }
        long ChatId { get; set; }

        public Payload(string info, long chatId)
        {
            Info = info;
            ChatId = chatId;
        }
    }

    public class Response
    {
        public string Answer { get; set; }
        public long ChatId { get; set; }

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