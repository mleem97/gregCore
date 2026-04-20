namespace gregCore.Core.Models;

public sealed record ThrottleMetrics
{
    public int TotalQueued { get; init; }
    public int TotalCompleted { get; init; }
    public int CurrentActive { get; init; }
    public int MaxConcurrent { get; init; }
    public int QueueDepth { get; init; }
}
