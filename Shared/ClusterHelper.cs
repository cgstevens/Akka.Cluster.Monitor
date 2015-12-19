using Akka.Actor;
using Akka.Cluster;
using Akka.Event;

namespace Shared
{
    /// <summary>
    /// Actor responsible for processing commands
    /// </summary>
    public class ClusterHelper : ReceiveActor
    {
        public class RemoveMember
        {
        }
        public class MemberDown
        {
            public MemberDown(string host, int? port, string protocol, string system)
            {
                Host = host;
                Port = port;
                Protocol = protocol;
                System = system;
            }

            public string Host { get; private set; }
            public int? Port { get; private set; }
            public string Protocol { get; private set; }
            public string System { get; private set; }
        }
        public class MemberLeave
        {
            public MemberLeave(string host, int? port, string protocol, string system)
            {
                Host = host;
                Port = port;
                Protocol = protocol;
                System = system;
            }

            public string Host { get; private set; }
            public int? Port { get; private set; }
            public string Protocol { get; private set; }
            public string System { get; private set; }
        }

        private readonly ILoggingAdapter _logger = Context.GetLogger();
        protected Cluster Cluster = Cluster.Get(Context.System);
        
        public ClusterHelper()
        {
            Ready();
        }

        protected override void PostStop()
        {
            Cluster.Unsubscribe(Self);
        }

        private void Ready()
        {
            Receive<RemoveMember>(mem =>
            {
                _logger.Info("Removing Member: {0}", Cluster.SelfAddress);
                Cluster.Leave(Cluster.SelfAddress);
            });

            Receive<MemberDown>(address =>
            {
                Address add = new Address(address.Protocol, address.System, address.Host, address.Port);
                _logger.Info("Forcing the Member down: {0}", add.ToString());
                Cluster.Down(add);
            });

            Receive<MemberLeave>(address =>
            {
                Address add = new Address(address.Protocol, address.System, address.Host, address.Port);
                _logger.Info("Forcing Member to leave cluster: {0}", add.ToString());
                Cluster.Leave(add);
            });
        }

    }
}