using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

public static class GregSandboxHelper
{
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        string normalizedFull = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;
        string normalizedBase = Path.GetFullPath(baseDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar) + Path.DirectorySeparatorChar;

        return normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
    }
}
