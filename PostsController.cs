using EduCompSN_Clean.Models;
using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EduCompSN_Clean.Controllers
{
    public class PostsController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext(); // تم تغيير الاسم من db إلى _context

        // عرض جميع المنشورات
        public ActionResult Index()
        {
            var posts = _context.Posts.OrderByDescending(p => p.CreatedAt).ToList();
            return View(posts);
        }

        // عرض منشور واحد بالتفصيل (للمشاركة)
        public ActionResult Details(int? id)
        {
            if (id == null) return RedirectToAction("Index");
            var post = _context.Posts.Find(id.Value);
            if (post == null) return HttpNotFound();
            var user = _context.Users.Find(post.UserId);
            ViewBag.UserName = user?.FullName ?? "Unknown";
            return View(post);
        }

        // نموذج إنشاء منشور جديد (GET)
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // حفظ منشور جديد (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Post post, HttpPostedFileBase image)
        {
            if (ModelState.IsValid)
            {
                post.UserId = (int)Session["UserId"];
                post.CreatedAt = DateTime.Now;

                if (image != null && image.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                    string path = Server.MapPath("~/Uploads/Posts/");
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    image.SaveAs(Path.Combine(path, fileName));
                    post.ImagePath = "/Uploads/Posts/" + fileName;
                }

                _context.Posts.Add(post);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(post);
        }

        // تعديل منشور (GET) – فقط لصاحب المنشور أو المدير
        [Authorize]
        public ActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return HttpNotFound();

            int currentUserId = (int)Session["UserId"];
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            if (post.UserId != currentUserId && !isAdmin)
                return new HttpUnauthorizedResult("You are not authorized to edit this post.");

            return View(post);
        }

        // تعديل منشور (POST)
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post, HttpPostedFileBase image)
        {
            var existingPost = _context.Posts.Find(post.Id);
            if (existingPost == null) return HttpNotFound();

            int currentUserId = (int)Session["UserId"];
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            if (existingPost.UserId != currentUserId && !isAdmin)
                return new HttpUnauthorizedResult();

            existingPost.Content = post.Content;

            if (image != null && image.ContentLength > 0)
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string path = Server.MapPath("~/Uploads/Posts/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                image.SaveAs(Path.Combine(path, fileName));
                existingPost.ImagePath = "/Uploads/Posts/" + fileName;
            }

            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // حذف منشور (GET) – عرض صفحة تأكيد
        [Authorize]
        public ActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return HttpNotFound();

            int currentUserId = (int)Session["UserId"];
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            if (post.UserId != currentUserId && !isAdmin)
                return new HttpUnauthorizedResult();

            return View(post);
        }

        // حذف منشور (POST) – تأكيد
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return HttpNotFound();

            int currentUserId = (int)Session["UserId"];
            bool isAdmin = Session["IsAdmin"] != null && (bool)Session["IsAdmin"];
            if (post.UserId != currentUserId && !isAdmin)
                return new HttpUnauthorizedResult();

            // حذف التعليقات والإعجابات المرتبطة
            var comments = _context.Comments.Where(c => c.PostId == id);
            _context.Comments.RemoveRange(comments);
            var likes = _context.Likes.Where(l => l.PostId == id);
            _context.Likes.RemoveRange(likes);
            _context.Posts.Remove(post);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}