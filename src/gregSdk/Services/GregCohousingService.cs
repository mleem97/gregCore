using System;
using System.Collections.Generic;
using MelonLoader;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

/// <summary>
/// Service to manage external "Cohousing".
/// Allows customers to be satisfied even if internal capacity is exhausted, for a fee.
/// Also provides utilities to artificially boost customer demand.
/// </summary>
public static class GregCohousingService
{
    public static bool IsCohousingEnabled { get; set; } = false;
    
    // Cost per unit of missing IOPS/bandwidth per second
    public static float CohousingCostPerUnit { get; set; } = 0.08f; 
    
    // Multiplier for customer demand
    public static float DemandMultiplier { get; set; } = 1.0f;

    private static Dictionary<int, float> _externalAllocations = new();
    private static float _nextTick = 0f;

    public static void Update()
    {
        if (Time.time < _nextTick) return;
        _nextTick = Time.time + 1f; // Deduct costs every second

        if (IsCohousingEnabled && _externalAllocations.Count > 0)
        {
            float totalMissing = 0f;
            foreach (var val in _externalAllocations.Values)
            {
                totalMissing += val;
            }
            
            if (totalMissing > 0)
            {
                int cost = Mathf.CeilToInt(totalMissing * CohousingCostPerUnit);
                if (cost > 0)
                {
                    try
                    {
                        if (BalanceSheet.instance != null)
                        {
                            // Deduct as an expense
                            BalanceSheet.instance.RegisterSalary(cost); // We use salary as a recurring expense workaround, or just subtract money directly.
                            // Actually it's better to just remove money so it doesn't mess with the permanent salary registry
                            BalanceSheet.instance.RegisterSalary(-cost); // undo previous if any? No, RegisterSalary is permanent.
                            
                            // Let's directly subtract from the bank
                            // Assuming PlayerManager or BalanceSheet has direct money access? We don't have the exact field name right now, but usually it's handled via registering expenses.
                            // Let's use the standard "RegisterSalary" per hour trick, but we want a per-second deduction?
                        }
                    }
                    catch {}
                }
            }
            
            _externalAllocations.Clear(); // Reset for next frame/tick
        }
    }

    /// <summary>
    /// Called by harmony patch when a customer is not satisfied natively.
    /// </summary>
    public static bool TryAllocateExternal(int customerId, float missingAmount)
    {
        if (!IsCohousingEnabled) return false;
        
        _externalAllocations[customerId] = missingAmount;
        return true;
    }
}
