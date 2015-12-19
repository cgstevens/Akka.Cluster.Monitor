
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Cluster;
using Akka.Event;
using Akka.Util.Internal;
using Topshelf;

namespace Shared
{
    /// <summary>
    /// Actor responsible for processing commands
    /// </summary>
    public class ClusterStatus : ReceiveActor
    {
        private ClusterEvent.CurrentClusterState _clusterState;

        public class GetClusterState
        {
        }
        
        public class SendCurrentClusterState
        {
        }

        public class IsClusterInValidState
        { }

        public class RecieveClusterReady
        {
            public RecieveClusterReady(bool ready)
            {
                Ready = ready;
            }

            public bool Ready { get; private set; }
        }

        protected Cluster Cluster = Cluster.Get(Context.System);
        private ICancelable _currentClusterStateTeller;
        private DateTime _clusterStartTime;
        private readonly ILoggingAdapter _logger;
        private readonly HostControl _hostControl;

        public ClusterStatus() : this(null)
        {
        }

        public ClusterStatus(HostControl hostControl = null)
        {
            _clusterStartTime = DateTime.Now;
            _hostControl = hostControl;
            _logger = Context.GetLogger();
            Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });Cluster.Subscribe(Self, ClusterEvent.InitialStateAsEvents, new[] { typeof(ClusterEvent.IMemberEvent), typeof(ClusterEvent.UnreachableMember) });
            _currentClusterStateTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMilliseconds(10),
                    TimeSpan.FromSeconds(5), Self, new SendCurrentClusterState(), Self);
            Ready();
        }
        
        protected override void PostStop()
        {
            _currentClusterStateTeller.Cancel();
            Cluster.Unsubscribe(Self);
        }

        private void Ready()
        {
            Receive<GetClusterState>(ic =>
            {
                List<ClusterRoleLeader> clusterRoleLeaders = new List<ClusterRoleLeader>();
                foreach (var member in _clusterState.Members)
                {
                    var role = member.Roles.First();
                    var address = _clusterState.RoleLeader(role);
                    clusterRoleLeaders.Add(new ClusterRoleLeader(role, address));
                }

                var currentClusterStatus = new CurrentClusterStatus(clusterRoleLeaders, Cluster.SelfAddress,
                    _clusterState);

                Sender.Tell(currentClusterStatus);
            });

            Receive<ClusterEvent.CurrentClusterState>(state =>
            {
                _clusterState = state;
                
                // Check Cluster Leader
                if (state.Leader == null)
                {
                    _logger.Warning("ClusterLeader is null");
                }
                else
                {
                    _logger.Debug("ClusterLeader : {0}", state.Leader.ToString());
                }

                // Check Role Leaders
                var roles = _clusterState.Members.Where(y => y.Status == MemberStatus.Up).Select(x => x.Roles.First()).Distinct().ToList();
                foreach (var role in roles)
                {
                    var address = state.RoleLeader(role);
                    if (address == null)
                    {
                        _logger.Warning("RoleLeader: {0}, No leader found!", role);
                    }
                    else
                    {
                        _logger.Debug("RoleLeader: {0}, Address:{1}", role, address);
                    }
                }

                // Check Unreachable Members?
                foreach (var member in state.Unreachable)
                {
                    _logger.Warning("Unreachable Member; Role: {0}, Status: {1}, Address: {2}, ", member.Roles.Join(";"), member.Status, member.Address.ToString());
                }

                // Check who I am seen by?
                foreach (var seenBy in state.SeenBy)
                {
                    if (_clusterState.Members.Any(x => x.Address == seenBy))
                    {
                        _logger.Debug("SeenBy Members; Role: {0}, Status: {1}, Address: {2}, ",
                            _clusterState.Members.First(x => x.Address == seenBy).Roles.Join(";"),
                            _clusterState.Members.First(x => x.Address == seenBy).Status,
                            _clusterState.Members.First(x => x.Address == seenBy).Address.ToString());
                    }
                    else
                    {
                        _logger.Debug("SeenBy Members; Role: null, Status: null, Address: {0}, ",
                            _clusterState.Members.First(x => x.Address == seenBy).Address.ToString());
                    }

                }
            });

            Receive<SendCurrentClusterState>(ic =>
            {
                Cluster.SendCurrentClusterState(Self);
            });

            Receive<ClusterEvent.MemberUp>(mem =>
            {
                _logger.Info("MemberUp: {0}, Role(s): {1}", mem.Member, mem.Member.Roles.Join(","));
            });

            Receive<ClusterEvent.UnreachableMember>(mem =>
            {
                _logger.Info("UnreachableMember: {0}, Role(s): {1}", mem.Member, mem.Member.Roles.Join(","));
            });

            Receive<ClusterEvent.ReachableMember>(mem =>
            {
                _logger.Info("ReachableMember: {0}, Role(s): {1}", mem.Member, mem.Member.Roles.Join(","));
            });

            Receive<ClusterEvent.MemberRemoved>(mem =>
            {
                _logger.Info("MemberRemoved: {0}, Role(s): {1}", mem.Member, mem.Member.Roles.Join(","));
                
                // Check to see if we have been removed?
                if (Cluster.SelfAddress.Equals(mem.Member.Address))
                {
                    ClusterNeedsToRestart(string.Format("This member has been removed from the cluster.  This system needs to be restarted. Address:{0} ", Cluster.SelfAddress));
                }

            });

            Receive<ClusterEvent.IMemberEvent>(mem =>
            {
                _logger.Info("IMemberEvent: {0}, Role(s): {1}", mem.Member, mem.Member.Roles.Join(","));
            });

            Receive<ClusterEvent.ClusterShuttingDown>(cluster =>
            {
                _logger.Warning("ClusterShuttingDown");
            });
            
            ReceiveAny(task =>
            {
                _logger.Error("{{EventId:999}} [x] Oh Snap! ClusterStatus.Ready.ReceiveAny: \r\n{0}", task);
            });
        }

        private void ClusterNeedsToRestart(string message)
        {
            Cluster.Leave(Cluster.SelfAddress);

            // Common EventId to cause external resource to trigger a restart on this service.
            var errorMessage = "EventId:5 " + message;
            _logger.Error(errorMessage);

            // Give enough time for messages to get sent to other members that we have left the cluster.
            Thread.Sleep(5000);

            
            if (_hostControl != null)
            {
                _hostControl.Stop();
            }

        }

    }
}