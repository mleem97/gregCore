/// <file-summary>
/// Schicht:      Core
/// Zweck:        Datenmodell für das Manifest eines Mods (mod.json).
/// Maintainer:   Reines DTO, serializer-agnostisch.
/// </file-summary>

namespace gregCore.Core.Models;

public record ModManifest
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = "1.0.0";
    public string PersistentId { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public IReadOnlyList<string> Dependencies { get; init; } = Array.Empty<string>();
}
