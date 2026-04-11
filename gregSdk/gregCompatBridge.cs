using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using MelonLoader;

namespace gregFramework.gregCoreLoader;

/// <summary>
/// Resolves deprecated hook name spellings to canonical greg.* names.
/// Mappings are loaded from greg_hooks.json (hook.legacy → hook.name).
/// </summary>
public static class gregCompatBridge
{
    private static readonly object Sync = new();
    private static Dictionary<string, string> _legacyToCanonical = new(StringComparer.Ordinal);
    private static bool _loaded;

    /// <summary>Built-in redirects for older native-pipeline hook spellings (pre–gregNativeEventHooks).</summary>
    private static readonly Dictionary<string, string> NativeAliasToCanonical = BuildNativeAliases();

    private static Dictionary<string, string> BuildNativeAliases()
    {
        var d = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["greg.PLAYER.MoneyChanged"] = gregNativeEventHooks.PlayerCoinChanged,
            ["greg.PLAYER.XpChanged"] = gregNativeEventHooks.PlayerXpChanged,
            ["greg.Economy.Balance.OnChanged"] = gregNativeEventHooks.PlayerCoinChanged,
            ["greg.Game.XP.OnGained"] = gregNativeEventHooks.PlayerXpChanged,
            ["greg.Customer.Reputation.OnChanged"] = gregNativeEventHooks.PlayerReputationChanged,
            ["greg.SERVER.PowerToggled"] = gregNativeEventHooks.ServerPowerButton,
            ["greg.SERVER.Degraded"] = gregNativeEventHooks.ServerItIsBroken,
            ["greg.SERVER.Repaired"] = gregNativeEventHooks.ServerDeviceRepaired,
            ["greg.SERVER.ClientAssigned"] = gregNativeEventHooks.ServerCustomerChanged,
            ["greg.RACK.DevicePlaced"] = gregNativeEventHooks.ServerInsertedInRack,
            ["greg.NETWORK.CableConnected"] = gregNativeEventHooks.ServerRegisterLink,
            ["greg.NETWORK.CableDisconnected"] = gregNativeEventHooks.ServerUnregisterLink,
            ["greg.RACK.Removed"] = gregNativeEventHooks.RackButtonUnmountRack,
            ["greg.NETWORK.LinkDown"] = gregNativeEventHooks.NetworkBrokenSwitchAdded,
            ["greg.NETWORK.LinkUp"] = gregNativeEventHooks.NetworkBrokenSwitchRemoved,
            ["greg.NETWORK.TrafficThreshold"] = gregNativeEventHooks.NetworkNetWatchDispatched,
            ["greg.UI.StoreCartCheckedOut"] = gregNativeEventHooks.SystemButtonCheckOut,
            ["greg.UI.StoreItemAdded"] = gregNativeEventHooks.SystemButtonBuyShopItem,
            ["greg.UI.StoreItemRemoved"] = gregNativeEventHooks.SystemSpawnedItemRemoved,
            ["greg.EMPLOYEE.Hired"] = gregNativeEventHooks.SystemButtonConfirmHire,
            ["greg.EMPLOYEE.Terminated"] = gregNativeEventHooks.SystemButtonConfirmFireEmployee,
            ["greg.Customer.ContractSigned"] = gregNativeEventHooks.SystemButtonCustomerChosen,
            ["greg.Customer.SlaRestored"] = gregNativeEventHooks.CustomerAppRequirementsSatisfied,
            ["greg.Customer.SlaBreached"] = gregNativeEventHooks.CustomerAppRequirementsFailed,
            ["greg.Game.Time.OnDayChanged"] = gregNativeEventHooks.SystemGameDayAdvanced,
            ["greg.Game.Time.OnMonthChanged"] = gregNativeEventHooks.SystemSnapshotSaved,
            ["greg.Game.Save.OnCompleted"] = gregNativeEventHooks.SystemGameSaved,
            ["greg.Game.Load.OnCompleted"] = gregNativeEventHooks.SystemGameLoaded,
            ["greg.Game.Save.OnRequested"] = gregNativeEventHooks.SystemAutoSaveRequested,
            ["greg.GAMEPLAY.RoomExpanded"] = gregNativeEventHooks.SystemButtonBuyWall,
            ["greg.Framework.Hooks.OnBridgeInstalled"] = gregNativeEventHooks.SystemHookBridgeInstalled,
            ["greg.Framework.Hooks.OnBridgeTriggered"] = gregNativeEventHooks.SystemHookBridgeTriggered,
            ["greg.Framework.Unknown.OnEvent"] = gregNativeEventHooks.UnknownNativeEvent,
        };
        return d;
    }

    public static void EnsureLoaded(string jsonPath = null)
    {
        lock (Sync)
        {
            if (_loaded)
                return;

            var path = jsonPath ?? ResolveDefaultJsonPath();
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                MelonLogger.Warning("[gregCore] greg_hooks.json not found; legacy hook resolution disabled.");
                _loaded = true;
                return;
            }

            try
            {
                var text = File.ReadAllText(path);
                using var doc = JsonDocument.Parse(text);
                if (!doc.RootElement.TryGetProperty("hooks", out var hooks) || hooks.ValueKind != JsonValueKind.Array)
                {
                    _loaded = true;
                    return;
                }

                foreach (var el in hooks.EnumerateArray())
                {
                    if (!el.TryGetProperty("name", out var nameProp))
                        continue;
                    var canonical = nameProp.GetString();
                    if (string.IsNullOrWhiteSpace(canonical))
                        continue;
                    if (!el.TryGetProperty("legacy", out var legacyProp))
                        continue;
                    var legacy = legacyProp.GetString();
                    if (string.IsNullOrWhiteSpace(legacy))
                        continue;
                    _legacyToCanonical[legacy] = canonical;
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Warning($"[gregCore] Failed to parse greg_hooks.json: {ex.Message}");
            }

            _loaded = true;
        }
    }

    private static string ResolveDefaultJsonPath()
    {
        try
        {
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (string.IsNullOrEmpty(dir))
                return null;
            return Path.Combine(dir, "greg_hooks.json");
        }
        catch
        {
            return null;
        }
    }

    public static bool TryGetCanonical(string hookName, out string gregName)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(hookName))
        {
            gregName = null;
            return false;
        }

        lock (Sync)
        {
            if (NativeAliasToCanonical.TryGetValue(hookName, out var nativeMapped))
            {
                gregName = nativeMapped;
                return true;
            }
        }

        if (hookName.StartsWith("greg.", StringComparison.Ordinal))
        {
            gregName = hookName;
            return true;
        }

        lock (Sync)
        {
            if (_legacyToCanonical.TryGetValue(hookName, out var mapped))
            {
                gregName = mapped;
                return true;
            }
        }

        gregName = null;
        return false;
    }

    public static string Resolve(string hookName)
    {
        EnsureLoaded();
        if (string.IsNullOrWhiteSpace(hookName))
            return hookName;

        lock (Sync)
        {
            if (NativeAliasToCanonical.TryGetValue(hookName, out var nativeModern))
            {
                MelonLogger.Warning($"[gregCore] Deprecated hook id redirected to canonical '{nativeModern}'.");
                return nativeModern;
            }

            if (_legacyToCanonical.TryGetValue(hookName, out var modern))
            {
                MelonLogger.Warning($"[gregCore] Deprecated hook id redirected to canonical '{modern}'.");
                return modern;
            }
        }

        return hookName;
    }
}

