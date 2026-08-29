using System;
using System.ComponentModel.DataAnnotations;

namespace EduCompSN_Clean.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Content { get; set; }
        public string ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}