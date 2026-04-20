using System;
using System.Collections.Generic;
using gregCore.Core.Abstractions;
using gregCore.Core.Models;

namespace gregCore.Core.Events;

/// <summary>
/// Synchronous Hook Bus for Game Interception.
/// Manages all 1771 native hooks and ensures safe dispatching.
/// </summary>
public sealed class GregHookBus
{
    private readonly IGregLogger _logger;
    private readonly Dictionary<string, List<Action<EventPayload>>> _hooks = new();
    private readonly HashSet<string> _disabledHooks = new();

    public GregHookBus(IGregLogger logger)
    {
        _logger = logger.ForContext("HookBus");
    }

    /// <summary>
    /// Subscribes to a specific hook.
    /// </summary>
    public void On(string hookName, Action<EventPayload> handler)
    {
        if (!_hooks.ContainsKey(hookName))
            _hooks[hookName] = new List<Action<EventPayload>>();
        
        _hooks[hookName].Add(handler);
        _logger.Info($"Listener registriert für Hook: {hookName}");
    }

    /// <summary>
    /// Dispatches a hook synchronously.
    /// </summary>
    public void Dispatch(string hookName, EventPayload payload)
    {
        if (_disabledHooks.Contains(hookName))
            return;

        if (_hooks.TryGetValue(hookName, out var handlers))
        {
            foreach (var handler in handlers)
            {
                try
                {
                    handler(payload);
                }
                catch (Exception ex)
                {
                    _logger.Error($"Fehler in Hook-Handler für {hookName}: {ex.Message}", ex);
                }
            }
        }
    }

    /// <summary>
    /// Disables a hook temporarily (e.g., due to performance or stability issues).
    /// </summary>
    public void SetHookStatus(string hookName, bool enabled)
    {
        if (enabled)
            _disabledHooks.Remove(hookName);
        else
            _disabledHooks.Add(hookName);
        
        _logger.Warning($"Hook-Status geändert: {hookName} -> {(enabled ? "Enabled" : "Disabled")}");
    }

    public bool IsHookEnabled(string hookName) => !_disabledHooks.Contains(hookName);
}
