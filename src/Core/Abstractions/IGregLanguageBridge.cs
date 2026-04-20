/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für Skript-Sprachen-Brücken (Lua/JS).
/// Maintainer:   Ermöglicht das Hinzufügen neuer Skriptsprachen ohne Core-Änderung.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregLanguageBridge
{
    void Initialize();
    void ExecuteScript(string scriptContent);
}
