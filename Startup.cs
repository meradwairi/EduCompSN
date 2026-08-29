using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EduCompSN_Clean.Startup))]
namespace EduCompSN_Clean
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}