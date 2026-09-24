/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Platform-neutral file enumeration (Proton/Wine/Linux ↔ Windows).
/// Maintainer:  Windows filesystems are case-insensitive, Linux/Proton paths are not —
///               "*.dll" won't find "MOD.DLL" there. Always use this helper.
/// </file-summary>

namespace gregCore.Infrastructure.IO;

public static class GregFileSystem
{
    public static IEnumerable<string> EnumerateFilesByExtension(
        string directory, string extension, SearchOption option = SearchOption.TopDirectoryOnly,
        bool includeDeactivated = false)
    {
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return Enumerable.Empty<string>();
        if (!extension.StartsWith(".", StringComparison.Ordinal)) extension = "." + extension;
        try
        {
            var files = Directory.EnumerateFiles(directory, "*", option)
                .Where(f => f.EndsWith(extension, StringComparison.OrdinalIgnoreCase));
            if (!includeDeactivated)
                files = files.Where(f => !GregDeactivatedGuard.IsDeactivatedPath(f));
            return files
                .OrderBy(x => x, StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }

    public static IEnumerable<string> EnumerateDirectories(
        string directory, bool includeDeactivated = false)
    {
        if (string.IsNullOrWhiteSpace(directory) || !Directory.Exists(directory))
            return Enumerable.Empty<string>();
        try
        {
            var dirs = Directory.EnumerateDirectories(directory).AsEnumerable();
            if (!includeDeactivated)
                dirs = dirs.Where(d => !GregDeactivatedGuard.IsDeactivatedPath(d));
            return dirs.OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToArray();
        }
        catch
        {
            return Enumerable.Empty<string>();
        }
    }
}
