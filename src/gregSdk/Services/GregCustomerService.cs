using System;
using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace greg.Sdk.Services;

/// <summary>
/// Full-featured Customer/App data service for gregIPAM.
/// Wraps CustomerBase IL2CPP interactions with null-safe accessors.
/// </summary>
public static class GregCustomerService
{
    public static List<CustomerBase> GetAllCustomerBases()
    {
        var result = new List<CustomerBase>();
        try
        {
            var all = UnityEngine.Object.FindObjectsOfType<CustomerBase>(true);
            foreach (var cb in all)
            {
                if (cb != null) result.Add(cb);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetAllCustomerBases failed: {ex.Message}");
        }
        return result;
    }

    public static CustomerBase GetByCustomerId(int customerId)
    {
        try
        {
            var all = UnityEngine.Object.FindObjectsOfType<CustomerBase>(true);
            foreach (var cb in all)
            {
                if (cb != null && cb.customerID == customerId) return cb;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetByCustomerId({customerId}) failed: {ex.Message}");
        }
        return null;
    }

    public static Dictionary<int, string> GetSubnetsPerApp(int customerId)
    {
        var result = new Dictionary<int, string>();
        try
        {
            var cb = GetByCustomerId(customerId);
            if (cb == null) return result;
            var native = cb.GetSubnetsPerApp();
            if (native == null) return result;
            foreach (var kvp in native)
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetSubnetsPerApp({customerId}) failed: {ex.Message}");
        }
        return result;
    }

    public static Dictionary<int, int> GetVlanIdsPerApp(int customerId)
    {
        var result = new Dictionary<int, int>();
        try
        {
            var cb = GetByCustomerId(customerId);
            if (cb == null) return result;
            var native = cb.GetVlanIdsPerApp();
            if (native == null) return result;
            foreach (var kvp in native)
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetVlanIdsPerApp({customerId}) failed: {ex.Message}");
        }
        return result;
    }

    public static Dictionary<int, string[]> GetUsableIpsPerApp(int customerId)
    {
        var result = new Dictionary<int, string[]>();
        try
        {
            var cb = GetByCustomerId(customerId);
            if (cb == null) return result;
            if (cb.usableIpsPerApp == null) return result;
            foreach (var kvp in cb.usableIpsPerApp)
            {
                var ips = new List<string>();
                if (kvp.Value != null)
                {
                    for (int i = 0; i < kvp.Value.Length; i++)
                    {
                        var ip = kvp.Value[i];
                        if (!string.IsNullOrWhiteSpace(ip))
                            ips.Add(ip);
                    }
                }
                result[kvp.Key] = ips.ToArray();
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetUsableIpsPerApp({customerId}) failed: {ex.Message}");
        }
        return result;
    }

    public static Dictionary<int, int> GetAppToServerType(int customerId)
    {
        var result = new Dictionary<int, int>();
        try
        {
            var cb = GetByCustomerId(customerId);
            if (cb == null) return result;
            if (cb.appIdToServerType == null) return result;
            foreach (var kvp in cb.appIdToServerType)
            {
                result[kvp.Key] = kvp.Value;
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetAppToServerType({customerId}) failed: {ex.Message}");
        }
        return result;
    }

    public static bool IsIpValidForCustomer(int customerId, string ip)
    {
        try
        {
            var cb = GetByCustomerId(customerId);
            return cb != null && cb.IsIPPresent(ip);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] IsIpValidForCustomer failed: {ex.Message}");
            return false;
        }
    }

    public static int GetAppIdForIp(int customerId, string ip)
    {
        try
        {
            var cb = GetByCustomerId(customerId);
            if (cb == null) return -1;
            return cb.GetAppIDForIP(ip);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetAppIdForIp failed: {ex.Message}");
            return -1;
        }
    }

    public static string GetSubnetForIp(int customerId, string ip)
    {
        try
        {
            var subnets = GetSubnetsPerApp(customerId);
            var appId = GetAppIdForIp(customerId, ip);
            if (appId >= 0 && subnets.TryGetValue(appId, out var cidr))
                return cidr;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetSubnetForIp failed: {ex.Message}");
        }
        return null;
    }

    public static int GetVlanForIp(int customerId, string ip)
    {
        try
        {
            var vlans = GetVlanIdsPerApp(customerId);
            var appId = GetAppIdForIp(customerId, ip);
            if (appId >= 0 && vlans.TryGetValue(appId, out var vlanId))
                return vlanId;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CustomerService] GetVlanForIp failed: {ex.Message}");
        }
        return 0;
    }

    public static float GetCurrentSpeed(int customerId)
    {
        try
        {
            var cb = GetByCustomerId(customerId);
            return cb?.currentSpeed ?? 0f;
        }
        catch { return 0f; }
    }

    public static float GetRequiredSpeed(int customerId)
    {
        try
        {
            var cb = GetByCustomerId(customerId);
            return cb?.currentTotalAppSpeeRequirements ?? 0f;
        }
        catch { return 0f; }
    }

    public static float GetSpeedSatisfaction(int customerId)
    {
        var required = GetRequiredSpeed(customerId);
        if (required <= 0.001f) return 1f;
        return GetCurrentSpeed(customerId) / required;
    }
}
