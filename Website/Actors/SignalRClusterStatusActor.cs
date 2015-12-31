using System;
using Akka.Actor;
using Akka.Cluster;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Shared;
using Shared.Actors;
using Website.Hubs;

namespace Website.Actors
{
    /// <summary>
    /// Actor used to wrap a signalr hub
    /// </summary>
    public class SignalRClusterStatusActor : ReceiveActor
    {
        #region Messages
        
        public class GetClusterStatus
        {
        }

        
        #endregion

        private ClusterStateHub _hub;
        protected ICancelable ClusterStatusTeller;
        protected Cluster Cluster = Cluster.Get(Context.System);

        public SignalRClusterStatusActor()
        {
            ClusterStatusTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(1), Self, new ClusterStatus.SendCurrentClusterState(), Self);

            Ready();
        }

        public void Ready()
        {
            Receive<ClusterEvent.CurrentClusterState>(state =>
            {
                _hub.PushClusterState(state, Cluster.SelfAddress);
            });

            Receive<ClusterStatus.SendCurrentClusterState>(ic =>
            {
                Cluster.SendCurrentClusterState(Self);
            });
        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("clusterStateHub") as ClusterStateHub;
        }


    }
}