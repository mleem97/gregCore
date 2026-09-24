// GregHost.cs — copy this file into your mod (any folder under src/).
//
// Runtime probe: detects whether gregCore is present WITHOUT a hard
// dependency at runtime (pure type-name lookup, no direct type access).
// With core: use central services. Without: local minimal fallbacks
// (standalone mode, base features only).
//
// RULE: methods touching gregCore types may ONLY run when HasCore is true,
// and must live in their own methods (otherwise JIT TypeLoad when the
// gregCore DLL is missing). Typical call site:
//
//     if (GregHost.HasCore)
//     {
//         try { RegisterCoreExtras(); } catch { /* best-effort */ }
//     }
//
// And RegisterCoreExtras() (own method!) holds all gregCore.* calls.
// For F1-hub wiring see gregCore.UI.GregMenuBinding (one call instead
// of hand-rolled opener + closer).
//
// Adjust the namespace to your mod. Keep the probe type string as-is
// (it must match a type inside gregCore.dll).

using System;

namespace YourModNamespace;

public static class GregHost
{
    private const string ProbeType = "gregCore.UI.GregNotificationManager, gregCore";
    private static bool? _hasCore;

    public static bool HasCore
    {
        get
        {
            if (_hasCore == null)
            {
                try { _hasCore = Type.GetType(ProbeType) != null; }
                catch { _hasCore = false; }
            }
            return _hasCore.Value;
        }
    }

    // Test hook only (e.g. force standalone behavior).
    public static void OverrideForTesting(bool? value)
    {
        _hasCore = value;
    }
}
