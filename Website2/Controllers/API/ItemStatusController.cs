using System.Web.Http;
using Akka.Actor;
using Microsoft.AspNet.SignalR;
using Website2.Actors;
using Website2.Hubs;

namespace Website2.Controllers.API
{
    public class ItemStatusController : ApiController
    {
        IHubContext _hubContext;

        public ItemStatusController()
        {
            _hubContext = GlobalHost.ConnectionManager.GetHubContext<ItemStatusHub>();
        }
        
        [HttpPost]
        public void SubscribeToWorkers(string connectionId)
        {
            SystemActors.SignalRItemStatusActor.Tell(new SignalRItemStatusActor.Subscribe(connectionId), ActorRefs.Nobody);
        }

        [HttpPost]
        public void UnsubscribeToWorkers(string connectionId)
        {
            SystemActors.SignalRItemStatusActor.Tell(new SignalRItemStatusActor.UnSubscribe(connectionId), ActorRefs.Nobody);
        }
        
    }
}
