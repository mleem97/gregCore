/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Mod contract: mods register with ID/name/version and their
///               menu IDs. Basis for dashboard, diagnostics ("which
///               mod holds which lock?") and settings-wide features.
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Simple registry over static state; logic covered by inspection.")]
public static class GregModRegistry
{
    public sealed class Entry
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        public string[] Menus { get; set; } = System.Array.Empty<string>();
    }

    private static readonly Dictionary<string, Entry> _mods =
        new Dictionary<string, Entry>(System.StringComparer.OrdinalIgnoreCase);

    public static void Register(string id, string name, string version, string[] menus)
    {
        if (string.IsNullOrWhiteSpace(id)) return;
        lock (_mods)
        {
            _mods[id] = new Entry
            {
                Id = id,
                Name = string.IsNullOrEmpty(name) ? id : name,
                Version = version ?? "",
                Menus = menus ?? System.Array.Empty<string>(),
            };
        }
        MelonLogger.Msg($"[gregCore][Mods] Registriert: {name} v{version} ({id}).");
    }

    public static IReadOnlyList<Entry> All()
    {
        lock (_mods) { return new List<Entry>(_mods.Values); }
    }
}
