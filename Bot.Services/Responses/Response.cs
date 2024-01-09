using Telegram.Bot.Types.ReplyMarkups;


namespace EgeBot.Bot.Services.Responses
{
    public class Response
    {
        public string Answer { get; set; }
        public long ChatId { get; set; }
        public IReplyMarkup Markup { get; set; }

        public Response(string answer, long chatId, IReplyMarkup markup)
        {
            Answer = answer;
            ChatId = chatId;
            Markup = markup;
        }

        public Response(string answer, long chatId)
        {
            Answer = answer;
            ChatId = chatId;
            Markup = new ReplyKeyboardRemove();
        }

        public Response(long chatId)
        {
            Answer = "do_not_send";
            ChatId = chatId;
            Markup = new ReplyKeyboardRemove();
        }
    }
}