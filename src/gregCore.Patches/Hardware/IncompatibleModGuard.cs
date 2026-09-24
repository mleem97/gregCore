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
                    if (melon == null) continue;
                    string modName = "";
                    try { modName = melon.Info?.Name ?? ""; } catch { continue; }
                    string asmName = "";
                    try { asmName = melon.GetType()?.Assembly?.GetName()?.Name ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    if (string.IsNullOrEmpty(modName) && string.IsNullOrEmpty(asmName)) continue;

                    // Never touch own assembly (self-protection).
                    if (string.Equals(asmName, "gregCore", StringComparison.OrdinalIgnoreCase))
                        continue;

                    string hay = (modName + " " + asmName).ToLowerInvariant();
                    bool hit = false;
                    foreach (var m in Markers)
                    {
                        if (hay.Contains(m)) { hit = true; break; }
                    }
                    if (!hit) continue;

                    string key = string.IsNullOrEmpty(asmName) ? modName : asmName;
                    if (!_handled.Add(key)) continue; // schon behandelt

                    string version = "";
                    try { version = melon.Info?.Version ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                    try
                    {
                        var harmony = melon.HarmonyInstance;
                        if (harmony != null)
                        {
                            harmony.UnpatchSelf();
                            anyDisabled = true;
                            MelonLogger.Warning($"[gregCore][HwId] Disabled incompatible ID mod " +
                                $"'{modName}' v{version} (gregID only).");
                            try
                            {
                                gregCore.UI.GregNotificationManager.Show(
                                    $"'{modName}' disabled — gregID active.",
                                    gregCore.UI.GregNotificationManager.GregToastType.Warning, 6f);
                            }
                            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                        }
                        else
                        {
                            MelonLogger.Msg($"[gregCore][HwId] '{modName}': no Harmony instance, skipped.");
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"[gregCore][HwId] Unpatch failed '{modName}': " +
                            $"{ex.GetBaseException().Message}");
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return anyDisabled;
    }
}
