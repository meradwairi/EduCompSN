using System.Web.Mvc;
using EduCompSN_Clean.Models;
using System.Linq;

namespace EduCompSN_Clean.Controllers
{
    public class CompaniesController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        // عرض جميع الشركات مع البحث (غير حساس لحالة الأحرف)
        public ActionResult Index(string search)
        {
            var companies = _context.Users.Where(u => u.Role == "Company");

            if (!string.IsNullOrEmpty(search))
            {
                search = search.Trim().ToLower();
                companies = companies.Where(c => c.FullName.ToLower().Contains(search) ||
                                                 (c.CompanyName != null && c.CompanyName.ToLower().Contains(search)));
            }

            return View(companies.OrderBy(c => c.FullName).ToList());
        }

        // عرض صفحة تفاصيل شركة معينة (بما فيها منشوراتها)
        public ActionResult Details(int id)
        {
            var company = _context.Users.Find(id);
            if (company == null || company.Role != "Company")
                return HttpNotFound();

            var posts = _context.Posts.Where(p => p.UserId == id).OrderByDescending(p => p.CreatedAt).ToList();
            ViewBag.Posts = posts;
            return View(company);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}