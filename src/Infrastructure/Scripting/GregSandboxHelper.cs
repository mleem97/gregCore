using System;
using System.IO;

namespace gregCore.Infrastructure.Scripting
{
    public static class GregSandboxHelper
    {
        public static bool IsPathInsideDirectory(string fullPath, string baseDir)
        {
            string normalizedFull = Path.GetFullPath(fullPath).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string normalizedBase = Path.GetFullPath(baseDir).TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            normalizedBase += Path.DirectorySeparatorChar;

            return string.Equals(normalizedFull, normalizedBase.TrimEnd(Path.DirectorySeparatorChar), StringComparison.OrdinalIgnoreCase)
                   || normalizedFull.StartsWith(normalizedBase, StringComparison.OrdinalIgnoreCase);
        }
    }
}
