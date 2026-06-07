using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

public static class GregSandboxHelper
{
    /// <summary>
    /// Validates if a given full path is securely contained within the specified base directory,
    /// preventing prefix-matching path traversal bypasses.
    /// </summary>
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        string normalizedFull = Path.GetFullPath(fullPath);
        string normalizedBase = Path.GetFullPath(baseDir);

        if (!normalizedBase.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            normalizedBase += Path.DirectorySeparatorChar;
        }

        string rawBase = Path.GetFullPath(baseDir);
        if (normalizedFull.Equals(rawBase, StringComparison.OrdinalIgnoreCase))
            return true;

        return normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
    }
}
