using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(IO_3_2.Startup))]
namespace IO_3_2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
