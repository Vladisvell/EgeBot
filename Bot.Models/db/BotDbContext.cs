using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Models.db
{
    public class BotDbContext : DbContext
    {
        public BotDbContext(DbContextOptions<BotDbContext> options) : base(options) { }

        public BotDbContext(string connectionString) : base(GetOptions<BotDbContext>(connectionString)) { }

        protected static DbContextOptions<TContext> GetOptions<TContext>(string connectionString) where TContext : DbContext
        {
            var optionsBuilder = new DbContextOptionsBuilder<TContext>();
            optionsBuilder.UseLazyLoadingProxies().UseNpgsql(connectionString); 
            return optionsBuilder.Options;
        }

        public DbSet<User> User { get; set; }

        public DbSet<Task> Task { get; set; }

        public DbSet<TaskKim> TaskKim { get; set; }

        public DbSet<Theory> Theory { get; set; }

        public DbSet<Topic> Topic { get; set; }

        public DbSet<UserTask> UserTask { get; set; }

        public DbSet<Subject> Subject { get; set; }
    }
}
