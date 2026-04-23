/// <file-summary>
/// Schicht:      Core
/// Zweck:        Thread-sichere Implementierung des IGregEventBus.
/// Maintainer:   Publish läuft synchron auf dem aufrufenden Thread. Caching für Hotpath.
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
            return false;
        }
        
        return PublishDirect(hookName, payload);
    }

    internal void FlushDeferredEvents()
    {
        if (_disposed) return;
        while (_deferredEvents.TryDequeue(out var ev))
        {
            if (_governor != null && !_governor.CanDispatchEvent()) break;
            PublishDirect(ev.hookName, ev.payload);
        }
    }

    private bool PublishDirect(string hookName, EventPayload payload)
    {
        if (_disposed) return true;
        Action<EventPayload>[]? handlersToInvoke = null;

        _rwLock.EnterReadLock();
        try
        {
            if (_isDirty)
            {
                _rwLock.ExitReadLock();
                _rwLock.EnterWriteLock();
                try
                {
                    if (_isDirty)
                    {
                        _cachedHandlers.Clear();
                        foreach (var kvp in _handlers)
                        {
                            _cachedHandlers[kvp.Key] = kvp.Value.ToArray();
                        }
                        _isDirty = false;
                    }
                }
                finally
                {
                    _rwLock.ExitWriteLock();
                    _rwLock.EnterReadLock();
                }
            }

            _cachedHandlers.TryGetValue(hookName, out handlersToInvoke);
        }
        finally
        {
            _rwLock.ExitReadLock();
        }

        if (handlersToInvoke == null || handlersToInvoke.Length == 0) return true;

        var currentPayload = payload with { HookName = hookName };

        foreach (var handler in handlersToInvoke)
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
                _logger.Error($"Fehler in Handler für {hookName}", ex);
            }
        }

        return true;
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
