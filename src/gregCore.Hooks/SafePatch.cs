using System;
using HarmonyLib;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Base class for all framework patches (Harmony layer).
/// Ensures the game does not crash on patch errors (prefix returns true).
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
    /// Safe method for raising a hook.
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
            _logger?.Error($"Error triggering hook {hookName}", ex);
        }
    }
}
