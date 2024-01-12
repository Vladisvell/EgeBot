using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using EgeBot.Bot.Models.Enums;

namespace EgeBot.Bot.Models
{
    [Table("user")]
    //[Microsoft.EntityFrameworkCore.Index(nameof(ChatId), IsUnique = true)]
    public class User
    {
        //[Key]
        //[Column("id")]
        //public int Id { get; set; }

        [Key]
        [Column("id")]
        public required long Id { get; set; }

        [Column("nick_name")]
        public string NickName { get; set; } = "булочка";

        [Column("is_admin")]
        public bool IsAdmin { get; set; } = false; 

        [ForeignKey("topicId")]
        public virtual Topic? SettingTopic { get; set; }

        [Column("complexity")]
        public Complexity SettingComplexity { get; set; } = Complexity.easy;

        public virtual ICollection<UserTask> UserTasks { get; set; }
    }
}
