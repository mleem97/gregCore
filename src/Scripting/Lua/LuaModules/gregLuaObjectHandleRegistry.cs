using System;
using System.Collections.Generic;
using System.Threading;

namespace greg.Core.Scripting.Lua;

/// <summary>
/// Thread-safe registry mapping integer handles to live .NET/Il2Cpp objects.
/// Lua scripts use these handles to reference game objects without holding direct managed references.
/// Handles are periodically pruned when objects become null (destroyed Unity objects).
/// </summary>
internal static class gregLuaObjectHandleRegistry
{
    private static readonly Dictionary<int, WeakReference<object>> Handles = new();
    private static int _nextHandle;

    public static int Register(object obj)
    {
        if (obj == null) return 0;
        int handle = Interlocked.Increment(ref _nextHandle);
        lock (Handles) { Handles[handle] = new WeakReference<object>(obj); }
        return handle;
    }

    public static object Resolve(int handle)
    {
        if (handle <= 0) return null;
        lock (Handles)
        {
            if (!Handles.TryGetValue(handle, out var wr)) return null;
            if (wr.TryGetTarget(out var obj)) return obj;
            Handles.Remove(handle);
            return null;
        }
    }

    public static T Resolve<T>(int handle) where T : class
    {
        return Resolve(handle) as T;
    }

    public static void Release(int handle)
    {
        if (handle <= 0) return;
        lock (Handles) { Handles.Remove(handle); }
    }

    public static void Prune()
    {
        lock (Handles)
        {
            var dead = new List<int>();
            foreach (var kv in Handles)
            {
                if (!kv.Value.TryGetTarget(out _))
                    dead.Add(kv.Key);
            }
            for (int i = 0; i < dead.Count; i++)
                Handles.Remove(dead[i]);
        }
    }

    public static void Clear()
    {
        lock (Handles) { Handles.Clear(); }
    }
}


