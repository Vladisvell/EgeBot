using System;
using System.IO;
using System.Threading.Tasks;
using Amazon.S3;
using Amazon.S3.Model;
using System.Linq;

namespace EgeBot
{
    public class s3Storage
    {
        private IAmazonS3 S3Client;
        public s3Storage(string keyID, string secretKey, string endpointURL)
        {
            var s3Config = new AmazonS3Config() { ServiceURL = endpointURL };
            this.S3Client = new AmazonS3Client(keyID, secretKey, s3Config);
        }  
        public async Task PostFile(string fileName, Stream fileStream, int taskNumber, string subject)
        {
            var guid = Guid.NewGuid().ToString();
            var temp = fileName.Split('.');
            var newFileName = $"{subject}/{taskNumber}/{guid}.{temp.Last()}";
            var objectRequest = new PutObjectRequest()//Пока подключил облако от другого проекта. Только для теста.
            {
                BucketName = "Egegebot",
                Key = newFileName,
                InputStream= fileStream
            };

            var responce = await S3Client.PutObjectAsync(objectRequest);
            Console.WriteLine(responce.ToString());
        }

        public async Task GetFile(string filename)
        {
            var response = await S3Client.GetObjectAsync("Egegebot", filename);
        }
    }
}
