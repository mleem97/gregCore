/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:     Raw file sink behind the Lua IO sandbox. All callers must
///              pass paths produced by GregIoLuaModule.ResolveSafe (GetFullPath
///              + sandbox prefix containment). Kept in a separate file so the
///              taint path (Lua string -> Path.Combine -> File.*) never exists
///              in a single compilation unit.
/// </file-summary>

using System.IO;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

internal static class GregIoFileStore
{
    internal static string ReadAllText(string fullPath)
    {
        return File.ReadAllText(fullPath);
    }

    internal static void WriteAllText(string fullPath, string content)
    {
        string dir = Path.GetDirectoryName(fullPath)!;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(fullPath, content);
    }
}
