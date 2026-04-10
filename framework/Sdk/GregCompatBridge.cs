using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using MelonLoader;

namespace gregFramework.Core;

/// <summary>
/// Resolves deprecated hook name spellings to canonical greg.* names.
/// Mappings are loaded from greg_hooks.json (hook.legacy → hook.name).
/// </summary>
public static class GregCompatBridge
{
    private static readonly object Sync = new();
    private static Dictionary<string, string> _legacyToCanonical = new(StringComparer.Ordinal);
    private static bool _loaded;

    /// <summary>Built-in redirects for older native-pipeline hook spellings (pre–GregNativeEventHooks).</summary>
    private static readonly Dictionary<string, string> NativeAliasToCanonical = BuildNativeAliases();

    private static Dictionary<string, string> BuildNativeAliases()
    {
        var d = new Dictionary<string, string>(StringComparer.Ordinal)
        {
            ["greg.PLAYER.MoneyChanged"] = GregNativeEventHooks.PlayerCoinChanged,
            ["greg.PLAYER.XpChanged"] = GregNativeEventHooks.PlayerXpChanged,
            ["greg.Economy.Balance.OnChanged"] = GregNativeEventHooks.PlayerCoinChanged,
            ["greg.Game.XP.OnGained"] = GregNativeEventHooks.PlayerXpChanged,
            ["greg.Customer.Reputation.OnChanged"] = GregNativeEventHooks.PlayerReputationChanged,
            ["greg.SERVER.PowerToggled"] = GregNativeEventHooks.ServerPowerButton,
            ["greg.SERVER.Degraded"] = GregNativeEventHooks.ServerItIsBroken,
            ["greg.SERVER.Repaired"] = GregNativeEventHooks.ServerDeviceRepaired,
            ["greg.SERVER.ClientAssigned"] = GregNativeEventHooks.ServerCustomerChanged,
            ["greg.RACK.DevicePlaced"] = GregNativeEventHooks.ServerInsertedInRack,
            ["greg.NETWORK.CableConnected"] = GregNativeEventHooks.ServerRegisterLink,
            ["greg.NETWORK.CableDisconnected"] = GregNativeEventHooks.ServerUnregisterLink,
            ["greg.RACK.Removed"] = GregNativeEventHooks.RackButtonUnmountRack,
            ["greg.NETWORK.LinkDown"] = GregNativeEventHooks.NetworkBrokenSwitchAdded,
            ["greg.NETWORK.LinkUp"] = GregNativeEventHooks.NetworkBrokenSwitchRemoved,
            ["greg.NETWORK.TrafficThreshold"] = GregNativeEventHooks.NetworkNetWatchDispatched,
            ["greg.UI.StoreCartCheckedOut"] = GregNativeEventHooks.SystemButtonCheckOut,
            ["greg.UI.StoreItemAdded"] = GregNativeEventHooks.SystemButtonBuyShopItem,
            ["greg.UI.StoreItemRemoved"] = GregNativeEventHooks.SystemSpawnedItemRemoved,
            ["greg.EMPLOYEE.Hired"] = GregNativeEventHooks.SystemButtonConfirmHire,
            ["greg.EMPLOYEE.Terminated"] = GregNativeEventHooks.SystemButtonConfirmFireEmployee,
            ["greg.Customer.ContractSigned"] = GregNativeEventHooks.SystemButtonCustomerChosen,
            ["greg.Customer.SlaRestored"] = GregNativeEventHooks.CustomerAppRequirementsSatisfied,
            ["greg.Customer.SlaBreached"] = GregNativeEventHooks.CustomerAppRequirementsFailed,
            ["greg.Game.Time.OnDayChanged"] = GregNativeEventHooks.SystemGameDayAdvanced,
            ["greg.Game.Time.OnMonthChanged"] = GregNativeEventHooks.SystemSnapshotSaved,
            ["greg.Game.Save.OnCompleted"] = GregNativeEventHooks.SystemGameSaved,
            ["greg.Game.Load.OnCompleted"] = GregNativeEventHooks.SystemGameLoaded,
            ["greg.Game.Save.OnRequested"] = GregNativeEventHooks.SystemAutoSaveRequested,
            ["greg.GAMEPLAY.RoomExpanded"] = GregNativeEventHooks.SystemButtonBuyWall,
            ["greg.Framework.Hooks.OnBridgeInstalled"] = GregNativeEventHooks.SystemHookBridgeInstalled,
            ["greg.Framework.Hooks.OnBridgeTriggered"] = GregNativeEventHooks.SystemHookBridgeTriggered,
            ["greg.Framework.Unknown.OnEvent"] = GregNativeEventHooks.UnknownNativeEvent,
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
