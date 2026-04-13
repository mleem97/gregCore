using System;
using System.Collections.Generic;
using MelonLoader;

namespace greg.Sdk.Services;

/// <summary>
/// Type-safe publish/subscribe event bus for greg ecosystem.
/// Complements the string-based gregEventDispatcher with generic type safety.
/// </summary>
public static class GregEventBus
{
    private static readonly object _lock = new();
    private static readonly Dictionary<Type, Dictionary<string, Delegate>> _handlers = new();

    public static void Subscribe<T>(string listenerId, Action<T> handler)
    {
        if (string.IsNullOrWhiteSpace(listenerId) || handler == null) return;

        lock (_lock)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var dict))
            {
                dict = new Dictionary<string, Delegate>();
                _handlers[type] = dict;
            }
            dict[listenerId] = handler;
            MelonLogger.Msg($"[EventBus] Subscribed: {listenerId} → {type.Name}");
        }
    }

    public static void Publish<T>(T eventData)
    {
        List<KeyValuePair<string, Delegate>> snapshot;

        lock (_lock)
        {
            var type = typeof(T);
            if (!_handlers.TryGetValue(type, out var dict) || dict.Count == 0)
                return;

            snapshot = new List<KeyValuePair<string, Delegate>>(dict);
        }

        foreach (var kvp in snapshot)
        {
            try
            {
                if (kvp.Value is Action<T> handler)
                    handler(eventData);
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[EventBus] Handler '{kvp.Key}' for {typeof(T).Name} threw: {ex.Message}");
            }
        }
    }

    public static void Unsubscribe<T>(string listenerId)
    {
        if (string.IsNullOrWhiteSpace(listenerId)) return;

        lock (_lock)
        {
            var type = typeof(T);
            if (_handlers.TryGetValue(type, out var dict))
            {
                dict.Remove(listenerId);
                if (dict.Count == 0) _handlers.Remove(type);
            }
        }
    }

    public static void UnsubscribeAll(string listenerId)
    {
        if (string.IsNullOrWhiteSpace(listenerId)) return;

        lock (_lock)
        {
            var emptyTypes = new List<Type>();
            foreach (var kvp in _handlers)
            {
                kvp.Value.Remove(listenerId);
                if (kvp.Value.Count == 0) emptyTypes.Add(kvp.Key);
            }
            foreach (var t in emptyTypes) _handlers.Remove(t);
        }
        MelonLogger.Msg($"[EventBus] Unsubscribed all for: {listenerId}");
    }
}

