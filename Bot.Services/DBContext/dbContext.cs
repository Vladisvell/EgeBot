using EgeBot.Bot.Models;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Models.Enums;
using EgeBot.Bot.Services.Responses.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.DBContext
{
    public class DBContext
    {
        private readonly BotDbContext db;
        public readonly Dictionary<string, Func<List<string>, Task<ResponseCode>>> loadKeys;

        public DBContext(BotDbContext db)
        {
            this.db = db;
            loadKeys = new Dictionary<string, Func<List<string>, Task<ResponseCode>>>()
            {
                ["задание"] = LoadTaskKim,
                ["тему"] = LoadTopic,
                ["теорию"] = LoadTheory,
                ["задачу"] = LoadTask
            };
        }

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

        public async Task<List<Topic>?> GetTopicsByTaskKimId(long taskKimId)
        {
            var taskKim = await db.TaskKim.FindAsync(taskKimId);
            if (taskKim != null)
                return taskKim.Topics.ToList();
            return null;
        }

        public async Task<ResponseCode> SetSettingComplexity(long userId, Complexity complexity)
        {
            var user = await db.User.FindAsync(userId);
            user.SettingComplexity = complexity;
            await db.SaveChangesAsync();
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> SetSettingTopic(long userId, int taskKimType, int topicPos = 1)
        {
            var user = await db.User.FindAsync(userId);
            var topics = await db.Topic.Where(x => x.TaskKim.Type == taskKimType).ToListAsync();
            if (topics == null)
                return ResponseCode.NotFound;
            var topic = topics[topicPos - 1];
            user.SettingTopic = topic;
            await db.SaveChangesAsync();
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> Load(string text)
        {
            var data = text.Split("\n").ToList();
            var status = await loadKeys[data[0]](data[1..]);
            return status;
        }

        public async Task<ResponseCode> LoadTaskKim(List<string> data)
        {
            foreach (var item in data)
            {
                var i = item.Split(' ');
                var taskKim = new TaskKim() { Type = int.Parse(i[0]), Title = string.Join(" ", i[1..]) };
                try
                {
                    await db.AddAsync(taskKim); 
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return ResponseCode.InvalidOperation;
                }
            }
            return ResponseCode.OK;
        }
        

        public async Task<ResponseCode> LoadTopic(List<string> data)
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
                    return ResponseCode.InvalidOperation;
                }
            }
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> LoadTheory(List<string> data)
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
                    return ResponseCode.InvalidOperation;
                }
            }
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> LoadTask(List<string> data)
        {
            for (var index = 0; index < data.Count; index += 4)
            {
                var i = data[index].Split(' ');
                try
                {
                    var topic = await db.Topic.Where(x => x.TaskKim.Type == int.Parse(i[0]) && x.Title == string.Join(" ", i.Skip(1))).FirstOrDefaultAsync();
                    var complexity = (Complexity)Enum.Parse(typeof(Complexity), data[index+2]);
                    var task = new Models.Task() { Topic = topic, Text = data[index + 1], CorrectAnswer = data[index+3], Complexity = complexity };
                    await db.AddAsync(task);
                    await db.SaveChangesAsync();
                }
                catch
                {
                    return ResponseCode.InvalidOperation;
                }
            }
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> ChangeNickName(long userId, string nickName)
        {
            var user = await db.User.FindAsync(userId);
            user.NickName = nickName;
            await db.SaveChangesAsync();
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> AddTaskToUser(User user,long taskId)
        {
            var task = await db.Task.Where(x => x.Id == taskId).FirstOrDefaultAsync();
            var userTask = new UserTask() { Task = task, User = user};
            await db.AddAsync(userTask);
            await db.SaveChangesAsync();
            return ResponseCode.OK;
        }

        public async Task<ResponseCode> AddAnswerToUserTask(UserTask userTask, string answer)
        {
            userTask.UserAnswer = answer;
            await db.SaveChangesAsync();
            return ResponseCode.OK;
        }

        //public async Task<string> GetAnswerByTaskId(long taskId)
        //{
        //    var task = await db.Task.Where(x=> x.Id == taskId).FirstOrDefaultAsync();
        //    return task.CorrectAnswer;
        //}

        public async Task<Models.Task> GetTaskByUser(User user)
        {
            var completedTasks = new List<Models.Task>();
            if (user.UserTasks != null)
                completedTasks = user.UserTasks.Select(x => x.Task).ToList();
            var task = await db.Task.Where(x => x.Topic == user.SettingTopic && x.Complexity==user.SettingComplexity && !completedTasks.Any(y=> y == x)).FirstOrDefaultAsync();
            return task;
        }

        public async Task<Theory> GetTheoryByUser(User user)
        {
            var theory = await db.Theory.Where(x => x.Topic == user.SettingTopic).FirstOrDefaultAsync();
            return theory;
        }
    }
}
