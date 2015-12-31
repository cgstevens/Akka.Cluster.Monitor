using System.Collections.Generic;
using System.Linq;
using System.Net;
using Akka.Actor;
using Akka.Event;
using Akka.Routing;
using Shared.Commands;
using Shared.Helpers;
using Shared.States;

namespace Shared.Actors
{
    public class WorkerCoordinator : ReceiveActor
    {
        public class GetJobData
        {
            public GetJobData(JobInfo jobInfo)
            {
                JobInfo = jobInfo;
            }

            public JobInfo JobInfo { get; private set; }
        }

        public class RecievedDataItems
        {
            public RecievedDataItems(IReadOnlyList<Item> items)
            {
                Items = items;
            }

            public IReadOnlyList<Item> Items { get; private set; }

        }

        public class ProcessItems
        {
            public ProcessItems(IReadOnlyList<Item> items)
            {
                Items = items;
            }

            public IReadOnlyList<Item> Items { get; private set; }
        }


        public class ErrorRecievingItems
        {
            public ErrorRecievingItems(HttpStatusCode statusCode, string message)
            {
                StatusCode = statusCode;
                Message = message;
            }

            public HttpStatusCode StatusCode { get; private set; }

            public string Message { get; private set; }
        }

        #region Constants

        public const string GetItemData = "getitemdata";
        public const string ItemWorker = "itemworker";

        #endregion

        protected IActorRef GetItemDataRef;
        protected readonly IActorRef WorkerTracker;
        protected readonly IActorRef JobWorker;
        protected IActorRef ItemWorkerRouter;
        protected Job Job;
        protected JobStats Stats;
        private readonly ILoggingAdapter _logger = Context.GetLogger();

        public WorkerCoordinator(Job job, IActorRef jobWorker, IActorRef workerTracker)
        {
            Job = job;
            WorkerTracker = workerTracker;
            JobWorker = jobWorker;
            Stats = new JobStats(Job);
            Become(ReadyToStart);
        }

        protected override void PreStart()
        {
            // Create our RqHubData actor
            if (Context.Child(GetItemData).Equals(ActorRefs.Nobody))
            {
                GetItemDataRef = Context.ActorOf(
                    Props.Create(() => new ItemDataWorker(Job, Self)), GetItemData);
            }

            // Create our weighted share actor
            if (Context.Child(ItemWorker).Equals(ActorRefs.Nobody))
            {
                ItemWorkerRouter = Context.ActorOf(
                    Props.Create(() => new ItemWorker(Self)).WithRouter(FromConfig.Instance),//.WithRouter(new RoundRobinPool(25)),
                    ItemWorker);
            }
        }

        private void ReadyToStart()
        {
            Receive<GetJobData>(ic =>
            {
                Stats = new JobStats(Job);
                JobWorker.Tell(Stats.DeepClone(), Self);
                JobWorker.Tell(JobStatus.GetJobData, Self);
                Become(GetItemsToProcess);
                GetItemDataRef.Tell(new GetJobData(Job.JobInfo), Self);
            });
            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! WorkerCoordinator.ReadyToStart.ReceiveAny: \r\n{0}", task);
            });
        }
        
        private void GetItemsToProcess()
        {
            Receive<RecievedDataItems>(ic => !ic.Items.Any(), ic =>
            {
                JobWorker.Tell(JobStatus.RecievedDataItems, Self);
                _logger.Info("No items found so no work to do right now; Work Complete.");
                StopJob();
            });

            Receive<RecievedDataItems>(ic =>
            {
                JobWorker.Tell(JobStatus.RecievedDataItems, Self);
                _logger.Info("Ready to process {0} items", ic.Items.Count());
                WorkerTracker.Tell(new RecievedDataItems(ic.Items.ToList()), Self);
                JobWorker.Tell(Stats.DeepClone());

                BecomeProcessingItems();
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! WorkerCoordinator.GetInvoiceToProcess.ReceiveAny: \r\n{0}", task);
            });
        }

        private void BecomeProcessingItems()
        {
            JobWorker.Tell(JobStatus.ProcessingItems, Self);
            JobWorker.Tell(Stats.DeepClone(), Self);
            Become(ProcessingItems);
        }

        private void ProcessingItems()
        {
            //Received word from the WorkerTracker that we need to process invoices
            Receive<ProcessItems>(process =>
            {
                Stats = Stats.WithItemsDiscovered(process.Items.Count);
                JobWorker.Tell(Stats.DeepClone(), Self);

                foreach (var item in process.Items)
                {
                    //hand the work off to the weightedshare workers
                    ItemWorkerRouter.Tell(new ItemWorker.ProcessItemResult(item.Copy()), Self);
                }
                
            });

            Receive<WorkerTracker.AllItemsCompleted>(ic =>
            {
                _logger.Info("{0} Items processed.", ic.Items.Count);
                
                StopJob();
            });

            Receive<WorkerTracker.CompletedItem>(completed =>
            {
                Stats = Stats.WithItemCompleted();
                WorkerTracker.Tell(completed, Self);
                JobWorker.Tell(Stats.DeepClone(), Self);
            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! WorkerCoordinator.ProcessingWeightedShare.ReceiveAny: \r\n{0}", task);
            });
        }
        
        private void StopJob()
        {
            JobWorker.Tell(new StopJob(Job, Stats.DeepClone(), Self));

            Stats = Stats.Reset();
            Become(ReadyToStart);
        }
    }
}
