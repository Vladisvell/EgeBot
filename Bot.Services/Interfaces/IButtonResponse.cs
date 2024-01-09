using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IButtonResponse
    {
        IReplyMarkup Markup {get;}
    }
}
