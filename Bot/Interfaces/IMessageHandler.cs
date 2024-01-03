using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot
{
    /// <summary>
    /// Interface for incoming messages, basically a scenario.
    /// Made for future implementations through reflection
    /// </summary>
    public interface IMessageHandler
    {
        Response Respond(string message, string chatID);
    }
}
