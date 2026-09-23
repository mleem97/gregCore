/// <file-summary>
/// Schicht:      Core
/// Zweck:        Service-Vertrag für das erweiterte native Mod-System.
/// Maintainer:   Beobachtet Il2Cpp.ModLoader und verwaltet GregCore-native DLL-Plugins.
/// </file-summary>

namespace gregCore.Core.Abstractions;

/// <summary>
/// Snapshot eines im Spiel geladenen nativen Plugins (Il2Cpp.IModPlugin).
/// </summary>
public record NativePluginSnapshot
{
    public string TypeName { get; init; } = string.Empty;
    public string AssemblyName { get; init; } = string.Empty;
}

/// <summary>
/// Snapshot des Spiel-ModLoaders.
/// </summary>
public record GameModLoaderSnapshot
{
    public bool IsAvailable { get; init; }
    public int LoadedPluginCount { get; init; }
    public int NextModId { get; init; }
    public IReadOnlyList<NativePluginSnapshot> LoadedPlugins { get; init; } = Array.Empty<NativePluginSnapshot>();
}

/// <summary>
/// Snapshot eines von GregCore geladenen nativen DLL-Plugins.
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
