using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot
{
    public static partial class MessageHandler
    {
        private static Dictionary<string, Func<string, long, Response>> CommandDictionary =
            new Dictionary<string, Func<string, long, Response>>
            {
                {"/cat", CatReturner},
                {"/question", Questioner},
                {"/setCategory", SetCategory},
                {"/answer", Answer },
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

        /// <summary>
        /// Sets category of tasks. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Response SetCategory(string text, long chatId)
        {
            return new Response("What is the meaning of life?", chatId);
        }

        /// <summary>
        /// Handles input for answer. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        public static Response Answer(string text, long chatId)
        {
            return new Response("What is the meaning of life?", chatId);
        }
    }
}
