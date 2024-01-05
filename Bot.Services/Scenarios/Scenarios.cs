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
        [MessageHandler("мяу")]
        public Task<Response> CatReturner(string text, long chatId)
        {
            return Task.FromResult(new Response("MEOOOOOOOOOOOOOOOOOOOOOOOOOOOOW", chatId,
                new Payload(ReplyKeyboardWrapper(new[] { "/cat", "/question" }))));
        }

        private ReplyKeyboardMarkup ReplyKeyboardWrapper(string[] answers)
        {
            var keyboardButtons = answers.Select(x => new KeyboardButton(x));
            return new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }

        /// <summary>
        /// Returns a random question. Intended to be used with database
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns>Response with question</returns>
        [MessageHandler("Вопрос")]
        public Task<Response> Questioner(string text, long chatId)
        {
            return Task.FromResult(new Response("What is the meaning of life?", chatId));
        }

        /// <summary>
        /// Sets category of tasks. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [MessageHandler("Выбрать_Категорию")]
        public Task<Response> SetCategory(string text, long chatId)
        {
            return Task.FromResult(new Response("What is the meaning of life?", chatId));
        }

        /// <summary>
        /// Handles input for answer. Supposed to work with persistent data
        /// </summary>
        /// <param name="text"></param>
        /// <param name="chatId"></param>
        /// <returns></returns>
        [MessageHandler("Ответ")]
        public Task<Response> Answer(string text, long chatId)
        {
            return Task.FromResult(new Response(string.Format("Вы ответили: {0}", text), chatId));
        }

        [MessageHandler("Статистика")]
        public Task<Response> GetStats(string text, long chatId)
        {
            return Task.FromResult(new Response("Вот статистика по заданиям: (Not implemented yet)", chatId));
        }
    }
}
