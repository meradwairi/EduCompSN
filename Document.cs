using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Document Title")]
        public string Title { get; set; }

        [StringLength(500)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [StringLength(500)]
        public string FilePath { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime UploadedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}