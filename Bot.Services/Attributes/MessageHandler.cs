using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services
{
    public class MessageHandlerAttribute : Attribute
    {
        public string MyMessageToHandle { get; private set; }

        public MessageHandlerAttribute(string name)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentNullException(string.Format("{0} received empty message to handle.", GetType()));
            MyMessageToHandle = name;
        }
    }
}
