using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FilesAgain.Startup))]
namespace FilesAgain
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
