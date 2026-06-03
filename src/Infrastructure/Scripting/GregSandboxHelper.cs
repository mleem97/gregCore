using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

/// <summary>
/// Helper methods for sandbox security and path validation.
/// </summary>
public static class GregSandboxHelper
{
    /// <summary>
    /// Validates if a given full path truly resides within the base directory.
    /// This prevents path traversal vulnerabilities and prefix-matching bypasses
    /// (e.g., baseDir="/app/data", fullPath="/app/data-secret/file").
    /// </summary>
    /// <param name="fullPath">The fully resolved path to check.</param>
    /// <param name="baseDir">The fully resolved base directory.</param>
    /// <returns>True if fullPath is within baseDir, false otherwise.</returns>
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        if (string.IsNullOrWhiteSpace(fullPath) || string.IsNullOrWhiteSpace(baseDir))
            return false;

        string normalizedFull = Path.GetFullPath(fullPath);
        string normalizedBase = Path.GetFullPath(baseDir);

        // Ensure the base directory string ends with a directory separator
        // so that a simple StartsWith check does not succeed on prefix matches.
        if (!normalizedBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            normalizedBase += Path.DirectorySeparatorChar;
        }

        // An exact match of the directory itself is allowed, or it must start with the base directory + separator
        return normalizedFull.Equals(Path.GetFullPath(baseDir), StringComparison.OrdinalIgnoreCase) ||
               normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
    }
}
