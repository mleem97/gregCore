using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting
{
    public static class GregSandboxHelper
    {
        public static bool IsPathInsideDirectory(string fullPath, string baseDir)
        {
            string normalizedFullPath = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string normalizedBaseDir = Path.GetFullPath(baseDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            return normalizedFullPath.Equals(normalizedBaseDir, StringComparison.OrdinalIgnoreCase) ||
                   normalizedFullPath.StartsWith(normalizedBaseDir + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
        }
    }
}
