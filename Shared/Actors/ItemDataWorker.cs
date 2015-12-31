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
    public class ItemDataWorker : ReceiveActor, IWithUnboundedStash
    {
        public class Cancel
        {
        }
        public class GetItems
        {
        }
        

        public class Finished
        {
            public Finished(IReadOnlyList<Item> items, Exception exception)
            {
                Items = items;
                Exception = exception;
            }

            public IReadOnlyList<Item> Items { get; private set; }
            public Exception Exception { get; private set; }
        }

        private readonly Job _job;
        private readonly ILoggingAdapter _logger;
        protected readonly IActorRef WorkerCoordinatorActorRef;

        private CancellationTokenSource _cancel;
        public IStash Stash { get; set; }

        public ItemDataWorker(Job job, IActorRef workerCoordinatorActor)
        {
            _job = job;
            _cancel = new CancellationTokenSource();
            _logger = Context.GetLogger();
            WorkerCoordinatorActorRef = workerCoordinatorActor;
            Become(Ready);
        }

        private void Ready()
        {
            Receive<WorkerCoordinator.GetJobData>(process =>
            {
                var self = Self; // closure

                Task.Run(() =>
                {
                    // ... work
                    _logger.Info("Get Items");

                    List<Item> items = new List<Item>();

                    try
                    {
                        var rnd = new Random().Next(-250, 250);
                        for (int i = 0; i < rnd; i++)
                        {
                            items.Add(new Item(i));
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "GetItems");
                    }
                    
                    return items;

                }, _cancel.Token).ContinueWith(x =>
                {
                    if (x.IsCanceled || x.IsFaulted)
                        return new Finished(null, x.Exception);

                    return new Finished(x.Result, null);
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(self);

                // switch behavior
                Become(Working);

            });

            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! GetItemData.Ready.ReceiveAny: \r\n{0}", task);
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
                
                WorkerCoordinatorActorRef.Tell(new WorkerCoordinator.RecievedDataItems(f.Items.ToList()));

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
