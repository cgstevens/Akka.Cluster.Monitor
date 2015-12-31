using Microsoft.AspNet.SignalR;
using Shared.Commands;
using Shared.States;
using Website.Actors;

namespace Website.Hubs
{
    public class ItemStatusHub : Hub
    {
        public void PushSubscriptionStatus(SubscriptionStatus subscriptionStatus)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ItemStatusHub>();
            dynamic allClients = context.Clients.All.broadcastSubscriptionStatus(subscriptionStatus);
        }

        public void PushJobWorkerInfo(JobWorkerInfo jobWorkerInfo)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ItemStatusHub>();
            dynamic allClients = context.Clients.All.broadcastJobWorkerInfo(jobWorkerInfo.Job, jobWorkerInfo.JobStatusUpdate, jobWorkerInfo.JobWorkerRefToString != null ? jobWorkerInfo.JobWorkerRefToString : "No Worker", jobWorkerInfo.PreviousJobStatusUpdate);
        }

        public void ClusterRoutes(SignalRItemStatusActor.ClusterRoutes clusterRoutes)
        {
            var context = GlobalHost.ConnectionManager.GetHubContext<ItemStatusHub>();
            dynamic allClients = context.Clients.All.broadcastClusterRoutes(clusterRoutes);
        }
    }
}