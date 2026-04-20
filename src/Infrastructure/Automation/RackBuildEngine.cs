using System.Collections;
using UnityEngine;

namespace gregCore.Infrastructure.Automation;

internal sealed class RackBuildEngine
{
    private readonly IGregLogger _logger;
    private readonly CableLayingEngine _cabling;

    internal RackBuildEngine(IGregLogger logger, CableLayingEngine cabling)
    {
        _logger = logger.ForContext(nameof(RackBuildEngine));
        _cabling = cabling;
    }

    internal IEnumerator BuildRackCoroutine(RackBuildConfig config, IProgress<AutomationProgress>? progress, Action<AutomationResult> onComplete)
    {
        _logger.Info($"Baue Rack {config.RackId}...");
        yield return null;
        
        // Placeholder for installation logic
        int count = config.Servers.Length;
        onComplete(AutomationResult.Success(count, $"Rack {config.RackId} erfolgreich bestückt."));
    }
}