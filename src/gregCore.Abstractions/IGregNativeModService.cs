/// <file-summary>
/// Layer:       Core
/// Purpose:     Service contract for the extended native mod system.
/// Maintainer:  Observes Il2Cpp.ModLoader and manages GregCore-native DLL plugins.
/// </file-summary>

namespace gregCore.Core.Abstractions;

/// <summary>
/// Snapshot of a native plugin loaded in the game (Il2Cpp.IModPlugin).
/// </summary>
public record NativePluginSnapshot
{
    public string TypeName { get; init; } = string.Empty;
    public string AssemblyName { get; init; } = string.Empty;
}

/// <summary>
/// Snapshot of the game mod loader.
/// </summary>
public record GameModLoaderSnapshot
{
    public bool IsAvailable { get; init; }
    public int LoadedPluginCount { get; init; }
    public int NextModId { get; init; }
    public IReadOnlyList<NativePluginSnapshot> LoadedPlugins { get; init; } = Array.Empty<NativePluginSnapshot>();
}

/// <summary>
/// Snapshot of a native DLL plugin loaded by GregCore.
/// </summary>
public record GregNativePluginSnapshot
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string AssemblyPath { get; init; } = string.Empty;
}

public interface IGregNativeModService
{
    GameModLoaderSnapshot RefreshGameSnapshot();
    IReadOnlyList<GregNativePluginSnapshot> GetGregNativePlugins();
    int LoadGregNativePlugins();
    bool UnloadGregNativePlugin(string id);
    bool RequestGameReload();
}
