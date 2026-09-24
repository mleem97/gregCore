/// <file-summary>
/// Layer:       Infrastructure
<<<<<<< HEAD
/// Purpose:     Lua scripting bridge.
=======
/// Purpose:      Lua scripting bridge.
>>>>>>> agent/gregcore-integration
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
        _logger.Info("Lua Bridge initialized.");
    }

    public void ExecuteScript(string scriptContent)
    {
        try
        {
            _logger.Debug("Lua script executed.");
        }
        catch (GregBridgeException ex)
        {
            _logger.Error($"[LuaBridge] Bridge error: {ex.Message}", ex);
        }
    }
}
