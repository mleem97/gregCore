/// <file-summary>
/// Layer:       Infrastructure (IO)
/// Purpose:     Central guard: no plugin/mod may load from a `.deactivated`
///              folder. Only gregCore itself may touch `.deactivated` content,
///              and only for explicit user activation (move out of
///              `.deactivated`, never load in place).
/// Maintainer:  Single source of truth for `.deactivated` detection.
/// </file-summary>

namespace gregCore.Infrastructure.IO;

public static class GregDeactivatedGuard
{
    public const string DeactivatedFolderName = ".deactivated";

    /// <summary>
    /// True when any path segment equals `.deactivated` (case-insensitive).
    /// Covers files and directories on Windows and Linux/Proton.
    /// </summary>
    public static bool IsDeactivatedPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        try
        {
            var segments = path.Split(
                new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar },
                StringSplitOptions.RemoveEmptyEntries);
            return segments.Any(s => string.Equals(
                s, DeactivatedFolderName, StringComparison.OrdinalIgnoreCase));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Defense-in-depth for loaders: returns false + logs when a deactivated
    /// path reaches a loader. Callers must skip the entry.
    /// </summary>
    public static bool ShouldSkip(string? path) => IsDeactivatedPath(path);

    /// <summary>
    /// Filters directory enumerations (files and folders).
    /// </summary>
    public static IEnumerable<string> ExcludeDeactivated(IEnumerable<string> paths)
    {
        if (paths == null) return Enumerable.Empty<string>();
        return paths.Where(p => !IsDeactivatedPath(p));
    }
}
