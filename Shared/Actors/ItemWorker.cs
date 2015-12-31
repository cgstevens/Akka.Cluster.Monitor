using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using Shared.States;

namespace Shared.Actors
{
    public class ItemWorker : ReceiveActor, IWithUnboundedStash
    {
        public class Cancel
        {
        }

        public class ProcessItemResult
        {
            public ProcessItemResult(Item item)
            {
                Item = item;
            }

            public Item Item { get; private set; }
        }

        public class ProcessItemCompleted
        {
            public ProcessItemCompleted(Exception exception)
            {
                Exception = exception;
            }

            public Exception Exception { get; private set; }
        }

        private readonly ILoggingAdapter _logger;
        protected readonly IActorRef CoordinatorActor;
        public IStash Stash { get; set; }
        private CancellationTokenSource _cancel;

        private Item _currentWorkingItem;

        public ItemWorker(IActorRef coordinatorActor)
        {
            _cancel = new CancellationTokenSource();
            _logger = Context.GetLogger();
            CoordinatorActor = coordinatorActor;
            Become(Ready);
        }

        private void Ready()
        {
            Receive<ProcessItemResult>(ic =>
            {
                var self = Self; // closure
                _currentWorkingItem = ic.Item;
                Task.Run(() =>
                {
                    // ... work
                    HandleItem(_currentWorkingItem);

                }, _cancel.Token).ContinueWith(x =>
                {
                    if (x.IsCanceled || x.IsFaulted)
                        return new ProcessItemCompleted(x.Exception);

                    return new ProcessItemCompleted(null);
                }, TaskContinuationOptions.AttachedToParent & TaskContinuationOptions.ExecuteSynchronously)
                .PipeTo(self);

                // switch behavior
                Become(Working);

            });
            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! WeightShareWorker.Ready.ReceiveAny: \r\n{0}", task);
            });

        }

        private void Working()
        {
            Receive<Cancel>(cancel =>
            {
                _cancel.Cancel(); // cancel work
                BecomeReady();
            });
            Receive<ProcessItemCompleted>(ic =>
            {
                if (ic.Exception != null)
                {
                    _logger.Error(ic.Exception, ic.Exception.Message);
                }

                CoordinatorActor.Tell(new WorkerTracker.CompletedItem(_currentWorkingItem.Copy(), Self));
                BecomeReady();
            });
            Receive<ProcessItemResult>(ic =>
            {
                Stash.Stash();
            });
            ReceiveAny(task =>
            {
                _logger.Error(" [x] Oh Snap! Working.ReceiveAny: \r\n{0}", task);
            });

        }

        // Reset any state objects before we are ready to accept a new task.
        private void BecomeReady()
        {
            _currentWorkingItem = new Item(-1);
            _cancel = new CancellationTokenSource();
            Stash.UnstashAll();
            Become(Ready);
        }

        private void HandleItem(Item item)
        {
            _logger.Info("Working ItemId:{0};", item.Id);
            var watch = Stopwatch.StartNew();

            try
            {
                // TODO: Do your work here
                var rnd = new Random().Next(125, 2750);
                Thread.Sleep(rnd);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error processing ItemId:{0};", item.Id);
            }

            watch.Stop();
            var elapsed = watch.Elapsed;
            _logger.Info("ItemId:{0} took {1:c}", item.Id, elapsed );

        }
    }
}
