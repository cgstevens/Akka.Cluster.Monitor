using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Shared.Commands;
using Shared.Helpers;
using Shared.States;

namespace Shared.Actors
{
    public class JobMaster : ReceiveActor, IWithUnboundedStash
    {
        public class TellJobSubscribersTheStatus
        {
        }

        public const string JobCoordinatorRouterName = "jobcoordinator";
        public IStash Stash { get; set; }
        protected IActorRef JobCoordinatorRouter;
        protected IActorRef JobTaskerRef;
        protected ConcurrentDictionary<string, IActorRef> JobSubscribers = new ConcurrentDictionary<string, IActorRef>();
        protected IStartJob JobToStart;
        private readonly ILoggingAdapter _logger = Context.GetLogger();
        private ConcurrentDictionary<Job, JobWorkerInfo> CurrentJobs = new ConcurrentDictionary<Job, JobWorkerInfo>();
        protected ICancelable JobStatusTeller;

        public JobMaster()
        {
            Ready();
        }

        protected override void PreStart()
        {
            JobTaskerRef = Context.ActorOf(Props.Create(() => new JobTasker(Self)), ActorPaths.JobTaskerActor.Name);

            if (Context.Child(JobCoordinatorRouterName).Equals(ActorRefs.Nobody))
            {
                JobCoordinatorRouter =
                    Context.ActorOf(
                        Props.Create(() => new JobCoordinator())
                            .WithRouter(FromConfig.Instance), JobCoordinatorRouterName);
            }
            else //in the event of a restart
            {
                JobCoordinatorRouter = Context.Child(JobCoordinatorRouterName);
            }

            JobStatusTeller = Context.System.Scheduler.ScheduleTellRepeatedlyCancelable(TimeSpan.FromMilliseconds(20),
                    TimeSpan.FromSeconds(1), Self, new TellJobSubscribersTheStatus(), Self);
        }


        protected override void PreRestart(Exception reason, object message)
        {
            /* Don't kill the children */
            PostStop();
        }

        private void BecomeReady()
        {
            JobToStart = null;
            Become(Ready);
            Stash.UnstashAll();
        }

