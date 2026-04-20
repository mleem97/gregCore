/// <file-summary>
/// Schicht:      Core
/// Zweck:        Enthält die Versionsnummer der GameAPITable für FFI.
/// Maintainer:   Nach jeder ABI-Änderung in GameApiTable.cs muss diese Version erhöht werden.
/// </file-summary>

namespace gregCore.Core.Models;

public static class ApiTableVersion
{
    public const int Current = 12;
}
