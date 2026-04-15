using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2Cpp;
using HarmonyLib;

namespace greg.Sdk.Services;

/// <summary>
/// Background service that monitors network health and prevents "No Traffic" bugs
/// by ensuring native game caches (NetworkMap) are invalidated when components change.
/// </summary>
public static class GregNetworkSanityService
{
    private static float _nextAutoFix = 0f;
    private const float AUTO_FIX_INTERVAL = 60f; // Every minute check for deadlocks

    public static void Update()
    {
        if (Time.time < _nextAutoFix) return;
        _nextAutoFix = Time.time + AUTO_FIX_INTERVAL;

        // Auto-fix if we detect total traffic collapse (all customers unsatisfied)
        CheckAndFixNetworkDeadlock();
    }

    private static void CheckAndFixNetworkDeadlock()
    {
        var customers = GregCustomerService.GetAllCustomerBases();
        if (customers.Count == 0) return;

        bool allFailing = true;
        foreach (var c in customers)
        {
            if (c.currentSpeed > 0.1f)
            {
                allFailing = false;
                break;
            }
        }

        if (allFailing)
        {
            MelonLogger.Warning("[GregNetworkSanity] DETECTED TOTAL NETWORK COLLAPSE! Applying emergency RebuildMap...");
            GregResetSwitchService.ForceNetworkRebuild("auto_recovery_collapse");
        }
    }
}

/// <summary>
/// Proactive patches to prevent the "No Traffic" bug at its source.
/// </summary>
[HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SetVlanDisallowed))]
internal static class Patch_NetworkSwitch_VlanChanged
{
    static void Postfix(NetworkSwitch __instance)
    {
        // Whenever a VLAN is blocked, we MUST rebuild the map or traffic might hang
        GregResetSwitchService.ForceNetworkRebuild("vlan_block");
    }
}

[HarmonyPatch(typeof(NetworkSwitch), nameof(NetworkSwitch.SetVlanAllowed))]
internal static class Patch_NetworkSwitch_VlanAllowed
{
    static void Postfix(NetworkSwitch __instance)
    {
        GregResetSwitchService.ForceNetworkRebuild("vlan_allow");
    }
}
