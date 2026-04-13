using System;
using greg.Sdk;
using greg.Exporter;

namespace greg.Core.Scripting;

/// <summary>
/// High-level event bus for gregCore hooks.
/// Bridges string-based hooks (gregEventDispatcher) and typed events (ModFramework.Events).
/// </summary>
public static class GregHookBus
{
    /// <summary>
    /// Fires a string-based hook with a payload.
    /// </summary>
    public static void Fire(string hookName, object payload = null)
    {
        gregEventDispatcher.Emit(hookName, payload);
    }

    /// <summary>
    /// Subscribes to a string-based hook.
    /// </summary>
    public static void On(string hookName, Action<object> handler, string modId = null)
    {
        gregEventDispatcher.On(hookName, handler, modId);
    }

    /// <summary>
    /// Subscribes to ALL string-based hooks (for monitoring/debugging).
    /// Note: This requires a modification to gregEventDispatcher to support global monitoring.
    /// For now, we'll implement a simple callback mechanism.
    /// </summary>
    private static Action<string, object> _onAny;
    [ThreadStatic]
    private static bool _isNotifyingAny;

    public static void OnAny(Action<string, object> handler)
    {
        _onAny += handler;
    }

    /// <summary>
    /// Internally used by the dispatcher to notify global listeners.
    /// </summary>
    internal static void NotifyAny(string hookName, object payload)
    {
        if (_isNotifyingAny)
            return;

        var handlers = _onAny;
        if (handlers == null)
            return;

        _isNotifyingAny = true;
        try
        {
            foreach (Delegate handler in handlers.GetInvocationList())
            {
                try
                {
                    ((Action<string, object>)handler).Invoke(hookName, payload);
                }
                catch (Exception ex)
                {
                    MelonLoader.MelonLogger.Warning($"[gregCore] OnAny handler '{handler.Method.DeclaringType?.Name}.{handler.Method.Name}' threw and was removed: {ex.Message}");
                    _onAny -= (Action<string, object>)handler;
                }
            }
        }
        finally
        {
            _isNotifyingAny = false;
        }
    }
}
