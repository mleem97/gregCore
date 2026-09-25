/// <file-summary>
/// Layer:      UI
/// Purpose:     Key-HUD registry (gregCore kit). Mods report their
///              hotkeys ("F9" -> "Music"), GregHud renders them as
///              key bar on the right edge (like the game hints).
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace gregCore.UI;

[ExcludeFromCodeCoverage(Justification = "Static registry over in-memory state; needs running game for UI.")]
public static class GregHudRegistry
{
    public sealed class Entry
    {
        public string ModId { get; set; }
        public string Key { get; set; }
        public string Label { get; set; }
    }

    private static readonly Dictionary<string, Entry> _entries =
        new Dictionary<string, Entry>(System.StringComparer.OrdinalIgnoreCase);

    // key e.g. "F9", label e.g. "Music". One entry per mod (update possible).
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
