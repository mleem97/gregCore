using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

// IL2CPP type alias: game uses 'Server' not 'NetworkServer'
using NetworkServer = Il2Cpp.Server;

namespace greg.Sdk.Services;

public class ServerInfo
{
    public string ServerId;
    public int CustomerId;
    public string Ip;
    public int ServerType;
    public bool IsOn;
    public bool IsBroken;
    public int RackPositionUID;
    public string RackId;
    public string SlotId;
    public int EolTime;
    public Server Instance;
}

/// <summary>
/// Discovers and catalogs all NetworkServer instances in the game world.
/// Uses label-based parsing for rack/slot identification (no GetComponentInParent).
/// </summary>
public static class GregServerDiscoveryService
{
    private static readonly Regex LabelRegex = new Regex(
        @"^(.*)-([A-Z]-\d{1,3}|[A-Z]\d{2,3})$",
        RegexOptions.Compiled);

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

    public static List<ServerInfo> ScanAll()
    {
        var result = new List<ServerInfo>();
        try
        {
            var servers = UnityEngine.Object.FindObjectsOfType<NetworkServer>(true);
            foreach (var sv in servers)
            {
                string sId = GetServerProp(sv, "serverId");
                if (string.IsNullOrEmpty(sId)) sId = GetServerProp(sv, "serverID");
                if (string.IsNullOrWhiteSpace(sId)) continue;

                string rackId = "", slotId = "";
                ParseLabel(sv.name, ref rackId, ref slotId);

                string cIdStr = GetServerProp(sv, "customerId");
                if (string.IsNullOrEmpty(cIdStr)) cIdStr = GetServerProp(sv, "customerID");
                int.TryParse(cIdStr, out int cId);

                string ip = GetServerProp(sv, "ip");
                if (string.IsNullOrEmpty(ip)) ip = GetServerProp(sv, "IP");

                result.Add(new ServerInfo
                {
                    ServerId = sId,
                    CustomerId = cId,
                    Ip = ip,
                    ServerType = sv.serverType,
                    IsOn = sv.isOn,
                    IsBroken = sv.isBroken,
                    RackPositionUID = sv.rackPositionUID,
                    RackId = rackId,
                    SlotId = slotId,
                    EolTime = sv.eolTime,
                    Instance = sv
                });
            }
            MelonLogger.Msg($"[ServerDiscovery] Scanned {result.Count} servers");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[ServerDiscovery] ScanAll failed: {ex.Message}");
        }
        return result;
    }


    public static ServerInfo GetById(string serverId)
    {
        try
        {
            var servers = UnityEngine.Object.FindObjectsOfType<NetworkServer>(true);
            foreach (var sv in servers)
            {
                string sId = GetServerProp(sv, "serverId");
                if (string.IsNullOrEmpty(sId)) sId = GetServerProp(sv, "serverID");

                if (sId == serverId)
                {
                    string rackId = "", slotId = "";
                    ParseLabel(sv.name, ref rackId, ref slotId);

                    string cIdStr = GetServerProp(sv, "customerId");
                    if (string.IsNullOrEmpty(cIdStr)) cIdStr = GetServerProp(sv, "customerID");
                    int.TryParse(cIdStr, out int cId);

                    string ip = GetServerProp(sv, "ip");
                    if (string.IsNullOrEmpty(ip)) ip = GetServerProp(sv, "IP");

                    return new ServerInfo
                    {
                        ServerId = sId,
                        CustomerId = cId,
                        Ip = ip,
                        ServerType = sv.serverType,
                        IsOn = sv.isOn,
                        IsBroken = sv.isBroken,
                        RackPositionUID = sv.rackPositionUID,
                        RackId = rackId,
                        SlotId = slotId,
                        EolTime = sv.eolTime,
                        Instance = sv
                    };
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[ServerDiscovery] GetById({serverId}) failed: {ex.Message}");
        }
        return null;
    }

    public static List<ServerInfo> GetByCustomer(int customerId)
    {
        var all = ScanAll();
        return all.FindAll(s => s.CustomerId == customerId);
    }

    public static List<ServerInfo> GetByRack(string rackId)
    {
        var all = ScanAll();
        return all.FindAll(s => s.RackId == rackId);
    }

    public static List<ServerInfo> GetOnline()
    {
        var all = ScanAll();
        return all.FindAll(s => s.IsOn && !s.IsBroken);
    }

    public static List<ServerInfo> GetWithoutIp()
    {
        var all = ScanAll();
        return all.FindAll(s => s.IsOn && !s.IsBroken && string.IsNullOrWhiteSpace(s.Ip));
    }

    public static List<ServerInfo> GetByEolRisk(int thresholdDays)
    {
        var all = ScanAll();
        return all.FindAll(s => s.EolTime <= thresholdDays);
    }

    private static void ParseLabel(string name, ref string rackId, ref string slotId)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        var match = LabelRegex.Match(name);
        if (match.Success)
        {
            rackId = match.Groups[1].Value;
            slotId = match.Groups[2].Value;
        }
    }
}

