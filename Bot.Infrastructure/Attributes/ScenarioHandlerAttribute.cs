using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioHandlerAttribute : Attribute
    {
        public string MyMessageToHandle { get; private set; }

        public ScenarioHandlerAttribute(string name)
        {
            if (name == null || name.Length == 0)
                throw new ArgumentNullException(string.Format("{0} received empty message to handle.", GetType()));
            MyMessageToHandle = name;
        }

    }
}
