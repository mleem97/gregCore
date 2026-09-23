/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Plattform-neutrale Datei-Enumeration (Proton/Wine/Linux ↔ Windows).
/// Maintainer:   Windows-Dateisysteme sind case-insensitiv, Linux/Proton-Pfade nicht —
///               "*.dll" findet dort "MOD.DLL" nicht. Immer diesen Helper nutzen.
/// </file-summary>

namespace gregCore.Infrastructure.IO;

public static class GregFileSystem
{
    public static IEnumerable<string> EnumerateFilesByExtension(
        string directory, string extension, SearchOption option = SearchOption.TopDirectoryOnly)
    {
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return Enumerable.Empty<string>();
        if (!extension.StartsWith(".", StringComparison.Ordinal)) extension = "." + extension;
        try
        {
            return Directory.EnumerateFiles(directory, "*", option)
                .Where(f => f.EndsWith(extension, StringComparison.OrdinalIgnoreCase))
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}
