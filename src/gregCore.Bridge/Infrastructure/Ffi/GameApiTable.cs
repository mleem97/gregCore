/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Defines the GameAPITable for FFI interop.
/// Maintainer:  ABI-CRITICAL! Append new fields ONLY at the end! Bump the version!
/// </file-summary>

namespace gregCore.Infrastructure.Ffi;

// ABI-CRITICAL: This struct defines the binary interface
// to all native mods (Rust, Go, C++).
// RULE 1: Append new fields ONLY at the end — never reorder!
// RULE 2: Bump ApiTableVersion after every change!
// RULE 3: Removed fields are ONLY marked as [Obsolete], never deleted!

[StructLayout(LayoutKind.Sequential)]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "S1104:Make this field private and encapsulate it in a public property", Justification = "ABI-CRITICAL FFI struct: fields must stay sequential public fields for native interop. See RULE 1-3 above.")]
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
