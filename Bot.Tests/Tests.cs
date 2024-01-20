#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using Telegram.Bot;

namespace EgeBot.Bot.Tests
{    
    [TestFixture]
    public class FrameworkChecks
    {
        /// <summary>
        /// If this test is not ran, then test framework does not work.
        /// </summary>
        [Test]
        public void CheckTestFrameworkWorks()
        {
            Assert.Pass();
        }
    }

    /// <summary>
    /// This class is responsible for validating configuration settings.
    /// </summary>
    [TestFixture]
    public class ConfigurationChecks
    {
        bool isConfigAvailable = false;
        IConfigurationRoot configuration;

        public ConfigurationChecks()
        {
            var configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.SetBasePath(Directory.GetCurrentDirectory());
            configurationBuilder.AddJsonFile("appsettings_example/appsettings.json");
            configuration = configurationBuilder.Build();
            isConfigAvailable = true;
        }

        [Test]
        public void DoesConfigurationFileExist()
        {
            Assert.That(isConfigAvailable, Is.True);
        }

        //[Test]
        //public void CheckBotTokenValid()
        //{
        //    Assert.DoesNotThrowAsync(async () => {
        //        var botClient = new TelegramBotClient(configuration.GetConnectionString("BotToken"));
        //        await botClient.GetMeAsync();                
        //        }, $"Provided Bot Token is invalid.\n");
        //}

        //[Test]
        //public void CheckS3TokenValid()
        //{
        //    var keyID = configuration.GetConnectionString("KeyID");
        //    var secretKey = configuration.GetConnectionString("SecretKey");
        //    var endpointURL = configuration.GetConnectionString("EndpointURL");

        //    var s3Config = new AmazonS3Config() { ServiceURL = endpointURL };
        //    IAmazonS3 S3Client = new AmazonS3Client(keyID, secretKey, s3Config);            
        //}
    }
}
#endif