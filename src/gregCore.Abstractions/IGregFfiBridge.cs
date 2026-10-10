/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for Foreign Function Interface (Win32 FFI).
/// Maintainer:   Loads and binds native libraries (C++/Rust).
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregFfiBridge
{
    void Initialize();
    void LoadNativeMod(string dllPath);
}
