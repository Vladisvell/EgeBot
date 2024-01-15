using EgeBot.Bot.Infrastructure;
using EgeBot.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IUpdateHandler
    {
        UpdateType[] validTypes { get; }
        long GetChatId(Update update);
        Task<Data> Parse(Update update);
        Task<Response> Generate(Data data, long chatId);
    }
}
