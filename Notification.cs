using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        [StringLength(100)]
        public string Type { get; set; }
        [StringLength(500)]
        public string Message { get; set; }
        public int? SourceId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [StringLength(500)]
        public string Url { get; set; }   // ⬅️ هذه الخاصية كانت مفقودة
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}