        private void SearchingForJob()
        {
            //not able to start more jobs right now
            Receive<IStartJob>(s =>
            {
                Stash.Stash();
            });

            Receive<JobFound>(ic => ic.JobWorker == null, ic =>
            {
                if (ic.Job.Equals(JobToStart.Job))
                {
                    JobWorkerInfo workerInfo;
                    if (CurrentJobs.TryGetValue(ic.Job, out workerInfo))
                    {
                        workerInfo.JobStatusUpdate.Status = ic.JobStatus;
                        CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
                    }

                    _logger.Error("{0} : Could not create worker", ic.Job.JobInfo.Id);
                    BecomeReady();
                }
                else
                {
                    _logger.Error("JobFound Worker Null: {0} is running but we are passing in {1}", JobToStart.Job.JobInfo.Id, ic.Job.JobInfo.Id);
                }
            });

            Receive<JobFound>(ic =>
            {
                if (ic.Job.Equals(JobToStart.Job))
                {
                    _logger.Info("{0} : JobFound : {1}", JobToStart.Job.JobInfo.Id, ic.JobWorker.Path.ToString());


                    JobWorkerInfo workerInfo;
                    if (CurrentJobs.TryGetValue(JobToStart.Job, out workerInfo))
                    {
                        workerInfo.JobStatusUpdate.Status = ic.JobStatus;
                        workerInfo.SetWorkerRef(ic.JobWorker);
                        CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
                    }

                    Context.Watch(ic.JobWorker);
                    ic.JobWorker.Tell(JobToStart);
                    BecomeReady();
                }
                else
                {
                    _logger.Error("JobFound: {0} is running but we are passing in {1}", JobToStart.Job.JobInfo.Id, ic.Job.JobInfo.Id);
                }
            });

            Receive<FindAvailableWorkers>(ic => ic.NodeCount == 0, ic =>
            {
                if (ic.StartJob.Job.Equals(JobToStart.Job))
                {
                    _logger.Error("{0} : No Routes found.", ic.StartJob.Job.JobInfo.Id);
                    JobWorkerInfo workerInfo;
                    if (CurrentJobs.TryGetValue(ic.StartJob.Job, out workerInfo))
                    {
                        workerInfo.JobStatusUpdate.Status = JobStatus.NoRoutes;
                        CurrentJobs.AddOrUpdate(ic.StartJob.Job, workerInfo, (key, oldvalue) => workerInfo);
                    }
                }
                else
                {
                    // This shouldn't happen.
                    _logger.Error("FindAvailableWorkers NULL: {0} is running but we are passing in {1}", JobToStart.Job.JobInfo.Id, ic.StartJob.Job.JobInfo.Id);
                }
                BecomeReady();
            });
            
            Receive<FindAvailableWorkers>(ic =>
            {
                if (ic.StartJob.Job.Equals(JobToStart.Job))
                {
                    JobWorkerInfo workerInfo;
                    if (CurrentJobs.TryGetValue(ic.StartJob.Job, out workerInfo))
                    {
                        workerInfo.JobStatusUpdate.Status = JobStatus.FindAvailableWorkers;
                        CurrentJobs.AddOrUpdate(ic.StartJob.Job, workerInfo, (key, oldvalue) => workerInfo);
                    }

                    //JobCoordinatorRouter.Tell(new JobCoordinator.GetJobWorker(JobToStart.Job, Self));
                    JobCoordinatorRouter.Ask(new JobCoordinator.GetJobWorker(JobToStart.Job, Self), TimeSpan.FromSeconds(15))
                    .ContinueWith(
                        tr =>
                        {
                            if (tr.IsFaulted)
                            {
                                _logger.Error(tr.Exception, "{0} : Faulted GetJobWorker", JobToStart.Job.JobInfo.Id);
                                return new JobFound(JobToStart.Job, null, JobStatus.Terminated);
                            }
                            if (tr.IsCanceled)
                            {
                                _logger.Error(tr.Exception, "{0} : Canceled GetJobWorker; Did we timeout trying to get a worker?", JobToStart.Job.JobInfo.Id);
                                return new JobFound(JobToStart.Job, null, JobStatus.Canceled);
                            }

                            JobCoordinator.ReturnJobWorkerReference workerRef = (JobCoordinator.ReturnJobWorkerReference)tr.Result;

                            if (!ic.StartJob.Job.Equals(JobToStart.Job))
                            {
                                _logger.Error("{0} : Wrong Worker!!! {1} : {2}", JobToStart.Job.JobInfo.Id, ic.StartJob.Job, workerRef.JobWorker.Path);
                            }

                            _logger.Info("{0} : ReturnJobWorkerReference : {1}", JobToStart.Job.JobInfo.Id, workerRef.JobWorker.Path);
                            return new JobFound(JobToStart.Job, workerRef.JobWorker, JobStatus.WorkerFound);
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);

                }
                else
                {
                    // The only way this happens is if we are GetJobWorker and 1 or all workers have died which caused us to be
                    _logger.Error("FindAvailableWorkers: {0} is running but we are passing in {1}", JobToStart.Job.JobInfo.Id, ic.StartJob.Job.JobInfo.Id);
                }
            });

            Receive<JobStatusUpdate>(ic =>
            {
                JobWorkerInfo workerInfo;
                if (CurrentJobs.TryGetValue(ic.Job, out workerInfo))
                {
                    workerInfo.JobStatusUpdate = ic;
                    CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
                }
            });

            Receive<Terminated>(terminated =>
            {
                RemoveTerminatedReferences(terminated);
            });

            Receive<JobWorker.JobComplete>(ic =>
            {
                HandleJobComplete(ic);
            });

            Receive<JobWorker.ReceivedJobStatus>(ic =>
            {
                HandleReceivedJobStatus(ic);
            });

            Receive<TellJobSubscribersTheStatus>(ic =>
            {
                HandleTellJobSubscribersTheStatus();
            });

            Receive<ISubscribeToAllJobs>(ic =>
            {
                _logger.Info("{0} : Subscribing to all jobs", Sender);
                HandleSubscribingToJobs(ic.Requestor);
            });

            Receive<IUnSubscribeToAllJobs>(ic =>
            {
                _logger.Info("{0} : UnSubscribing to all jobs", Sender);
                HandleUnSubscribingToJobs(ic.Requestor);
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobMaster.SearchingForJob.ReceiveAny: \r\n{0}", task);
            });
        }

        private void HandleReceivedJobStatus(JobWorker.ReceivedJobStatus ic)
        {
            JobWorkerInfo workerInfo;
            if (CurrentJobs.TryGetValue(ic.Job, out workerInfo))
            {
                workerInfo.JobStatusUpdate = ic.RunningStatus;

                // If we are finished then we shouldn't have a worker reference.
                if (workerInfo.JobStatusUpdate.Status == JobStatus.Finished)
                {
                    workerInfo.SetWorkerRef(null);
                    _logger.Warning("{0} : Removing JobWorkerRef as the status is Finished.  This might be due to not receiving the JobComplete message???", ic.Job.JobInfo.Id);
                }

                CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
            }
        }

        private void HandleUnSubscribingToJobs(IActorRef requestor)
        {
            Context.Unwatch(requestor);
            IActorRef actor;
            JobSubscribers.TryRemove(requestor.Path.ToString(), out actor);
            requestor.Tell(new SubscriptionStatus(false, "You successfully unsubscribed to all jobs."));
        }

        private void RemoveTerminatedReferences(Terminated terminated)
        {
            var currentJobs = CurrentJobs.ToList();

            if (currentJobs.Any(x => x.Value.JobWorkerRef != null && x.Value.JobWorkerRef.Equals(terminated.ActorRef)))
            {
                var jobKey = CurrentJobs.First(x => x.Value.JobWorkerRef != null && x.Value.JobWorkerRef.Equals(terminated.ActorRef)).Key;
                _logger.Error("{0} : Remote connection [{1}] died", jobKey.JobInfo.Id, terminated.ActorRef);

                JobWorkerInfo workerInfo;
                if (CurrentJobs.TryGetValue(jobKey, out workerInfo))
                {
                    workerInfo.SetWorkerRef(null);
                    workerInfo.JobStatusUpdate.Status = JobStatus.Terminated;
                    CurrentJobs.AddOrUpdate(jobKey, workerInfo, (key, oldvalue) => workerInfo);
                }
            }
            else if (JobSubscribers.ContainsKey(terminated.ActorRef.Path.ToString()))
            {
                Context.Unwatch(terminated.ActorRef);
                IActorRef actor;
                JobSubscribers.TryRemove(terminated.ActorRef.Path.ToString(), out actor);
                _logger.Error("JobSuscriber: Remote connection [{0}] died", actor);
            }
            else
            {
                _logger.Error("Remote connection [{0}] died", terminated.ActorRef);
            }

        }

        private void HandleTellJobSubscribersTheStatus()
        {
            var subscribers = JobSubscribers.ToList();
            var jobs = CurrentJobs.ToList();


            foreach (var jobSubscriber in subscribers)
            {
                if (jobs.Count == 0)
                {
                    jobSubscriber.Value.Tell(new JobWorkerInfo(new Job(new JobInfo("No Job Found"))));
                }
                else
                {
                    foreach (var jobWorkerInfo in jobs)
                    {
                        jobSubscriber.Value.Tell(jobWorkerInfo.Value.DeepClone());
                    }
                }
            }

        }

        private void HandleSubscribingToJobs(IActorRef sender)
        {
            if (!JobSubscribers.ContainsKey(sender.Path.ToString()))
            {
                if (JobSubscribers.TryAdd(sender.Path.ToString(), sender))
                {
                    Context.Watch(sender);
                    sender.Tell(new SubscriptionStatus(true, "You successfully subscribed to all jobs."));
                }
            }
            else
            {
                sender.Tell(new SubscriptionStatus(true, "You are already subscribed to all jobs."));
            }
        }

        private void Ready()
        {
            Receive<IStartJob>(start =>
            {
                _logger.Info("{0} : StartJob", start.Job.JobInfo.Id);

                JobToStart = start;
                if (!CurrentJobs.ContainsKey(start.Job))
                {
                    CurrentJobs.TryAdd(start.Job,
                        new JobWorkerInfo(start.Job)
                        {
                            JobStatusUpdate = new JobStatusUpdate(start.Job) { Status = JobStatus.New }
                        });

                    // Start Search
                    JobCoordinatorRouter.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                    {
                        if (tr.IsCanceled || tr.IsFaulted)
                        {
                            _logger.Error(tr.Exception, "{0} : JobCoordinatorRouter was {1} ", start.Job.JobInfo.Id);
                            return new FindAvailableWorkers(0, start);
                        }

                        return new FindAvailableWorkers(tr.Result.Members.Count(), start);
                    }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
                    Become(SearchingForJob);
                }
                else
                {
                    JobWorkerInfo workerInfo;
                    CurrentJobs.TryGetValue(start.Job, out workerInfo);

                    if (workerInfo.JobWorkerRef != null)
                    {
                        // If we have a JobWorkerRef and the job has started or is running then subscribe and check status.
                        if (workerInfo.JobStatusUpdate.Status == JobStatus.Starting ||
                            workerInfo.JobStatusUpdate.Status == JobStatus.Running ||
                            workerInfo.JobStatusUpdate.Status == JobStatus.GetItemToProcess ||
                            workerInfo.JobStatusUpdate.Status == JobStatus.ProcessingItem)
                        {
                            workerInfo.JobWorkerRef.Tell(new SubscribeToJob(start.Job, start.Requestor), Self);
                            workerInfo.JobWorkerRef.Tell(new JobWorker.CheckJobStatus(), Self);
                        }

                        // If we have a JobWorkerRef and the status is WorkerFound... 
                        // The actor must still exists becuase we are watching it and if something is wrong with it then it would timeout and be terminated.
                        // Possible reason is the JobWorker did not recieve the IStartJob message.  
                        if (workerInfo.JobStatusUpdate.Status == JobStatus.WorkerFound)
                        {
                            workerInfo.JobWorkerRef.Tell(start.Job);
                        }
                    }
                    else
                    {
                        // Start Search
                        workerInfo.PreviousJobStatusUpdate.Insert(0, workerInfo.JobStatusUpdate);
                        workerInfo.JobStatusUpdate = new JobStatusUpdate(start.Job) { Status = JobStatus.New };

                        if (workerInfo.PreviousJobStatusUpdate.Count == 11)
                        {
                            workerInfo.PreviousJobStatusUpdate.Remove(workerInfo.PreviousJobStatusUpdate.Last());
                        }
                        
                        CurrentJobs.AddOrUpdate(start.Job, workerInfo, (key, oldvalue) => workerInfo);

                        JobCoordinatorRouter.Ask<Routees>(new GetRoutees()).ContinueWith(tr =>
                        {
                            if (tr.IsFaulted)
                            {
                                _logger.Error(tr.Exception, "{0} : JobCoordinatorRouter was Faulted ", start.Job.JobInfo.Id);
                                return new FindAvailableWorkers(0, start);
                            }
                            if (tr.IsCanceled)
                            {
                                _logger.Error(tr.Exception, "{0} : JobCoordinatorRouter was Canceled ", start.Job.JobInfo.Id);
                                return new FindAvailableWorkers(0, start);
                            }

                            return new FindAvailableWorkers(tr.Result.Members.Count(), start);
                        }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously).PipeTo(Self);
                        Become(SearchingForJob);
                    }
                }
            });
            
            Receive<TellJobSubscribersTheStatus>(ic =>
            {
                HandleTellJobSubscribersTheStatus();
            });

            Receive<JobWorker.ReceivedJobStatus>(ic =>
            {
                HandleReceivedJobStatus(ic);
            });

            Receive<Terminated>(terminated =>
            {
                RemoveTerminatedReferences(terminated);
            });

            Receive<JobStatusUpdate>(ic =>
            {
                JobWorkerInfo workerInfo;
                CurrentJobs.TryGetValue(ic.Job, out workerInfo);
                if (workerInfo != null)
                {
                    workerInfo.JobStatusUpdate = ic;
                    CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
                }

            });

            Receive<JobWorker.JobComplete>(ic =>
            {
                HandleJobComplete(ic);
            });

            Receive<ISubscribeToAllJobs>(ic =>
            {
                _logger.Info("{0} : Subscribing to all jobs", Sender);
                HandleSubscribingToJobs(ic.Requestor);
            });

            Receive<IUnSubscribeToAllJobs>(ic =>
            {
                _logger.Info("{0} : UnSubscribing to all jobs", Sender);
                HandleUnSubscribingToJobs(ic.Requestor);
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobMaster.Ready.ReceiveAny: \r\n{0}", task);
            });
        }

        private void HandleJobComplete(JobWorker.JobComplete ic)
        {
            _logger.Info("{0} : JobComplete : {1}", ic.Job.JobInfo.Id, ic.JobStatusUpdate.TotalElapsed);
            JobWorkerInfo workerInfo;
            CurrentJobs.TryGetValue(ic.Job, out workerInfo);

            if (workerInfo != null)
            {
                if (workerInfo.JobWorkerRef != null)
                {
                    Context.Unwatch(workerInfo.JobWorkerRef);
                }

                workerInfo.JobStatusUpdate = ic.JobStatusUpdate;
                workerInfo.SetWorkerRef(null);
                CurrentJobs.AddOrUpdate(ic.Job, workerInfo, (key, oldvalue) => workerInfo);
            }

        }

        #region Messages
        public class FindAvailableWorkers
        {
            public FindAvailableWorkers(int nodeCount, IStartJob startJob)
            {
                NodeCount = nodeCount;
                StartJob = startJob;
            }

            public int NodeCount { get; private set; }
            public IStartJob StartJob { get; private set; }
        }

        public class JobFound
        {
            public JobFound(Job job, IActorRef jobWorker, JobStatus jobStatus)
            {
                JobWorker = jobWorker;
                Job = job;
                JobStatus = jobStatus;
            }

            public Job Job { get; private set; }

            public IActorRef JobWorker { get; private set; }

            public JobStatus JobStatus { get; private set; }
        }

        #endregion

    }
}