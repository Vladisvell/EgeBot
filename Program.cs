using System.IO;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EgeBot.Bot;
using EgeBot.Bot.Models.db;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot
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
            var tokenString = config.GetConnectionString("BotToken");
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton<IS3Storage, s3Storage>();
            builder.Services.AddDbContext<BotDbContext>(options => options.UseNpgsql(connectionString));
            using IHost host = builder.Build();

            var bot = new Bot.Bot(tokenString, host.Services.GetService<IS3Storage>(), host.Services.GetService<BotDbContext>());
            bot.Run();

            host.Run();
        }
    }
}