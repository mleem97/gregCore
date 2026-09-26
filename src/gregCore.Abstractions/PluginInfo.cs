/// <file-summary>
/// Layer:       Core
/// Purpose:     Data model for the metadata of a loaded plugin.
/// Maintainer:  Pure DTO, serializer-agnostic.
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
