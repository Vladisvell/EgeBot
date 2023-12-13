using System.IO;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EgeBot.Bot;

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

            //builder.Services.AddDbContext<BotDbContext>(options => options.UseNpgsql(connectionString));
            var bot = new Bot.Bot(tokenString);
            bot.Run();
            
            using IHost host = builder.Build();
            host.Run();
        }
    }
}