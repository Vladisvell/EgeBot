using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses.Generators
{
    public class CallbackQueryFromStringsGenerator : IMarkupGenerator
    {
        IReplyMarkup markup;

        public IReplyMarkup Generate()
        {
            return markup;
        }

        public CallbackQueryFromStringsGenerator(List<string> strings, int maxRowElementsCount = 3)
        {
            if (strings.Count % 2 != 0)
                throw new ArgumentException("Количество строк во входном списке strings должно быть чётным!");
            List<List<InlineKeyboardButton>> inlineKeyboardButtons = new();
            var counter = 0;
            var currlist = new List<InlineKeyboardButton>();
            for (int i = 0; i < strings.Count; i += 2)
            {
                if (counter < maxRowElementsCount)
                {
                    currlist.Add(InlineKeyboardButton.WithCallbackData(text: strings[i], callbackData: strings[i + 1]));
                    counter++;
                }
                if (counter >= maxRowElementsCount)
                {
                    inlineKeyboardButtons.Add(currlist);
                    currlist = new List<InlineKeyboardButton>();
                    counter = 0;
                }
            }
            if (currlist.Count > 0)
            {
                inlineKeyboardButtons.Add(currlist);
            }
            InlineKeyboardMarkup inlineKeyboard = new(inlineKeyboardButtons);
            markup = inlineKeyboard;
        }
    }
}