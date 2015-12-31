using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Shared.States;

namespace Shared.Actors
{
    public class JobsToRunData : ReceiveActor, IWithUnboundedStash
    {
        public class Cancel
        {
        }

        public class Finished
        {
            public Finished(IReadOnlyList<JobInfo> jobs, Exception exception)
            {
                Jobs = jobs;
                Exception = exception;
            }

            public IReadOnlyList<JobInfo> Jobs { get; private set; }
            public Exception Exception { get; private set; }
        }

        public class RecievedJobs
        {
            public RecievedJobs(IReadOnlyList<JobInfo> jobs, IActorRef requestor)
            {
                Requestor = requestor;
                Jobs = jobs;
            }

            public IReadOnlyList<JobInfo> Jobs { get; private set; }
            public IActorRef Requestor { get; private set; }

        }

        public class GetJobsToRun { }
        private readonly ILoggingAdapter _logger;
        private readonly IActorRef _jobTaskerActorRef;
        private CancellationTokenSource _cancel;
        public IStash Stash { get; set; }

        public JobsToRunData(IActorRef jobTaskerActor)
        {
            _cancel = new CancellationTokenSource();
            _logger = Context.GetLogger();
            _jobTaskerActorRef = jobTaskerActor;
            Become(Ready);
        }

        private void Ready()
        {
            Receive<GetJobsToRun>(process =>
            {
                var self = Self; // closure
                
                Task.Run(() =>
                {
                    var jobList = new List<JobInfo>();

                    try
                    {
                        var jobCount = 10;

                        for (int i = 0; i < jobCount; i++)
                        {
                            var jobName = new JobInfo("MyPetMonkey" + i);
                            jobList.Add(jobName);
                        }

                        for (int i = 0; i < jobCount; i++)
                        {
                            var jobName = new JobInfo("MonsterTruck" + i);
                            jobList.Add(jobName);
                        }

                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "GetJobsToRun");
                    }

                    return jobList;

                }, _cancel.Token).ContinueWith(x =>
                {
                    if (x.IsCanceled || x.IsFaulted)
                        return new Finished(new List<JobInfo>(), x.Exception);

                    _logger.Info("Starting {0} Jobs.", x.Result.Count());
                    return new Finished(x.Result, null);
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(self);

                // switch behavior
                Become(Working);

            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! JobsToRunData.Ready.ReceiveAny: \r\n{0}", task);
            });

        }

        private void Working()
        {
            Receive<Cancel>(cancel =>
            {
                _cancel.Cancel(); // cancel work
                BecomeReady();
            });
            Receive<Finished>(f =>
            {
                if (f.Exception != null)
                {
                    _logger.Error(f.Exception, f.Exception.Message);
                }

                _jobTaskerActorRef.Tell(new RecievedJobs(f.Jobs, Self), Self);
                BecomeReady();
            });
            ReceiveAny(o => Stash.Stash());
        }

        private void BecomeReady()
        {
            _cancel = new CancellationTokenSource();
            Stash.UnstashAll();
            Become(Ready);
        }
    }
}
