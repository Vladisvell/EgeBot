using System.Collections.Generic;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Infrastructure
{
    public static class KeyboardGenerator
    {
        public static IReplyMarkup Get(List<string> keys)
        {
            var rows = new List<KeyboardButton[]>();
            var cols = new List<KeyboardButton>();
            foreach (var t in keys)
            {
                cols.Add(new KeyboardButton(t));
                rows.Add(cols.ToArray());
                cols = new List<KeyboardButton>();
            }
            var rkm = new ReplyKeyboardMarkup(rows.ToArray());
            return rkm;
        }

        public static IReplyMarkup GetInline(List<string> strings, int maxRowElementsCount, string command = "")
        {
            List<List<InlineKeyboardButton>> inlineKeyboardButtons = new();
            var counter = 0;
            var currlist = new List<InlineKeyboardButton>();
            for (int i = 0; i < strings.Count; i++)
            {
                var data = $"{command} {strings[i]}";
                if (command.Length == 0)
                {
                    if (i % 2 != 0)
                        continue;
                    data = $"{strings[i + 1]}";
                }
                if (counter < maxRowElementsCount)
                {
                    currlist.Add(InlineKeyboardButton.WithCallbackData(text: strings[i], callbackData: data));
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
            return inlineKeyboard;
        }
    }
}
