using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

// IL2CPP type alias: game uses 'Server' not 'NetworkServer'
using NetworkServer = Il2Cpp.Server;

namespace greg.Sdk.Services;

public enum AssignMode { Sequential, LowestFirst, Random }

/// <summary>
/// IP address management service with triple-fallback setter (direct → reflection → IL2CPP-interop).
/// Includes CIDR math, subnet validation, and next-free-IP logic.
/// </summary>
public static class GregIpService
{
    private static string _setIpMethod = "unknown";

    private static string GetServerProp(Server sv, string name)
    {
        if (sv == null) return "";
        try {
            var prop = sv.GetType().GetProperty(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop != null) return prop.GetValue(sv)?.ToString() ?? "";
            var field = sv.GetType().GetField(name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (field != null) return field.GetValue(sv)?.ToString() ?? "";
        } catch {}
        return "";
    }

    public static string GetIp(NetworkServer sv)
    {
        string ip = GetServerProp(sv, "ip");
        if (string.IsNullOrEmpty(ip)) ip = GetServerProp(sv, "IP");
        return ip;
    }

    public static bool HasIp(NetworkServer sv)
    {
        var ip = GetIp(sv);
        return !string.IsNullOrWhiteSpace(ip) && ip != "0.0.0.0";
    }

    public static bool SetIp(NetworkServer sv, string ip)
    {
        if (sv == null || string.IsNullOrWhiteSpace(ip)) return false;

        // Attempt 1: Reflection Setter (Property)
        try
        {
            var prop = sv.GetType().GetProperty("ip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(sv, ip);
                if (GetIp(sv) == ip) { _setIpMethod = "reflection-prop"; return true; }
            }
        }
        catch { }

        // Attempt 3: Reflection Setter (Field)
        try
        {
            var field = sv.GetType().GetField("ip", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
            {
                field.SetValue(sv, ip);
                if (GetIp(sv) == ip) { _setIpMethod = "reflection-field"; return true; }
            }
        }
        catch { }

        // Attempt 4: IL2CPP SetIP method / Interop
        try
        {
            var method = sv.GetType().GetMethod("SetIP", BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (method != null)
            {
                method.Invoke(sv, new object[] { ip });
                if (GetIp(sv) == ip) { _setIpMethod = "interop"; return true; }
            }
        }
        catch { }

        string sId = GetServerProp(sv, "serverId");
        if (string.IsNullOrEmpty(sId)) sId = GetServerProp(sv, "serverID");

        MelonLogger.Error($"[IpService] SetIp FAILED for {sId}: all methods exhausted");
        _setIpMethod = "FAILED";
        return false;
    }


    public static string GetSetIpMethod() => _setIpMethod;

    public static bool IsValidIp(string ip)
    {
        if (string.IsNullOrWhiteSpace(ip)) return false;
        var parts = ip.Split('.');
        if (parts.Length != 4) return false;
        foreach (var p in parts)
        {
            if (!int.TryParse(p, out int v) || v < 0 || v > 255) return false;
        }
        return true;
    }

    public static bool IsIpInSubnet(string ip, string cidr)
    {
        if (!IsValidIp(ip) || string.IsNullOrWhiteSpace(cidr)) return false;
        try
        {
            var parts = cidr.Split('/');
            if (parts.Length != 2) return false;
            if (!int.TryParse(parts[1], out int prefix)) return false;

            int ipInt = IpToInt(ip);
            int netInt = IpToInt(parts[0]);
            int mask = prefix == 0 ? 0 : (int)(0xFFFFFFFF << (32 - prefix));

            return (ipInt & mask) == (netInt & mask);
        }
        catch { return false; }
    }

    public static string GetNetworkAddress(string ip, int prefix)
    {
        int ipInt = IpToInt(ip);
        int mask = prefix == 0 ? 0 : (int)(0xFFFFFFFF << (32 - prefix));
        return IntToIp(ipInt & mask);
    }

    public static string GetNextFreeIp(string cidr, List<string> allowedPool,
                                        List<string> usedIps, AssignMode mode)
    {
        if (allowedPool == null || allowedPool.Count == 0) return null;

        var usedSet = new HashSet<string>(usedIps ?? new List<string>());
        var free = allowedPool.Where(ip => !usedSet.Contains(ip)).ToList();

        if (free.Count == 0)
        {
            MelonLogger.Warning($"[IpService] Pool exhausted for {cidr}");
            return null;
        }

        switch (mode)
        {
            case AssignMode.LowestFirst:
                free.Sort((a, b) => IpToInt(a).CompareTo(IpToInt(b)));
                return free[0];
            case AssignMode.Random:
                return free[UnityEngine.Random.Range(0, free.Count)];
            case AssignMode.Sequential:
            default:
                return free[0];
        }
    }

    public static int IpToInt(string ip)
    {
        var parts = ip.Split('.');
        return (int.Parse(parts[0]) << 24) |
               (int.Parse(parts[1]) << 16) |
               (int.Parse(parts[2]) << 8) |
               int.Parse(parts[3]);
    }

    public static string IntToIp(int ip)
    {
        return $"{(ip >> 24) & 0xFF}.{(ip >> 16) & 0xFF}.{(ip >> 8) & 0xFF}.{ip & 0xFF}";
    }
}

