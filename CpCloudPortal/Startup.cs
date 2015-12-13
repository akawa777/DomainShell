using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CpCloudPortal.Startup))]
namespace CpCloudPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
