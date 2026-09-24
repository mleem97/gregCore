/// <file-summary>
/// Schicht:      UI
/// Zweck:        Tasten-HUD-Registry (gregCore Baukasten). Mods melden ihre
///               Hotkeys an ("F9" -> "Music"), GregHud rendert sie als
///               Tastenleiste am rechten Rand (wie die Spiel-Hints).
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Static registry over in-memory state; needs running game for UI.")]
public static class GregHudRegistry
{
    public sealed class Entry
    {
        public string ModId;
        public string Key;
        public string Label;
    }

    private static readonly Dictionary<string, Entry> _entries =
        new Dictionary<string, Entry>(System.StringComparer.OrdinalIgnoreCase);

    // key z.B. "F9", label z.B. "Music". Pro Mod ein Eintrag (Update moeglich).
    public static void Register(string modId, string key, string label)
    {
        if (string.IsNullOrWhiteSpace(modId) || string.IsNullOrWhiteSpace(key)) return;
        lock (_entries)
        {
            _entries[modId] = new Entry
            {
                ModId = modId,
                Key = key.Trim(),
                Label = string.IsNullOrWhiteSpace(label) ? modId : label.Trim(),
            };
        }
        try { GregHud.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static void Unregister(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId)) return;
        lock (_entries) { _entries.Remove(modId); }
        try { GregHud.Refresh(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    public static IReadOnlyList<Entry> All()
    {
        lock (_entries)
        {
            var list = new List<Entry>(_entries.Values);
            list.Sort((a, b) => string.Compare(a.Key, b.Key, System.StringComparison.OrdinalIgnoreCase));
            return list;
        }
    }
}
