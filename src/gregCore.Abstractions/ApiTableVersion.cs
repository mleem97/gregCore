/// <file-summary>
/// Layer:       Core
/// Purpose:      Holds the GameAPITable version number for FFI.
/// Maintainer:   Bump this version after every ABI change in GameApiTable.cs.
/// </file-summary>

namespace gregCore.Core.Models;

public static class ApiTableVersion
{
    public static int Current { get; } = 12;
}
