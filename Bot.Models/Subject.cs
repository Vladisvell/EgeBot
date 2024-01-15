using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Models
{
    [Microsoft.EntityFrameworkCore.Index(nameof(Title), IsUnique = true)]
    public class Subject
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("title")]
        [MaxLength(100)]
        public required string Title { get; set; }

        public virtual ICollection<TaskKim> TasksKim { get; set; }
    }
}
