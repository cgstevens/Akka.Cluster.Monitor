using System.Configuration;
using System.Diagnostics;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Website.Startup))]
namespace Website
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            if (!Debugger.IsAttached)
            {
                // Any connection or hub wire up and configuration should go here

                // TODO: Uncomment to enable backplane.
                // string sqlConnectionString = ConfigurationManager.ConnectionStrings["SignalR"].ConnectionString;
                // GlobalHost.DependencyResolver.UseSqlServer(sqlConnectionString);
            }

            app.MapSignalR();
        }
    }
}
