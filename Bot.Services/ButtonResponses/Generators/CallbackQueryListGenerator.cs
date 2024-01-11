using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses.Generators
{
    public class CallbackQueryListGenerator : IMarkupGenerator
    {
        IReplyMarkup markup;

        public IReplyMarkup Generate()
        {
            return markup;
        }

        public CallbackQueryListGenerator(List<string> strings)
        {
            var keyboardButtons = strings.Select(x => new[] { InlineKeyboardButton.WithCallbackData(text: x, callbackData: $"{x} {x}") });
            InlineKeyboardMarkup inlineKeyboard = new(keyboardButtons);
            markup = inlineKeyboard;
        }

        public CallbackQueryListGenerator(List<string> strings, string command)
        {
            var keyboardButtons = strings.Select(x => new[] { InlineKeyboardButton.WithCallbackData(text: x, callbackData: $"{command} {x}") });
            InlineKeyboardMarkup inlineKeyboard = new(keyboardButtons);
            markup = inlineKeyboard;
        }

        public CallbackQueryListGenerator(List<string> strings, string command, int maxRowElementsCount = 3)
        {
            List<List<InlineKeyboardButton>> inlineKeyboardButtons = new();
            var counter = 0;
            var currlist = new List<InlineKeyboardButton>();
            for (int i = 0; i < strings.Count; i++)
            {
                if (counter < maxRowElementsCount)
                {
                    currlist.Add(InlineKeyboardButton.WithCallbackData(text: strings[i], callbackData: $"{command} {strings[i]}"));
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

    public enum GeneratorOptions
    {
        None,
        VerticalAlign
    }
}
