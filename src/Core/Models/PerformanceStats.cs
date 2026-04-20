namespace gregCore.Core.Models;

public sealed record PerformanceStats
{
    public PerformanceProfile Profile { get; init; } = new PerformanceProfile();
    public ResourceSnapshot Resources { get; init; } = new ResourceSnapshot();
    public ThrottleMetrics Throttle { get; init; } = new ThrottleMetrics();
    public int QueueDepth { get; init; }
}
