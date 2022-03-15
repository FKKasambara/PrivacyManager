using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PrivacyManager.Startup))]
namespace PrivacyManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
