using System;
using System.Linq;
using System.Web.Mvc;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        // POST: Comments/AddComment
        [HttpPost]
        public JsonResult AddComment(int postId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return Json(new { success = false, message = "Comment cannot be empty." });

            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return Json(new { success = false, message = "You must be logged in." });

            int userId = Convert.ToInt32(userIdObj);

            var comment = new Comment
            {
                PostId = postId,
                UserId = userId,
                Content = content,
                CreatedAt = DateTime.Now
            };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return Json(new { success = true });
        }

        // GET: Comments/GetComments
        public JsonResult GetComments(int postId)
        {
            var comments = _context.Comments
                .Where(c => c.PostId == postId)
                .OrderByDescending(c => c.CreatedAt)
                .Select(c => new
                {
                    c.Id,
                    c.Content,
                    CreatedAt = c.CreatedAt.ToString("dd MMM yyyy HH:mm"),
                    UserName = c.User.FullName
                })
                .ToList();

            return Json(comments, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}