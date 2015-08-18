using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CarsCatalog.Startup))]
namespace CarsCatalog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
