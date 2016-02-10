using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyStoryMaker.Startup))]
namespace MyStoryMaker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
