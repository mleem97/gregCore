using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting;

public static class GregSandboxHelper
{
    public static bool IsPathInsideDirectory(string fullPath, string baseDir)
    {
        string normalizedBaseDir = baseDir;
        if (!normalizedBaseDir.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
            !normalizedBaseDir.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
        {
            normalizedBaseDir += Path.DirectorySeparatorChar;
        }

        return fullPath.StartsWith(normalizedBaseDir, StringComparison.OrdinalIgnoreCase);
    }
}
