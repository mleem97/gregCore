using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

public static class GregSandboxHelper
{
    /// <summary>
    /// Checks if a given path is inside the base directory, avoiding prefix-matching path traversal vulnerabilities.
    /// </summary>
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        string normalizedFullPath = Path.GetFullPath(fullPath);
        string normalizedBaseDir = Path.GetFullPath(baseDir);

        if (!normalizedBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
            !normalizedBaseDir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        {
            normalizedBaseDir += Path.DirectorySeparatorChar;
        }

        return normalizedFullPath.StartsWith(normalizedBaseDir, StringComparison.OrdinalIgnoreCase) ||
               string.Equals(normalizedFullPath, Path.GetFullPath(baseDir), StringComparison.OrdinalIgnoreCase);
    }
}
