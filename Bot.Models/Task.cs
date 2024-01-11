using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgeBot.Bot.Models.Enums;

namespace EgeBot.Bot.Models
{
    public class Task
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("topic")]
        [ForeignKey("TopicId")]
        public virtual required Topic Topic { get; set; }

        [Column("text")]
        public required string Text { get; set; }

        [Column("image")]
        public byte[]? Image { get; set; }

        [Column("correct_answer")]
        public required string CorrectAnswer { get; set; }

        [Column("complexity")]
        public required Complexity Complexity { get; set; }

        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
