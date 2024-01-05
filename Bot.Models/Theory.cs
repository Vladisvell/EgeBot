using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EgeBot.Bot.Models
{
    [Table("theory")]
    public class Theory
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("topic")]
        [ForeignKey("TopicId")]
        [Index("IX_TopicUnique", 1, IsUnique = true)]
        public virtual required Topic Topic { get; set; }

        [Column("text")]
        public required string Text { get; set; }
    }
}
