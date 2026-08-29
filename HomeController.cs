using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private AppDbContext db = new AppDbContext();

        // GET: Home/Index
        public ActionResult Index()
        {
            var posts = db.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            var userIds = posts.Select(p => p.UserId).Distinct().ToList();
            var users = db.Users.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u.FullName);

            var postViewModels = new List<PostWithUserViewModel>();
            foreach (var post in posts)
            {
                int likeCount = db.Likes.Count(l => l.PostId == post.Id);
                string fullName = users.ContainsKey(post.UserId) ? users[post.UserId] : "Unknown";
                postViewModels.Add(new PostWithUserViewModel
                {
                    Post = post,
                    UserFullName = fullName,
                    PosterUserId = post.UserId,
                    LikeCount = likeCount
                });
            }
            return View(postViewModels);
        }

        // POST: Home/CreatePost
        [HttpPost]
        public JsonResult CreatePost(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "الرجاء كتابة محتوى المنشور" });

            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "الرجاء تسجيل الدخول" });

            int userId = Convert.ToInt32(userIdObj);
            var post = new Post
            {
                Content = content,
                UserId = userId,
                CreatedAt = DateTime.Now
            };
            db.Posts.Add(post);
            db.SaveChanges();
            return Json(new { success = true, postId = post.Id });
        }

        // POST: Home/ToggleLike
        [HttpPost]
        public JsonResult ToggleLike(int postId)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "يجب تسجيل الدخول" });

            int userId = Convert.ToInt32(userIdObj);
            var existingLike = db.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);
            if (existingLike != null)
            {
                db.Likes.Remove(existingLike);
                db.SaveChanges();
                int newCount = db.Likes.Count(l => l.PostId == postId);
                return Json(new { success = true, newLikeCount = newCount });
            }
            else
            {
                var like = new Like { PostId = postId, UserId = userId };
                db.Likes.Add(like);
                db.SaveChanges();
                int newCount = db.Likes.Count(l => l.PostId == postId);
                return Json(new { success = true, newLikeCount = newCount });
            }
        }

        // GET: Home/Search
        public ActionResult Search(string query)
        {
            ViewBag.SearchQuery = query;
            if (string.IsNullOrWhiteSpace(query))
                return View("Index", new List<PostWithUserViewModel>());

            var posts = db.Posts.Where(p => p.Content.Contains(query)).OrderByDescending(p => p.CreatedAt).ToList();
            var userIds = posts.Select(p => p.UserId).Distinct().ToList();
            var users = db.Users.Where(u => userIds.Contains(u.Id)).ToDictionary(u => u.Id, u => u.FullName);

            var results = new List<PostWithUserViewModel>();
            foreach (var post in posts)
            {
                int likeCount = db.Likes.Count(l => l.PostId == post.Id);
                string fullName = users.ContainsKey(post.UserId) ? users[post.UserId] : "Unknown";
                results.Add(new PostWithUserViewModel
                {
                    Post = post,
                    UserFullName = fullName,
                    PosterUserId = post.UserId,
                    LikeCount = likeCount
                });
            }
            return View("Index", results);
        }

        // GET: Home/About
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            return View();
        }

        // GET: Home/Contact
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}