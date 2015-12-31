using System;
using System.Collections.Generic;

namespace Shared.States
{
    [Serializable]
    public class JobInfo : IEquatable<JobInfo>
    {
        public JobInfo(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public bool Equals(JobInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((JobInfo)obj);
        }

        public override int GetHashCode()
        {
            return (Id != null ? Id.GetHashCode() : 0);
        }

        private sealed class JobInfoEqualityComparer : IEqualityComparer<JobInfo>
        {
            public bool Equals(JobInfo x, JobInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x.Id, y.Id);
            }

            public int GetHashCode(JobInfo obj)
            {
                return (obj.Id.GetHashCode());
            }
        }

        private static readonly IEqualityComparer<JobInfo> JobInfoComparerInstance = new JobInfoEqualityComparer();

        public static IEqualityComparer<JobInfo> JobInfoComparer
        {
            get { return JobInfoComparerInstance; }
        }
    }
}
