using System;
using Shared.States;

namespace Shared.Commands
{
    public interface IStatusUpdate
    {
        Job Job { get; }
        JobStats Stats { get; set; }
        DateTime StartTime { get; }
        DateTime? EndTime { get; set; }
        TimeSpan TotalElapsed { get; }
        JobStatus Status { get; set; }
    }
}