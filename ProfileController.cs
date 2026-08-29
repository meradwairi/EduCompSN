using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GET: Profile/Index (عرض بروفايل المستخدم الحالي)
        public ActionResult Index()
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = Convert.ToInt32(userIdObj);
            return RedirectToAction("Details", new { id = currentUserId });
        }

        // GET: Profile/Details/{id} (عرض بروفايل أي مستخدم)
        public ActionResult Details(int id)
        {
            object currentUserIdObj = Session["UserId"];
            if (currentUserIdObj == null)
                return RedirectToAction("Login", "Account");

            int currentUserId = Convert.ToInt32(currentUserIdObj);
            var user = _context.Users.Find(id);
            if (user == null)
                return HttpNotFound();

            // جلب منشورات المستخدم مع عدد اللايكات
            var posts = _context.Posts
                .Where(p => p.UserId == id)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PostWithUserViewModel
                {
                    Post = p,
                    UserFullName = user.FullName,
                    PosterUserId = id,
                    LikeCount = _context.Likes.Count(l => l.PostId == p.Id)
                }).ToList();

            // تحميل البيانات الإضافية فقط للمستخدمين العاديين (وليس شركة/جامعة)
            List<Experience> experiences = new List<Experience>();
            List<Education> educations = new List<Education>();
            List<Skill> skills = new List<Skill>();
            List<UserDocument> documents = new List<UserDocument>();

            if (user.Role != "Company" && user.Role != "University")
            {
                experiences = _context.Experiences.Where(e => e.UserId == id).OrderByDescending(e => e.StartDate).ToList();
                educations = _context.Educations.Where(ed => ed.UserId == id).OrderByDescending(ed => ed.StartYear).ToList();
                skills = _context.Skills.Where(s => s.UserId == id).ToList();
                documents = _context.UserDocuments.Where(d => d.UserId == id).ToList();
            }

            var model = new ProfileViewModel
            {
                User = user,
                UserPosts = posts,
                Experiences = experiences,
                Educations = educations,
                Skills = skills,
                Documents = documents,
                PostsCount = posts.Count,
                FollowersCount = 0,
                FollowingCount = 0,
                CurrentUserId = currentUserId  // مهم لتحديد ما إذا كان صاحب البروفايل هو المستخدم الحالي
            };
            return View("Index", model); // استخدم نفس View Index.cshtml
        }

        // GET: Profile/EditProfile
        public ActionResult EditProfile()
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(userIdObj);
            var user = _context.Users.Find(userId);
            if (user == null)
                return HttpNotFound();
            return View(user);
        }

        // POST: Profile/EditProfile (يستخدم FormCollection)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(FormCollection form)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Login", "Account");
            int userId = Convert.ToInt32(userIdObj);
            var user = _context.Users.Find(userId);
            if (user == null)
                return HttpNotFound();

            user.FullName = form["FullName"];
            user.Bio = form["Bio"];
            user.Location = form["Location"];

            // الصورة الشخصية
            if (Request.Files["profileImage"] != null && Request.Files["profileImage"].ContentLength > 0)
            {
                var file = Request.Files["profileImage"];
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                file.SaveAs(Path.Combine(path, fileName));
                user.ProfilePicture = "/Uploads/" + fileName;
            }

            // صورة الغلاف
            if (Request.Files["coverImage"] != null && Request.Files["coverImage"].ContentLength > 0)
            {
                var file = Request.Files["coverImage"];
                string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                string path = Server.MapPath("~/Uploads/");
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                file.SaveAs(Path.Combine(path, fileName));
                string coverPath = "/Uploads/" + fileName;
                if (user.Role == "Company")
                    user.CompanyCover = coverPath;
                else if (user.Role == "University")
                    user.UniversityCover = coverPath;
            }

            // معلومات الشركة أو الجامعة
            if (user.Role == "Company")
            {
                user.CompanyName = form["CompanyName"];
                user.CompanyDescription = form["CompanyDescription"];
                user.CompanyWebsite = form["CompanyWebsite"];
                user.CompanySize = form["CompanySize"];
                user.Industry = form["Industry"];
            }
            else if (user.Role == "University")
            {
                user.UniversityName = form["UniversityName"];
                user.UniversityDescription = form["UniversityDescription"];
                user.UniversityWebsite = form["UniversityWebsite"];
                int fy;
                if (int.TryParse(form["FoundedYear"], out fy))
                    user.FoundedYear = fy;
                int ns;
                if (int.TryParse(form["NumberOfStudents"], out ns))
                    user.NumberOfStudents = ns;
            }

            _context.Configuration.ValidateOnSaveEnabled = false;
            _context.SaveChanges();
            _context.Configuration.ValidateOnSaveEnabled = true;

            return RedirectToAction("Index");
        }

        // POST: AddExperience
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddExperience(Experience experience)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            int userId = Convert.ToInt32(userIdObj);
            experience.UserId = userId;
            _context.Experiences.Add(experience);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: DeleteExperience
        [HttpPost]
        public ActionResult DeleteExperience(int id)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            int userId = Convert.ToInt32(userIdObj);
            var exp = _context.Experiences.Find(id);
            if (exp != null && exp.UserId == userId)
            {
                _context.Experiences.Remove(exp);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: AddEducation
        [HttpPost]
        public ActionResult AddEducation(Education education)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            education.UserId = Convert.ToInt32(userIdObj);
            _context.Educations.Add(education);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: DeleteEducation
        [HttpPost]
        public ActionResult DeleteEducation(int id)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            int userId = Convert.ToInt32(userIdObj);
            var edu = _context.Educations.Find(id);
            if (edu != null && edu.UserId == userId)
            {
                _context.Educations.Remove(edu);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: AddSkill
        [HttpPost]
        public ActionResult AddSkill(string skillName)
        {
            if (!string.IsNullOrWhiteSpace(skillName))
            {
                object userIdObj = Session["UserId"];
                if (userIdObj != null)
                {
                    int userId = Convert.ToInt32(userIdObj);
                    _context.Skills.Add(new Skill { UserId = userId, Name = skillName.Trim() });
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // POST: DeleteSkill
        [HttpPost]
        public ActionResult DeleteSkill(int id)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            int userId = Convert.ToInt32(userIdObj);
            var skill = _context.Skills.Find(id);
            if (skill != null && skill.UserId == userId)
            {
                _context.Skills.Remove(skill);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // POST: UploadDocument
        [HttpPost]
        public ActionResult UploadDocument(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0 && file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                object userIdObj = Session["UserId"];
                if (userIdObj != null)
                {
                    int userId = Convert.ToInt32(userIdObj);
                    string fileName = Path.GetFileName(file.FileName);
                    string uniqueName = Guid.NewGuid().ToString() + "_" + fileName;
                    string uploadFolder = Server.MapPath("~/Uploads/");
                    if (!Directory.Exists(uploadFolder))
                        Directory.CreateDirectory(uploadFolder);
                    string path = Path.Combine(uploadFolder, uniqueName);
                    file.SaveAs(path);
                    var doc = new UserDocument
                    {
                        UserId = userId,
                        FileName = fileName,
                        FilePath = "/Uploads/" + uniqueName,
                        UploadedAt = DateTime.Now
                    };
                    _context.UserDocuments.Add(doc);
                    _context.SaveChanges();
                }
            }
            return RedirectToAction("Index");
        }

        // POST: DeleteDocument
        [HttpPost]
        public ActionResult DeleteDocument(int id)
        {
            object userIdObj = Session["UserId"];
            if (userIdObj == null)
                return RedirectToAction("Index");
            int userId = Convert.ToInt32(userIdObj);
            var doc = _context.UserDocuments.Find(id);
            if (doc != null && doc.UserId == userId)
            {
                string fullPath = Server.MapPath(doc.FilePath);
                if (System.IO.File.Exists(fullPath))
                    System.IO.File.Delete(fullPath);
                _context.UserDocuments.Remove(doc);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Profile/GetUserInfo (مستخدم في الدردشة)
        public JsonResult GetUserInfo(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            return Json(new { fullName = user.FullName, email = user.Email }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}