using gregCore.PublicApi;

namespace gregCore.Infrastructure.Performance;

public sealed class GregPerformanceGovernor : IGregPerformanceGovernor, IDisposable
{
    private readonly GregFrameRateLimiter _fpsLimiter;
    private readonly GregRequestThrottler _throttler;
    private readonly GregResourceMonitor _monitor;
    private readonly GregMemoryPressureHandler _memHandler;
    private readonly GregOperationQueue _queue;
    private readonly IGregLogger _logger;
    private PerformanceProfile _profile;
    private int _eventsThisFrame;

    internal GregPerformanceGovernor(GregApiContext ctx, PerformanceProfile? profile = null)
    {
        _logger = ctx.Logger.ForContext(nameof(GregPerformanceGovernor));
        _profile = profile ?? PerformanceProfile.Balanced;
        
        _fpsLimiter = new GregFrameRateLimiter(ctx.Logger, _profile);
        _throttler = new GregRequestThrottler(ctx.Logger, _profile);
        _monitor = new GregResourceMonitor(ctx.Logger, ctx.EventBus, _profile);
        _memHandler = new GregMemoryPressureHandler(ctx.Logger, ctx.EventBus, _profile);
        _queue = new GregOperationQueue(_throttler, ctx.Logger, _profile.MaxQueuedOperations);
        
        // Performance-Patches initialisieren (Throttle, Cleanup, etc.)
        GregPerformancePatches.Initialize();
        ApplyPatchSettings(_profile);
        
        _monitor.Start(5000);
        _logger.Info($"[Governor] Initialisiert mit Prefix-Architektur + Performance-Patches.");
    }

    public void OnUpdate()
    {
        _fpsLimiter.OnUpdate();
        _memHandler.OnUpdate();
        _monitor.CacheUnityMemoryStats();
        _eventsThisFrame = 0;
    }

    public bool CanDispatchEvent()
    {
        if (_eventsThisFrame >= _profile.MaxEventsPerFrame) return false;
        _eventsThisFrame++;
        return true;
    }

    internal Task<T> QueueOperationAsync<T>(string name, Func<Task<T>> op, OperationPriority prio = OperationPriority.Normal, CancellationToken ct = default)
        => _queue.EnqueueAsync(name, op, prio, ct);

    internal void Configure(PerformanceProfile profile)
    {
        _profile = profile;
        _fpsLimiter.Apply(profile);
        _throttler.UpdateProfile(profile);
        _queue.UpdateLimit(profile.MaxQueuedOperations);
        ApplyPatchSettings(profile);
    }

    private void ApplyPatchSettings(PerformanceProfile profile)
    {
        var quality = Math.Clamp(profile.QualityLevel, 0, 4);
        GregPerformancePatches.CanvasThrottleEnabled = quality < 4;
        GregPerformancePatches.CanvasUpdateInterval = quality switch { 0 => 0.25f, 1 => 0.15f, 2 => 0.1f, _ => 0.05f };
        GregPerformancePatches.IndicatorThrottleEnabled = true;
        GregPerformancePatches.IndicatorUpdateInterval = quality <= 0 ? 0.2f : quality <= 2 ? 0.1f : 0.05f;
        GregPerformancePatches.PulsatingThrottleEnabled = true;
        GregPerformancePatches.PulsatingUpdateInterval = quality <= 0 ? 0.15f : quality <= 2 ? 0.05f : 0.025f;
        GregPerformancePatches.NpcThrottleEnabled = true;
        GregPerformancePatches.NpcThrottleDistance = quality <= 0 ? 10f : quality <= 2 ? 15f : 25f;
        GregPerformancePatches.NpcThrottleInterval = quality <= 0 ? 0.35f : quality <= 2 ? 0.2f : 0.1f;
        GregPerformancePatches.AsyncRouteEvalEnabled = false;
    }

    internal PerformanceStats GetStats() => new PerformanceStats { 
        Profile = _profile, 
        Resources = _monitor.GetLatest(), 
        Throttle = _throttler.GetMetrics(), 
        QueueDepth = _queue.QueueDepth 
    };

    public void Dispose()
    {
        _fpsLimiter.Dispose();
        _throttler.Dispose();
        _monitor.Dispose();
        _queue.Dispose();
    }
}
