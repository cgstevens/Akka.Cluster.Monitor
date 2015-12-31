using System.Net;
using System.Threading;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Akka.Actor;
using Shared;
using Shared.Actors;
using Website.Actors;

namespace Website
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected static ActorSystem ClusterSystem;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfig.CustomizeConfig(GlobalConfiguration.Configuration);
            AuthConfig.RegisterAuth();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Ssl3;

            
            ClusterSystem = ActorSystem.Create(ActorPaths.ActorSystem);
            SystemActors.CommandProcessor = ClusterSystem.ActorOf(Props.Create(() => new CommandProcessor()), "commands");
            SystemActors.SignalRItemStatusActor = ClusterSystem.ActorOf(Props.Create(() => new SignalRItemStatusActor()), "signalritemstatus");
            SystemActors.ClusterStatus = ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus()), ActorPaths.ClusterStatusActor.Name);
            SystemActors.SignalRClusterStatusActor = ClusterSystem.ActorOf(Props.Create(() => new SignalRClusterStatusActor()), "signalrclusterstatus");
            SystemActors.ServiceStatusActor = ClusterSystem.ActorOf(Props.Create(() => new ClusterServiceStatusActor()), "clusterservicestatus");
            SystemActors.ClusterHelper = ClusterSystem.ActorOf(Props.Create(() => new ClusterHelper()), "clusterhelper");
            
        }

        protected void Application_Stop()
        {
            SystemActors.ClusterHelper.Tell(new ClusterHelper.RemoveMember());
            
            Thread.Sleep(5000); // Give the Remove time to actually remove...

            ClusterSystem.Shutdown();
        }
    }
}