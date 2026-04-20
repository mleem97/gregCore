/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Definiert die GameAPITable für FFI Interop.
/// Maintainer:   ABI-KRITISCH! Neue Felder NUR ans Ende anhängen! Version erhöhen!
/// </file-summary>

namespace gregCore.Infrastructure.Ffi;

// ABI-KRITISCH: Dieses Struct definiert die binäre Schnittstelle
// zu allen nativen Mods (Rust, Go, C++).
// REGEL 1: Neue Felder NUR ans Ende anhängen — niemals reordnen!
// REGEL 2: Nach jeder Änderung ApiTableVersion erhöhen!
// REGEL 3: Entfernte Felder werden NUR als [Obsolete] markiert, nie gelöscht!

[StructLayout(LayoutKind.Sequential)]
public struct GameApiTable
{
    // [GREG_SYNC_REVIEW_REQUIRED]
    public IntPtr GetVersion;
    public IntPtr RegisterEventHandler;
    public IntPtr SendNetworkMessage;
}

public static class ApiTableGuard
{
    public static void AssertVersion(int expectedVersion)
    {
        if (ApiTableVersion.Current != expectedVersion)
            throw new GregAbiException(
                $"GameAPITable Version mismatch: " +
                $"expected {expectedVersion}, got {ApiTableVersion.Current}. " +
                $"Native mod muss neu kompiliert werden!");
    }
}
