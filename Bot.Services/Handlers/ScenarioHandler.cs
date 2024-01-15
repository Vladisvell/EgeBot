using EgeBot.Bot.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgeBot.Bot.Infrastructure.Attributes;
using System.Net;
using EgeBot.Bot.Models;
using EgeBot.Bot.Infrastructure;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot.Services.Handlers
{
    public class ScenarioHandler : IScenarioHandler
    {
        private readonly DbContext _context;
        public Dictionary<string, Func<List<string>, Task<HttpStatusCode>>> AdminCommands { get; init; }
        public Dictionary<User, Models.Task> Memory { get; init; }

        public ScenarioHandler(DbContext context)
        {
            _context = context;
            Memory = new Dictionary<User, Models.Task>();
            AdminCommands = new Dictionary<string, Func<List<string>, Task<HttpStatusCode>>>()
            {
                ["задание"] = _context.LoadTaskKim,
                ["тему"] = _context.LoadTopic,
                ["теорию"] = _context.LoadTheory,
                ["задачу"] = _context.LoadTask
            };
        }

        [ScenarioHandler("/start")]
        public async Task<Response> Start(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            return new Response($"Приветствую тебя, {user.NickName}! Я твой персональный помощник для подготовки к ЕГЭ по информатике.\nВведи /help, чтобы узнать мои возможности");
        }

        [ScenarioHandler("/help")]
        public async Task<Response> Help(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            var buttons = new List<string>() { "Настройки", "Теория", "Тренировка" };
            var answer = new StringBuilder("Настройки - выбор номера, темы, сложности и никнейма\nТеория - получение теории в сотвецтвии с настройками\nТренировка - получение задачи в сотвецтвии с настройками");
            if (user.IsAdmin)
            {
                buttons.Add("Загрузить");
                answer.Append("\nЗагрузить - загрузить номера, темы, теорию и задачи");
            }
            return new Response(answer.ToString(), KeyboardGenerator.Get(buttons));
        }

        [ScenarioHandler("Настройки")]
        public async Task<Response> Settings(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            var buttons = new List<string>() { "Выбрать номер", "Номер", "Выбрать тему", "Тема", "Выбрать сложность", "Сложность", "Выбрать никнейм", "Никнейм" };
            return new Response("Вот, что я умею ᕦ(ò_óˇ)ᕤ", KeyboardGenerator.GetInline(buttons, 1));
        }

        [ScenarioHandler("Загрузить")]
        public async Task<Response> Load(string text, long chatId)
        {
            var user = await _context.GetUser(chatId);
            if (!user.IsAdmin)
                return Response.Forbidden;
            if (text.Length == 0)
            {
                var commands = AdminCommands.Keys.Select(x => $"Загрузить {x}").ToList();
                return new Response($"Команды:\n{string.Join("\n", commands)}");
            }
            var data = text.Split("\n").ToList();

            var status = await AdminCommands[data[0]](data[1..]);
            if (status == HttpStatusCode.OK)
                return new Response($"Успех (ﾉ◕ヮ◕)ﾉ*: ･ﾟ✧");
            return Response.BadRequest;
        }

        [ScenarioHandler("Номер")]
        public async Task<Response> ChooseTaskKim(string text, long userId)
        {
            var tasksKim = await _context.GetAllTaskKim();
            if (tasksKim.Count == 0)
                return Response.Forbidden;
            if (text.Length == 0)
            {
                var responseText = tasksKim.Select(x => $"{x.Type}.{x.Title}").ToList();
                var buttons = tasksKim.Select(x => x.Type.ToString()).ToList();
                return new Response($"Выбери номер задания:\n{string.Join("\n", responseText)}", KeyboardGenerator.GetInline(buttons, 3, "Номер"));
            }
            var status = await _context.SetSettingTopic(userId, int.Parse(text));
            if (status == HttpStatusCode.OK)
                return ChooseTopic("", userId).Result;
            return Response.BadRequest;
        }

        [ScenarioHandler("Тема")]
        public async Task<Response> ChooseTopic(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            if (user.SettingTopic == null)
                return Response.EmptyTaskKim(user.NickName);
            var taskKim = user.SettingTopic.TaskKim;
            if (text.Length == 0)
            {
                var topics = taskKim.Topics.ToList();
                if (topics.Count == 0)
                    return Response.Forbidden;
                var topicsWithIndex = topics.Select((x, i) => $"{i + 1}.{x.Title}").ToList();
                var indexes = topics.Select((x, i) => (i + 1).ToString()).ToList();
                return new Response($"Выбери тему для {taskKim.Type} задания:\n{string.Join("\n", topicsWithIndex)}", KeyboardGenerator.GetInline(indexes, 5, "Тема"));
            }
            var status = await _context.SetSettingTopic(user.Id, taskKim.Type, int.Parse(text));
            if (status == HttpStatusCode.OK)
                return new Response("Ура! Теперь можно и задачки порешать :)");
            return Response.BadRequest;
        }

        [ScenarioHandler("Никнейм")]
        public async Task<Response> NickName(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            if (text.Length == 0)
                return new Response($"Твой никнейм: {user.NickName}\nЧтобы изменить, введи: Никнейм ТЕКСТ");
            var status = await _context.ChangeNickName(user, text);
            if (status == HttpStatusCode.OK)
                return new Response($"Теперь я буду называть тебя {text} :з");
            return Response.BadRequest;
        }

        [ScenarioHandler("Сложность")]
        public async Task<Response> SetSettingComplexity(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            if (text.Length == 0)
            {
                var complexities = Enum.GetNames(typeof(Complexity)).ToList();
                return new Response($"Текущая сложность: {user.SettingComplexity.ToString()}\nТы можешь ее изменить", KeyboardGenerator.GetInline(complexities, 3, "Сложность"));
            }
            var complexity = (Complexity)Enum.Parse(typeof(Complexity), text);
            var status = await _context.SetSettingComplexity(user, complexity);
            if (status == HttpStatusCode.OK)
                return new Response($"Установлена сложность: {complexity.ToString()}");
            return Response.BadRequest;
        }

        [ScenarioHandler("Тренировка")]
        public async Task<Response> Practice(string text, long chatId)
        {
            var user = await _context.GetUser(chatId);
            if (user.SettingTopic == null)
                return Response.EmptyTaskKim(user.NickName);
            var task = await _context.GetTaskByUser(user);
            if (task == null)
                return new Response("Похоже ты решил все задачи по этой теме :D");
            Memory[user] = task;
            var buttons = new List<string>() { "Получить теорию", "Теория" };
            return new Response($"Задача по теме '{user.SettingTopic.Title}' задания номер {user.SettingTopic.TaskKim.Type}\n{task.Text}\n Для ответа введи: Ответ ТЕКСТ", KeyboardGenerator.GetInline(buttons, 1), task.FilePath);
        }

        [ScenarioHandler("Ответ")]
        public async Task<Response> Answer(string text, long chatId)
        {
            var user = await _context.GetUser(chatId);
            if (text.Length == 0)
                return Response.BadRequest;
            try
            {
                var task = Memory[user];
                var answer = task.CorrectAnswer;
                var response = await _context.CreateUserTask(user, task, text);
                if (text == answer)
                {
                    return new Response($"Молодец, {user.NickName}! Это правильный ответ (*≧ω≦*)");
                }
                Memory.Remove(user);
                return new Response($"Правильный ответ: {answer}\nНе волнуйся, {user.NickName}, в следующий раз у тебя обязательно получится!");
            }
            catch
            {
                return Response.BadRequest;
            }
        }

        [ScenarioHandler("Теория")]
        public async Task<Response> GetTheory(string text, long userId)
        {
            var user = await _context.GetUser(userId);
            if (user.SettingTopic == null)
                return Response.EmptyTaskKim(user.NickName);
            var theory = await _context.GetTheoryByUser(user);
            if (theory == null)
                return Response.Forbidden;
            return new Response($"Теория по теме '{user.SettingTopic.Title}' задания номер {user.SettingTopic.TaskKim.Type}\n{theory.Text}");
        }
    }
}
