using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Cluster;

namespace Shared
{
    public class ClusterRoleLeader
    {
        public ClusterRoleLeader(string role, Address address)
        {
            Role = role;
            Address = address;
        }
        public string Role { get; private set; }
        public Address Address { get; private set; }
    }

    public class CurrentClusterStatus
    {
        public CurrentClusterStatus(List<ClusterRoleLeader> clusterRoleLeaders, Address currentClusterAddress, ClusterEvent.CurrentClusterState clusterState)
        {
            ClusterRoleLeaders = clusterRoleLeaders;
            CurrentClusterAddress = currentClusterAddress;
            ClusterState = clusterState;
        }
        public List<ClusterRoleLeader> ClusterRoleLeaders { get; private set; }
        public Address CurrentClusterAddress { get; private set; }
        public ClusterEvent.CurrentClusterState ClusterState { get; private set; }
    }
}
