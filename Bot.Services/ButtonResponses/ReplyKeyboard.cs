using EgeBot.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses
{
    public class ReplyKeyboard : IButtonResponse
    {
        
        IReplyMarkup IButtonResponse.Markup => markup;

        private IReplyMarkup markup { get; set; }

        public ReplyKeyboard(params string[] values)
        {
            if (values == null)
                markup = new ReplyKeyboardRemove();

            var keyboardButtons = values.Select(x => new KeyboardButton(x));
            markup = new ReplyKeyboardMarkup(keyboardButtons)
            {
                ResizeKeyboard = true
            };
        }
    }
}
