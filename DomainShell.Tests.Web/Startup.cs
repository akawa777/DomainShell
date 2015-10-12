using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DomainShell.Tests.Web.Startup))]
namespace DomainShell.Tests.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
