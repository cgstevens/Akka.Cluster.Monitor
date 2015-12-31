using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Akka.Actor;
using Akka.Cluster;
using Shared;
using Shared.Actors;

namespace Tasker.Api
{
    public class ClusterStatusController : ApiController
    {
        public ClusterEvent.CurrentClusterState ClusterState;

        public ClusterStatusController()
        {
            if (Program.ClusterStatus.IsNobody())
            {
                Program.ClusterStatus = Program.ClusterSystem.ActorOf(Props.Create(() => new ClusterStatus()),
                    ActorPaths.ClusterStatusActor.Name);
            }
        }

        public HttpResponseMessage Get(string id)
        {
            var t1 = Program.ClusterStatus.Ask(new ClusterStatus.GetClusterState(), TimeSpan.FromSeconds(2))
                .ContinueWith(
                        tr =>
                        {
                            ClusterState = (ClusterEvent.CurrentClusterState)tr.Result;
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously);

            t1.Wait(TimeSpan.FromSeconds(2));


            var retval = Request.CreateResponse(HttpStatusCode.OK, new
            {
                clusterState = ClusterState
            });
            return retval;
        }
    }
}
