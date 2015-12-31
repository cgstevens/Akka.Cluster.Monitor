using System.Web.Http;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Shared;
using Shared.Actors;
using Website.Actors;
using Website.Hubs;

namespace Website.Controllers.API
{
    public class ReportController : ApiController
    {
        IHubContext _hubContext;

        public ReportController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ClusterStateHub>();
        }
        
        [HttpPost]
        public void MemberLeaveCluster(string host, int? port, string protocol, string system)
        {
            SystemActors.ClusterHelper.Tell(new ClusterHelper.MemberLeave(host, port, protocol, system));
        }

        [HttpPost]
        public void DownMember(string host, int? port, string protocol, string system)
        {
            SystemActors.ClusterHelper.Tell(new ClusterHelper.MemberDown(host, port, protocol, system));
        }

    }
}
