using System.Diagnostics;
using Microsoft.Owin;
using Owin;
using System.Configuration;
using Microsoft.AspNet.SignalR;

[assembly: OwinStartupAttribute(typeof(Website2.Startup))]
namespace Website2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            //if (!Debugger.IsAttached)
            //{
                // Any connection or hub wire up and configuration should go here

                // TODO: Uncomment to enable backplane.
                string sqlConnectionString = ConfigurationManager.ConnectionStrings["SignalR"].ConnectionString;
                GlobalHost.DependencyResolver.UseSqlServer(sqlConnectionString);
            //}

            app.MapSignalR();
        }
    }
}
