/// <file-summary>
/// Layer:       Core (Networking)
/// Purpose:     Save DTOs for network devices (router, firewall, SFP, LACP,
///               cables, endpoints, link labels, port VLAN filters): create,
///               fill, read. Upsert only where a key exists
///               (Cable.cableID, LACP.groupId); other lists position-based.
///               All best-effort.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;
using UnityEngine;

namespace gregCore.Core.Networking;

[ExcludeFromCodeCoverage(Justification = "Live Il2Cpp interop against game assemblies; needs running game.")]
public static partial class GregNetworkDeviceSaves
{
    // ── DTOs ─────────────────────────────────────────────────────────────────

    public sealed class SubnetRoute
    {
        public int SourceVlanId { get; set; }
        public string SubnetCidr { get; set; } = "";
        public int TargetVlanId { get; set; }
        public string TargetIp { get; set; } = "";
    }

    public sealed class OwnedSubnet
    {
        public int VlanId { get; set; }
        public string SubnetCidr { get; set; } = "";
    }

    public sealed class RouteEntry
    {
        public int RouteId { get; set; }
        public int SourceVlanId { get; set; }
        public string SourceIp { get; set; } = "";
        public int TargetVlanId { get; set; }
        public string TargetIp { get; set; } = "";
    }

    public sealed class RouterSave
    {
        public int Asn { get; set; }
        public int NextRouteId { get; set; }
        public List<SubnetRoute> RoutingTable { get; set; } = new List<SubnetRoute>();
        public List<OwnedSubnet> OwnedSubnets { get; set; } = new List<OwnedSubnet>();
        public List<RouteEntry> Routes { get; set; } = new List<RouteEntry>();
    }

    public sealed class FilterRule
    {
        public int PortIndex { get; set; }
        public string SourceIpCidr { get; set; } = "";
        public string DestIpCidr { get; set; } = "";
        public int NetworkPort { get; set; }
        public string Protocol { get; set; } = "";
        public bool Bidirectional { get; set; }
        public bool Allow { get; set; }
    }

    public sealed class FirewallSave
    {
        public string ClusterIP { get; set; } = "";
        public List<FilterRule> FilterRules { get; set; } = new List<FilterRule>();
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
}
