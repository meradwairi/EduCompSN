using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.Entity;
using EduCompSN_Clean.Models;

namespace EduCompSN_Clean
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            Database.SetInitializer<AppDbContext>(null);
        }
    }
}