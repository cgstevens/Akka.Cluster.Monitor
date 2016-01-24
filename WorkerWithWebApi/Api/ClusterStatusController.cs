using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using Akka.Actor;
using Shared;
using Shared.Actors;
using Shared.Commands;
using Website.Actors;

namespace WorkerWithWebApi.Api
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ClusterStatusController : ApiController
    {
        public CurrentClusterStatus CurrentClusterState;

        public ClusterStatusController()
        {
            if (Program.ClusterStatus.IsNobody())
            {
                Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus()),
                    ActorPaths.ClusterStatusActor.Name);
            }
        }

        public CurrentClusterStatus Get(string id)
        {
            CurrentClusterState = new CurrentClusterStatus(null, null, null);

            var t1 = Program.ClusterStatus.Ask(new ClusterStatus.GetClusterState(), TimeSpan.FromSeconds(2))
                .ContinueWith(
                        tr =>
                        {
                            CurrentClusterState = (CurrentClusterStatus)tr.Result;
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);

            t1.Wait(TimeSpan.FromSeconds(2));

            
            return CurrentClusterState;
        }
        
        public HttpResponseMessage GetClusterState()
        {
            var t1 = Program.ClusterStatus.Ask(new ClusterStatus.GetClusterState(), TimeSpan.FromSeconds(2))
                .ContinueWith(
                        tr =>
                        {
                            CurrentClusterState = (CurrentClusterStatus)tr.Result;
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);

            t1.Wait(TimeSpan.FromSeconds(2));


            var retval = Request.CreateResponse(HttpStatusCode.OK, new
            {
                clusterState = CurrentClusterState
            });
            return retval;
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
