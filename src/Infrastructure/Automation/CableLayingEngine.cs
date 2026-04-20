using System.Collections;
using UnityEngine;

namespace gregCore.Infrastructure.Automation;

internal sealed class CableLayingEngine
{
    private readonly IGregLogger _logger;
    private readonly IGregEventBus _bus;

    internal CableLayingEngine(IGregLogger logger, IGregEventBus bus)
    {
        _logger = logger.ForContext(nameof(CableLayingEngine));
        _bus = bus;
    }

    internal IEnumerator LayCableCoroutine(string src, string tgt, CableType type, Action<AutomationResult> onComplete)
    {
        yield return null;
        var map = global::Il2Cpp.NetworkMap.instance;
        if (map == null)
        {
            onComplete(AutomationResult.Failure("NetworkMap nicht gefunden."));
            yield break;
        }

        try {
            map.Connect(src, tgt);
            _logger.Info($"Kabel verlegt: {src} -> {tgt} ({type})");
            onComplete(AutomationResult.Success(1));
        } catch (Exception ex) {
            onComplete(AutomationResult.Failure("Fehler beim Verlegen", ex.Message));
        }
    }

    internal IEnumerator LayStarTopologyCoroutine(string[] devices, string switchId, CableType type, IProgress<AutomationProgress>? progress, Action<AutomationResult> onComplete)
    {
        int done = 0;
        for (int i = 0; i < devices.Length; i++)
        {
            progress?.Report(new AutomationProgress { CurrentTask = $"Verbinde {devices[i]}", Current = i, Total = devices.Length });
            AutomationResult? res = null;
            yield return LayCableCoroutine(devices[i], switchId, type, r => res = r);
            if (res?.IsSuccess == true) done++;
            yield return null;
        }
        onComplete(AutomationResult.Partial(done, devices.Length));
    }
}