using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using Telegram.Bot;
using Telegram.Bot.Types;


namespace EgeBot.Bot
{
    /**
     * Classes here responsible for handling incoming messages and delegating to subsystems.
     * TODO: remove chatId dependency.
     */

    

    public static class MessageHandler
    {
        public static Dictionary<string, Func<string, long, Response>> dict =
            new Dictionary<string, Func<string, long, Response>>
            {
                {"/cat", CatReturner},
                {"/question", Questioner}
            };

        /// <summary>
        /// Am I supposed to say that this thing basically meows?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns>Response with meow.</returns>
        public static Response CatReturner(string text, long chatId)
        {
            return new Response("MEOOOOOOOOOOOOOOOOOOOOOOOOOOOOW", chatId);
        }

        /// <summary>
        /// Returns a random question. Intended to be used with database
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns>Response with question</returns>
        public static Response Questioner(string text, long chatId)
        {
            return new Response("What is the meaning of life?", chatId);
        }

        public static Response HandleUpdate(Update update)
        {
            // Only process Message updates: https://core.telegram.org/bots/api#message
            if (update.Message is not { } message)
                return ErrorHandler(update.Message.Chat.Id, ErrorCodes.NotImplemented);
            // Only process text messages
            if (message.Text is not { } messageText)
                return ErrorHandler(update.Message.Chat.Id, ErrorCodes.NotImplemented);

            if (update.Message.Text.Length > 0)
                //This is where magic begins
                if (dict.ContainsKey(update.Message.Text))
                    return dict[update.Message.Text].Invoke(update.Message.Text, update.Message.Chat.Id);
                else
                    return new Response(update.Message.Text, update.Message.Chat.Id);
            else
                return ErrorHandler(update.Message.Chat.Id, ErrorCodes.InvalidOperation);
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