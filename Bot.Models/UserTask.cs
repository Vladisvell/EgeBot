using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EgeBot.Bot.Services.Interfaces;

namespace EgeBot.Bot.Models
{
    [Table("user_task")]
    public class UserTask
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user")]
        [ForeignKey("ChatId")]
        [Index("IX_UserTask", 1, IsUnique = true)]
        public virtual required User User { get; set; }

        [Column("task")]
        [ForeignKey("TaskcId")]
        [Index("IX_UserTask", 2, IsUnique = true)]
        public virtual required Task Task { get; set; }

        [Column("user_answer")]
        [MaxLength(100)]
        public string UserAnswer { get; set; } = "NOT";
    }
}
