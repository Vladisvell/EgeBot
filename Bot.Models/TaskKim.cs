using EgeBot.Bot.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace EgeBot.Bot.Models
{
    [Table("task_kim")]
    [Microsoft.EntityFrameworkCore.Index(nameof(Type), IsUnique = true)]
    public class TaskKim
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("type")]
        public required int Type { get; set; }

        [Column("title")]
        [MaxLength(100)]
        public required string Title { get; set; }

        [Column("subject")]
        [ForeignKey("SubjectId")]
        public virtual required Subject Subject { get; set; }

        public virtual ICollection<Topic> Topics { get; set; }
    }
}
