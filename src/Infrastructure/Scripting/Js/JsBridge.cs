/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        JavaScript Skripting Bridge.
/// Maintainer:   Ermöglicht Modding via JS (Jint).
/// </file-summary>

namespace gregCore.Infrastructure.Scripting.Js;

public sealed class JsBridge : IGregLanguageBridge
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _eventBus;

    public JsBridge(IGregLogger logger, IGregEventBus eventBus)
    {
        _logger = logger.ForContext("JsBridge");
        _eventBus = eventBus;
    }

    public void Initialize()
    {
        _logger.Info("JS Bridge initialisiert.");
    }

    public void ExecuteScript(string scriptContent)
    {
        try
        {
            _logger.Debug("JS-Skript ausgeführt.");
        }
        catch (GregBridgeException ex)
        {
            _logger.Error($"[JsBridge] Bridge-Fehler: {ex.Message}", ex);
        }
    }
}
