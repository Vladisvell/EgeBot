using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.Interfaces
{
    public interface IS3Storage
    {
        public Task<string> PostFile(string fileName, Stream fileStream, string subject);

        public Task<GetObjectResponse> GetFile(string filename);
    }
}
