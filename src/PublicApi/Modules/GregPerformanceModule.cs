using gregCore.Infrastructure.Performance;

namespace gregCore.PublicApi.Modules;

public sealed class GregPerformanceModule
{
    private readonly GregPerformanceGovernor _governor;
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;

    internal GregPerformanceModule(GregApiContext ctx, GregPerformanceGovernor governor)
    {
        _governor = governor;
        _logger = ctx.Logger.ForContext(nameof(GregPerformanceModule));
        _bus = ctx.EventBus;
    }

    // ── Profile Management ───────────────────────────────────────────────────
    public void Configure(PerformanceProfile profile) => _governor.Configure(profile);
    public void UseProfile(PerformanceProfile profile) => _governor.Configure(profile);
    public PerformanceStats GetStats() => _governor.GetStats();
    public ResourceSnapshot GetResourceSnapshot() => _governor.GetStats().Resources;

    public PerformanceProfile Balanced => PerformanceProfile.Balanced;
    public PerformanceProfile HighPerformance => PerformanceProfile.HighPerformance;
    public PerformanceProfile LowEnd => PerformanceProfile.LowEnd;
    public PerformanceProfile DatacenterOptimal => PerformanceProfile.DatacenterOptimal;

    // ── Frame-Cap ────────────────────────────────────────────────────────────
    public void SetTargetFPS(int fps) => Configure(GetProfile() with { TargetFps = fps });
    public void SetUnfocusedFPS(int fps) => Configure(GetProfile() with { UnfocusedFps = fps });
    public void SetVSync(bool enabled) => Configure(GetProfile() with { EnableVSync = enabled });
    public void ThrottleWhenUnfocused(bool enabled, int unfocusedFps = 15) => Configure(GetProfile() with { ThrottleWhenUnfocused = enabled, UnfocusedFps = unfocusedFps });

    // ── Memory ───────────────────────────────────────────────────────────────
    public void SetGCInterval(int seconds) => Configure(GetProfile() with { GcIntervalSeconds = seconds });
    public void SetStreamingMipmaps(bool enabled, float budgetMb = 512f) => Configure(GetProfile() with { EnableStreamingMipmaps = enabled, StreamingMipmapBudgetMb = budgetMb });
    public void SetTextureResolution(int level) => Configure(GetProfile() with { TextureResolution = level });

    // ── Graphics / Quality ───────────────────────────────────────────────────
    public void SetQualityLevel(int level) => Configure(GetProfile() with { QualityLevel = level });
    public void SetShadowDistance(float distance) => Configure(GetProfile() with { ShadowDistance = distance });
    public void SetCameraFarClip(float distance) => Configure(GetProfile() with { CameraFarClip = distance });
    public void SetLodBias(float bias) => Configure(GetProfile() with { LodBias = bias });

    // ── HDRP Post-Process ────────────────────────────────────────────────────
    public void SetSSAO(bool enabled) => Configure(GetProfile() with { DisableSSAO = !enabled });
    public void SetContactShadows(bool enabled) => Configure(GetProfile() with { DisableContactShadows = !enabled });
    public void SetGlobalIllumination(bool enabled) => Configure(GetProfile() with { DisableGlobalIllumination = !enabled });
    public void SetSSR(bool enabled) => Configure(GetProfile() with { DisableSSR = !enabled });
    public void SetVolumetricFog(bool enabled) => Configure(GetProfile() with { DisableVolumetricFog = !enabled });

    // ── HDRP Pipeline Asset ──────────────────────────────────────────────────
    public void SetMaxShadowRequests(int count) => Configure(GetProfile() with { MaxShadowRequests = count });
    public void SetDecals(bool enabled) => Configure(GetProfile() with { DisableDecals = !enabled });

    // ── Upscaling ────────────────────────────────────────────────────────────
    public void SetUpscaling(bool enabled, int dlssMode = 0, int fsrMode = 0) => Configure(GetProfile() with { ForceUpscaling = enabled, DLSSQualityMode = dlssMode, FSRQualityMode = fsrMode });
    public void SetDLSSMode(int mode) => Configure(GetProfile() with { DLSSQualityMode = mode });
    public void SetFSRMode(int mode) => Configure(GetProfile() with { FSRQualityMode = mode });

    // ── Simulation ───────────────────────────────────────────────────────────
    public void SetRouteEvalCooldown(float seconds) => Configure(GetProfile() with { RouteEvalCooldownSeconds = seconds });
    public void SetAutoSaveInterval(float minutes) => Configure(GetProfile() with { AutoSaveIntervalMinutes = minutes });

    // ── Operations ───────────────────────────────────────────────────────────
    public Task<T> QueueOperation<T>(string name, Func<Task<T>> operation, OperationPriority priority = OperationPriority.Normal)
        => _governor.QueueOperationAsync(name, operation, priority);

    public void SetMaxConcurrentOperations(int max) => Configure(GetProfile() with { MaxConcurrentOps = max });

    // ── Events ───────────────────────────────────────────────────────────────
    public event Action<ResourceSnapshot>? OnResourceUpdate
    {
        add => _bus.Subscribe("greg.performance.ResourceSnapshot", p => value?.Invoke(_governor.GetStats().Resources));
        remove => _bus.Unsubscribe("greg.performance.ResourceSnapshot", p => value?.Invoke(_governor.GetStats().Resources));
    }

    private PerformanceProfile GetProfile() => _governor.GetStats().Profile;
}
