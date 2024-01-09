using EgeBot.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses
{
    public class CallbackButton : IButtonResponse
    {
        public IReplyMarkup Markup => markup;

        private IReplyMarkup markup;

        public CallbackButton(params string[] values)
        {
            if (values == null)
            {
                markup = new ReplyKeyboardRemove();
                return;
            }
            var keyboardButtons = values.Select(x => InlineKeyboardButton.WithCallbackData(text: x, callbackData: x));

            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                keyboardButtons,
            });

            markup = inlineKeyboard;
        }
    }
}
