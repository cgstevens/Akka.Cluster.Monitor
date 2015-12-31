using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Routing;
using Shared.Actors;
using Shared.Commands;
using Shared.States;

namespace Website.Actors
{
    /// <summary>
    /// Actor responsible for processing commands
    /// </summary>
    public class CommandProcessor : ReceiveActor
    {
        #region Messages

        public class AttemptJobStart
        {
        }

        public class AttemptSubscription
        {
        }

        public class AttemptUnSubscribe
        {
        }

        public class StartJobRoutes
        {
            public StartJobRoutes(int nodeCount)
            {
                NodeCount = nodeCount;
            }
            public int NodeCount { get; private set; }
        }
        
        #endregion

        protected readonly IActorRef CommandRouter;

        public CommandProcessor()
        {
            CommandRouter = Context.ActorOf(Props.Create(() => new RemoteJobActor()).WithRouter(FromConfig.Instance), "tasker");
            Receives();
        }

        private void Receives()
        {
            Receive<AttemptJobStart>(attempt =>
            {
                
                CommandRouter.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                {
                    var route = new StartJobRoutes(tr.Result.Members.Count());
                    return route;

                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self, Sender);
            });

            Receive<StartJobRoutes>(ic => ic.NodeCount == 0, ic =>
            {
                Sender.Tell(ic);
            });

            Receive<StartJobRoutes>(ic =>
            {
                Sender.Tell(ic);
                

                // TODO:  Need to create a task to get the markts and to run them all.
                JobInfo job = new JobInfo("RunMe");
                var startJob = new StartJob(new Job(job), Sender);

                CommandRouter.Tell(startJob);

                JobInfo job2 = new JobInfo("MyPetMonkey1");
                var startJob2 = new StartJob(new Job(job2), Sender);

                CommandRouter.Tell(startJob2);

                JobInfo job3 = new JobInfo("AkkaNet");
                var startJob3 = new StartJob(new Job(job3), Sender);

                CommandRouter.Tell(startJob3);
            });

            Receive<AttemptSubscription>(attempt =>
            {
                CommandRouter.Tell(new SubscribeToAllJobs(Sender), Self);
                CommandRouter.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                {
                    var message = string.Empty;

                    if (!tr.Result.Members.Any())
                    {
                        message = "Could not find item tasker.  Is the Tasker service running?";
                    }
                    else
                    {
                        message = "Item tasker is running.";
                    }
                    
                    return new SignalRItemStatusActor.ClusterRoutes(message);
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Sender);
                Sender.Tell(new SubscribeToAllJobs(Sender), Self);
            });

            Receive<AttemptUnSubscribe>(attempt =>
            {
                CommandRouter.Tell(new UnSubscribeToAllJobs(Sender), Self);
                CommandRouter.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                {
                    var grrr =
                        new SignalRItemStatusActor.ClusterRoutes(string.Format("{0} has {1} routees: {2}", CommandRouter,
                            tr.Result.Members.Count(),
                            string.Join(",",
                                tr.Result.Members.Select(
                                    y => y.ToString()))));

                    return grrr;
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Sender);
                Sender.Tell(new UnSubscribeToAllJobs(Sender), Self);
            });
        }
    }
}