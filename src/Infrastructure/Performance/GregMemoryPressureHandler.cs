using System.Collections;
using UnityEngine;
using MelonLoader;

namespace gregCore.Infrastructure.Performance;

internal sealed class GregMemoryPressureHandler
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;
    private readonly PerformanceProfile _profile;
    private DateTime _lastGcRun = DateTime.MinValue;
    private DateTime _lastCriticalRun = DateTime.MinValue;

    internal GregMemoryPressureHandler(IGregLogger logger, IGregEventBus bus, PerformanceProfile profile)
    {
        _logger = logger.ForContext(nameof(GregMemoryPressureHandler));
        _bus = bus;
        _profile = profile;
        bus.Subscribe("greg.performance.RamWarning", OnRamWarning);
        bus.Subscribe("greg.performance.RamCritical", OnRamCritical);
    }

    internal void OnUpdate()
    {
        if (_profile.GcIntervalSeconds <= 0) return;
        var now = DateTime.UtcNow;
        if ((now - _lastGcRun).TotalSeconds < _profile.GcIntervalSeconds) return;
        _lastGcRun = now;
        GC.Collect(0, GCCollectionMode.Optimized, false);
    }

    private void OnRamWarning(EventPayload p)
    {
        _logger.Warning("[Memory] RAM Warning - Gen1 GC");
        Task.Run(() => { GC.Collect(1, GCCollectionMode.Optimized, false); });
    }

    private void OnRamCritical(EventPayload p)
    {
        // UNITY MAIN THREAD REQUIRED
        MelonCoroutines.Start(UnloadUnusedCoroutine());
    }

    private IEnumerator UnloadUnusedCoroutine()
    {
        var now = DateTime.UtcNow;
        if ((now - _lastCriticalRun).TotalSeconds < 30) yield break;
        _lastCriticalRun = now;

        _logger.Warning("[Memory] RAM KRITISCH - UnloadUnusedAssets + Full GC");

        var unloadOp = global::UnityEngine.Resources.UnloadUnusedAssets();
        while (!unloadOp.isDone) yield return null;

        _logger.Info("[Memory] UnloadUnusedAssets abgeschlossen");

        GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced, false, true);
        GC.WaitForPendingFinalizers();
        
        _bus.Publish("greg.performance.PressureCleanup", new EventPayload { OccurredAtUtc = DateTime.UtcNow, Data = new Dictionary<string, object>() });
    }
}
