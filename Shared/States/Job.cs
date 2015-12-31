using System;

namespace Shared.States
{
    [Serializable]
    public class Job : IEquatable<Job>
    {
        public Job(JobInfo jobInfo)
        {
            JobInfo = jobInfo;
        }

        public JobInfo JobInfo { get; private set; }

        #region Equality

        public bool Equals(Job other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(JobInfo.Id, other.JobInfo.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Job)obj);
        }

        public override int GetHashCode()
        {
            return (JobInfo.Id != null ? JobInfo.Id.GetHashCode() : 0);
        }

        #endregion

        public override string ToString()
        {
            return JobInfo.Id;
        }
    }

    
}
