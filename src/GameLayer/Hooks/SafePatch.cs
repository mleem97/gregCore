using System;
using HarmonyLib;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Basisklasse für alle Framework-Patches (Harmony Layer).
/// Stellt sicher, dass bei Fehlern im Patch das Spiel nicht abstürzt (Prefix returns true).
/// </summary>
public abstract class SafePatch
{
    protected static IGregLogger? _logger;
    protected static Core.Events.GregHookBus? _hookBus;

    public static void Setup(IGregLogger logger, Core.Events.GregHookBus hookBus)
    {
        _logger = logger.ForContext("HarmonyPatch");
        _hookBus = hookBus;
    }

    /// <summary>
    /// Sichere Methode zur Auslösung eines Hooks.
    /// </summary>
    protected static void TriggerHook(string hookName, params object[] data)
    {
        try
        {
            if (_hookBus == null) return;

            var payloadData = new Dictionary<string, object>
            {
                ["Trigger"] = "NativePatch"
            };

            for (int i = 0; i < data.Length; i += 2)
            {
                if (i + 1 < data.Length)
                    payloadData[data[i].ToString()!] = data[i + 1];
            }

            var payload = new Core.Models.EventPayload
            {
                HookName = hookName,
                OccurredAtUtc = DateTime.UtcNow,
                Data = payloadData,
                IsCancelable = false,
                IsCancelled = false
            };

            _hookBus.Dispatch(hookName, payload);
        }
        catch (Exception ex)
        {
            _logger?.Error($"Fehler beim Auslösen von Hook {hookName}", ex);
        }
    }
}
