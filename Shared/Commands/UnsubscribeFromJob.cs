using Akka.Actor;
using Shared.States;

namespace Shared.Commands
{
    /// <summary>
    /// Unsuscribe an actor from a given <see cref="Job"/>
    /// </summary>
    public class UnsubscribeFromJob : IUnsubscribeFromJob
    {
        public UnsubscribeFromJob(Job job, IActorRef subscriber)
        {
            Subscriber = subscriber;
            Job = job;
        }

        public Job Job { get; private set; }
        public IActorRef Subscriber { get; private set; }
    }

    public class SubscriptionStatus
    {
        public SubscriptionStatus(bool isSubscribed, string message)
        {
            IsSubscribed = isSubscribed;
            Message = message;
        }

        public bool IsSubscribed { get; private set; }
        public string Message { get; private set; }
    }
}