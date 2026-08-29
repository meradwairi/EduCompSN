using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class ChatMessage
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FromUserId { get; set; }

        [Required]
        public int ToUserId { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime SentAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;

        [ForeignKey("FromUserId")]
        public virtual User FromUser { get; set; }

        [ForeignKey("ToUserId")]
        public virtual User ToUser { get; set; }
    }
}