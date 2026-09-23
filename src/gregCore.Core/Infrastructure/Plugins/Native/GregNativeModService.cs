/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Fassade über Spiel-ModLoader-Beobachtung und GregCore-native Plugins.
/// Maintainer:   Einzige Anlaufstelle für native Mod-Verwaltung.
/// </file-summary>

using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Plugins.Native;

public sealed class GregNativeModService : IGregNativeModService
{
    private readonly GameModLoaderBridge _bridge;
    private readonly GregNativePluginLoader _loader;
    private readonly IGregLogger _logger;
    private readonly string _nativeDir;

    public GregNativeModService(GameModLoaderBridge bridge, GregNativePluginLoader loader, IGregLogger logger, string nativeDir)
    {
        _bridge = bridge;
        _loader = loader;
        _logger = logger.ForContext("NativeModService");
        _nativeDir = nativeDir;
    }

    public GameModLoaderSnapshot RefreshGameSnapshot()
    {
        var snapshot = _bridge.Refresh();
        _logger.Info($"Game-ModLoader: verfügbar={snapshot.IsAvailable}, plugins={snapshot.LoadedPluginCount}");
        return snapshot;
    }

    public IReadOnlyList<GregNativePluginSnapshot> GetGregNativePlugins() => _loader.GetLoaded();

    public int LoadGregNativePlugins()
    {
        try
        {
            if (!Directory.Exists(_nativeDir)) Directory.CreateDirectory(_nativeDir);
            var count = _loader.LoadAll(_nativeDir);
            _logger.Info($"{count} Greg-native Plugins geladen ({_nativeDir}).");
            return count;
        }
        catch (Exception ex)
        {
            _logger.Error("Greg-native Plugin-Laden fehlgeschlagen", ex);
            return 0;
        }
    }

    public bool UnloadGregNativePlugin(string id) => _loader.Unload(id);

    public bool RequestGameReload() => _bridge.RequestReload();

    public void Shutdown() => _loader.Shutdown();
}
