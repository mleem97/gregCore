/// <file-summary>
/// Schicht:      Core
/// Zweck:        Datenmodell für die Metadaten eines geladenen Plugins.
/// Maintainer:   Reines DTO, serializer-agnostisch.
/// </file-summary>

namespace gregCore.Core.Models;

public record PluginInfo
{
    public string AssemblyPath { get; init; } = string.Empty;
    public ModManifest Manifest { get; init; } = new();
    public bool IsNative { get; init; }
}
