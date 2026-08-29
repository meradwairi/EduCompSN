using System.ComponentModel.DataAnnotations;

namespace EduCompSN_Clean.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Role")]
        public string Role { get; set; } = "Student";

        [Display(Name = "Admin")]
        public bool IsAdmin { get; set; }

        // ===== المعلومات الشخصية للجميع =====
        [Display(Name = "Profile Picture")]
        public string ProfilePicture { get; set; }

        [Display(Name = "Bio")]
        [DataType(DataType.MultilineText)]
        public string Bio { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }

        // ===== خاص بالجامعة (University) =====
        [Display(Name = "University Name")]
        public string UniversityName { get; set; }

        [Display(Name = "University Cover")]
        public string UniversityCover { get; set; }

        [Display(Name = "University Website")]
        [Url]
        public string UniversityWebsite { get; set; }

        [Display(Name = "University Description")]
        [DataType(DataType.MultilineText)]
        public string UniversityDescription { get; set; }

        [Display(Name = "Founded Year")]
        public int? FoundedYear { get; set; }

        [Display(Name = "Number of Students")]
        public int? NumberOfStudents { get; set; }

        // ===== خاص بالشركة (Company) =====
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        [Display(Name = "Company Cover")]
        public string CompanyCover { get; set; }

        [Display(Name = "Company Website")]
        [Url]
        public string CompanyWebsite { get; set; }

        [Display(Name = "Company Description")]
        [DataType(DataType.MultilineText)]
        public string CompanyDescription { get; set; }

        [Display(Name = "Company Size")]
        public string CompanySize { get; set; }

        [Display(Name = "Industry")]
        public string Industry { get; set; }
    }
}