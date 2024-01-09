using EgeBot.Bot.Models;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Models.Enums;

//using EgeBot.Bot.Schemas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services
{
    public class DBService
    {
        private readonly BotDbContext db;
        public List<TaskKim> AllTaskKim { get; private set; }

        public DBService(BotDbContext db) 
        { 
            this.db = db;
            //GetAllTaskKim();
        }

        public async Task<User> GetUser(long chat_id)
        {
            using (db)
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
        }

        private async System.Threading.Tasks.Task GetAllTaskKim()
        {
            using (db)
            {
                AllTaskKim = await db.TaskKim.ToListAsync();
            }
        }

        public async System.Threading.Tasks.Task SetSettingComplexity(int userId ,Complexity complexity)
        {
            using (db)
            {
                var user = await db.User.FindAsync(userId);
                user.SettingComplexity = complexity;
                await db.SaveChangesAsync();
            }
        }

        public async System.Threading.Tasks.Task SetSettingTopic(int userId, Topic topic)
        {
            using (db)
            {
                var user = await db.User.FindAsync(userId);
                user.SettingTopic = topic;
                await db.SaveChangesAsync();
            }
        }

        //public async Task<string> GetUserNickName()
        //{
        //    using (db)
        //    {

        //    }
        //}

        public async Task<List<Topic>> GetAllTopics()
        {
            using (db)
            {
                var topics = await db.Topic.ToListAsync();
                foreach (var topic in topics) Console.WriteLine($"{topic.Id} {topic.Title} {topic.TaskKim.Id} {topic.TaskKim.Title}");
                return topics;
            }
        }

        //public async Task<string[]> GetAllTaskKim()
        //{
        //    using (db)
        //    {
        //        var tasksKim = await db.TaskKim.ToListAsync();
        //        foreach (var taskKim in tasksKim) Console.WriteLine($"{taskKim.Id} {taskKim.Title}");
        //        return tasksKim.Select(x => x.Type.ToString() + x.Title).ToArray();
        //    }
        //}

        public async Task<string> GetTheoryByTopic(int topicId)
        {
            using (db)
            {
                var theory = await db.Theory.Where(x => x.Topic.Id == topicId).FirstOrDefaultAsync();
                Console.WriteLine(theory.Text);
                return theory.Text;
            }
        }

        public async Task<List<Theory>> GetTheoryByTaskKim(int taskKimId)
        {
            using (db)
            {
                var theories = await db.Theory.Where(x => x.Topic.TaskKim.Id == taskKimId).ToListAsync();
                foreach (var theory in theories) Console.WriteLine($"{theory.Topic.Title} {theory.Text}");
                return theories;
            }
        }

        public async Task<List<Models.Task>> GetAllTasksByTopic(int topicId)
        {
            using (db)
            {
                var tasks = await db.Task.Where(x => x.Topic.Id == topicId).ToListAsync();
                return tasks;
            }
        }

        public async Task<List<Models.Task>> GetAllTasksByTaskKim(int taskKimId)
        {
            using (db)
            {
                var tasks = await db.Task.Where(x => x.Topic.TaskKim.Id == taskKimId).ToListAsync();
                return tasks;
            }
        }
    }
}
