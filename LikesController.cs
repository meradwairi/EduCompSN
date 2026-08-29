using System.Web.Mvc;
using EduCompSN_Clean.Models;
using System.Linq;

namespace EduCompSN_Clean.Controllers
{
    public class LikesController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        [HttpPost]
        [Authorize]
        public JsonResult Toggle(int postId)
        {
            int userId = (int)Session["UserId"];
            var existingLike = _context.Likes.FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
            {
                _context.Likes.Remove(existingLike);
                _context.SaveChanges();
                int newCount = _context.Likes.Count(l => l.PostId == postId);
                return Json(new { success = true, isLiked = false, likeCount = newCount });
            }
            else
            {
                var like = new Like { PostId = postId, UserId = userId };
                _context.Likes.Add(like);
                _context.SaveChanges();
                int newCount = _context.Likes.Count(l => l.PostId == postId);
                return Json(new { success = true, isLiked = true, likeCount = newCount });
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}