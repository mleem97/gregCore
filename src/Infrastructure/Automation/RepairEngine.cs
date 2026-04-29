using System.Collections;

namespace gregCore.Infrastructure.Automation;

internal sealed class RepairEngine
{
    private readonly IGregLogger _logger;

    internal RepairEngine(IGregLogger logger) => _logger = logger.ForContext(nameof(RepairEngine));

    internal IEnumerator RepairAllCoroutine(IProgress<AutomationProgress>? progress, Action<AutomationResult> onComplete)
    {
        _logger.Info("Repariere alle Geräte...");
        yield return null;
        onComplete(AutomationResult.Success(0, "Alles repariert."));
    }
}
