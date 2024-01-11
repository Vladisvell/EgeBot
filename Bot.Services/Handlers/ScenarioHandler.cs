using EgeBot.Bot.Services.ButtonResponses;
using EgeBot.Bot.Models;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Models.Enums;
using EgeBot.Bot.Services.Handlers.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using EgeBot.Bot.Services.ButtonResponses.Generators;
using Microsoft.VisualBasic;
using System.Reflection.Emit;
using Telegram.Bot.Types;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using EgeBot.Bot.Services.DBContext;
using EgeBot.Bot.Services.Responses;
using EgeBot.Bot.Services.Responses.Enums;

namespace EgeBot.Bot.Services.Handlers
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
        private readonly DBContext.DBContext dbService;
        public ScenarioHandler(BotDbContext connectionDbString)
        {
            dbService = new DBContext.DBContext(connectionDbString);
        }

        [MessageHandler("/start")]
        public async Task<Response> Start(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            return new Response($"Приветствую тебя, {user.NickName}! Я твой персональный помощник для подготовки к ЕГЭ по информатике. Для начала советую выбрать номер задания ;)", chatId);
        }

        [MessageHandler("/help")]
        public async Task<Response> Help(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            var buttons = new List<string>() { "Выбрать номер", "Номер", "Выбрать тему", "Тема", "Получить теорию", "Теория","Тренеровка", "Тренеровка", "Выбрать никнейм", "Никнейм" };
            if (user.IsAdmin)
                buttons.AddRange(new List<string>() { "Загрузить", "Загрузить"});
            CallbackQueryFromStringsGenerator generator = new(buttons, 1);
            return new Response("Вот что я умею ᕦ(ò_óˇ)ᕤ", chatId, generator.Generate());
        }

        [MessageHandler("Загрузить")]
        public async Task<Response> Load(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            if (!user.IsAdmin)
                return new Response("Нет доступа", chatId);
            if (text.Length == 0)
            {
                var commands = dbService.loadKeys.Keys.Select(x => $"Загрузить {x}").ToList();
                return new Response($"Команды:\n{String.Join("\n", commands)}", chatId);
            }
            await dbService.Load(text);
            return new Response($"Успех (ﾉ◕ヮ◕)ﾉ*: ･ﾟ✧", chatId);
        }

        [MessageHandler("Номер")]
        public async Task<Response> ChooseTaskKim(string text, long chatId)
        {
            var tasksKim = await dbService.GetAllTaskKim();
            if (tasksKim == null)
                return new Response("Пока недоступно :(", chatId);
            if (text.Length == 0)
            {
                var responseText = tasksKim.Select(x => $"{x.Type}.{x.Title}").ToList();
                CallbackQueryListGenerator generator = new(tasksKim.Select(x => x.Type.ToString()).ToList(), "Номер", 3);
                return new Response($"Выбери номер задания:\n{String.Join("\n",responseText)}", chatId, generator.Generate());;
            }
            await dbService.SetSettingTopic(chatId, int.Parse(text));
            return ChooseTopic("", chatId).Result;
        }

        [MessageHandler("Тема")]
        public async Task<Response> ChooseTopic(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            if (text.Length == 0)
            {
                if (user.SettingTopic == null)
                    return new Response($"{user.NickName}, не торопись. Сначала нужно выбрать номер задания", chatId);
                var taskKimId = user.SettingTopic.TaskKim.Id;
                var topics = await dbService.GetTopicsByTaskKimId(taskKimId);
                if (topics == null)
                    return new Response("Пока недоступно :(", chatId);
                var topicsWithIndex = topics.Select((x, i) => $"{i + 1}.{x.Title}").ToList();
                var indexes = topics.Select((x, i) => (i + 1).ToString()).ToList();
                CallbackQueryListGenerator generator = new(indexes, "Тема", 4);
                return new Response($"Выбери тему для {topics[0].TaskKim.Type} задания:\n{String.Join("\n", topicsWithIndex)}", chatId, generator.Generate());
            }
            var status = await dbService.SetSettingTopic(chatId, user.SettingTopic.TaskKim.Type,int.Parse(text));
            if (status == ResponseCode.OK)
                return new Response("Ура! Теперь можно и задачки порешать :)", chatId);
            return new Response($"Что-то пошло не так", chatId);
        }

        [MessageHandler("Никнейм")]
        public async Task<Response> NickName(string text, long chatId)
        {
            if (text.Length == 0)
            {
                var user = await dbService.GetUser(chatId);
                return new Response($"Твой никнейм: {user.NickName}\nЧтобы изменить, введи слово Никнейм и название\nПример: Никнейм булочка", chatId);
            }
            await dbService.ChangeNickName(chatId, text);
            return new Response($"Теперь я буду называть тебя {text} :з", chatId);
        }

        [MessageHandler("Сложность")]
        public async Task<Response> SetSettingComplexity(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            if (text.Length == 0)
            {
                var complexities = Enum.GetNames(typeof(Complexity)).ToList();
                CallbackQueryListGenerator generator = new(complexities, "Сложность", 3);
                return new Response($"Текущая сложность: {user.SettingComplexity.ToString()}\nТы можешь ее изменить", chatId, generator.Generate());
            }
            var complexity = (Complexity)Enum.Parse(typeof(Complexity), text);
            var status = await dbService.SetSettingComplexity(chatId, complexity);
            if (status == ResponseCode.OK)
                return new Response($"Установлена сложность: {complexity.ToString()}", chatId);
            return new Response($"Что-то пошло не так", chatId);
        }

        [MessageHandler("Тренеровка")]
        public async Task<Response> SetSettingTaskKim(string text, long chatId)
        {
            //var a = Enum.GetNames(typeof(Complexity));
            return new Response("ddd", chatId);
        }               

        [MessageHandler("Теория")]
        public async Task<Response> GeTheory(string text, long chatId)
        {
            var user = await dbService.GetUser(chatId);
            var theory = await dbService.GetTheoryByUser(user);
            if (theory == null)
                return new Response("Пока недоступно :(", chatId);
            return new Response($"Теория по теме {user.SettingTopic.Title} задания номер {user.SettingTopic.TaskKim.Type}\n{theory.Text}", chatId);
        }
    }
}
