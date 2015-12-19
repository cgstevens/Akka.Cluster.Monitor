using Akka.Actor;

namespace Website.Actors
{
    /// <summary>
    /// Static class used to work around weird SignalR constructors
    /// 
    /// (need to learn how to wire this up properly in signalr)
    /// </summary>
    public static class SystemActors
    {
        public static IActorRef SignalRClusterStatusActor = ActorRefs.Nobody;

        public static IActorRef ClusterHelper = ActorRefs.Nobody;

        public static IActorRef ClusterStatus = ActorRefs.Nobody;
    }
}