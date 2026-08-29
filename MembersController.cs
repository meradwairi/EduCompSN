using System.Linq;
using System.Web.Mvc;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean.Controllers
{
    public class MembersController : Controller
    {
        private readonly AppDbContext db = new AppDbContext();

        // GET: Members/Companies?search=...
        public ActionResult Companies(string search)
        {
            var companies = db.Users.Where(u => u.Role == "Company").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                companies = companies.Where(c => c.FullName.Contains(search) ||
                                                 c.Email.Contains(search) ||
                                                 (c.CompanyName != null && c.CompanyName.Contains(search)));
            }

            ViewBag.Search = search;
            return View(companies.ToList());
        }

        // GET: Members/Members?search=...
        public ActionResult Members(string search)
        {
            var members = db.Users.Where(u => u.Role == "Student").AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                members = members.Where(m => m.FullName.Contains(search) ||
                                             m.Email.Contains(search));
            }

            ViewBag.Search = search;
            return View(members.ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}