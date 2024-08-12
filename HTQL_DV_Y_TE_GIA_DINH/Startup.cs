using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HTQL_DV_Y_TE_GIA_DINH.Startup))]
namespace HTQL_DV_Y_TE_GIA_DINH
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
