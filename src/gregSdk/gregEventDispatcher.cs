using System;
using System.Buffers;
using System.Collections.Generic;
using MelonLoader;

namespace greg.Sdk;

/// <summary>
/// Central event bus for gregModLoader hook payloads.
/// </summary>
public static class gregEventDispatcher
{
    private const int MaxEmitDepth = 64;

    private static readonly object Sync = new();
    private static readonly Dictionary<string, List<Subscription>> Handlers = new(StringComparer.Ordinal);
    private static readonly Dictionary<string, List<CancelableSubscription>> CancelableHandlers = new(StringComparer.Ordinal);
    [ThreadStatic]
    private static int _emitDepth;

    private sealed class Subscription
    {
        public Action<object> Handler;
        public string ModId;
    }

    private sealed class CancelableSubscription
    {
        public Func<object, bool> Handler;
        public string ModId;
    }

    public static void On(string hookName, Action<object> handler, string modId = null)
    {
        if (string.IsNullOrWhiteSpace(hookName) || handler == null)
            return;

        lock (Sync)
        {
            if (!Handlers.TryGetValue(hookName, out var list))
            {
                list = new List<Subscription>();
                Handlers[hookName] = list;
            }

            list.Add(new Subscription { Handler = handler, ModId = modId });
        }
    }

    public static void Once(string hookName, Action<object> handler, string modId = null)
    {
        if (handler == null)
            return;

        Action<object> wrapper = null;
        wrapper = payload =>
        {
            handler(payload);
            Off(hookName, wrapper);
        };
        On(hookName, wrapper, modId);
    }

    public static void Off(string hookName, Action<object> handler)
    {
        if (handler == null || string.IsNullOrWhiteSpace(hookName))
            return;

        lock (Sync)
        {
            if (!Handlers.TryGetValue(hookName, out var list))
                return;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(list[i].Handler, handler))
                    list.RemoveAt(i);
            }

            if (list.Count == 0)
                Handlers.Remove(hookName);
        }
    }

    public static void Emit(string hookName, object payload = null)
    {
        if (string.IsNullOrWhiteSpace(hookName))
            return;

        if (_emitDepth >= MaxEmitDepth)
        {
            MelonLogger.Warning($"[gregCore] Emit depth limit reached ({MaxEmitDepth}) for '{hookName}'. Event dropped to prevent recursion overflow.");
            return;
        }

        _emitDepth++;

        try
        {
            // Notify global monitor
            greg.Core.Scripting.GregHookBus.NotifyAny(hookName, payload);

            Subscription[] snapshot;
            int snapshotCount;
            lock (Sync)
            {
                if (!Handlers.TryGetValue(hookName, out var list) || list.Count == 0)
                    return;

                snapshotCount = list.Count;
                snapshot = ArrayPool<Subscription>.Shared.Rent(snapshotCount);
                for (var i = 0; i < snapshotCount; i++)
                    snapshot[i] = list[i];
            }

            try
            {
                for (var i = 0; i < snapshotCount; i++)
                {
                    var sub = snapshot[i];
                    try
                    {
                        sub.Handler?.Invoke(payload);
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"[gregCore] Handler for '{hookName}' threw: {ex.Message}");
                    }
                }
            }
            finally
            {
                Array.Clear(snapshot, 0, snapshotCount);
                ArrayPool<Subscription>.Shared.Return(snapshot);
            }
        }
        finally
        {
            _emitDepth--;
        }
    }

    public static void OnCancelable(string hookName, Func<object, bool> handler, string modId = null)
    {
        if (string.IsNullOrWhiteSpace(hookName) || handler == null)
            return;

        lock (Sync)
        {
            if (!CancelableHandlers.TryGetValue(hookName, out var list))
            {
                list = new List<CancelableSubscription>();
                CancelableHandlers[hookName] = list;
            }

            list.Add(new CancelableSubscription { Handler = handler, ModId = modId });
        }
    }

    public static void OffCancelable(string hookName, Func<object, bool> handler)
    {
        if (handler == null || string.IsNullOrWhiteSpace(hookName))
            return;

        lock (Sync)
        {
            if (!CancelableHandlers.TryGetValue(hookName, out var list))
                return;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(list[i].Handler, handler))
                    list.RemoveAt(i);
            }

            if (list.Count == 0)
                CancelableHandlers.Remove(hookName);
        }
    }

    public static bool InvokeCancelable(string hookName, object payload = null)
    {
        if (string.IsNullOrWhiteSpace(hookName))
            return true;

        CancelableSubscription[] snapshot;
        int snapshotCount;
        lock (Sync)
        {
            if (!CancelableHandlers.TryGetValue(hookName, out var list) || list.Count == 0)
                return true;

            snapshotCount = list.Count;
            snapshot = ArrayPool<CancelableSubscription>.Shared.Rent(snapshotCount);
            for (var i = 0; i < snapshotCount; i++)
                snapshot[i] = list[i];
        }

        try
        {
            for (var i = 0; i < snapshotCount; i++)
            {
                var sub = snapshot[i];
                try
                {
                    if (sub.Handler != null && !sub.Handler(payload))
                        return false;
                }
                catch (Exception ex)
                {
                    MelonLogger.Warning($"[gregCore] Cancelable handler for '{hookName}' threw: {ex.Message}");
                }
            }
        }
        finally
        {
            Array.Clear(snapshot, 0, snapshotCount);
            ArrayPool<CancelableSubscription>.Shared.Return(snapshot);
        }

        return true;
    }

    public static void UnregisterAll(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
            return;

        lock (Sync)
        {
            foreach (var kv in Handlers)
            {
                var list = kv.Value;
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(list[i].ModId, modId, StringComparison.Ordinal))
                        list.RemoveAt(i);
                }
            }

            var emptyKeys = new List<string>();
            foreach (var kv in Handlers)
            {
                if (kv.Value.Count == 0)
                    emptyKeys.Add(kv.Key);
            }

            foreach (var k in emptyKeys)
                Handlers.Remove(k);

            foreach (var kv in CancelableHandlers)
            {
                var list = kv.Value;
                for (var i = list.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(list[i].ModId, modId, StringComparison.Ordinal))
                        list.RemoveAt(i);
                }
            }

            emptyKeys.Clear();
            foreach (var kv in CancelableHandlers)
            {
                if (kv.Value.Count == 0)
                    emptyKeys.Add(kv.Key);
            }

            foreach (var k in emptyKeys)
                CancelableHandlers.Remove(k);
        }

        MelonLogger.Msg($"[gregCore] Unregistered handlers and cancelable handlers for mod '{modId}'.");
    }
}




