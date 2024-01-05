using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Attributes
{
    public class ButtonResponseAttribute : Attribute
    {
        public string?[] Buttons;

        public ButtonResponseAttribute(params string[] values)
        {
            if (values == null || values.Length == 0)
                Buttons = null;

            Buttons = values;
        }

    }
}