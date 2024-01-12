using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Services.DBContext
{
    public class Status
    {
        private readonly bool isOk;
        private readonly string status;

        public Status(bool isOk, string status)
        {
            this.isOk = isOk;
            this.status = status;
        }
    }
}
