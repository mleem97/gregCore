/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Manages the lifecycle of loaded native mods.
/// Maintainer:  Encapsulates FFI calls and error handling for native code.
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
            // Guard: niemals aus `.deactivated` laden.
            if (global::gregCore.Infrastructure.IO.GregDeactivatedGuard.IsDeactivatedPath(path))
            {
                _logger.Warning($"Native mod uebersprungen (deaktiviert): {path}");
                continue;
            }
            try
            {
                _ffiBridge.LoadNativeMod(path);
            }
            catch (Exception ex)
            {
                _logger.Error($"Error loading {path}", ex);
            }
        }
    }
}
