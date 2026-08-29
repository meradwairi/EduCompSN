using System.Web.Mvc;
using EduCompSN_Clean.Models;
using System.Linq;

namespace EduCompSN_Clean.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext(); // تغيير الاسم

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["IsAdmin"] == null || !(bool)Session["IsAdmin"])
            {
                filterContext.Result = new HttpUnauthorizedResult("You are not authorized.");
                return;
            }
            base.OnActionExecuting(filterContext);
        }

        public ActionResult Dashboard()
        {
            ViewBag.UserCount = _context.Users.Count();
            ViewBag.PostCount = _context.Posts.Count();
            ViewBag.CommentCount = _context.Comments.Count();
            return View();
        }

        public ActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            return RedirectToAction("Users");
        }

        public ActionResult Posts()
        {
            var posts = _context.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        [HttpPost]
        public ActionResult DeletePost(int id)
        {
            var post = _context.Posts.Find(id);
            if (post != null)
            {
                _context.Posts.Remove(post);
                _context.SaveChanges();
            }
            return RedirectToAction("Posts");
        }

        public ActionResult Comments()
        {
            var comments = _context.Comments.OrderByDescending(c => c.CreatedAt).ToList();
            return View(comments);
        }

        [HttpPost]
        public ActionResult DeleteComment(int id)
        {
            var comment = _context.Comments.Find(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
            return RedirectToAction("Comments");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}