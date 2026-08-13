namespace gregCore.Infrastructure.Performance;

internal sealed class GregOperationQueue : IDisposable
{
    private readonly GregRequestThrottler _throttler;
    private readonly IGregLogger _logger;
    private readonly PriorityQueue<QueuedOperation, int> _queue = new PriorityQueue<QueuedOperation, int>();
    private readonly SemaphoreSlim _processLock = new SemaphoreSlim(1, 1);
    private int _maxQueueSize;
    private bool _isDisposed;

    internal GregOperationQueue(GregRequestThrottler throttler, IGregLogger logger, int maxQueueSize)
    {
        _throttler = throttler;
        _logger = logger.ForContext(nameof(GregOperationQueue));
        _maxQueueSize = Math.Max(1, maxQueueSize);
    }

    internal async Task<T> EnqueueAsync<T>(string name, Func<Task<T>> operation, OperationPriority priority = OperationPriority.Normal, CancellationToken ct = default)
    {
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);
        if (ct.IsCancellationRequested)
        {
            tcs.TrySetCanceled(ct);
            return await tcs.Task.ConfigureAwait(false);
        }

        var op = new QueuedOperation(name, async () => {
            try { tcs.SetResult(await _throttler.ExecuteOperationAsync(name, operation, priority, ct)); }
            catch (Exception ex) { tcs.SetException(ex); }
        }, (int)priority, cancellationToken => tcs.TrySetCanceled(cancellationToken));

        Exception? rejection = null;
        lock (_queue)
        {
            if (_isDisposed)
            {
                rejection = new ObjectDisposedException(nameof(GregOperationQueue));
            }
            else if (_queue.Count >= _maxQueueSize)
            {
                rejection = new InvalidOperationException($"Operation queue limit reached ({_maxQueueSize}).");
            }
            else
            {
                _queue.Enqueue(op, -(int)priority);
            }
        }
        if (rejection != null)
        {
            tcs.TrySetException(rejection);
            if (rejection is InvalidOperationException)
                _logger.Warning($"[Queue] Rejected '{name}': queue limit {_maxQueueSize} reached.");
            return await tcs.Task.ConfigureAwait(false);
        }
        _ = ProcessQueueAsync(ct);
        return await tcs.Task.ConfigureAwait(false);
    }

    private async Task ProcessQueueAsync(CancellationToken ct)
    {
        if (!await _processLock.WaitAsync(0)) return;
        try {
            while (true) {
                QueuedOperation? op;
                lock (_queue) { if (!_queue.TryDequeue(out op, out _)) break; }
                if (ct.IsCancellationRequested)
                {
                    op.Cancel(ct);
                    continue;
                }
                try { await op.Execute(); } catch (Exception ex) { _logger.Error($"[Queue] Fehlgeschlagen: {op.Name}", ex); }
            }
        } finally { _processLock.Release(); }
    }

    internal int QueueDepth { get { lock (_queue) return _queue.Count; } }
    internal void UpdateLimit(int maxQueueSize) => Volatile.Write(ref _maxQueueSize, Math.Max(1, maxQueueSize));
    public void Dispose() { if (!_isDisposed) { _isDisposed = true; _processLock.Dispose(); } }

    private sealed record QueuedOperation(string Name, Func<Task> Execute, int Priority, Action<CancellationToken> CancelAction)
    {
        public void Cancel(CancellationToken cancellationToken) => CancelAction(cancellationToken);
    }
}
