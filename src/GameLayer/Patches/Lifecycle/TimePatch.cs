using gregCore.GameLayer.Hooks;

namespace gregCore.GameLayer.Patches.Lifecycle;

internal static class TimePatch
{
    private static int _lastDay = -1;

    internal static void OnUpdate(global::Il2Cpp.TimeController __instance)
    {
        try
        {
            int currentDay = __instance.day;
            if (_lastDay >= 0 && currentDay != _lastDay)
            {
                var payload = EventPayloadBuilder.ForGeneric("greg.lifecycle.DayEnded", new Dictionary<string, object>
                {
                    { "Day", currentDay },
                    { "PreviousDay", _lastDay }
                });
                
                // Emit for both canonical and legacy
                HookIntegration.Emit(HookName.Create("lifecycle", "DayEnded").ToString(), payload);
                HookIntegration.Emit(HookName.Create("system", "GameDayAdvanced").ToString(), payload);
            }
            _lastDay = currentDay;
        }
        catch (Exception ex)
        {
            HookIntegration.LogPatchError(nameof(OnUpdate), ex);
        }
    }
}

