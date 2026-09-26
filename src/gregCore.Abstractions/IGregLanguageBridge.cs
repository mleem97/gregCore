/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for scripting language bridges (Lua/JS).
/// Maintainer:   Allows adding new scripting languages without core changes.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregLanguageBridge
{
    void Initialize();
    void ExecuteScript(string scriptContent);
}
