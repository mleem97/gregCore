/// <file-summary>
/// Schicht:      GameLayer
/// Zweck:        Bindet Harmony-Patches an den IGregEventBus.
/// Maintainer:   Kennt alle Patch-Klassen, installiert sie via Harmony.
/// </file-summary>

using HarmonyLib;

namespace gregCore.GameLayer.Hooks;

internal static class HookIntegration
{
    private static IGregEventBus _bus = null!;
    private static IGregLogger _logger = null!;

    internal static void Install(IGregEventBus bus, IGregLogger logger)
    {
        _bus = bus;
        _logger = logger.ForContext("HookIntegration");

        var harmony = new HarmonyLib.Harmony("com.teamgreg.gregcore");

        SafePatch(harmony, typeof(Il2Cpp.Player), nameof(Il2Cpp.Player.UpdateCoin), typeof(gregCore.GameLayer.Patches.Economy.PlayerPatch), nameof(gregCore.GameLayer.Patches.Economy.PlayerPatch.OnCoinUpdated));
    }

    internal static void Emit(HookName hook, EventPayload payload)
    {
        try { _bus.Publish(hook.Full, payload); }
        catch (Exception ex)
        {
            _logger.Error($"Emit fehlgeschlagen für {hook.Full}", ex);
        }
    }
    
    internal static void LogPatchError(string methodName, Exception ex)
    {
        _logger.Error($"Patch-Ausführung fehlgeschlagen in {methodName}", ex);
    }

    private static void SafePatch(HarmonyLib.Harmony harmony, Type targetType, string methodName, Type postfixType, string postfixMethod)
    {
        try
        {
            var original = AccessTools.Method(targetType, methodName);
            var postfix = new HarmonyLib.HarmonyMethod(postfixType, postfixMethod);
            harmony.Patch(original, postfix: postfix);
            _logger.Debug($"Patch installiert: {targetType.Name}.{methodName}");
        }
        catch (Exception ex)
        {
            _logger.Warning($"Patch fehlgeschlagen: {targetType.Name}.{methodName} — {ex.Message}");
        }
    }
}
