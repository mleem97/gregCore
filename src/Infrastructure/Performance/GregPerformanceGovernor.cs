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
        _queue = new GregOperationQueue(_throttler, ctx.Logger);
        
        _monitor.Start(5000);
        _logger.Info($"[Governor] Initialisiert mit Prefix-Architektur.");
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
