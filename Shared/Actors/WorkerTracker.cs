using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Event;
using Shared.States;

namespace Shared.Actors
{
    public class WorkerTracker : ReceiveActor
    {
        public class ResetTrackerBuckets
        {
        }
        public class AllItemsCompleted
        {
            public AllItemsCompleted(IList<Item> items)
            {
                Items = items;
            }

            public IList<Item> Items { get; private set; }
        }

        public class CompletedItem
        {
            public CompletedItem(Item item, IActorRef completedBy)
            {
                Item = item;
                CompletedBy = completedBy;
            }

            public Item Item { get; private set; }
            public IActorRef CompletedBy { get; private set; }
        }

        public class RecievedItems
        {
            public RecievedItems(IList<Item> items )
            {
                Items = items;
            }

            public IList<Item> Items { get; private set; }
        }

        private Dictionary<Item, ObjectStatus> _currentItemRecords;
        private ILoggingAdapter _logger;

        public WorkerTracker()
            : this(new Dictionary<Item, ObjectStatus>())
        {

        }

        public WorkerTracker(Dictionary<Item, ObjectStatus> currentItems)
        {
            _logger = Context.GetLogger();
            _currentItemRecords = currentItems;
            InitialReceives();
        }

        private void InitialReceives()
        {
            Receive<ResetTrackerBuckets>(ic =>
            {
                _currentItemRecords = new Dictionary<Item, ObjectStatus>();
            });

            Receive<WorkerCoordinator.RecievedDataItems>(check =>
            {
                foreach (var item in check.Items)
                {
                    if (!_currentItemRecords.ContainsKey(item))
                    {
                        _currentItemRecords.Add(item, new ObjectStatus());
                    }
                }

                Sender.Tell(new WorkerCoordinator.ProcessItems(_currentItemRecords.Keys.ToList()), Self);
            });

            Receive<CompletedItem>(ic =>
            {
                _currentItemRecords[ic.Item].MarkAsComplete(ic.CompletedBy);
                if (_currentItemRecords.Count(x => x.Value.IsComplete) == _currentItemRecords.Count)
                {
                    Sender.Tell(new AllItemsCompleted(_currentItemRecords.Keys.ToList()), Self);
                }
            });
            
        }
    }
}