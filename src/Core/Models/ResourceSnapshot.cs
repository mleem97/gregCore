namespace gregCore.Core.Models;

public sealed record ResourceSnapshot
{
    public DateTime TimestampUtc { get; init; } = DateTime.UtcNow;
    
    // .NET Managed Heap & System RAM
    public int RamUsedMb { get; init; }
    public int PrivateMemoryMb { get; init; }
    public int GcTotalMemoryMb { get; init; }
    
    // Unity Engine Memory (Native)
    public int UnityAllocatedMb { get; init; }
    public int UnityReservedMb { get; init; }
    public int UnityUnusedMb { get; init; }
    
    // System Metrics
    public int GcGen0Collections { get; init; }
    public int GcGen1Collections { get; init; }
    public int GcGen2Collections { get; init; }
    public int ThreadCount { get; init; }
    
    public string Summary => $"RAM:{RamUsedMb}MB Unity:{UnityAllocatedMb}MB GC:{GcTotalMemoryMb}MB Threads:{ThreadCount}";
}
