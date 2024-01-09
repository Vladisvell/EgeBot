using EgeBot.Bot.Services.Attributes;
using EgeBot.Bot.Services.ButtonResponses;
using EgeBot.Bot.Models;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Services.Attributes;
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
        /// 
        private readonly DBService dbService;
        public ScenarioHandler(BotDbContext connectionDbString)
        {
            dbService = new DBService(connectionDbString);
        }

        private string GetAllTaskKim()
        {
            var allTaskKim = dbService.AllTaskKim.Select(x => $"{x.Type}. {x.Title}").ToList();
            return String.Join("\n", allTaskKim);
        }

        [MessageHandler("/start")]
        public async Task<Response> Start(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            return new Response($"Приветствую тебя, {user.NickName}! Я твой персональный помощник для подготовки к ЕГЭ по информатике. Чтобы начать, выбери номер задания ;)\n", chatId);
        }

        [MessageHandler("/help")]
        [ButtonResponse(typeof(ReplyKeyboard),"Выбрать номер", "Выбрать тему", "Выбрать сложность", "Теория", "Тренеровка", "Выбрать имя", "Статистика")]
        public async Task<Response> Help(string text, long chatId)
        {
            return new Response("ᕦ(ò_óˇ)ᕤ", chatId);
        }

        [MessageHandler("Выбрать сложность")]
        [ButtonResponse(typeof(CallbackButton),"easy", "medium", "hard")]
        public async Task<Response> SetSettingComplexity(string text, long chatId)
        {
            //var a = Enum.GetNames(typeof(Complexity));
            return new Response("", chatId);
        }

        [MessageHandler("Выбрать номер")]
        [ButtonResponse(typeof(CallbackButton),"easy", "medium", "hard")]
        public async Task<Response> SetSettingTaskKim(string text, long chatId)
        {
            //var a = Enum.GetNames(typeof(Complexity));
            return new Response("", chatId);
        }



        [MessageHandler("мяу")]
        [ButtonResponse(typeof(CallbackButton), "мяу", "Вопрос")]
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

        [MessageHandler("Вопрос")]
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

        [MessageHandler("Выбрать_Категорию")]
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

        [MessageHandler("Ответ")]
        public async Task<Response> Answer(string text, long chatId)
        {
            return new Response(string.Format("Вы ответили: {0}", text), chatId);
        }

        [MessageHandler("Статистика")]
        public async Task<Response> GetStats(string text, long chatId)
        {
            return new Response("Вот статистика по заданиям: (Not implemented yet)", chatId);
        }
    }
}
