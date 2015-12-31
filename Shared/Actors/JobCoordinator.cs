using Akka.Actor;
using Akka.Event;
using Shared.States;

namespace Shared.Actors
{
    public class JobCoordinator : ReceiveActor, IWithUnboundedStash
    {
        public class GetJobWorker
        {
            public GetJobWorker(Job job, IActorRef jobTrackingMaster)
            {
                Job = job;
                JobTrackingMaster = jobTrackingMaster;
            }

            public Job Job { get; private set; }
            public IActorRef JobTrackingMaster { get; private set; }
        }

        public class ReturnJobWorkerReference
        {
            public ReturnJobWorkerReference(IActorRef jobWorker)
            {
                JobWorker = jobWorker;
            }

            public IActorRef JobWorker { get; private set; }
        }
        private readonly ILoggingAdapter _logger;
        public IStash Stash { get; set; }

        public JobCoordinator()
        {
            _logger = Context.GetLogger();
            Become(Ready);
        }

        private void Ready()
        {
            Receive<GetJobWorker>(ic =>
            {
                IActorRef jobWorker;
                if (Context.Child(ic.Job.JobInfo.Id).Equals(ActorRefs.Nobody))
                {
                    jobWorker = Context.ActorOf(Props.Create(() => new JobWorker(ic.Job, ic.JobTrackingMaster))
                        , ic.Job.JobInfo.Id);
                }
                else
                {
                    jobWorker = Context.Child(ic.Job.JobInfo.Id);
                }

                Sender.Tell(new ReturnJobWorkerReference(jobWorker));
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobCoordinator.Ready.ReceiveAny: \r\n{0}", task);
            });
        }
    }
}
