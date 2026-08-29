using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EduCompSN_Clean.Models
{
    public class Follow
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int FollowerId { get; set; }

        [Required]
        public int FollowedId { get; set; }

        [ForeignKey("FollowerId")]
        public virtual User Follower { get; set; }

        [ForeignKey("FollowedId")]
        public virtual User Followed { get; set; }
    }
}