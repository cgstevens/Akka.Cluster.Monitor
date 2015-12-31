using System;

namespace Shared.States
{
    [Serializable]
    public class JobStats
    {
        public JobStats(Job key)
        {
            Key = key;
        }

        public Job Key { get; private set; }

        public int TotalItemsDiscovered { get; private set; }
        public int TotalItemsProcessed { get; private set; }
      
        /// <summary>
        /// Deep copy funtion
        /// </summary>
        private JobStats Copy(int? itemsDiscovered = null, int? itemsProcessed = null)
        {
            return new JobStats(Key)
            {
                TotalItemsDiscovered = itemsDiscovered ?? TotalItemsDiscovered,
                TotalItemsProcessed = itemsProcessed ?? TotalItemsProcessed
            };
        }
        
        public JobStats WithItemCompleted()
        {
            return Copy(itemsProcessed: TotalItemsProcessed + 1);
        }

        public JobStats WithItemsDiscovered(int count)
        {
            return Copy(itemsDiscovered: count);
        }
        
        public JobStats Reset()
        {
            return new JobStats(Key);
        }

        public override string ToString()
        {
            return
                string.Format(
                    "Items Discovered: {0} -- Processed {1}",
                    TotalItemsDiscovered, TotalItemsProcessed);
        }
    }
}