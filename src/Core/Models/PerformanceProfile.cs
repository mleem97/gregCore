namespace gregCore.Core.Models;

public sealed record PerformanceProfile
{
    public int TargetFps { get; init; } = 60;
    public int UnfocusedFps { get; init; } = 15;
    public int LoadingFps { get; init; } = 30;
    public int MaxConcurrentOps { get; init; } = 3;
    public int MaxConcurrentRequests { get; init; } = 4;
    public int MaxEventsPerFrame { get; init; } = 20;
    public int RamWarningMb { get; init; } = 3072;
    public int RamCriticalMb { get; init; } = 4096;
    public int GcIntervalSeconds { get; init; } = 30;
    public bool EnableVSync { get; init; } = true;
    public int QualityLevel { get; init; } = 2;
    public float ShadowDistance { get; init; } = 50f;
    public int TextureResolution { get; init; } = 0;

    public static PerformanceProfile Balanced => new PerformanceProfile();
    public static PerformanceProfile HighPerformance => new PerformanceProfile
    {
        TargetFps = 144,
        UnfocusedFps = 30,
        MaxConcurrentOps = 8,
        EnableVSync = false,
        QualityLevel = 4,
        GcIntervalSeconds = 60,
    };
    public static PerformanceProfile LowEnd => new PerformanceProfile
    {
        TargetFps = 30,
        UnfocusedFps = 10,
        MaxConcurrentOps = 1,
        MaxEventsPerFrame = 10,
        RamWarningMb = 2048,
        RamCriticalMb = 3072,
        EnableVSync = true,
        QualityLevel = 0,
        ShadowDistance = 20f,
        TextureResolution = 2,
        GcIntervalSeconds = 15,
    };
}