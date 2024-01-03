using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot
{
    /// <summary>
    /// Here are classes for handling incoming messages
    /// Made for future implementations through reflections
    /// </summary>
    public class RandomQuestion : IMessageHandler
    {
        public Response Respond(string message, string chatID)
        {
            throw new NotImplementedException();
        }
    }

    public class AnswerQuestion : IMessageHandler
    {
        Response IMessageHandler.Respond(string message, string chatID)
        {
            throw new NotImplementedException();
        }
    }

    public class SetCategory : IMessageHandler
    {
        public Response Respond(string message, string chatID)
        {
            throw new NotImplementedException();
        }
    }

    public class RandomCategory : IMessageHandler
    {
        public Response Respond(string message, string chatID)
        {
            throw new NotImplementedException();
        }
    }

    public class GetAnswer : IMessageHandler
    {
        public Response Respond(string message, string chatID)
        {
            throw new NotImplementedException();
        }
    }
}
