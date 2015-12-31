using System;
using System.Collections.Generic;
using Akka.Actor;
using Shared.Commands;

namespace Shared.States
{
    [Serializable]
    public class JobWorkerInfo
    {
        public JobWorkerInfo(Job job)
        {
            Job = job;
            PreviousJobStatusUpdate = new List<JobStatusUpdate>();
        }
        public Job Job { get; set; }

        public string JobWorkerRefToString { get; set; }

        [NonSerialized()]
        public IActorRef JobWorkerRef;// { get; set; }

        public JobStatusUpdate JobStatusUpdate { get; set; }
        public List<JobStatusUpdate> PreviousJobStatusUpdate { get; set; }
        
        public void SetWorkerRef(IActorRef workerRef)
        {
            JobWorkerRef = workerRef;
            JobWorkerRefToString = workerRef != null ? workerRef.Path.ToString() : null;
        }
    }
}
