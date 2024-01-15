using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System.Linq;
using Microsoft.Extensions.Configuration;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot.Infrastructure
{
    public class s3Storage : IS3Storage
    {
        private IAmazonS3 S3Client;
        public s3Storage(IConfigurationRoot config)
        {
            var keyID = config.GetConnectionString("KeyID");
            var secretKey = config.GetConnectionString("SecretKey");
            var endpointURL = config.GetConnectionString("EndpointURL");

            var s3Config = new AmazonS3Config() { ServiceURL = endpointURL };
            S3Client = new AmazonS3Client(keyID, secretKey, s3Config);
        }

        public async Task<string> PostFile(string fileName, Stream fileStream, string subject)
        {
            var guid = Guid.NewGuid().ToString();
            var temp = fileName.Split('.');
            var newFileName = $"{subject}/{guid}.{temp.Last()}";
            var objectRequest = new PutObjectRequest()
            {
                BucketName = "Egegebot",
                Key = newFileName,
                InputStream = fileStream
            };

            var responce = await S3Client.PutObjectAsync(objectRequest);
            Console.WriteLine(responce.ToString());
            return newFileName;
        }

        public async Task<GetObjectResponse> GetFile(string filename)
        {
            var request = new GetObjectRequest
            {
                BucketName = "Egegebot",
                Key = filename
            };

            return await S3Client.GetObjectAsync(request);
        }
    }
}