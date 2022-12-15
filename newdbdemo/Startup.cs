using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(newdbdemo.Startup))]
namespace newdbdemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
