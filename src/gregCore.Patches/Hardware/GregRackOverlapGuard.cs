using System;
using System.Collections.Generic;
using MelonLoader;

namespace gregCore.GameLayer.Patches.Hardware;

/// <summary>
/// Load-time diagnostic for duplicate rack position claims (Mantis #19).
///
/// Vanilla lets two devices share one rack position index when mixed sizes
/// (4U/8U/...) stack densely — insertion-order dependent, hence "random".
/// The switch then binds the port to the wrong device: raw-name fallback in
/// the port list, multiple subnets on one port, dark device screens, VLAN
/// mixups. Moving the device one slot down (a free index) fixes it, which
/// is exactly the reported workaround.
///
/// Diagnostic only: loudly loggable for Mantis reports, never mutates.
/// Runs best-effort from GregNetworkIdHealing.Postfix after route
/// evaluation; any failure degrades to silence (CONVENTIONS.md).
/// </summary>
internal static class GregRackOverlapGuard
{
    private sealed class Claim
    {
        public string Label;
        public global::Il2Cpp.Rack Rack;
        public int Start;
        public int Size;
    }

    internal static int ScanAndReport()
    {
        try
        {
            var claims = CollectClaims();
            int groups = 0;
            for (int i = 0; i < claims.Count; i++)
            {
                for (int j = i + 1; j < claims.Count; j++)
                {
                    if (Overlaps(claims[i], claims[j]))
                    {
                        groups++;
                        MelonLogger.Warning(
                            $"[gregCore][HwId] Rack overlap: '{claims[i].Label}' and "
                            + $"'{claims[j].Label}' share rack positions "
                            + $"[{Math.Max(claims[i].Start, claims[j].Start)}.."
                            + $"{Math.Min(claims[i].Start + claims[i].Size, claims[j].Start + claims[j].Size) - 1}] "
                            + $"— the switch may bind the port to the wrong device "
                            + $"(raw names, merged subnets, dark screens; see Mantis #19). "
                            + $"Workaround: move one device to a free slot and reconnect.");
                    }
                }
            }
            if (groups > 0)
                MelonLogger.Warning($"[gregCore][HwId] Rack overlap scan: {groups} overlapping pair(s), {claims.Count} devices checked.");
            return groups;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return 0;
    }

    private static bool Overlaps(Claim a, Claim b)
    {
        try
        {
            if (a?.Rack == null || b?.Rack == null) return false;
            if (!(a.Rack == b.Rack)) return false;
            return a.Start < b.Start + b.Size && b.Start < a.Start + a.Size;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return false; }
    }

    private static List<Claim> CollectClaims()
    {
        var claims = new List<Claim>();
        try
        {
            var netMap = global::Il2Cpp.NetworkMap.instance;
            if (netMap != null)
            {
                if (netMap.servers != null)
                {
                    foreach (var kvp in netMap.servers)
                    {
                        global::Il2Cpp.Server srv = null;
                        try { srv = kvp.Value?.TryCast<global::Il2Cpp.Server>(); } catch { continue; }
                        if (srv == null) continue;
                        AddClaim(claims, "Server", ReadId(() => srv.ServerID), ReadRackPos(() => srv.currentRackPosition), ReadSize(() => srv.sizeInU));
                    }
                }
                if (netMap.switches != null)
                {
                    foreach (var kvp in netMap.switches)
                    {
                        global::Il2Cpp.NetworkSwitch sw = null;
                        try { sw = kvp.Value?.TryCast<global::Il2Cpp.NetworkSwitch>(); } catch { continue; }
                        if (sw == null) continue;
                        AddClaim(claims, "Switch", ReadId(() => sw.switchId), ReadRackPos(() => sw.currentRackPosition), ReadSize(() => sw.sizeInU));
                    }
                }
            }
            global::Il2Cpp.PatchPanel[] panels = null;
            try { panels = UnityEngine.Object.FindObjectsOfType<global::Il2Cpp.PatchPanel>(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            if (panels != null)
            {
                foreach (var pp in panels)
                {
                    if (pp == null) continue;
                    AddClaim(claims, "PatchPanel", ReadId(() => pp.patchPanelId), ReadRackPos(() => pp.currentRackPosition), ReadSize(() => pp.sizeInU));
                }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return claims;
    }

    private static string ReadId(Func<string> read)
    {
        try { return read() ?? "?"; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return "?"; }
    }

    private static global::Il2Cpp.RackPosition ReadRackPos(Func<global::Il2Cpp.RackPosition> read)
    {
        try { return read(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return null; }
    }

    private static int ReadSize(Func<int> read)
    {
        try
        {
            int size = read();
            return size > 0 ? size : 1;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return 1; }
    }

    private static void AddClaim(List<Claim> claims, string type, string id,
        global::Il2Cpp.RackPosition rackPos, int size)
    {
        try
        {
            if (rackPos == null) return;
            global::Il2Cpp.Rack rack = null;
            int index = -1;
            try { rack = rackPos.rack; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return; }
            try { index = rackPos.positionIndex; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ return; }
            if (rack == null || index < 0) return;
            claims.Add(new Claim { Label = $"{type} {id}", Rack = rack, Start = index, Size = size });
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }
}
