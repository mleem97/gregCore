using System.Collections;
using UnityEngine;
using MelonLoader;

namespace gregCore.Infrastructure.Automation;

internal sealed class DeliveryZoneEngine
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;

    internal DeliveryZoneEngine(IGregLogger logger, IGregEventBus bus)
    {
        _logger = logger.ForContext(nameof(DeliveryZoneEngine));
        _bus = bus;
    }

    internal IEnumerator ProcessAllCoroutine(IProgress<AutomationProgress>? progress, Action<AutomationResult> onComplete)
    {
        _logger.Debug("Starte Lieferzonen-Verarbeitung...");
        yield return null;

        var items = FindItems();
        if (items.Count == 0)
        {
            onComplete(AutomationResult.Success(0, "Keine Items gefunden."));
            yield break;
        }

        int processed = 0;
        foreach (var item in items)
        {
            progress?.Report(new AutomationProgress { CurrentTask = $"Verarbeite {item.name}", Current = processed, Total = items.Count });
            yield return null; // Spread over frames
            
            // Logic placeholder: Move item
            _logger.Debug($"Verschiebe Item: {item.name}");
            processed++;
        }

        onComplete(AutomationResult.Success(processed, $"{processed} Items aus Lieferzone verarbeitet."));
    }

    private List<global::Il2Cpp.Item> FindItems()
    {
        var result = new List<global::Il2Cpp.Item>();
        var objects = global::UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.Item>();
        if (objects == null) return result;
        
        foreach (var obj in objects)
        {
            // Simple logic: Items near certain coords or with certain names
            result.Add(obj);
        }
        return result;
    }
}