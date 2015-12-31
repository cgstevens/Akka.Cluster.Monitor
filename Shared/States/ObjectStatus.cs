using Akka.Actor;

namespace Shared.States
{
    public class ObjectStatus
    {
        public bool IsComplete { get; private set; }
        public IActorRef CompletedBy { get; private set; }
        
        public void MarkAsComplete(IActorRef completedBy)
        {
            IsComplete = true;
            CompletedBy = completedBy;
        }
    }
}
