using System.Web.Mvc;
using EduCompSN_Clean.Models;
using System.Linq;

namespace EduCompSN_Clean.Controllers
{
    public class UsersController : Controller
    {
        private readonly AppDbContext _context = new AppDbContext();

        public ActionResult Index(string search)
        {
            var users = _context.Users.AsQueryable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                string term = search.Trim();
                users = users.Where(u => u.FullName.Contains(term) || u.Email.Contains(term));
            }
            return View(users.OrderBy(u => u.FullName).ToList());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) _context.Dispose();
            base.Dispose(disposing);
        }
    }
}