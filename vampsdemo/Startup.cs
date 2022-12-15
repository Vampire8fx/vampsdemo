using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(vampsdemo.Startup))]
namespace vampsdemo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {            
        }
    }
}
