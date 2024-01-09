using EgeBot.Bot.Services.Attributes;
using EgeBot.Bot.Services.ButtonResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.Scenarios
{
    public class ScenarioHandler
    {
        /// <summary>
        /// Am I supposed to say that this thing basically meows?
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns>Response with meow.</returns>
        [CommandHandler("мяу")]
        [ButtonResponse(typeof(CallbackButton),"мяу", "Вопрос")]
        public async Task<Response> CatReturner(string text, long chatId)
        {
            return new Response("MEOOOOOOOOOOOOOOOOOOOOOOOOOOOOW", chatId);
        }

        /// <summary>
        /// Returns a random question. Intended to be used with database
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns>Response with question</returns>
        [CommandHandler("Вопрос")]
        [ButtonResponse(typeof(ReplyKeyboard), "мяу")]
        public async Task<Response> Questioner(string text, long chatId)
        {
            return new Response("What is the meaning of life?", chatId);
        }

        /// <summary>
        /// Sets category of tasks. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [CommandHandler("Выбрать_Категорию")]
        public async Task<Response> SetCategory(string text, long chatId)
        {
            return new Response("What is the meaning of life?", chatId);
        }

        /// <summary>
        /// Handles input for answer. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [CommandHandler("Ответ")]
        public async Task<Response> Answer(string text, long chatId)
        {
            return new Response(string.Format("Вы ответили: {0}", text), chatId);
        }

        [CommandHandler("Статистика")]
        public async Task<Response> GetStats(string text, long chatId)
        {
            return new Response("Вот статистика по заданиям: (Not implemented yet)", chatId);
        }
    }
}
