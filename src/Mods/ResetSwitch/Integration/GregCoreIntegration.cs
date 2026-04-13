using System.Linq;
using System.Reflection;
using MelonLoader;

namespace greg.Mods.ResetSwitch.Integration;

public static class GregCoreIntegration
{
    public static bool IsPresent { get; private set; }
    private static MethodInfo _fireMethod;

    public static void TryInit()
    {
        IsPresent = MelonMod.RegisteredMelons.Any(m => m.Info.Name == "gregCore");
        if (IsPresent)
        {
            MelonLogger.Msg("[greg.Mods.ResetSwitch] gregCore detected. Integration active.");
            ResolveHookBus();
        }
    }

    public static bool FireBeforeReset(string switchId, string switchName, string rackId, string reason)
    {
        if (!IsPresent)
        {
            return true;
        }

        var payload = $"{{\"switchId\":\"{switchId}\",\"switchName\":\"{switchName}\",\"rackId\":\"{rackId}\",\"reason\":\"{reason}\"}}";
        return Fire("greg.Mods.ResetSwitch.BeforeReset", payload);
    }

    public static void FireSwitchReset(string switchId, string switchName, string rackId, string backupPath)
    {
        if (!IsPresent)
        {
            return;
        }

        var payload = $"{{\"switchId\":\"{switchId}\",\"switchName\":\"{switchName}\",\"rackId\":\"{rackId}\",\"timestamp\":\"{System.DateTime.UtcNow:O}\",\"backupPath\":\"{backupPath ?? string.Empty}\"}}";
        Fire("greg.Mods.ResetSwitch.SwitchReset", payload);
    }

    public static void FireScanComplete(int total, int noFlow, int degraded, int ok)
    {
        if (!IsPresent)
        {
            return;
        }

        var payload = $"{{\"total\":{total},\"noFlow\":{noFlow},\"degraded\":{degraded},\"ok\":{ok}}}";
        Fire("greg.Mods.ResetSwitch.ScanComplete", payload);
    }

    public static void FireBackupCreated(string backupPath, int switchCount)
    {
        if (!IsPresent)
        {
            return;
        }

        var payload = $"{{\"backupPath\":\"{backupPath}\",\"switchCount\":{switchCount}}}";
        Fire("greg.Mods.ResetSwitch.BackupCreated", payload);
    }

    private static void ResolveHookBus()
    {
        try
        {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                var type = assembly.GetType("greg.Core.Hooks.GregHookBus")
                           ?? assembly.GetType("GregCore.Hooks.GregHookBus")
                           ?? assembly.GetType("gregCore.Hooks.GregHookBus");

                if (type == null)
                {
                    continue;
                }

                _fireMethod = type.GetMethod("Fire", BindingFlags.Public | BindingFlags.Static);
                if (_fireMethod != null)
                {
                    MelonLogger.Msg("[greg.Mods.ResetSwitch] GregHookBus.Fire resolved.");
                    return;
                }
            }
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[greg.Mods.ResetSwitch] HookBus resolve failed: {ex.Message}");
        }
    }

    private static bool Fire(string eventName, string payload)
    {
        if (_fireMethod == null)
        {
            ResolveHookBus();
        }

        if (_fireMethod == null)
        {
            MelonLogger.Msg($"[greg.Mods.ResetSwitch] Hook fallback: {eventName} {payload}");
            return true;
        }

        try
        {
            var parameters = _fireMethod.GetParameters();
            object result;

            if (parameters.Length == 2)
            {
                result = _fireMethod.Invoke(null, new object[] { eventName, payload });
            }
            else if (parameters.Length == 1)
            {
                result = _fireMethod.Invoke(null, new object[] { eventName });
            }
            else
            {
                result = _fireMethod.Invoke(null, null);
            }

            if (result is bool boolResult)
            {
                return boolResult;
            }

            return true;
        }
        catch (System.Exception ex)
        {
            MelonLogger.Warning($"[greg.Mods.ResetSwitch] Hook fire failed for {eventName}: {ex.Message}");
            return true;
        }
    }
}
