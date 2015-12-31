using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Akka.Actor;
using Akka.Event;
using Shared.Commands;
using Shared.Helpers;
using Shared.States;

namespace Shared.Actors
{
    public class JobWorker : ReceiveActor, IWithUnboundedStash
    {
        #region Messages

        public class JobCanStart
        {
            public JobCanStart(Job job)
            {
                Job = job;
            }

            public Job Job { get; private set; }
        }

        public class CheckJobStatus
        {
        }

        public class ReceivedJobStatus
        {
            public ReceivedJobStatus(Job job, JobStatusUpdate runningStatus)
            {
                Job = job;
                RunningStatus = runningStatus;
            }
            public Job Job { get; private set; }
            public JobStatusUpdate RunningStatus { get; private set; }
        }

        public class JobComplete
        {
            public JobComplete(Job job, JobStatusUpdate jobStatusUpdate)
            {
                Job = job;
                JobStatusUpdate = jobStatusUpdate;
            }

            public Job Job { get; private set; }
            public JobStatusUpdate JobStatusUpdate { get; private set; }
        }

        #endregion

        public const string JobTracker = "tracker";
        public const string CoordinatorRouterName = "workercoordinator";
        protected readonly Job Job;
        protected HashSet<IActorRef> Subscribers = new HashSet<IActorRef>();
        protected JobStatusUpdate RunningStatus;
        protected JobStats TotalStats
        {
            get { return RunningStatus.Stats; }
            set { RunningStatus.Stats = value; }
        }

        protected IActorRef CoordinatorRouter;
        protected IActorRef WorkerTracker;
        protected IActorRef JobTrackingMaster;
        private readonly ILoggingAdapter _logger;

        public IStash Stash { get; set; }

        public JobWorker(Job job, IActorRef jobTrackingMaster)
        {
            _logger = Context.GetLogger();
            Job = job;
            RunningStatus = new JobStatusUpdate(Job);
            TotalStats = new JobStats(Job);
            JobTrackingMaster = jobTrackingMaster;

            // Make the JobTrackingMaster a default subscriber so that it receives all updates.
            Subscribers.Add(JobTrackingMaster);

            BecomeReady();
        }

        protected override void PreStart()
        {

        }

        private void BecomeReady()
        {
            // does JobTracker exist already?
            if (Context.Child(JobTracker).Equals(ActorRefs.Nobody))
            {
                WorkerTracker = Context.ActorOf(Props.Create(() => new WorkerTracker()),
                    JobTracker);
            }
            else
            {
                WorkerTracker = Context.Child(JobTracker);
            }


            if (Context.Child(CoordinatorRouterName).Equals(ActorRefs.Nobody))
            {
                CoordinatorRouter =
                    Context.ActorOf(
                        Props.Create(() => new WorkerCoordinator(Job, Self, WorkerTracker))
                            , CoordinatorRouterName);
            }
            else //in the event of a restart
            {
                CoordinatorRouter = Context.Child(CoordinatorRouterName);
            }
            Become(Ready);
        }

        private void Ready()
        {
            // kick off the job
            Receive<IStartJob>(start =>
            {
                _logger.Info("JobWorker.Ready.IStartJob");

                // Need to reset tracking buckets.
                WorkerTracker.Tell(new WorkerTracker.ResetTrackerBuckets());
                RunningStatus = new JobStatusUpdate(Job) { Status = JobStatus.Starting };
                TotalStats = new JobStats(Job);
                RunningStatus.Stats = TotalStats;

                if (!Subscribers.Contains(start.Requestor))
                {
                    Subscribers.Add(start.Requestor);
                }

                PublishJobStatus();

                Self.Tell(new JobCanStart(start.Job));
            });

            Receive<JobCanStart>(start =>
            {
                RunningStatus.Status = JobStatus.Running;

                CoordinatorRouter.Tell(new WorkerCoordinator.GetJobData(start.Job.JobInfo));

                Become(Started);
                Stash.UnstashAll();
            });

            Receive<JobCanStart>(start =>
            {
                _logger.Warning("Can't start job yet. No routees.");
            });


            Receive<CheckJobStatus>(start =>
            {
                Sender.Tell(new ReceivedJobStatus(Job, RunningStatus), Self);
            });

            Receive<ReceiveTimeout>(ic =>
            {
                _logger.Error("JobWorker.Ready.ReceiveTimeout: \r\n{0}", ic);
            });

            Receive<ISubscribeToJob>(subscribe =>
            {
                Stash.Stash();
            });

            ReceiveAny(o =>
            {
                _logger.Error("JobWorker.Ready.ReceiveAny and stashing: \r\n{0}", o);
                Stash.Stash();
            });

        }

        private void Started()
        {
            Receive<IStartJob>(start =>
            {
                //treat the additional StartJob like a subscription
                if (start.Job.Equals(Job) && !Subscribers.Contains(start.Requestor))
                    Subscribers.Add(start.Requestor);
            });

            Receive<ISubscribeToJob>(subscribe =>
            {
                if (subscribe.Job.Equals(Job) && !Subscribers.Contains(subscribe.Subscriber))
                    Subscribers.Add(subscribe.Subscriber);
            });

            Receive<IUnsubscribeFromJob>(unsubscribe =>
            {
                if (unsubscribe.Job.Equals(Job))
                    Subscribers.Remove(unsubscribe.Subscriber);
            });

            Receive<JobStats>(stats =>
            {
                TotalStats = stats.DeepClone();
                PublishJobStatus();
            });

            Receive<JobStatus>(ic =>
            {
                RunningStatus.Status = ic;
            });

            Receive<StopJob>(stop =>
            {
                _logger.Info("JobWorker.Started.StopJob");
                TotalStats = stop.JobStats;
                EndJob(JobStatus.Finished);
            });

            Receive<ReceiveTimeout>(timeout =>
            {
                _logger.Warning("JobWorker.Started.ReceiveTimeout: job could be working OR something happened OR an operation is taking longer than expected.: \r\n{0}", timeout);
            });

            Receive<CheckJobStatus>(start =>
            {
                Sender.Tell(new ReceivedJobStatus(Job, RunningStatus));
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobWorker.Started.ReceiveAny: \r\n{0}", task);
            });
        }

        private void EndJob(JobStatus finalStatus)
        {
            RunningStatus.Status = finalStatus;
            RunningStatus.EndTime = DateTime.UtcNow;

            // Since message can come out of order I want to make sure that this message is last.
            // For now going to delay by a 125 milliseconds.
            Thread.Sleep(125);

            PublishJobStatus();

            JobTrackingMaster.Tell(new JobComplete(Job, RunningStatus));
            Become(Ready);
            Subscribers.Clear();
            Subscribers.Add(JobTrackingMaster); // Add the Master back after clearing.
        }


        private void PublishJobStatus()
        {
            RunningStatus.Stats = TotalStats;
            RunningStatus.CurrentTime = DateTime.UtcNow;
            foreach (var sub in Subscribers.ToList())
            {
                sub.Tell(RunningStatus.DeepClone(), Self);
            }
        }
    }
}
