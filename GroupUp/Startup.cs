using System.Web.ModelBinding;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GroupUp.Startup))]
namespace GroupUp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
