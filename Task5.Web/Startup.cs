using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Task5.Web.Startup))]
namespace Task5.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
