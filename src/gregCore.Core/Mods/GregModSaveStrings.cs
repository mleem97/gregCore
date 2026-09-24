/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     String persistence on top of GregModSave (ModItemSaveData):
///              UTF-16 text chunked into saveIntArray, keyed by modFolderName.
///              Encode/Decode are pure managed logic (unit-tested); the Il2Cpp
///              list glue is best-effort like GregModSave and excluded from
///              coverage (needs the running game).
/// Maintainer:  UpsertText()/TryReadText() for per-save mod text (notes, ...).
/// </file-summary>

using System;
using System.Diagnostics.CodeAnalysis;

namespace gregCore.Core.Mods;

public static class GregModSaveStrings
{
    /// <summary>Default cap so one text entry never bloats the save.</summary>
    public const int DefaultMaxChars = 1800;

    // ── Pure managed codec (unit-tested) ─────────────────────────────────────

    public static int[] Encode(string? text, int maxChars = DefaultMaxChars)
    {
        try
        {
            if (string.IsNullOrEmpty(text)) return Array.Empty<int>();
            int cap = Math.Clamp(maxChars, 0, 8192);
            int len = Math.Min(text.Length, cap);
            var units = new int[len];
            for (int i = 0; i < len; i++) units[i] = text[i];
            return units;
        }
        catch { return Array.Empty<int>(); }
    }

    public static string Decode(int[]? units, int maxChars = DefaultMaxChars)
    {
        try
        {
            if (units == null || units.Length == 0) return "";
            int cap = Math.Clamp(maxChars, 0, 8192);
            int len = Math.Min(units.Length, cap);
            var chars = new char[len];
            for (int i = 0; i < len; i++)
            {
                int u = units[i];
                chars[i] = (u < 0 || u > 0xFFFF) ? '?' : (char)u;
            }
            return new string(chars);
        }
        catch { return ""; }
    }

    /// <summary>Packs a title + body pair with the "\n" convention.</summary>
    public static string CombineTitleBody(string? title, string? body)
    {
        try { return (title ?? "") + "\n" + (body ?? ""); }
        catch { return "\n"; }
    }

    /// <summary>Splits a <see cref="CombineTitleBody"/> pair. Never throws.</summary>
    public static void SplitTitleBody(string? text, out string title, out string body)
    {
        title = "";
        body = "";
        try
        {
            if (string.IsNullOrEmpty(text)) return;
            int nl = text.IndexOf('\n');
            if (nl < 0) { title = text; return; }
            title = text.Substring(0, nl);
            body = text.Substring(nl + 1);
        }
        catch { title = ""; body = ""; }
    }

    // ── Il2Cpp list glue (needs the game; excluded from coverage) ───────────

    [ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
    public static global::Il2Cpp.ModItemSaveData? UpsertText(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData>? list,
        string folder,
        string? text,
        int maxChars = DefaultMaxChars)
    {
        if (list == null || string.IsNullOrEmpty(folder)) return null;
        try
        {
            var dto = new GregModSave.ItemSave
            {
                ModFolderName = folder,
                SaveIntArray = Encode(text, maxChars),
            };
            return GregModSave.Upsert(list, dto);
        }
        catch { return null; }
    }

    [ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
    public static bool TryReadText(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.ModItemSaveData>? list,
        string folder,
        out string text)
    {
        text = "";
        if (list == null || string.IsNullOrEmpty(folder)) return false;
        try
        {
            var all = GregModSave.ReadAll(list);
            if (all == null) return false;
            foreach (var dto in all)
            {
                try
                {
                    if (dto == null) continue;
                    if (!string.Equals(dto.ModFolderName ?? "", folder, StringComparison.OrdinalIgnoreCase))
                        continue;
                    text = Decode(dto.SaveIntArray);
                    return true;
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }
}
