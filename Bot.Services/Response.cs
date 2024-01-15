using Telegram.Bot.Types.ReplyMarkups;

namespace EgeBot.Bot.Services
{
    public class Response
    {
        public readonly string Text;
        public readonly IReplyMarkup Markup;
        public readonly string? PathToDownload;

        public static Response Forbidden => new Response("Пока недоступно :(");
        public static Response BadRequest => new Response("Упс! Ты что-то перепутал");
        public static Response UnknownCommand => new Response("Я тебя не понимаю :(");
        public static Response EmptyTaskKim(string nickName) => new Response($"{nickName}, не торопись. Сначала нужно выбрать номер задания");

        public Response(string answer, IReplyMarkup markup, string? filePath = null)
        {
            Text = answer;
            Markup = markup;
            PathToDownload = filePath;
        }

        public Response(string answer)
        {
            Text = answer;
        }
    }
}
