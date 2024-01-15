using Telegram.Bot.Types;

namespace EgeBot.Bot.Infrastructure
{
    public class Data
    {
        public string Command { get; private set; }
        public string Argument { get; set; }
        public Document? Document { get; private set; }

        public static Data Empty => new Data("", "");

        public Data(string command, string argument)
        {
            Command = command;
            Argument = argument;
        }

        public Data(string command, string argument, Document document)
        {
            Command = command;
            Argument = argument;
            Document = document;
        }
    }
}
