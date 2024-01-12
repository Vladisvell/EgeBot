using Amazon.S3.Model;
using System.IO;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IS3Storage
    {
        public Task PostFile(string fileName, Stream fileStream, int taskNumber, string subject);

        public Task<GetObjectResponse> GetFile(string filename);
    }
}