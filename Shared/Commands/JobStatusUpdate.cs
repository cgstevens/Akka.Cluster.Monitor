using System;
using Shared.States;

namespace Shared.Commands
{
    public enum JobStatus
    {
        Running,
        Starting,
        Finished,
        New,
        Terminated,
        FindAvailableWorkers,
        IsFaulted,
        WorkerFound,
        FailedToStart,
        NoRoutes,
        Canceled,
        GetItemToProcess,
        ProcessingItem,
        GetJobData,
        ProcessingItems,
        RecievedDataItems
    }

    [Serializable]
    public class JobStatusUpdate : IStatusUpdate
    {
        public JobStatusUpdate() { }

        public JobStatusUpdate(Job job)
        {
            Job = job;
            StartTime = DateTime.UtcNow;
            CurrentTime = DateTime.UtcNow;
            Status = JobStatus.New;
        }

        public Job Job { get; private set; }

        public JobStats Stats { get; set; }

        public DateTime StartTime { get; private set; }

        public DateTime CurrentTime { get; set; }

        public DateTime? EndTime { get; set; }
        
        public TimeSpan TotalElapsed
        {
            get
            {
                return ((EndTime ?? DateTime.UtcNow) - StartTime);
            }
        }
        
        public JobStatus Status { get; set; }

        public string StatusToString
        {
            get { return Status.ToString(); }
        }
    }
}
