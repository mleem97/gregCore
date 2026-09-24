/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Spiel-Lautstärken-Brücke (Master, Musik, Effekte, Racks):
///               setzen plus Settings-Reload. Für Mods (z.B. MusicPlayer),
///               die Vanilla-Mixer respektieren wollen. Alles best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregGameSettings
{
    public static global::Il2Cpp.SettingsVolume FindVolumeSettings()
    {
        global::Il2Cpp.SettingsVolume found = null;
        Try(() =>
        {
            var all = Resources.FindObjectsOfTypeAll<global::Il2Cpp.SettingsVolume>();
            if (all == null) return;
            foreach (var s in all)
            {
                if (s == null) continue;
                try
                {
                    var go = s.gameObject;
                    if (go != null && go.scene.IsValid() && go.scene.isLoaded)
                    {
                        found = s;
                        break;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return found;
    }

    public static bool SetMasterVolume(float volume)
    {
        return SetVolume("Master", volume, (s, v) => s.MasterVolume(v));
    }

    public static bool SetMusicVolume(float volume)
    {
        return SetVolume("Music", volume, (s, v) => s.MusicVolume(v));
    }

    public static bool SetEffectVolume(float volume)
    {
        return SetVolume("Effect", volume, (s, v) => s.EffectVolume(v));
    }

    public static bool SetRacksVolume(float volume)
    {
        return SetVolume("Racks", volume, (s, v) => s.RacksVolume(v));
    }

    public static bool ReloadSettings()
    {
        var inst = FindVolumeSettings();
        if (inst == null) return false;
        try
        {
            var _ = inst.gameObject; // liveness
            inst.LoadSettings();
            return true;
        }
        catch (Exception ex)
        {
            Warn($"LoadSettings fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static bool SetVolume(string label, float volume, Action<global::Il2Cpp.SettingsVolume, float> setter)
    {
        var inst = FindVolumeSettings();
        if (inst == null || setter == null) return false;
        float clamped = Math.Max(0f, Math.Min(1f, volume));
        try
        {
            var _ = inst.gameObject; // liveness
            setter(inst, clamped);
            return true;
        }
        catch (Exception ex)
        {
            Warn($"{label}-Volume fehlgeschlagen: {Base(ex)}");
            return false;
        }
    }

    private static string Base(Exception ex)
    {
        try { return ex != null ? ex.GetBaseException().Message : "?"; } catch { return "?"; }
    }

    private static void Warn(string message)
    {
        try { MelonLogger.Warning($"[gregCore][Net] GameSettings: {message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] GameSettings-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
