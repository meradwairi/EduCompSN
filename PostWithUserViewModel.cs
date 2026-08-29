using System;

namespace EduCompSN_Clean.Models
{
    public class PostWithUserViewModel
    {
        public Post Post { get; set; }
        public string UserFullName { get; set; }
        public bool IsLikedByUser { get; set; }
        public int LikeCount { get; set; }
        public int PosterUserId { get; set; }   // معرف كاتب المنشور
    }
}