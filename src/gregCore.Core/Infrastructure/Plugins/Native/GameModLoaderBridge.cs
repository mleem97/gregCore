/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Beobachtet den spiel-eigenen Il2Cpp.ModLoader (DllEntry/IModPlugin-System).
/// Maintainer:   Nur lesender Zugriff + LoadAllMods-Reload. Keine privaten Methoden per Reflection.
/// </file-summary>

using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Plugins.Native;

public sealed class GameModLoaderBridge
{
    private readonly IGregLogger _logger;

    public GameModLoaderBridge(IGregLogger logger)
    {
        _logger = logger.ForContext("GameModLoaderBridge");
    }

    public bool TryGetLoader(out global::Il2Cpp.ModLoader loader)
    {
        loader = null!;
        try
        {
            var instance = global::Il2Cpp.ModLoader.instance;
            if (instance == null || instance.Pointer == IntPtr.Zero) return false;
            loader = instance;
            return true;
        }
        catch (Exception ex)
        {
            _logger.Warning($"ModLoader.instance nicht verfügbar: {ex.Message}");
            return false;
        }
    }

    public GameModLoaderSnapshot Refresh()
    {
        if (!TryGetLoader(out var loader))
            return new GameModLoaderSnapshot { IsAvailable = false };

        var snapshots = new List<NativePluginSnapshot>();
        var nextModId = -1;
        try
        {
            var plugins = loader.loadedPlugins;
            if (plugins != null)
            {
                foreach (var plugin in plugins)
                {
                    try
                    {
                        if (plugin == null || plugin.Pointer == IntPtr.Zero) continue;
                        var managedType = plugin.GetType();
                        snapshots.Add(new NativePluginSnapshot
                        {
                            TypeName = managedType.FullName ?? "unknown",
                            AssemblyName = managedType.Assembly.GetName().Name ?? "unknown"
                        });
                    }
                    catch { /* einzelnes Plugin überspringen */ }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Warning($"loadedPlugins konnten nicht gelesen werden: {ex.Message}");
        }

        try { nextModId = loader.nextModID; }
        catch { /* optional */ }

        return new GameModLoaderSnapshot
        {
            IsAvailable = true,
            LoadedPluginCount = snapshots.Count,
            NextModId = nextModId,
            LoadedPlugins = snapshots
        };
    }

    /// <summary>
    /// Bittet den Spiel-Loader, alle ModPacks erneut zu laden.
    /// Lädt Mods zum Spielstart-Zeitpunkt; kein echtes Hot-Reload einzelner DLLs.
    /// </summary>
    public bool RequestReload()
    {
        if (!TryGetLoader(out var loader)) return false;
        try
        {
            loader.LoadAllMods();
            _logger.Info("Game-ModLoader Reload angefordert (LoadAllMods).");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error("Game-ModLoader Reload fehlgeschlagen", ex);
            return false;
        }
    }
}
