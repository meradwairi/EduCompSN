using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        // GET: Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var user = _context.Users.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    // تخزين بيانات المستخدم في Session
                    Session["UserId"] = user.Id;
                    Session["UserEmail"] = user.Email;
                    Session["UserFullName"] = user.FullName;
                    Session["IsAdmin"] = user.IsAdmin;
                    Session["UserRole"] = user.Role;

                    FormsAuthentication.SetAuthCookie(user.Email, model.RememberMe);
                    return RedirectToLocal(returnUrl);
                }
                ModelState.AddModelError("", "Invalid email or password.");
            }
            return View(model);
        }

        // GET: Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Role != "Student" && model.Role != "Company" && model.Role != "University")
                {
                    ModelState.AddModelError("Role", "Please select a valid role.");
                    return View(model);
                }

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Password = model.Password,
                    Role = model.Role,
                    IsAdmin = false,
                    UniversityName = (model.Role == "University") ? model.UniversityName : null,
                    CompanyName = (model.Role == "Company") ? model.CompanyName : null
                };
                _context.Users.Add(user);
                _context.SaveChanges();

                // جعل أول مستخدم مسجل هو Admin
                if (_context.Users.Count() == 1)
                {
                    user.IsAdmin = true;
                    _context.SaveChanges();
                }

                // تخزين بيانات المستخدم في Session
                Session["UserId"] = user.Id;
                Session["UserEmail"] = user.Email;
                Session["UserFullName"] = user.FullName;
                Session["IsAdmin"] = user.IsAdmin;
                Session["UserRole"] = user.Role;

                FormsAuthentication.SetAuthCookie(user.Email, false);
                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            // إنهاء جلسة Forms Authentication
            FormsAuthentication.SignOut();

            // مسح جميع بيانات Session
            Session.Clear();
            Session.Abandon();

            // حذف Cookie المصادقة بشكل آمن
            if (Request.Cookies[FormsAuthentication.FormsCookieName] != null)
            {
                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName)
                {
                    Expires = DateTime.Now.AddDays(-1),
                    HttpOnly = true
                };
                Response.Cookies.Add(cookie);
            }

            // منع التخزين المؤقت (Cache) لضمان عدم بقاء بيانات المستخدم
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now.AddDays(-1));
            Response.Cache.SetNoStore();

            // إعادة التوجيه إلى الصفحة الرئيسية
            return RedirectToAction("Index", "Home");
        }

        // دالة مساعدة لإعادة التوجيه إلى الصفحة المطلوبة
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                _context.Dispose();
            base.Dispose(disposing);
        }
    }
}