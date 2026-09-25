/// <file-summary>
/// Layer:       Core
/// Purpose:     Thread-safe implementation of IGregEventBus.
/// Maintainer:  Publish runs synchronously on the calling thread. Caching for the hot path.
/// </file-summary>

namespace gregCore.Core.Events;

public sealed class GregEventBus : IGregEventBus, IDisposable
{
    private readonly IGregLogger _logger;
    private readonly Dictionary<string, List<Action<EventPayload>>> _handlers = new();
    private readonly Dictionary<string, Action<EventPayload>[]> _cachedHandlers = new();
    private readonly ReaderWriterLockSlim _rwLock = new();
    private readonly System.Collections.Concurrent.ConcurrentQueue<(string hookName, EventPayload payload)> _deferredEvents = new();
    private IGregPerformanceGovernor? _governor;
    private bool _isDirty = true;
    private bool _disposed;
    private long _totalEventsProcessed;
    private long _totalEventsDeferred;

    public GregEventBus(IGregLogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger.ForContext("EventBus");
    }

    public void SetGovernor(IGregPerformanceGovernor governor) => _governor = governor;

    public void Subscribe(string hookName, Action<EventPayload> handler)
    {
        if (_disposed) return;
        ArgumentNullException.ThrowIfNull(hookName);
        ArgumentNullException.ThrowIfNull(handler);

        _rwLock.EnterWriteLock();
        try
        {
            if (!_handlers.TryGetValue(hookName, out var list))
            {
                list = new List<Action<EventPayload>>();
                _handlers[hookName] = list;
            }
            list.Add(handler);
            _isDirty = true;
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public void SubscribeOnce(string hookName, Action<EventPayload> handler)
    {
        if (_disposed) return;
        ArgumentNullException.ThrowIfNull(hookName);
        ArgumentNullException.ThrowIfNull(handler);

        Action<EventPayload>? wrapper = null;
        wrapper = payload =>
        {
            try
            {
                handler(payload);
            }
            finally
            {
                if (wrapper != null)
                    Unsubscribe(hookName, wrapper);
            }
        };

        Subscribe(hookName, wrapper);
    }

    public void Unsubscribe(string hookName, Action<EventPayload> handler)
    {
        if (_disposed) return;
        ArgumentNullException.ThrowIfNull(hookName);
        ArgumentNullException.ThrowIfNull(handler);

        _rwLock.EnterWriteLock();
        try
        {
            if (_handlers.TryGetValue(hookName, out var list))
            {
                list.Remove(handler);
                if (list.Count == 0)
                {
                    _handlers.Remove(hookName);
                }
                _isDirty = true;
            }
        }
        finally
        {
            _rwLock.ExitWriteLock();
        }
    }

    public bool Publish(string hookName, EventPayload payload)
    {
        if (_disposed) return true;
        ArgumentNullException.ThrowIfNull(hookName);

        if (_governor != null && !_governor.CanDispatchEvent())
        {
            _deferredEvents.Enqueue((hookName, payload));
            _totalEventsDeferred++;
            return false;
        }

        return PublishDirect(hookName, payload);
    }

    internal void FlushDeferredEvents()
    {
        if (_disposed) return;
        int flushed = 0;
        const int maxFlushPerFrame = 50;

        while (_deferredEvents.TryDequeue(out var ev) && flushed < maxFlushPerFrame)
        {
            if (_governor != null && !_governor.CanDispatchEvent()) break;
            PublishDirect(ev.hookName, ev.payload);
            flushed++;
        }
    }

    private bool PublishDirect(string hookName, EventPayload payload)
    {
        if (_disposed) return true;
        if (!TryGetHandlers(hookName, out Action<EventPayload>[] handlersToInvoke)) return true;
        if (handlersToInvoke == null || handlersToInvoke.Length == 0) return true;
        return InvokeAll(hookName, payload, handlersToInvoke);
    }

    private bool TryGetHandlers(string hookName, out Action<EventPayload>[] handlers)
    {
        handlers = null;
        try
        {
            _rwLock.EnterReadLock();
            try
            {
                if (_isDirty) RefreshCacheLocked();
                _cachedHandlers.TryGetValue(hookName, out handlers);
                return true;
            }
            finally
            {
                _rwLock.ExitReadLock();
            }
        }
        catch { return false; }
    }

    private void RefreshCacheLocked()
    {
        try
        {
            _rwLock.ExitReadLock();
            _rwLock.EnterWriteLock();
            try
            {
                if (!_isDirty) return;
                _cachedHandlers.Clear();
                foreach (var kvp in _handlers)
                {
                    _cachedHandlers[kvp.Key] = kvp.Value.ToArray();
                }
                _isDirty = false;
            }
            finally
            {
                _rwLock.ExitWriteLock();
                _rwLock.EnterReadLock();
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private bool InvokeAll(string hookName, EventPayload payload, Action<EventPayload>[] handlers)
    {
        try
        {
            var currentPayload = payload with { HookName = hookName };
            foreach (var handler in handlers)
            {
                try
                {
                    handler(currentPayload);
                    if (currentPayload.IsCancelable && currentPayload.IsCancelled)
                    {
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"Error in handler for {hookName}", ex);
                }
            }
            _totalEventsProcessed++;
            return true;
        }
        catch { return true; }
    }

    /// <summary>
    /// Returns statistics about event processing.
    /// </summary>
    public (long processed, long deferred, int handlerCount) GetStats()
    {
        _rwLock.EnterReadLock();
        try
        {
            int handlerCount = 0;
            foreach (var kvp in _handlers)
            {
                handlerCount += kvp.Value.Count;
            }
            return (_totalEventsProcessed, _totalEventsDeferred, handlerCount);
        }
        finally
        {
            _rwLock.ExitReadLock();
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _disposed = true; // Set flag BEFORE disposing lock
            _rwLock.Dispose();
        }
        _disposed = true;
    }
}
