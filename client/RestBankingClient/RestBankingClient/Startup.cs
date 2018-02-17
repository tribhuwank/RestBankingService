using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RestBankingClient.Startup))]
namespace RestBankingClient
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
