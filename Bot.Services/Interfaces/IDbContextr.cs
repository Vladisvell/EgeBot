using EgeBot.Bot.Models;
using EgeBot.Bot.Models.Enums;
using EgeBot.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IDbContextr
    {
        Task<User> GetUser(long chat_id);
        Task<List<TaskKim>> GetAllTaskKim();
        Task<HttpStatusCode> SetSettingTopic(long userId, int taskKimType, int topicPos = 1);
        Task<HttpStatusCode> ChangeNickName(User user, string nickName);
        Task<HttpStatusCode> SetSettingComplexity(User user, Complexity complexity);
        Task<Theory> GetTheoryByUser(User user);
        //Task<List<ContentResult<TaskKim>>> LoadTaskKim(List<string> data);
        Task<HttpStatusCode> LoadTopic(List<string> data);
        Task<HttpStatusCode> LoadTheory(List<string> data);
        Task<Models.Task> GetTaskByUser(User user);
    }
}
