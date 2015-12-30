using System.Collections.Generic;
using Microsoft.AspNet.SignalR;
using Website.Actors;

namespace Website.Hubs
{
    public class ServiceStatusHub : Hub
    {
        private List<ClusterServiceStatusActor.ServiceModel> _services;
        
        public void PushServiceClusterStatus(List<ClusterServiceStatusActor.ServiceModel> services, string hostName)
        {
            _services = services;
            var context = GlobalHost.ConnectionManager.GetHubContext<ServiceStatusHub>();
            dynamic allClients = context.Clients.All.broadcastServiceStatus(_services, hostName);
        }
    }
}