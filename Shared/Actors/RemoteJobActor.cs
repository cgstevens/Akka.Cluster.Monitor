using Akka.Actor;
using Shared.Commands;

namespace Shared.Actors
{
    /// <summary>
    /// Remote-deployed actor designed to help forward jobs to the remote hosts
    /// </summary>
    public class RemoteJobActor : ReceiveActor
    {
        public RemoteJobActor()
        {
            Receive<IStartJob>(start =>
            {
                Context.ActorSelection(ActorPaths.JobMasterActor.Path).Tell(start, Sender);
            });

            Receive<SubscribeToAllJobs>(start =>
            {
                Context.ActorSelection(ActorPaths.JobMasterActor.Path).Tell(Sender);
            });

            Receive<UnSubscribeToAllJobs>(start =>
            {
                Context.ActorSelection(ActorPaths.JobMasterActor.Path).Tell(Sender);
            });
        }
    }
}
