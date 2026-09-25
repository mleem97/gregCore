/// <file-summary>
/// Layer:   GameLayer (Patches/Hardware)
/// Purpose: Compatibility guard for the gregID system. Detects the old
///          separate 404-PersistentID mod and unpatches it (Harmony
///          UnpatchSelf) so two ID systems never rename devices at once
///          (ID churn, broken cables). Runs at mod init and again on
///          scene load. Best-effort, never fatal.
/// </file-summary>

using System;
using System.Collections.Generic;
using MelonLoader;

namespace gregCore.GameLayer.Patches.Hardware;

public static class IncompatibleModGuard
{
    // Fingerprints of the old 404 mod (mod or assembly name,
    // case-insensitive, substring). Tight — never matches own code.
    private static readonly string[] Markers =
    {
        "persistentid",
        "persistent id",
        "404nyanfound",
    };

    // Already handled assemblies (session): UnpatchSelf is idempotent,
    // but log + toast fire only once.
    private static readonly HashSet<string> _handled =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    public static bool DisableIncompatibleIdMods()
    {
        bool anyDisabled = false;
        try
        {
            var melons = MelonBase.RegisteredMelons;
            if (melons == null) return false;
            foreach (var melon in melons)
            {
                try
                {
                    if (TryDisableOne(melon)) anyDisabled = true;
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return anyDisabled;
    }

    private static bool TryDisableOne(MelonBase melon)
    {
        if (melon == null) return false;
        string modName = "";
        try { modName = melon.Info?.Name ?? ""; } catch { return false; }
        string asmName = "";
        try { asmName = melon.GetType()?.Assembly?.GetName()?.Name ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        if (string.IsNullOrEmpty(modName) && string.IsNullOrEmpty(asmName)) return false;

        // Never touch own assembly (self-protection).
        if (string.Equals(asmName, "gregCore", StringComparison.OrdinalIgnoreCase))
            return false;

        string hay = (modName + " " + asmName).ToLowerInvariant();
        if (!MatchesMarker(hay)) return false;

        string key = string.IsNullOrEmpty(asmName) ? modName : asmName;
        if (!_handled.Add(key)) return false; // already handled
        return UnpatchOne(melon, modName);
    }

    private static bool MatchesMarker(string hay)
    {
        foreach (var m in Markers)
        {
            if (hay.Contains(m)) return true;
        }
        return false;
    }

    private static bool UnpatchOne(MelonBase melon, string modName)
    {
        string version = "";
        try { version = melon.Info?.Version ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try
        {
            var harmony = melon.HarmonyInstance;
            if (harmony == null)
            {
                MelonLogger.Msg($"[gregCore][HwId] '{modName}': no Harmony instance, skipped.");
                return false;
            }
            harmony.UnpatchSelf();
            MelonLogger.Warning($"[gregCore][HwId] Disabled incompatible ID mod " +
                $"'{modName}' v{version} (gregID only).");
            NotifyDisabled(modName);
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[gregCore][HwId] Unpatch failed '{modName}': " +
                $"{ex.GetBaseException().Message}");
            return false;
        }
    }

    private static void NotifyDisabled(string modName)
    {
        try
        {
            gregCore.UI.GregNotificationManager.Show(
                $"'{modName}' disabled — gregID active.",
                gregCore.UI.GregNotificationManager.GregToastType.Warning, 6f);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
