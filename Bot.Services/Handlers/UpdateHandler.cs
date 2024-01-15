using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using EgeBot.Bot.Infrastructure.Attributes;
using EgeBot.Bot.Infrastructure;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot.Services.Handlers
{
    public class UpdateHandler : IUpdateHandler
    {
        private readonly IScenarioHandler _scenarioHelper;
        private readonly Dictionary<string, MethodInfo> _scenarioCommands;
        public UpdateType[] validTypes { get; init; }

        public long GetChatId(Update update) => update.Type == UpdateType.Message ? update.Message.Chat.Id : update.CallbackQuery.Message.Chat.Id;

        public UpdateHandler(IScenarioHandler scenarioHelper)
        {
            _scenarioHelper = scenarioHelper;
            _scenarioCommands = InitializeScenarioCommands(FindMethodsWithAttribute(typeof(ScenarioHandlerAttribute)));
            validTypes = new UpdateType[] { UpdateType.Message, UpdateType.CallbackQuery };
        }

        private IEnumerable<MethodInfo> FindMethodsWithAttribute(Type attribute)
        {
            var methods = _scenarioHelper.GetType()
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Where(m => m.GetCustomAttributes(attribute, false).Length > 0)
                .ToArray();
            return methods;
        }

        private Dictionary<string, MethodInfo> InitializeScenarioCommands(IEnumerable<MethodInfo> methods)
        {
            var scenarioCommands = new Dictionary<string, MethodInfo>();
            foreach (var method in methods)
            {
                var myCommand = (ScenarioHandlerAttribute)method.GetCustomAttribute(typeof(ScenarioHandlerAttribute)); ;
                if (myCommand != null)
                    scenarioCommands.Add(myCommand.MyMessageToHandle, method);
            }
            return scenarioCommands;
        }

        public async Task<Data> Parse(Update update)
        {
            var chatId = GetChatId(update);
            var text = update.CallbackQuery == null ? update.Message.Text : update.CallbackQuery.Data;
            var document = update.Message?.Document;
            if (document != null)
                text = update.Message.Caption;
            try
            {
                var cmdArgs = text.Trim().Split(" ", 2);
                var cmd = cmdArgs[0];
                var argument = cmdArgs.Length > 1 ? cmdArgs[1] : "";

                if (cmd != "Загрузить")
                    document = null;

                return new Data(cmd, argument, document);
            }
            catch
            {
                return Data.Empty;
            }
        }

        public async Task<Response> Generate(Data data, long chatId)
        {
            var fullparams = new object[] { data.Argument, chatId };

            try
            {
                var responsePayload = await (Task<Response>)_scenarioCommands[data.Command].Invoke(_scenarioHelper, fullparams);
                return responsePayload;
            }
            catch
            {
                return Response.UnknownCommand;
            }

        }
    }
}
