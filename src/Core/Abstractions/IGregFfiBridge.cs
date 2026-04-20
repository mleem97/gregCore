/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für Foreign Function Interface (Win32 FFI).
/// Maintainer:   Lädt und bindet native Bibliotheken (C++/Rust).
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregFfiBridge
{
    void Initialize();
    void LoadNativeMod(string dllPath);
}
