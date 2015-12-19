namespace Akka.Cluster.Monitor.Messages
{
    public class Messages
    {
        public class Initialize
        {
            public Initialize(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        public class Start
        {
        }

        public class Subscribe
        {
        }

        public class UnSubscribe
        {
        }
        public class AppLeaveCluster
        {
        }

        public class CurrentClusterState
        {
        }
        public class AppJoinCluster
        {
        }

        public class AppDownCluster
        {
        }
        

        public class AskMemberToLeaveCluster
        {
            public AskMemberToLeaveCluster(string address)
            {
                Address = address;
            }
            public string Address { get; private set; }
        }
        public class DownMember
        {
            public DownMember(string address)
            {
                Address = address;
            }
            public string Address { get; private set; }
        }
        

    }
}
