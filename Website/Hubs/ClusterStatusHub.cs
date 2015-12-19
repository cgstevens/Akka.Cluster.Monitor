using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Cluster;
using Microsoft.AspNet.SignalR;

namespace Website.Hubs
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

    public class ClusterStateHub : Hub
    {
        public void PushClusterState(ClusterEvent.CurrentClusterState clusterState, Address currentClusterAddress)
        {
            // Push ClusterState x to subscribed client.
            // So if client A subscibes to ClusterState A then it should only get A and not B.
            // TODO:
            
            List<ClusterRoleLeader> clusterRoleLeaders = new List<ClusterRoleLeader>();
            foreach (var member in clusterState.Members)
            {
                var role = member.Roles.First();
                var address = clusterState.RoleLeader(role);
                clusterRoleLeaders.Add(new ClusterRoleLeader(role, address));
            }

            var context = GlobalHost.ConnectionManager.GetHubContext<ClusterStateHub>();
            dynamic allClients = context.Clients.All.broadcastClusterState(clusterState, clusterRoleLeaders, currentClusterAddress);
        }
    }
}