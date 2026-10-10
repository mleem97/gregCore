/// <file-summary>
/// Layer:       Core
/// Purpose:     Data model for a mod manifest (mod.json).
/// Maintainer:  Pure DTO, serializer-agnostic.
/// </file-summary>

namespace gregCore.Core.Models;

public record ModManifest
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = "1.0.0";
    public string PersistentId { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public string Entrypoint { get; init; } = string.Empty;
    public string ApiVersion { get; init; } = "1.0.0";
    public string Loader { get; init; } = "MelonLoader";
    public IReadOnlyList<string> Dependencies { get; init; } = Array.Empty<string>();
}
