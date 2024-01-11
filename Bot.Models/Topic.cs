using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Models
{
    [Table("topic")]
    //[Microsoft.EntityFrameworkCore.Index(nameof(Title), IsUnique = true)]
    public class Topic
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("task_kim")]
        [ForeignKey("TaskKimId")]
        public virtual required TaskKim TaskKim { get; init; }

        [Column("title")]
        [MaxLength(100)]
        //[Index("IX_TitleUnique", 1, IsUnique = true)]
        public required string Title { get; set; }

        public virtual ICollection<Task> Tasks { get; set; } 
    }
}
