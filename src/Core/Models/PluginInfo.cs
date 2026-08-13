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
    public string AssemblyName { get; init; } = string.Empty;
    public string AssemblyVersion { get; init; } = string.Empty;
    public string Sha256 { get; init; } = string.Empty;
    public IReadOnlyList<string> DeclaredDependencies { get; init; } = Array.Empty<string>();
    public string ScanStatus { get; init; } = "SCANNED";
    public string ModTypeName { get; init; } = string.Empty;
}
