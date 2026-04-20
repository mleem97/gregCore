namespace gregCore.Infrastructure.Performance;

internal sealed class GregOperationQueue : IDisposable
{
    private readonly GregRequestThrottler _throttler;
    private readonly IGregLogger _logger;
    private readonly PriorityQueue<QueuedOperation, int> _queue = new PriorityQueue<QueuedOperation, int>();
    private readonly SemaphoreSlim _processLock = new SemaphoreSlim(1, 1);
    private bool _isDisposed;

    internal GregOperationQueue(GregRequestThrottler throttler, IGregLogger logger)
    {
        _throttler = throttler;
        _logger = logger.ForContext(nameof(GregOperationQueue));
    }

    internal async Task<T> EnqueueAsync<T>(string name, Func<Task<T>> operation, OperationPriority priority = OperationPriority.Normal, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<T>();
        var op = new QueuedOperation(name, async () => {
            try { tcs.SetResult(await _throttler.ExecuteOperationAsync(name, operation, priority, ct)); }
            catch (Exception ex) { tcs.SetException(ex); }
        }, (int)priority);

        lock (_queue) { _queue.Enqueue(op, -(int)priority); }
        _ = ProcessQueueAsync(ct);
        return await tcs.Task;
    }

    private async Task ProcessQueueAsync(CancellationToken ct)
    {
        if (!await _processLock.WaitAsync(0)) return;
        try {
            while (true) {
                QueuedOperation? op;
                lock (_queue) { if (!_queue.TryDequeue(out op, out _)) break; }
                if (ct.IsCancellationRequested) break;
                try { await op.Execute(); } catch (Exception ex) { _logger.Error($"[Queue] Fehlgeschlagen: {op.Name}", ex); }
            }
        } finally { _processLock.Release(); }
    }

    internal int QueueDepth { get { lock (_queue) return _queue.Count; } }
    public void Dispose() { if (!_isDisposed) { _isDisposed = true; _processLock.Dispose(); } }

    private record QueuedOperation(string Name, Func<Task> Execute, int Priority);
}