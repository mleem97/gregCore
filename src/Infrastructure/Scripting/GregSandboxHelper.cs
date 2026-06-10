using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

public static class GregSandboxHelper
{
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        string normalizedFullPath = Path.GetFullPath(fullPath);
        string normalizedBaseDir = Path.GetFullPath(baseDir);

        if (!normalizedBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
            !normalizedBaseDir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        {
            normalizedBaseDir += Path.DirectorySeparatorChar;
        }

        return normalizedFullPath.StartsWith(normalizedBaseDir, StringComparison.OrdinalIgnoreCase);
    }
}
