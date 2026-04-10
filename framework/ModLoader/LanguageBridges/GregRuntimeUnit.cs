namespace DataCenterModLoader.LanguageBridges;

/// <summary>
/// Describes one runtime-loadable mod unit across language bridges.
/// </summary>
public sealed class GregRuntimeUnit
{
    public string Id { get; init; } = string.Empty;

    public string DisplayName { get; init; } = string.Empty;

    public string Language { get; init; } = string.Empty;

    public bool Enabled { get; init; }

    public bool SupportsHotReload { get; init; }

    public bool IsNativeModule { get; init; }
}
