/// <file-summary>
/// Layer:       Core (Diagnostics)
/// Purpose:     Process-wide game-build compatibility latch. GregDoctor
///              evaluates the game fingerprint once at boot; this latch makes
///              the verdict cheaply available to Harmony patches that must
///              fail safe on unknown builds (notably the Hardware-ID rewrite
///              system, which must never renumber devices it cannot verify).
///              Default is UNKNOWN (= not allowed): patches stay passive until
///              the boot verdict explicitly enables them.
/// </file-summary>

using System.Threading;

namespace gregCore.Core.Diagnostics;

public static class GregGameCompat
{
    private static int _evaluated; // 0 = unknown, 1 = verdict stored
    private static int _supported; // 0/1 once evaluated
    private static int _hwidWarned; // one-time log guard

    /// <summary>
    /// Called once at boot with the GregDoctor verdict.
    /// Supported = fingerprint match (Doctor ErrorCode empty).
    /// </summary>
    public static void MarkEvaluated(bool supported)
    {
        Interlocked.Exchange(ref _supported, supported ? 1 : 0);
        Interlocked.Exchange(ref _evaluated, 1);
    }

    /// <summary>True only after an explicit supported verdict.</summary>
    public static bool IsSupportedBuild =>
        Volatile.Read(ref _evaluated) == 1 && Volatile.Read(ref _supported) == 1;

    /// <summary>
    /// Gate for device-ID rewrites (HwId assign/heal patches). False on
    /// unknown or unsupported builds: patches must pass vanilla data through
    /// untouched (route-safe passthrough).
    /// </summary>
    public static bool HwIdRewritesAllowed => IsSupportedBuild;

    /// <summary>
    /// Logs the one-time loud warning when HwId rewrites stay disabled.
    /// Returns true when rewrites are allowed (convenience for patch guards).
    /// </summary>
    public static bool NotifyHwIdGate()
    {
        if (HwIdRewritesAllowed) return true;
        if (Interlocked.Exchange(ref _hwidWarned, 1) == 0)
        {
            try
            {
                MelonLoader.MelonLogger.Warning("[gregCore][HwId] DISABLED: unsupported/unknown game build — " +
                    "device IDs pass through untouched (vanilla behaviour). " +
                    "Update gregCore for full network-ID support.");
            }
            catch { /* logging must never break patches */ }
        }
        return false;
    }

    // Test hook: resets the latch (unit tests only).
    internal static void ResetForTests()
    {
        Interlocked.Exchange(ref _evaluated, 0);
        Interlocked.Exchange(ref _supported, 0);
        Interlocked.Exchange(ref _hwidWarned, 0);
    }
}
