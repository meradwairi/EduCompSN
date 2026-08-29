using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Experience
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }  // مثلاً: "Frontend Developer"

        [StringLength(100)]
        public string Company { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; } // null تعني مستمر

        [StringLength(500)]
        public string Description { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}