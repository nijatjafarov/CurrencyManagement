using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CurrencyManager.Startup))]
namespace CurrencyManager
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
