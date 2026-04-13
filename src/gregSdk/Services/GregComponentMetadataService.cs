using System;
using System.Collections.Generic;
using UnityEngine;
using Il2Cpp;

namespace greg.Sdk.Services;

public struct GregMetadataEntry
{
    public string Label;
    public string Value;
    public Color ValueColor;

    public GregMetadataEntry(string label, string value, Color? color = null)
    {
        Label = label;
        Value = value;
        ValueColor = color ?? Color.white;
    }
}

/// <summary>
/// SDK Service for extracting human-readable metadata from game components.
/// Unified source of truth for "JADE-style" tooltips.
/// </summary>
public static class GregComponentMetadataService
{
    public static List<GregMetadataEntry> GetMetadata(GregTargetInfo info)
    {
        var entries = new List<GregMetadataEntry>();
        if (info.TargetType == GregTargetType.None) return entries;

        switch (info.TargetType)
        {
            case GregTargetType.Rack:
                PopulateRackMetadata(entries, info.NativeComponent as Rack);
                break;
            case GregTargetType.Server:
                PopulateServerMetadata(entries, info.NativeComponent as Server);
                break;
            case GregTargetType.Switch:
                PopulateSwitchMetadata(entries, info.NativeComponent as NetworkSwitch);
                break;
            case GregTargetType.Cable:
                PopulateCableMetadata(entries, info.NativeComponent as CableLink);
                break;
            case GregTargetType.CableSpool:
                entries.Add(new GregMetadataEntry("Type", "CABLE SPOOL", new Color(0.8f, 1f, 1f)));
                break;
            case GregTargetType.NPC:
                PopulateNPCMetadata(entries, info.NativeComponent as Component);
                break;
        }

        // Common Metadata: HEX Color
        if (info.Entity != null)
        {
            var renderer = info.Entity.GetComponentInChildren<MeshRenderer>();
            if (renderer != null && renderer.material != null)
            {
                string hex = $"#{ColorUtility.ToHtmlStringRGB(renderer.material.color)}";
                entries.Add(new GregMetadataEntry("HEX", hex, new Color(1f, 0.98f, 0f)));
            }
        }

        return entries;
    }

    private static void PopulateRackMetadata(List<GregMetadataEntry> entries, Rack rack)
    {
        if (rack == null) return;
        
        int usedSlots = 0;
        if (rack.isPositionUsed != null)
        {
            foreach (int u in rack.isPositionUsed) if (u != 0) usedSlots++;
        }

        float totalIops = 0f;
        var servers = rack.GetComponentsInChildren<Server>();
        foreach (var s in servers) totalIops += s.currentProcessingSpeed;

        entries.Add(new GregMetadataEntry("ID", rack.gameObject.name));
        entries.Add(new GregMetadataEntry("DEVICES", usedSlots.ToString()));
        entries.Add(new GregMetadataEntry("IOPS", totalIops.ToString("F1"), new Color(0.8f, 1f, 1f)));
    }

    private static void PopulateServerMetadata(List<GregMetadataEntry> entries, Server server)
    {
        if (server == null) return;
        
        entries.Add(new GregMetadataEntry("ID", server.name));
        entries.Add(new GregMetadataEntry("IP", "0.0.0.0", server.isOn ? Color.white : Color.gray));
        entries.Add(new GregMetadataEntry("CUSTOMER", "Unknown"));
        entries.Add(new GregMetadataEntry("TYPE", server.serverType.ToString()));
        entries.Add(new GregMetadataEntry("STATUS", server.isOn ? (server.isBroken ? "BROKEN" : "ACTIVE") : "OFF", 
            server.isOn ? (server.isBroken ? Color.red : Color.green) : Color.gray));
    }

    private static void PopulateSwitchMetadata(List<GregMetadataEntry> entries, NetworkSwitch sw)
    {
        if (sw == null) return;
        entries.Add(new GregMetadataEntry("ID", sw.name));
        entries.Add(new GregMetadataEntry("PORTS", "0"));
        entries.Add(new GregMetadataEntry("STATUS", sw.isBroken ? "BROKEN" : "ACTIVE", sw.isBroken ? Color.red : Color.green));
    }

    private static void PopulateCableMetadata(List<GregMetadataEntry> entries, CableLink cable)
    {
        if (cable == null) return;
        entries.Add(new GregMetadataEntry("TYPE", cable.typeOfLink.ToString().ToUpper(), new Color(0.8f, 1f, 1f)));
        entries.Add(new GregMetadataEntry("SPEED", $"{cable.connectionSpeed} Gbps"));
        entries.Add(new GregMetadataEntry("LINK", "Wired"));
    }

    private static void PopulateNPCMetadata(List<GregMetadataEntry> entries, Component emp)
    {
        if (emp == null) return;
        entries.Add(new GregMetadataEntry("NAME", emp.gameObject.name));
        entries.Add(new GregMetadataEntry("JOB", "Employee"));
    }
}
