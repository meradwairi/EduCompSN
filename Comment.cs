using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public int PostId { get; set; }
        public int UserId { get; set; }
        [StringLength(500)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}