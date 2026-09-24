/// <file-summary>
/// Schicht:      Core (Networking)
/// Zweck:        Save-DTOs für Netzwerk-Devices (Router, Firewall, SFP, LACP,
///               Kabel, Endpunkte, Link-Labels, Port-VLAN-Filter): Erzeugen,
///               Fuellen, Lesen. Upsert nur wo ein Schlüssel existiert
///               (Cable.cableID, LACP.groupId); Listen sonst positionsbasiert.
///               Alles best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static class GregNetworkDeviceSaves
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class SubnetRoute
    {
        public int SourceVlanId;
        public string SubnetCidr = "";
        public int TargetVlanId;
        public string TargetIp = "";
    }

    public sealed class OwnedSubnet
    {
        public int VlanId;
        public string SubnetCidr = "";
    }

    public sealed class RouteEntry
    {
        public int RouteId;
        public int SourceVlanId;
        public string SourceIp = "";
        public int TargetVlanId;
        public string TargetIp = "";
    }

    public sealed class RouterSave
    {
        public int Asn;
        public int NextRouteId;
        public List<SubnetRoute> RoutingTable = new List<SubnetRoute>();
        public List<OwnedSubnet> OwnedSubnets = new List<OwnedSubnet>();
        public List<RouteEntry> Routes = new List<RouteEntry>();
    }

    public sealed class FilterRule
    {
        public int PortIndex;
        public string SourceIpCidr = "";
        public string DestIpCidr = "";
        public int NetworkPort;
        public string Protocol = "";
        public bool Bidirectional;
        public bool Allow;
    }

    public sealed class FirewallSave
    {
        public string ClusterIP = "";
        public List<FilterRule> FilterRules = new List<FilterRule>();
    }

    public sealed class SfpSave
    {
        public int PrefabID;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool IsInserted;
        public Vector3 PortPosition;
    }

    public sealed class LacpSave
    {
        public int GroupId;
        public string DeviceA = "";
        public string DeviceB = "";
        public List<int> CableIds = new List<int>();
    }

    public sealed class CableEndpoint
    {
        public string Type = "";
        public Vector3 Position;
        public int CustomerID;
        public string SwitchID = "";
        public string ServerID = "";
    }

    public sealed class CableSave
    {
        public int CableID;
        public CableEndpoint StartPoint = new CableEndpoint();
        public CableEndpoint EndPoint = new CableEndpoint();
        public List<Vector3> Waypoints = new List<Vector3>();
        public List<Vector3> MidPointPositions = new List<Vector3>();
        public float MaxSpeed;
        public Color CableColor = Color.white;
    }

    public sealed class CableLinkLabel
    {
        public Vector3 Position;
        public string LabelText = "";
    }

    public sealed class PortVlanFilter
    {
        public int PortIndex;
        public List<int> DisallowedVlanIds = new List<int>();
    }

    // ── Router ───────────────────────────────────────────────────────────────

    public static global::Il2Cpp.RouterSaveData CreateRouter(RouterSave dto)
    {
        global::Il2Cpp.RouterSaveData entry = null;
        try { entry = new global::Il2Cpp.RouterSaveData(); } catch { return null; }
        FillRouter(entry, dto);
        return entry;
    }

    public static void FillRouter(global::Il2Cpp.RouterSaveData entry, RouterSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.asn = dto.Asn);
        Try(() => entry.nextRouteId = dto.NextRouteId);
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.Router.SubnetRoute>();
            foreach (var r in dto.RoutingTable ?? new List<SubnetRoute>())
            {
                try
                {
                    var e = new global::Il2Cpp.Router.SubnetRoute();
                    e.sourceVlanId = r.SourceVlanId;
                    e.subnetCidr = r.SubnetCidr ?? "";
                    e.targetVlanId = r.TargetVlanId;
                    e.targetIp = r.TargetIp ?? "";
                    list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.routingTable = list;
        });
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.Router.OwnedSubnet>();
            foreach (var s in dto.OwnedSubnets ?? new List<OwnedSubnet>())
            {
                try
                {
                    var e = new global::Il2Cpp.Router.OwnedSubnet();
                    e.vlanId = s.VlanId;
                    e.subnetCidr = s.SubnetCidr ?? "";
                    list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.ownedSubnets = list;
        });
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.Router.RouteEntry>();
            foreach (var r in dto.Routes ?? new List<RouteEntry>())
            {
                try
                {
                    var e = new global::Il2Cpp.Router.RouteEntry();
                    e.routeId = r.RouteId;
                    e.sourceVlanId = r.SourceVlanId;
                    e.sourceIp = r.SourceIp ?? "";
                    e.targetVlanId = r.TargetVlanId;
                    e.targetIp = r.TargetIp ?? "";
                    list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.routes = list;
        });
    }

    public static RouterSave ReadRouter(global::Il2Cpp.RouterSaveData entry)
    {
        var dto = new RouterSave();
        if (entry == null) return dto;
        Try(() => dto.Asn = entry.asn);
        Try(() => dto.NextRouteId = entry.nextRouteId);
        Try(() =>
        {
            var list = entry.routingTable;
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null) continue;
                var r = new SubnetRoute();
                try { r.SourceVlanId = e.sourceVlanId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.SubnetCidr = e.subnetCidr ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.TargetVlanId = e.targetVlanId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.TargetIp = e.targetIp ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.RoutingTable.Add(r);
            }
        });
        Try(() =>
        {
            var list = entry.ownedSubnets;
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null) continue;
                var s = new OwnedSubnet();
                try { s.VlanId = e.vlanId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { s.SubnetCidr = e.subnetCidr ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.OwnedSubnets.Add(s);
            }
        });
        Try(() =>
        {
            var list = entry.routes;
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null) continue;
                var r = new RouteEntry();
                try { r.RouteId = e.routeId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.SourceVlanId = e.sourceVlanId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.SourceIp = e.sourceIp ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.TargetVlanId = e.targetVlanId; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.TargetIp = e.targetIp ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.Routes.Add(r);
            }
        });
        return dto;
    }

    // ── Firewall ─────────────────────────────────────────────────────────────

    public static global::Il2Cpp.FirewallSaveData CreateFirewall(FirewallSave dto)
    {
        global::Il2Cpp.FirewallSaveData entry = null;
        try { entry = new global::Il2Cpp.FirewallSaveData(); } catch { return null; }
        FillFirewall(entry, dto);
        return entry;
    }

    public static void FillFirewall(global::Il2Cpp.FirewallSaveData entry, FirewallSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.clusterIP = dto.ClusterIP ?? "");
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<global::Il2Cpp.Firewall.FilterRule>();
            foreach (var r in dto.FilterRules ?? new List<FilterRule>())
            {
                try
                {
                    var e = new global::Il2Cpp.Firewall.FilterRule();
                    e.portIndex = r.PortIndex;
                    e.sourceIpCidr = r.SourceIpCidr ?? "";
                    e.destIpCidr = r.DestIpCidr ?? "";
                    e.networkPort = r.NetworkPort;
                    if (Enum.TryParse<global::Il2Cpp.Firewall.Protocol>(r.Protocol, true, out var p))
                        e.protocol = p;
                    e.bidirectional = r.Bidirectional;
                    e.allow = r.Allow;
                    list.Add(e);
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.filterRules = list;
        });
    }

    public static FirewallSave ReadFirewall(global::Il2Cpp.FirewallSaveData entry)
    {
        var dto = new FirewallSave();
        if (entry == null) return dto;
        Try(() => dto.ClusterIP = entry.clusterIP ?? "");
        Try(() =>
        {
            var list = entry.filterRules;
            if (list == null) return;
            foreach (var e in list)
            {
                if (e == null) continue;
                var r = new FilterRule();
                try { r.PortIndex = e.portIndex; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.SourceIpCidr = e.sourceIpCidr ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.DestIpCidr = e.destIpCidr ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.NetworkPort = e.networkPort; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.Protocol = e.protocol.ToString(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.Bidirectional = e.bidirectional; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                try { r.Allow = e.allow; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                dto.FilterRules.Add(r);
            }
        });
        return dto;
    }

    // ── SFP ──────────────────────────────────────────────────────────────────

    public static global::Il2Cpp.SFPSaveData CreateSfp(SfpSave dto)
    {
        global::Il2Cpp.SFPSaveData entry = null;
        try { entry = new global::Il2Cpp.SFPSaveData(); } catch { return null; }
        FillSfp(entry, dto);
        return entry;
    }

    public static void FillSfp(global::Il2Cpp.SFPSaveData entry, SfpSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.prefabID = dto.PrefabID);
        Try(() => entry.position = dto.Position);
        Try(() => entry.rotation = dto.Rotation);
        Try(() => entry.isInserted = dto.IsInserted);
        Try(() => entry.portPosition = dto.PortPosition);
    }

    public static SfpSave ReadSfp(global::Il2Cpp.SFPSaveData entry)
    {
        var dto = new SfpSave();
        if (entry == null) return dto;
        Try(() => dto.PrefabID = entry.prefabID);
        Try(() => dto.Position = entry.position);
        Try(() => dto.Rotation = entry.rotation);
        Try(() => dto.IsInserted = entry.isInserted);
        Try(() => dto.PortPosition = entry.portPosition);
        return dto;
    }

    // ── LACP (Key: groupId) ──────────────────────────────────────────────────

    public static global::Il2Cpp.LACPGroupSaveData CreateLacp(LacpSave dto)
    {
        global::Il2Cpp.LACPGroupSaveData entry = null;
        try { entry = new global::Il2Cpp.LACPGroupSaveData(); } catch { return null; }
        FillLacp(entry, dto);
        return entry;
    }

    public static void FillLacp(global::Il2Cpp.LACPGroupSaveData entry, LacpSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.groupId = dto.GroupId);
        Try(() => entry.deviceA = dto.DeviceA ?? "");
        Try(() => entry.deviceB = dto.DeviceB ?? "");
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<int>();
            foreach (var id in dto.CableIds ?? new List<int>())
            {
                try { list.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.cableIds = list;
        });
    }

    public static LacpSave ReadLacp(global::Il2Cpp.LACPGroupSaveData entry)
    {
        var dto = new LacpSave();
        if (entry == null) return dto;
        Try(() => dto.GroupId = entry.groupId);
        Try(() => dto.DeviceA = entry.deviceA ?? "");
        Try(() => dto.DeviceB = entry.deviceB ?? "");
        Try(() =>
        {
            var list = entry.cableIds;
            if (list == null) return;
            foreach (var id in list)
            {
                try { dto.CableIds.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return dto;
    }

    public static global::Il2Cpp.LACPGroupSaveData UpsertLacp(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.LACPGroupSaveData> list, LacpSave dto)
    {
        if (list == null || dto == null) return null;
        global::Il2Cpp.LACPGroupSaveData found = null;
        Try(() =>
        {
            foreach (var entry in list)
            {
                if (entry == null) continue;
                int gid = -1;
                try { gid = entry.groupId; } catch { continue; }
                if (gid == dto.GroupId) { found = entry; break; }
            }
        });
        if (found != null) { FillLacp(found, dto); return found; }
        var created = CreateLacp(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    // ── Kabel (Key: cableID) ─────────────────────────────────────────────────

    public static global::Il2Cpp.CableEndpointSaveData CreateEndpoint(CableEndpoint dto)
    {
        global::Il2Cpp.CableEndpointSaveData entry = null;
        try { entry = new global::Il2Cpp.CableEndpointSaveData(); } catch { return null; }
        FillEndpoint(entry, dto);
        return entry;
    }

    public static void FillEndpoint(global::Il2Cpp.CableEndpointSaveData entry, CableEndpoint dto)
    {
        if (entry == null || dto == null) return;
        Try(() =>
        {
            if (Enum.TryParse<global::Il2Cpp.CableLink.TypeOfLink>(dto.Type, true, out var t))
                entry.type = t;
        });
        Try(() => entry.position = dto.Position);
        Try(() => entry.customerID = dto.CustomerID);
        Try(() => entry.switchID = dto.SwitchID ?? "");
        Try(() => entry.serverID = dto.ServerID ?? "");
    }

    public static CableEndpoint ReadEndpoint(global::Il2Cpp.CableEndpointSaveData entry)
    {
        var dto = new CableEndpoint();
        if (entry == null) return dto;
        Try(() => dto.Type = entry.type.ToString());
        Try(() => dto.Position = entry.position);
        Try(() => dto.CustomerID = entry.customerID);
        Try(() => dto.SwitchID = entry.switchID ?? "");
        Try(() => dto.ServerID = entry.serverID ?? "");
        return dto;
    }

    public static global::Il2Cpp.CableSaveData CreateCable(CableSave dto)
    {
        global::Il2Cpp.CableSaveData entry = null;
        try { entry = new global::Il2Cpp.CableSaveData(); } catch { return null; }
        FillCable(entry, dto);
        return entry;
    }

    public static void FillCable(global::Il2Cpp.CableSaveData entry, CableSave dto)
    {
        if (entry == null || dto == null) return;
        Try(() => entry.cableID = dto.CableID);
        Try(() => entry.startPoint = CreateEndpoint(dto.StartPoint ?? new CableEndpoint()));
        Try(() => entry.endPoint = CreateEndpoint(dto.EndPoint ?? new CableEndpoint()));
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<Vector3>();
            foreach (var v in dto.Waypoints ?? new List<Vector3>())
            {
                try { list.Add(v); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.waypoints = list;
        });
        Try(() =>
        {
            var list = new Il2CppSystem.Collections.Generic.List<Vector3>();
            foreach (var v in dto.MidPointPositions ?? new List<Vector3>())
            {
                try { list.Add(v); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
            entry.midPointPositions = list;
        });
        Try(() => entry.maxSpeed = dto.MaxSpeed);
        Try(() => entry.cableColor = dto.CableColor);
    }

    public static CableSave ReadCable(global::Il2Cpp.CableSaveData entry)
    {
        var dto = new CableSave();
        if (entry == null) return dto;
        Try(() => dto.CableID = entry.cableID);
        Try(() => dto.StartPoint = ReadEndpoint(entry.startPoint));
        Try(() => dto.EndPoint = ReadEndpoint(entry.endPoint));
        Try(() =>
        {
            var list = entry.waypoints;
            if (list == null) return;
            foreach (var v in list)
            {
                try { dto.Waypoints.Add(v); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        Try(() =>
        {
            var list = entry.midPointPositions;
            if (list == null) return;
            foreach (var v in list)
            {
                try { dto.MidPointPositions.Add(v); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        Try(() => dto.MaxSpeed = entry.maxSpeed);
        Try(() => dto.CableColor = entry.cableColor);
        return dto;
    }

    public static global::Il2Cpp.CableSaveData UpsertCable(
        Il2CppSystem.Collections.Generic.List<global::Il2Cpp.CableSaveData> list, CableSave dto)
    {
        if (list == null || dto == null) return null;
        global::Il2Cpp.CableSaveData found = null;
        Try(() =>
        {
            foreach (var entry in list)
            {
                if (entry == null) continue;
                int id = -1;
                try { id = entry.cableID; } catch { continue; }
                if (id == dto.CableID) { found = entry; break; }
            }
        });
        if (found != null) { FillCable(found, dto); return found; }
        var created = CreateCable(dto);
        if (created != null)
        {
            try { list.Add(created); } catch { return null; }
        }
        return created;
    }

    // ── Link-Labels / Port-VLAN-Filter ───────────────────────────────────────

    public static global::Il2Cpp.CableLinkLabelData CreateLinkLabel(CableLinkLabel dto)
    {
        global::Il2Cpp.CableLinkLabelData entry = null;
        try { entry = new global::Il2Cpp.CableLinkLabelData(); } catch { return null; }
        if (entry != null && dto != null)
        {
            Try(() => entry.position = dto.Position);
            Try(() => entry.labelText = dto.LabelText ?? "");
        }
        return entry;
    }

    public static CableLinkLabel ReadLinkLabel(global::Il2Cpp.CableLinkLabelData entry)
    {
        var dto = new CableLinkLabel();
        if (entry == null) return dto;
        Try(() => dto.Position = entry.position);
        Try(() => dto.LabelText = entry.labelText ?? "");
        return dto;
    }

    public static global::Il2Cpp.PortVlanFilterData CreatePortVlanFilter(PortVlanFilter dto)
    {
        global::Il2Cpp.PortVlanFilterData entry = null;
        try { entry = new global::Il2Cpp.PortVlanFilterData(); } catch { return null; }
        if (entry != null && dto != null)
        {
            Try(() => entry.portIndex = dto.PortIndex);
            Try(() =>
            {
                var list = new Il2CppSystem.Collections.Generic.List<int>();
                foreach (var id in dto.DisallowedVlanIds ?? new List<int>())
                {
                    try { list.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
                }
                entry.disallowedVlanIds = list;
            });
        }
        return entry;
    }

    public static PortVlanFilter ReadPortVlanFilter(global::Il2Cpp.PortVlanFilterData entry)
    {
        var dto = new PortVlanFilter();
        if (entry == null) return dto;
        Try(() => dto.PortIndex = entry.portIndex);
        Try(() =>
        {
            var list = entry.disallowedVlanIds;
            if (list == null) return;
            foreach (var id in list)
            {
                try { dto.DisallowedVlanIds.Add(id); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        });
        return dto;
    }

    private static void Try(Action action)
    {
        try { action?.Invoke(); }
        catch (Exception ex)
        {
            try { MelonLogger.Warning($"[gregCore][Net] DeviceSaves-Feld fehlgeschlagen: {ex.GetBaseException().Message}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }
}
