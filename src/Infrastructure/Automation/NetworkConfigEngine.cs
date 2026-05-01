using System.Collections;

namespace gregCore.Infrastructure.Automation;

internal sealed class NetworkConfigEngine
{
    private readonly IGregLogger _logger;

    internal NetworkConfigEngine(IGregLogger logger) => _logger = logger.ForContext(nameof(NetworkConfigEngine));

    internal IEnumerator SetupNetworkSegmentCoroutine(NetworkSegmentConfig config, IProgress<AutomationProgress>? progress, Action<AutomationResult> onComplete)
    {
        _logger.Info("Konfiguriere Netzwerk-Segment...");
        yield return null;
        onComplete(AutomationResult.Success(config.Devices.Length));
    }
}
