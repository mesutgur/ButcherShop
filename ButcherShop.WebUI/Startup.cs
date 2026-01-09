using Microsoft.Owin;
using Owin;
using ButcherShop.DataAccess.Context;

[assembly: OwinStartupAttribute(typeof(ButcherShop.WebUI.Startup))]
namespace ButcherShop.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
