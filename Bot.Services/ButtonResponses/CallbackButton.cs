using EgeBot.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services.ButtonResponses
{
    /// <summary>
    /// Класс для коллбеков к сообщению бота, атрибут задавать в формате "текст кнопки", "данные коллбека", "Текст кнопки 2, данные коллбека 2"
    /// </summary>
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
            var tuples = ConvertToTuples(values);
            var keyboardButtons = tuples.Select(x => InlineKeyboardButton.WithCallbackData(text: x.Item1, callbackData: x.Item2));

            InlineKeyboardMarkup inlineKeyboard = new(new[]
            {
                keyboardButtons,
            });

            markup = inlineKeyboard;
        }

        private static IEnumerable<Tuple<string,string>> ConvertToTuples(string[] values)
        {
            var tuples = new List<Tuple<string,string>>();
            if(values.Length % 2 != 0)
            {
                throw new ArgumentException("Число аргументов для Callback Button должно быть чётным.");
            }

            for(int i = 0; i < values.Length; i += 2)
            {
                tuples.Add(Tuple.Create(values[i], values[i+1]));
            }

            return tuples;
        }
    }
}
