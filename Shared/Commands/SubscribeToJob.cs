using Akka.Actor;
using Shared.States;

namespace Shared.Commands
{
    /// <summary>
    /// Subscribe an actor to a given <see cref="Job"/>
    /// </summary>
    public class SubscribeToJob : ISubscribeToJob
    {
        public SubscribeToJob(Job job, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Job = job;
        }

        public Job Job { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }

    public class SubscribeToAllJobs : ISubscribeToAllJobs
    {
        public SubscribeToAllJobs(IActorRef requestor)
        {
            Requestor = requestor;
        }

        public IActorRef Requestor { get; private set; }
        public object ConsistentHashKey { get { return Requestor; } }
    }

    public class UnSubscribeToAllJobs : IUnSubscribeToAllJobs
    {
        public UnSubscribeToAllJobs(IActorRef requestor)
        {
            Requestor = requestor;
        }

        public IActorRef Requestor { get; private set; }
        public object ConsistentHashKey { get { return Requestor; } }
    }
}
