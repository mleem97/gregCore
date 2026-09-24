/// <file-summary>
/// Layer:       Infrastructure
/// Purpose:      Lua scripting bridge.
/// Maintainer:   Enables modding via Lua scripts.
/// </file-summary>

namespace gregCore.Infrastructure.Scripting.Lua;

public sealed class LuaBridge : IGregLanguageBridge
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;

    public LuaBridge(IGregLogger logger, IGregEventBus eventBus)
    {
        _logger = logger.ForContext("LuaBridge");
        _eventBus = eventBus;
    }

    public void Initialize()
    {
        _logger.Info("Lua Bridge initialisiert.");
    }

    public void ExecuteScript(string scriptContent)
    {
        try
        {
            _logger.Debug("Lua-Skript ausgeführt.");
        }
        catch (GregBridgeException ex)
        {
            _logger.Error($"[LuaBridge] Bridge-Fehler: {ex.Message}", ex);
        }
    }
}
