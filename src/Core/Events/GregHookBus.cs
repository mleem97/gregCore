using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;

namespace gregCore.Core.Events;

/// <summary>
/// Synchronous Hook Bus for Game Interception.
/// Manages all native hooks and ensures safe dispatching.
/// Optimized with ConcurrentDictionary for high-frequency hooks.
/// </summary>
public sealed class GregHookBus
{
    private readonly IGregLogger _logger;
    private readonly ConcurrentDictionary<string, List<Action<EventPayload>>> _hooks = new();
    private readonly ConcurrentDictionary<string, bool> _disabledHooks = new();
    private long _totalHooksFired;
    private long _totalHookErrors;

    public GregHookBus(IGregLogger logger)
    {
        _logger = logger.ForContext("HookBus");
    }

    /// <summary>
    /// Subscribes to a specific hook.
    /// </summary>
    public void On(string hookName, Action<EventPayload> handler)
    {
        if (string.IsNullOrEmpty(hookName) || handler == null) return;

        var list = _hooks.GetOrAdd(hookName, _ => new List<Action<EventPayload>>());
        lock (list)
        {
            list.Add(handler);
        }
        _logger.Info($"Listener registriert für Hook: {hookName}");
    }

    /// <summary>
    /// Subscribes once to a hook (auto-unsubscribes after first invocation).
    /// </summary>
    public void Once(string hookName, Action<EventPayload> handler)
    {
        if (string.IsNullOrEmpty(hookName) || handler == null) return;

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
                    Off(hookName, wrapper);
            }
        };

        On(hookName, wrapper);
    }

    /// <summary>
    /// Unsubscribes from a hook.
    /// </summary>
    public void Off(string hookName, Action<EventPayload> handler)
    {
        if (string.IsNullOrEmpty(hookName) || handler == null) return;

        if (_hooks.TryGetValue(hookName, out var list))
        {
            lock (list)
            {
                list.Remove(handler);
            }
        }
    }

    /// <summary>
    /// Dispatches a hook synchronously.
    /// </summary>
    public void Dispatch(string hookName, EventPayload payload)
    {
        if (_disabledHooks.TryGetValue(hookName, out var disabled) && disabled)
            return;

        if (!_hooks.TryGetValue(hookName, out var handlers))
            return;

        Action<EventPayload>[] handlersCopy;
        lock (handlers)
        {
            handlersCopy = handlers.ToArray();
        }

        if (handlersCopy.Length == 0) return;

        var currentPayload = payload with { HookName = hookName };
        bool anySuccess = false;

        foreach (var handler in handlersCopy)
        {
            try
            {
                handler(currentPayload);
                anySuccess = true;
            }
            catch (Exception ex)
            {
                _totalHookErrors++;
                _logger.Error($"Fehler in Hook-Handler für {hookName}: {ex.Message}", ex);
            }
        }

        if (anySuccess)
            _totalHooksFired++;
    }

    /// <summary>
    /// Dispatches with automatic error recovery and logging.
    /// </summary>
    public bool TryDispatch(string hookName, EventPayload payload)
    {
        try
        {
            Dispatch(hookName, payload);
            return true;
        }
        catch (Exception ex)
        {
            _totalHookErrors++;
            _logger.Error($"Hook dispatch failed for {hookName}: {ex.Message}", ex);
            return false;
        }
    }

    /// <summary>
    /// Disables a hook temporarily (e.g., due to performance or stability issues).
    /// </summary>
    public void SetHookStatus(string hookName, bool enabled)
    {
        if (enabled)
            _disabledHooks.TryRemove(hookName, out _);
        else
            _disabledHooks[hookName] = true;

        _logger.Warning($"Hook-Status geändert: {hookName} -> {(enabled ? "Enabled" : "Disabled")}");
    }

    public bool IsHookEnabled(string hookName) => !_disabledHooks.ContainsKey(hookName);

    /// <summary>
    /// Returns hook bus statistics.
    /// </summary>
    public (long fired, long errors, int activeHooks) GetStats()
    {
        int activeHooks = 0;
        foreach (var kvp in _hooks)
        {
            lock (kvp.Value)
            {
                if (kvp.Value.Count > 0) activeHooks++;
            }
        }
        return (_totalHooksFired, _totalHookErrors, activeHooks);
    }

    /// <summary>
    /// Clears all hooks. Use with caution.
    /// </summary>
    public void Clear()
    {
        _hooks.Clear();
        _disabledHooks.Clear();
        _logger.Warning("All hooks cleared.");
    }
}
