using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(LeapSpring.MJC.Api.Startup))]
namespace LeapSpring.MJC.Api
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}