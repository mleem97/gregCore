namespace gregCore.Infrastructure.Performance;

internal sealed class GregRequestThrottler : IDisposable
{
    private readonly IGregLogger _logger;
    private SemaphoreSlim _opSemaphore;
    private SemaphoreSlim _reqSemaphore;
    private PerformanceProfile _profile;
    private int _totalQueued;
    private int _totalCompleted;
    private int _currentActive;

    internal GregRequestThrottler(IGregLogger logger, PerformanceProfile profile)
    {
        _logger = logger.ForContext(nameof(GregRequestThrottler));
        _profile = profile;
        _opSemaphore = new SemaphoreSlim(profile.MaxConcurrentOps);
        _reqSemaphore = new SemaphoreSlim(profile.MaxConcurrentRequests);
    }

    internal async Task<T> ExecuteOperationAsync<T>(string operationName, Func<Task<T>> operation, OperationPriority priority = OperationPriority.Normal, CancellationToken ct = default)
    {
        Interlocked.Increment(ref _totalQueued);
        await _opSemaphore.WaitAsync(ct);
        Interlocked.Increment(ref _currentActive);
        
        try {
            _logger.Debug($"[Throttle] START '{operationName}' (aktiv: {_currentActive}/{_profile.MaxConcurrentOps})");
            return await operation();
        } finally {
            Interlocked.Decrement(ref _currentActive);
            Interlocked.Increment(ref _totalCompleted);
            _opSemaphore.Release();
        }
    }

    internal void UpdateProfile(PerformanceProfile profile)
    {
        _profile = profile;
        var oldOp = _opSemaphore;
        var oldReq = _reqSemaphore;
        _opSemaphore = new SemaphoreSlim(profile.MaxConcurrentOps);
        _reqSemaphore = new SemaphoreSlim(profile.MaxConcurrentRequests);
        oldOp.Dispose();
        oldReq.Dispose();
    }

    internal ThrottleMetrics GetMetrics() => new ThrottleMetrics
    {
        TotalQueued = _totalQueued,
        TotalCompleted = _totalCompleted,
        CurrentActive = _currentActive,
        MaxConcurrent = _profile.MaxConcurrentOps,
        QueueDepth = _totalQueued - _totalCompleted - _currentActive
    };

    public void Dispose()
    {
        _opSemaphore.Dispose();
        _reqSemaphore.Dispose();
    }
}