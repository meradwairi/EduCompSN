using System.Collections.Generic;

namespace EduCompSN_Clean.Models
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<PostWithUserViewModel> UserPosts { get; set; }
        public List<Experience> Experiences { get; set; }
        public List<Education> Educations { get; set; }
        public List<Skill> Skills { get; set; }
        public List<UserDocument> Documents { get; set; }
        public int PostsCount { get; set; }
        public int FollowersCount { get; set; }
        public int FollowingCount { get; set; }
        public int CurrentUserId { get; set; }   // <-- هذه الخاصية المضافة
    }
}