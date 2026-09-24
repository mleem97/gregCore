/// <file-summary>
/// Schicht:      GameLayer (Patches/Hardware)
/// Zweck:        Kompatibilitaetswache fuer das gregID-System. Erkennt den
///               alten separaten 404-PersistentID-Mod und entpatcht ihn
///               (Harmony-UnpatchSelf), damit niemals zwei ID-Systeme
///               gleichzeitig Geraete umbenennen (ID-Churn, Kabelbrueche).
///               Laeuft bei Mod-Init (frueh geladene Mods) und erneut beim
///               Szenen-Laden (spaet geladene Mods). Best-effort, nie fatal.
/// </file-summary>

using System;
using System.Collections.Generic;
using MelonLoader;

namespace gregCore.GameLayer.Patches.Hardware;

public static class IncompatibleModGuard
{
    // Erkennungsmerkmale des alten 404-Mods (Mod-Name oder Assembly-Name,
    // case-insensitiv, Teiltreffer). Eng gefasst — trifft nichts Eigenes
    // ("gregCore" kommt in keinem Marker vor).
    private static readonly string[] Markers =
    {
        "persistentid",
        "persistent id",
        "404nyanfound",
    };

    // Bereits behandelte Assembly-Namen (Sitzung): UnpatchSelf ist idempotent,
    // aber Log + Toast sollen nur einmal kommen.
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
                    try { asmName = melon.GetType()?.Assembly?.GetName()?.Name ?? ""; } catch { }
                    if (string.IsNullOrEmpty(modName) && string.IsNullOrEmpty(asmName)) continue;

                    // Eigene Assembly nie anfassen (Selbstschutz).
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
                    try { version = melon.Info?.Version ?? ""; } catch { }
                    try
                    {
                        var harmony = melon.HarmonyInstance;
                        if (harmony != null)
                        {
                            harmony.UnpatchSelf();
                            anyDisabled = true;
                            MelonLogger.Warning($"[gregCore][HwId] Inkompatibler ID-Mod erkannt " +
                                $"('{modName}' v{version}): Patches entfernt — gregID bleibt das einzige ID-System.");
                            try
                            {
                                gregCore.UI.GregNotificationManager.Show(
                                    $"Inkompatibler ID-Mod '{modName}' deaktiviert — gregID aktiv.",
                                    gregCore.UI.GregNotificationManager.GregToastType.Warning, 6f);
                            }
                            catch { }
                        }
                        else
                        {
                            MelonLogger.Msg($"[gregCore][HwId] '{modName}' erkannt, aber keine Harmony-Instanz " +
                                "(inert, nichts zu entpatchen).");
                        }
                    }
                    catch (Exception ex)
                    {
                        MelonLogger.Warning($"[gregCore][HwId] Entpatchen von '{modName}' fehlgeschlagen: " +
                            $"{ex.GetBaseException().Message}");
                    }
                }
                catch { }
            }
        }
        catch { }
        return anyDisabled;
    }
}
