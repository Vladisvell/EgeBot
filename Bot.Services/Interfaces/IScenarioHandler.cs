using EgeBot.Bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IScenarioHandler
    {
        Dictionary<string, Func<List<string>, Task<HttpStatusCode>>> AdminCommands { get; init; }
    }
}
