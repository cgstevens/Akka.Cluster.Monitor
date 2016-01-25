using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Cluster;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Shared.Commands;
using Shared.States;
using Website2.Hubs;

namespace Website2.Actors
{
    /// <summary>
    /// Actor used to wrap a signalr hub
    /// </summary>
    public class SignalRItemStatusActor : ReceiveActor
    {
        #region Messages

        public class ClusterRoutes
        {
            public ClusterRoutes(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        public class SubscribeMessage
        {
            public SubscribeMessage(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        
        public class Subscribe
        {
            public Subscribe(string connectionId)
            {
                ConnectionId = connectionId;
            }

            public string ConnectionId { get; private set; }
        }

        public class SubscribeToWorkers
        {
        }

        public class UnSubscribeFromWorkers
        {
        }

        public class UnSubscribe
        {
            public UnSubscribe(string connectionId)
            {
                ConnectionId = connectionId;
            }

            public string ConnectionId { get; private set; }
        }

        #endregion

        private ItemStatusHub _hub;
        protected ICancelable ItemSubscriberTeller;
        protected Cluster Cluster = Cluster.Get(Context.System);
        private List<string> _subscribedClients = new List<string>(); 

        public SignalRItemStatusActor()
        {
            ItemSubscriberTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10), Self, new SubscribeToWorkers(), Self);

            Ready();
        }

        public void Ready()
        {
            Receive<SubscribeToWorkers>(ic =>
            {
                SystemActors.CommandProcessor.Tell(new CommandProcessor.AttemptSubscription(), Self);
            });

            Receive<UnSubscribeFromWorkers>(ic =>
            {
                SystemActors.CommandProcessor.Tell(new CommandProcessor.AttemptUnSubscribe(), Self);
            });

            Receive<Subscribe>(ic =>
            {
                if (!_subscribedClients.Contains(ic.ConnectionId))
                {
                    _subscribedClients.Add(ic.ConnectionId);
                }
            });

            Receive<UnSubscribe>(ic =>
            {
                _subscribedClients.Remove(ic.ConnectionId);
            });
            
            Receive<ClusterRoutes>(ic =>
            {
                _hub.ClusterRoutes(ic);
            });
            
            Receive<SubscriptionStatus>(ic =>
            {
                _hub.PushSubscriptionStatus(ic);
            });

            Receive<JobWorkerInfo>(ic =>
            {
                _hub.PushJobWorkerInfo(ic);
            });
        }

        protected override void PostStop()
        {
            ItemSubscriberTeller.Cancel();
            base.PostStop();
        }

        protected override void PreStart()
        {
            var hubManager = new DefaultHubManager(GlobalHost.DependencyResolver);
            _hub = hubManager.ResolveHub("itemStatusHub") as ItemStatusHub;
        }


    }
}