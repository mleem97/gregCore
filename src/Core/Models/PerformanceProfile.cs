namespace gregCore.Core.Models;

public sealed record PerformanceProfile
{
    // Frame-Cap
    public int TargetFps { get; init; } = 60;
    public int UnfocusedFps { get; init; } = 15;
    public int LoadingFps { get; init; } = 30;
    public bool EnableVSync { get; init; } = true;
    public bool ThrottleWhenUnfocused { get; init; } = true;

    // Concurrency
    public int MaxConcurrentOps { get; init; } = 3;
    public int MaxConcurrentRequests { get; init; } = 4;
    public int MaxEventsPerFrame { get; init; } = 20;

    // Memory
    public int RamWarningMb { get; init; } = 3072;
    public int RamCriticalMb { get; init; } = 4096;
    public int GcIntervalSeconds { get; init; } = 30;
    public bool EnableStreamingMipmaps { get; init; } = true;
    public float StreamingMipmapBudgetMb { get; init; } = 512f;

    // Graphics / Quality
    public int QualityLevel { get; init; } = 2;
    public float ShadowDistance { get; init; } = 50f;
    public int TextureResolution { get; init; } = 0; // 0=full, 1=half, 2=quarter
    public float CameraFarClip { get; init; } = 1000f;
    public float LodBias { get; init; } = 1.0f;

    // HDRP Post-Process
    public bool DisableSSAO { get; init; } = false;
    public bool DisableContactShadows { get; init; } = false;
    public bool DisableGlobalIllumination { get; init; } = false;
    public bool DisableSSR { get; init; } = false;
    public bool DisableVolumetricFog { get; init; } = false;

    // HDRP Pipeline Asset
    public int MaxShadowRequests { get; init; } = 512;
    public bool DisableDecals { get; init; } = false;

    // Upscaling
    public bool ForceUpscaling { get; init; } = false;
    public int DLSSQualityMode { get; init; } = 0; // 0=Perf, 1=Balanced, 2=Quality
    public int FSRQualityMode { get; init; } = 0;

    // Simulation
    public float RouteEvalCooldownSeconds { get; init; } = 2.0f;
    public float AutoSaveIntervalMinutes { get; init; } = 10.0f;

    public static PerformanceProfile Balanced => new PerformanceProfile();
    public static PerformanceProfile HighPerformance => new PerformanceProfile
    {
        TargetFps = 144,
        UnfocusedFps = 30,
        MaxConcurrentOps = 8,
        EnableVSync = false,
        QualityLevel = 4,
        GcIntervalSeconds = 60,
        CameraFarClip = 500f,
        LodBias = 1.5f,
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
        CameraFarClip = 80f,
        LodBias = 0.4f,
        DisableSSAO = true,
        DisableContactShadows = true,
        DisableGlobalIllumination = true,
        DisableSSR = true,
        MaxShadowRequests = 64,
        DisableDecals = true,
    };
    public static PerformanceProfile DatacenterOptimal => new PerformanceProfile
    {
        TargetFps = 60,
        UnfocusedFps = 10,
        ThrottleWhenUnfocused = true,
        EnableVSync = false,
        QualityLevel = 2,
        ShadowDistance = 20f,
        TextureResolution = 1,
        CameraFarClip = 80f,
        LodBias = 0.4f,
        DisableSSAO = true,
        DisableContactShadows = true,
        DisableGlobalIllumination = true,
        DisableSSR = true,
        DisableVolumetricFog = false,
        MaxShadowRequests = 64,
        DisableDecals = false,
        ForceUpscaling = true,
        DLSSQualityMode = 0,
        FSRQualityMode = 0,
        GcIntervalSeconds = 300,
        EnableStreamingMipmaps = true,
        StreamingMipmapBudgetMb = 512f,
        RouteEvalCooldownSeconds = 2.0f,
        AutoSaveIntervalMinutes = 10.0f,
    };
}
