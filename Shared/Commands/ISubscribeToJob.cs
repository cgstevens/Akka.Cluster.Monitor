using Akka.Actor;
using Shared.States;

namespace Shared.Commands
{
    public interface ISubscribeToJob
    {
        Job Job { get; }
        IActorRef Subscriber { get; }
    }
}