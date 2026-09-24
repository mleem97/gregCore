/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Crash-safe logging for Lua modules: MelonLogger throws
///               outside the game runtime (e.g. unit tests) while
///               logging itself - that must never break a mod call.
/// </file-summary>

using MelonLoader;

namespace gregCore.Infrastructure.Scripting.Lua.Modules;

internal static class LuaLog
{
    internal static void Error(string message)
    {
        try { MelonLogger.Error(message); } catch { }
    }

    internal static void Warning(string message)
    {
        try { MelonLogger.Warning(message); } catch { }
    }
}
