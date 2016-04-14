using System;
using System.Collections.Generic;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;

namespace Shared.Actors
{
    public class ClusterManager : ReceiveActor
    {
        public class SendCurrentClusterState
        {
        }
        public class SubscribeToManager
        {
        }
        public class UnSubscribeFromManager
        {
        }

        public class MemberDown
        {
            public MemberDown(string userName, Address address)
            {
                UserName = userName;
                Address = address;
            }

            public string UserName { get; private set; }
            public Address Address { get; private set; }
        }

        public class MemberLeave
        {
            public MemberLeave(string userName, Address address)
            {
                UserName = userName;
                Address = address;
            }

            public string UserName { get; private set; }
            public Address Address { get; private set; }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        protected Cluster Cluster = Cluster.Get(Context.System);
        private ICancelable _currentClusterStateTeller;
        protected ClusterEvent.CurrentClusterState _currentClusterState;
        private DateTime _clusterStartTime;
        private readonly HashSet<IActorRef> _clients = new HashSet<IActorRef>();

        public ClusterManager()
        {
            _clusterStartTime = DateTime.Now;
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.IReachabilityEvent) });
            _currentClusterStateTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromSeconds(2), Self, new SendCurrentClusterState(), Self);
            Ready();
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        private void Ready()
        {
            Receive<SubscribeToManager>(ic =>
            {
                _clients.Add(this.Sender);
                _logger.Info("Adding {0}", this.Sender.ToString());
            });

            Receive<UnSubscribeFromManager>(ic =>
            {
                _clients.Remove(this.Sender);
                _logger.Info("Removing {0}", this.Sender.ToString());
            });

            Receive<MemberDown>(ic =>
            {
                _logger.Warning("User {0} is forcing the following member down: {1}", ic.UserName, ic.Address.ToString());
                Cluster.Down(ic.Address);

                Thread.Sleep(5000);
            });

            Receive<MemberLeave>(ic =>
            {
                _logger.Warning("User {0} is asking the following member to leave the cluster: {1}", ic.UserName, ic.Address.ToString());
                Cluster.Leave(ic.Address);

                Thread.Sleep(5000);
            });

            Receive<ClusterEvent.CurrentClusterState>(state =>
            {
                _currentClusterState = state;
                foreach (var client in _clients)
                {
                    _logger.Info("Sending ClusterState to {0}", this.Sender.ToString());
                    client.Tell(_currentClusterState, Self);
                }
            });

            Receive<ClusterEvent.IReachabilityEvent>(mem =>
            {
                foreach (var client in _clients)
                {
                    client.Tell(mem, Self);
                }
            });

            Receive<ClusterEvent.IMemberEvent>(mem =>
            {
                foreach (var client in _clients)
                {
                    client.Tell(mem, Self);
                }
            });

            Receive<SendCurrentClusterState>(ic =>
            {
                Cluster.SendCurrentClusterState(Self);
            });

            ReceiveAny(task =>
            {
                _logger.Error("Oh Snap! ClusterSender.Ready.ReceiveAny: \r\n{0}", task);
            });
        }

    }
}