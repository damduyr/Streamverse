using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Streamverse.Startup))]
namespace Streamverse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
