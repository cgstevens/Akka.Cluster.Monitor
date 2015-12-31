using Akka.Actor;
using Shared.States;

namespace Shared.Commands
{
    /// <summary>
    /// Launch a new <see cref="Job"/>
    /// </summary>
    public class StartJob : IStartJob
    {
        public StartJob(Job job, IActorRef requestor)
        {
            Requestor = requestor;
            Job = job;
        }

        public Job Job { get; private set; }
        public IActorRef Requestor { get; private set; }
        public object ConsistentHashKey { get { return Job.JobInfo; } }
    }

    /// <summary>
    /// Kill a running <see cref="Job"/>
    /// </summary>
    public class StopJob
    {
        public StopJob(Job job, JobStats jobStats, IActorRef requestor)
        {
            Requestor = requestor;
            Job = job;
            JobStats = jobStats;
        }

        public Job Job { get; private set; }
        public IActorRef Requestor { get; private set; }
        public JobStats JobStats { get; private set; }
    }
}