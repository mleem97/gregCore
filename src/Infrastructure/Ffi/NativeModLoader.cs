/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Verwaltet den Lifecycle geladener nativer Mods.
/// Maintainer:   Kapselt FFI Calls und Fehlerbehandlung für natives Code.
/// </file-summary>

namespace gregCore.Infrastructure.Ffi;

public sealed class NativeModLoader
{
    private readonly IGregFfiBridge _ffiBridge;
    private readonly IGregLogger _logger;

    public NativeModLoader(IGregFfiBridge ffiBridge, IGregLogger logger)
    {
        _ffiBridge = ffiBridge;
        _logger = logger.ForContext("NativeModLoader");
    }

    public void LoadMods(IEnumerable<string> dllPaths)
    {
        foreach (var path in dllPaths)
        {
            try
            {
                _ffiBridge.LoadNativeMod(path);
            }
            catch (Exception ex)
            {
                _logger.Error($"Fehler beim Laden von {path}", ex);
            }
        }
    }
}
