using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Education
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string Degree { get; set; }  // "Bachelor of Computer Science"

        [StringLength(100)]
        public string FieldOfStudy { get; set; }

        [StringLength(100)]
        public string Institution { get; set; }

        public int? StartYear { get; set; }

        public int? EndYear { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}