using System.IO;
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EgeBot.Bot;
using EgeBot.Bot.Models.db;

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

            var keyID = config.GetConnectionString("KeyID");
            var SecretKey = config.GetConnectionString("SecretKey");
            var endpointURL = config.GetConnectionString("EndpointURL");
            var storage = new s3Storage(keyID, SecretKey, endpointURL);

            builder.Services.AddDbContext<BotDbContext>(options => options.UseNpgsql(connectionString));

            using IHost host = builder.Build();

            var a = host.Services.GetService<BotDbContext>();
            var bot = new Bot.Bot(tokenString, storage, a);
            bot.Run();

            host.Run();
        }
    }
}