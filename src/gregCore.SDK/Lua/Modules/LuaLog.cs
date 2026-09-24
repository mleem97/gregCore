/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Absturzsicheres Loggen fuer Lua-Module: MelonLogger wirft
///               ausserhalb der Spiel-Laufzeit (z.B. Unit-Tests) beim
///               Loggen selbst - das darf niemals einen Mod-Aufruf sprengen.
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
