/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Lokalisierungs-Brücke: Text per ID aus der Vanilla-
///               Sprachdatenbank lesen, Sprache wechseln. Alles best-effort.
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregLocalisation
{
    public static global::Il2Cpp.Localisation GetInstance()
    {
        try { return global::Il2Cpp.Localisation.instance; }
        catch { return null; }
    }

    public static string GetTextByID(int uid, string fallback)
    {
        var inst = GetInstance();
        if (inst == null) return fallback ?? "";
        try
        {
            string text = inst.ReturnTextByID(uid);
            return string.IsNullOrEmpty(text) ? (fallback ?? "") : text;
        }
        catch { return fallback ?? ""; }
    }

    public static bool ChangeLanguage(int languageUID)
    {
        var inst = GetInstance();
        if (inst == null) return false;
        try
        {
            inst.ChangeLocalisation(languageUID);
            return true;
        }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] ChangeLocalisation fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            return false;
        }
    }

    public static int GetLoadLanguageUID()
    {
        var inst = GetInstance();
        if (inst == null) return -1;
        try { return inst.loadLanguageUID; } catch { return -1; }
    }
}
