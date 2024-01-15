using EgeBot.Bot.Models.db;
using EgeBot.Bot.Models.Enums;
using EgeBot.Bot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Net;


namespace EgeBot.Bot.Services
{
    public class DbContexter(BotDbContext db)
    {
        private readonly BotDbContext _db = db;

        public async Task<User> GetUser(long chat_id)
        {
            var user = await db.User.FindAsync(chat_id);
            if (user == null)
            {
                user = new User { Id = chat_id };
                await db.User.AddAsync(user);
                await db.SaveChangesAsync();
            }
            return user;
        }

        public async Task<List<TaskKim>> GetAllTaskKim()
        {
            var tasksKim = await db.TaskKim.ToListAsync();
            return tasksKim;
        }

        public async Task<List<Topic>> GetTopicsByTaskKimId(long taskKimId)
        {
            var taskKim = await db.TaskKim.FindAsync(taskKimId);
            if (taskKim != null)
                return taskKim.Topics.ToList();
            return new List<Topic>();
        }

        public async Task<HttpStatusCode> SetSettingComplexity(User user, Complexity complexity)
        {
            user.SettingComplexity = complexity;
            try
            {
                await db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> SetSettingTopic(long userId, int taskKimType, int topicPos = 1)
        {
            var user = await GetUser(userId);
            var topics = await db.Topic.Where(x => x.TaskKim.Type == taskKimType).ToListAsync();
            if (topics == null)
                return HttpStatusCode.BadRequest;
            var topic = topics[topicPos - 1];
            user.SettingTopic = topic;
            await db.SaveChangesAsync();
            return HttpStatusCode.OK;
        }

        public async Task<Subject> GetSubject(string title = "Информатика")
        {
            var subject = await db.Subject.Where(x => x.Title == title).FirstOrDefaultAsync();
            if (subject == null)
            {
                subject = new Subject { Title = title };
                await db.Subject.AddAsync(subject);
                await db.SaveChangesAsync();
            }
            return subject;
        }

        public async Task<HttpStatusCode> LoadTaskKim(List<string> data)
        {
            var subject = await GetSubject();
            foreach (var item in data)
            {
                var i = item.Split(' ');
                var taskKim = new TaskKim() { Type = int.Parse(i[0]), Title = string.Join(" ", i[1..]), Subject = subject };
                try
                {
                    await db.AddAsync(taskKim);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> LoadTopic(List<string> data)
        {
            foreach (var item in data)
            {
                var i = item.Split(' ');
                try
                {
                    var taskKim = await db.TaskKim.Where(x => x.Type == int.Parse(i[0])).FirstOrDefaultAsync();
                    var topic = new Topic() { TaskKim = taskKim, Title = string.Join(" ", i[1..]) };
                    await db.AddAsync(topic);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> LoadTheory(List<string> data)
        {
            for (var index = 0; index < data.Count; index += 2)
            {
                var i = data[index].Split(' ');
                try
                {
                    var topic = await db.Topic.Where(x => x.TaskKim.Type == int.Parse(i[0]) && x.Title == string.Join(" ", i.Skip(1))).FirstOrDefaultAsync();
                    var theory = new Theory() { Topic = topic, Text = data[index + 1] };
                    await db.AddAsync(theory);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> LoadTask(List<string> data)
        {
            for (var index = 0; index < data.Count; index += 4)
            {
                var i = data[index].Split(' ');
                try
                {
                    var topic = await db.Topic.Where(x => x.TaskKim.Type == int.Parse(i[0]) && x.Title == string.Join(" ", i.Skip(1))).FirstOrDefaultAsync();
                    var complexity = (Complexity)Enum.Parse(typeof(Complexity), data[index + 2]);

                    var inf = data[index + 3].Split(" ");
                    var path = inf.Length == 2 ? inf[1] : null;

                    var task = new Models.Task() { Topic = topic, Text = data[index + 1], CorrectAnswer = inf[0], Complexity = complexity, FilePath = path };
                    await db.AddAsync(task);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return HttpStatusCode.BadRequest;
                }
            }
            return HttpStatusCode.OK;
        }

        public async Task<HttpStatusCode> ChangeNickName(User user, string nickName)
        {
            user.NickName = nickName;
            try
            {
                await db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.BadRequest;
            }
        }

        public async Task<HttpStatusCode> CreateUserTask(User user, Models.Task task, string answer)
        {
            var userTask = new UserTask() { Task = task, User = user, UserAnswer = answer };
            try
            {
                await db.AddAsync(userTask);
                await db.SaveChangesAsync();
                return HttpStatusCode.OK;
            }
            catch
            {
                return HttpStatusCode.BadRequest;
            }
        }


        public async Task<Models.Task> GetTaskByUser(User user)
        {
            var completedTasks = new List<Models.Task>();
            if (user.UserTasks != null)
                completedTasks = user.UserTasks.Select(x => x.Task).ToList();
            var task = await db.Task.Where(x => x.Topic == user.SettingTopic && x.Complexity == user.SettingComplexity && !completedTasks.Any(y => y == x)).FirstOrDefaultAsync();
            return task;
        }

        public async Task<Theory> GetTheoryByUser(User user)
        {
            try
            {
                var theory = await db.Theory.Where(x => x.Topic == user.SettingTopic).FirstOrDefaultAsync();
                return theory;
            }
            catch
            {
                return null;
            }
        }
    }
}
