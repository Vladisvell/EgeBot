using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses.Generators
{
    public interface IMarkupGenerator
    {
        IReplyMarkup Generate();
    }
}
