using System;
using Il2Cpp;
using gregCore.API;

namespace DataCenterModLoader;

/// <summary>
/// Per-frame carry state monitor.
/// </summary>
internal static class CarryStateMonitor
{
    private static int _prevNumObjects = 0;
    private static int _prevObjectInHand = 0;
    private static bool _initialized = false;
    private static bool _suppressNextDrop = false;
    private static long _suppressTick = 0;

    internal static void SuppressNextDrop()
    {
        _suppressNextDrop = true;
        _suppressTick = System.Diagnostics.Stopwatch.GetTimestamp();
    }

    internal static void Update()
    {
        try
        {
            var pm = PlayerManager.instance;
            if (pm == null) return;
            int curNumObjects = pm.numberOfObjectsInHand;
            int curObjectInHand = (int)pm.objectInHand;
            if (!_initialized)
            {
                InitState(curNumObjects, curObjectInHand);
                return;
            }
            ExpireSuppressFlag();
            CheckDropTransition(curNumObjects, curObjectInHand);
            _prevNumObjects = curNumObjects;
            _prevObjectInHand = curObjectInHand;
        }
        catch (Exception ex) { CrashLog.Log($"[WorldSync] CarryStateMonitor error: {ex.Message}"); }
    }

    private static void InitState(int curNumObjects, int curObjectInHand)
    {
        _prevNumObjects = curNumObjects;
        _prevObjectInHand = curObjectInHand;
        _initialized = true;
    }

    private static void ExpireSuppressFlag()
    {
        try
        {
            if (!_suppressNextDrop) return;
            long now = System.Diagnostics.Stopwatch.GetTimestamp();
            if ((now - _suppressTick) > System.Diagnostics.Stopwatch.Frequency / 2)
                _suppressNextDrop = false;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void CheckDropTransition(int curNumObjects, int curObjectInHand)
    {
        try
        {
            bool isDrop = _prevNumObjects > 0 && curNumObjects == 0 && _prevObjectInHand != 0 && curObjectInHand == 0;
            if (!isDrop) return;
            if (!_suppressNextDrop) return;
            _suppressNextDrop = false;
            Patch_UsableObject_InteractOnClick.ClearHeldObject();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    internal static void Reset()
    {
        _prevNumObjects = 0;
        _prevObjectInHand = 0;
        _initialized = false;
        _suppressNextDrop = false;
    }
}
