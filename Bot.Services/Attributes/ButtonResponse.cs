using EgeBot.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonResponseAttribute : Attribute
    {
        public IButtonResponse ButtonResponse;

        public ButtonResponseAttribute(Type buttonType, params string[] values)
        {
            var replyMarkupType = buttonType.GetInterface("IButtonResponse", true);
            if (replyMarkupType == null)            
                throw new ArgumentException(
                    String.Format("Reply button type was not IButtonResponse! Type given {0}", buttonType.Name)
                    );            

            ButtonResponse = Activator.CreateInstance(buttonType, values) as IButtonResponse;
        }

    }
}
