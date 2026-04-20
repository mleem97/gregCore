/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Win32 FFI Implementierung für native Mods.
/// Maintainer:   Kapselt LoadLibrary, GetProcAddress und FreeLibrary. Thread-safe!
/// </file-summary>

namespace gregCore.Infrastructure.Ffi;

public sealed class Win32FfiBridge : IGregFfiBridge, IDisposable
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;
    private readonly List<IntPtr> _loadedModules = new();
    private readonly object _syncRoot = new();
    private bool _disposed;

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr LoadLibrary(string dllToLoad);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
    
    [DllImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool FreeLibrary(IntPtr hModule);

    public Win32FfiBridge(IGregLogger logger, IGregEventBus eventBus)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(eventBus);
        _logger = logger.ForContext("Win32FfiBridge");
        _eventBus = eventBus;
    }

    public void Initialize()
    {
        _logger.Info("Win32 FFI Bridge initialisiert.");
    }

    public void LoadNativeMod(string dllPath)
    {
        ArgumentNullException.ThrowIfNull(dllPath);
        if (!dllPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("DLL path must end with .dll", nameof(dllPath));

        lock (_syncRoot)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(Win32FfiBridge));
            
            try
            {
                var hModule = LoadLibrary(dllPath);
                if (hModule == IntPtr.Zero)
                {
                    int error = Marshal.GetLastWin32Error();
                    throw new GregBridgeException($"Konnte native Mod {dllPath} nicht laden. Win32Error: {error}");
                }

                _loadedModules.Add(hModule);
                _logger.Info($"Native Mod {dllPath} geladen.");
            }
            catch (GregBridgeException ex)
            {
                _logger.Error($"[Win32FfiBridge] Bridge-Fehler: {ex.Message}", ex);
            }
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        
        lock (_syncRoot)
        {
            foreach (var hModule in _loadedModules)
            {
                FreeLibrary(hModule);
            }
            _loadedModules.Clear();
        }
        
        _disposed = true;
    }
}
