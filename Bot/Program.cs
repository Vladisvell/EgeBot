using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Infrastructure;
using EgeBot.Bot.Services.Interfaces;
using EgeBot.Bot.Services.Handlers;

namespace EgeBot.Bot
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings.json");
            var config = configurationBuilder.Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<IS3Storage, s3Storage>();
            builder.Services.AddDbContext<BotDbContext>(options => options.UseLazyLoadingProxies().UseNpgsql(connectionString));
            builder.Services.AddSingleton<DbContext, DbContext>();
            builder.Services.AddSingleton<IScenarioHandler, ScenarioHandler>();
            builder.Services.AddSingleton<IUpdateHandler, UpdateHandler>();
            builder.Services.AddSingleton<IBot, Bot>();

            using IHost host = builder.Build();

            var bot = host.Services.GetService<IBot>();
            bot.Run();

            host.Run();
        }
    }
}