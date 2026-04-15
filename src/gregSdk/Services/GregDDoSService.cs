using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

public static class GregDDoSService
{
    // CustomerID -> Mitigation Level
    // 0 = None, 1 = Hardware Firewall, 2 = LoadBalancer, 3 = External Cloudflare
    public static Dictionary<int, int> Mitigations = new();
    
    // CustomerID -> Attack Intensity
    public static Dictionary<int, float> ActiveAttacks = new();

    private static float _nextTick = 0f;

    public static void Update()
    {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + 5f; // Evaluate every 5 seconds

        // Randomly start an attack (rarely)
        if (UnityEngine.Random.value < 0.005f) // 0.5% chance every 5s
        {
            var customers = GregCustomerService.GetAllCustomerBases();
            if (customers.Count > 0)
            {
                var target = customers[UnityEngine.Random.Range(0, customers.Count)];
                float intensity = UnityEngine.Random.Range(1f, 5f);
                StartAttack(target.customerID, intensity);
            }
        }

        // Process active attacks
        var keys = new List<int>(ActiveAttacks.Keys);
        foreach (var cId in keys)
        {
            float intensity = ActiveAttacks[cId];
            int mitigation = GetMitigationLevel(cId);
            float effectiveIntensity = Mathf.Max(0f, intensity - mitigation);

            if (effectiveIntensity > 0)
            {
                // The attack bypasses mitigation
                var servers = GregServerDiscoveryService.GetByCustomer(cId);
                int knockedOff = 0;

                foreach(var srv in servers)
                {
                    if (srv.Instance != null && srv.Instance.isOn && !srv.Instance.isBroken)
                    {
                        // Chance to break server based on intensity
                        if (UnityEngine.Random.value < (effectiveIntensity * 0.02f))
                        {
                            srv.Instance.isBroken = true;
                            // Server doesn't have TurnOffCommonFunctions usually, we can invoke it safely if it exists, or just set isBroken.
                            // srv.Instance.TurnOffCommonFunctions();
                            knockedOff++;
                        }
                    }
                }

                if (knockedOff > 0)
                {
                    MelonLogger.Warning($"[DDoS] Attack on Customer {cId} knocked {knockedOff} servers offline!");
                }
            }

            // Gradually decrease attack intensity
            ActiveAttacks[cId] = intensity - 0.1f;
            if (ActiveAttacks[cId] <= 0f)
            {
                StopAttack(cId);
            }
        }
    }

    public static void StartAttack(int customerId, float intensity)
    {
        if (!ActiveAttacks.ContainsKey(customerId))
        {
            ActiveAttacks[customerId] = intensity;
            MelonLogger.Warning($"[DDoS] WARNING: DDoS attack started on Customer {customerId} with intensity {intensity:F1}");
            
            // Log differently if mitigated
            int mitigation = GetMitigationLevel(customerId);
            if (mitigation >= intensity)
            {
                MelonLogger.Msg($"[DDoS] Attack on Customer {customerId} was automatically mitigated by Level {mitigation} defenses.");
            }
        }
    }

    public static void StopAttack(int customerId)
    {
        if (ActiveAttacks.ContainsKey(customerId))
        {
            ActiveAttacks.Remove(customerId);
            MelonLogger.Msg($"[DDoS] Attack on Customer {customerId} has subsided.");
        }
    }

    public static void PurchaseMitigation(int customerId, int level)
    {
        Mitigations[customerId] = level;
        MelonLogger.Msg($"[DDoS] Customer {customerId} acquired Mitigation Level {level}");
    }

    public static int GetMitigationLevel(int customerId)
    {
        return Mitigations.TryGetValue(customerId, out var level) ? level : 0;
    }
}
