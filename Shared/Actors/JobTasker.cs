using System;
using System.Configuration;
using Akka.Actor;
using Akka.Event;
using Shared.Commands;
using Shared.States;

namespace Shared.Actors
{
    public class JobTasker : ReceiveActor, IWithUnboundedStash
    {
        public const string GetJobsToRunData = "getjobstorundata";
        private readonly ILoggingAdapter _logger;
        private IActorRef _jobsToRunActor;
        private readonly IActorRef _jobMaster;
        private ICancelable _jobsToRunTeller;
        public IStash Stash { get; set; }
        public int ScheduleDelay { get; set; }

        public JobTasker(IActorRef jobMaster)
        {
            ScheduleDelay = Convert.ToInt32(ConfigurationManager.AppSettings["JobTaskerScheduleDelay"]);
            _jobMaster = jobMaster;
            _logger = Context.GetLogger();
            BecomeReady();
        }

        protected override void PostStop()
        {
            _jobsToRunTeller.Cancel();
            base.PostStop();
        }

        protected override void PreStart()
        {
            _jobsToRunActor = Context.ActorOf(
                    Props.Create(() => new JobsToRunData(Self)),
                    GetJobsToRunData);
        }

        private void BecomeReady()
        {
            _jobsToRunTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(5), Self, new JobsToRunData.GetJobsToRun(), Self);
            Become(Ready);
        }

        public void Ready()
        {
            Receive<JobsToRunData.GetJobsToRun>(ic =>
            {
                _jobsToRunActor.Tell(new JobsToRunData.GetJobsToRun(), Self);
                Become(Working);
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobTasker.Ready.ReceiveAny: \r\n{0}", task);
            });
        }
        
        private void Working()
        {
            Receive<JobsToRunData.RecievedJobs>(f =>
            {
                foreach (var job in f.Jobs)
                {
                    // Keep telling to start the job... if the job is already running then it will just subscribe to the current working job.
                    _jobMaster.Tell(new StartJob(new Job(job), _jobMaster), Self);
                }

                Become(Ready);
            });
        }
    }
}