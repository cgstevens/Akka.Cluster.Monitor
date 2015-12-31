using Akka.Actor;
using Akka.Routing;
using Shared.States;

namespace Shared.Commands
{
    public interface IStartJob : IConsistentHashable
    {
        Job Job { get; }
        IActorRef Requestor { get; }
    }
}