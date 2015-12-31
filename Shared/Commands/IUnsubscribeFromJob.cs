using Akka.Actor;
using Shared.States;

namespace Shared.Commands
{
    public interface IUnsubscribeFromJob
    {
        Job Job { get; }
        IActorRef Subscriber { get; }
    }
}