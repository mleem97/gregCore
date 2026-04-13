using MoonSharp.Interpreter;

namespace greg.Core.Scripting.Lua;

/// <summary>
/// Implement this interface to inject C#-backed API into every Lua <see cref="Script"/>
/// managed by <see cref="LuaLanguageBridge"/>. Modules are registered before scripts run.
/// </summary>
public interface iGregLuaModule
{
    /// <summary>
    /// Populate <paramref name="greg"/> (the <c>greg</c> global table) with functions and sub-tables
    /// that Lua scripts can call.
    /// </summary>
    void Register(Script vm, Table greg);
}


