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

        // [GREG_SYNC_INSERT_PATCHES]

        SafePatch(harmony, typeof(Il2Cpp.Player), nameof(Il2Cpp.Player.UpdateCoin), typeof(gregCore.GameLayer.Patches.Economy.PlayerPatch), nameof(gregCore.GameLayer.Patches.Economy.PlayerPatch.OnCoinUpdated));
        
        // Block Pause-Menu when Console is open
        // NOTE: Interface patching via IUIActions might cause native crashes in Unity 6.
        // SafePatch(harmony, typeof(global::Il2Cpp.InputController.IUIActions), nameof(global::Il2Cpp.InputController.IUIActions.OnPause), typeof(gregCore.GameLayer.Patches.UI.InputPausePatch), nameof(gregCore.GameLayer.Patches.UI.InputPausePatch.Prefix), isPrefix: true);
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

    private static void SafePatch(HarmonyLib.Harmony harmony, Type targetType, string methodName, Type? patchType, string? patchMethod, bool isPrefix = false)
    {
        try
        {
            if (targetType == null)
            {
                _logger.Warning($"Patch-Ziel-Typ ist null für {methodName}");
                return;
            }

            var original = AccessTools.Method(targetType, methodName);
            if (original == null)
            {
                _logger.Warning($"Methode nicht gefunden: {targetType.FullName}.{methodName}");
                return;
            }

            if (patchType == null || string.IsNullOrEmpty(patchMethod))
            {
                _logger.Warning($"Patch-Implementierung fehlt für {methodName}");
                return;
            }

            var harmonyMethod = new HarmonyLib.HarmonyMethod(patchType, patchMethod);
            
            if (isPrefix)
                harmony.Patch(original, prefix: harmonyMethod);
            else
                harmony.Patch(original, postfix: harmonyMethod);

            _logger.Debug($"Patch installiert: {targetType.Name}.{methodName} ({(isPrefix ? "Prefix" : "Postfix")})");
        }
        catch (Exception ex)
        {
            _logger.Warning($"Patch fehlgeschlagen: {targetType?.Name}.{methodName} — {ex.Message}");
        }
    }
}
