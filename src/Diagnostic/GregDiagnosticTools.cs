using System.Collections.Generic;
using System.Linq;
using Il2Cpp;
using MelonLoader;
using UnityEngine;
using greg.Sdk.Services;

namespace greg.Diagnostic;

public static class GregDiagnosticTools
{
    public static void RunFullWorldAudit()
    {
        MelonLogger.Msg("\n[gregCore] --- WORLD DIAGNOSTIC AUDIT (Save Bug Analysis) ---");
        
        var switches = Object.FindObjectsOfType<NetworkSwitch>(true);
        int totalSwitches = switches.Length;
        int brokenSwitches = switches.Count(s => s != null && s.isBroken);
        
        MelonLogger.Msg($"[Audit] Scanned {totalSwitches} switches. {brokenSwitches} marked as BROKEN.");

        var ghostDefects = new List<string>();

        foreach (var sw in switches)
        {
            if (sw == null) continue;

            if (sw.isBroken)
            {
                // Logic Audit: Why is it broken?
                bool hasFlow = GregResetSwitchService.EvaluateDeepStatus(sw) == GregDeepFlowStatus.Active;
                
                // If it has flow or seems logically sound but is "broken", it's a ghost defect.
                if (hasFlow)
                {
                    ghostDefects.Add($"[GhostDefect] Switch {sw.switchId} (Label: {sw.label}) is BROKEN but has ACTIVE FLOW.");
                }

                // Check for Server/Rack components
                var rack = sw.GetComponentInParent<Rack>();
                if (rack != null)
                {
                    // If the rack is broken but all servers inside seem fine
                    var servers = rack.GetComponentsInChildren<Server>();
                    bool anyServerBroken = servers.Any(serv => serv.isBroken);
                    if (!anyServerBroken && !hasFlow && sw.isOn)
                    {
                        ghostDefects.Add($"[GhostDefect] Rack {rack.name} / Switch {sw.switchId} is BROKEN but no servers inside are broken.");
                    }
                }
            }
        }

        if (ghostDefects.Count > 0)
        {
            MelonLogger.Warning($"[Audit] DETECTED {ghostDefects.Count} GHOSH DEFECTS!");
            foreach (var defect in ghostDefects)
            {
                MelonLogger.Warning($"  {defect}");
            }
        }
        else
        {
            MelonLogger.Msg("[Audit] No ghost defects detected in current scene.");
        }

        MelonLogger.Msg("[gregCore] --- AUDIT COMPLETE ---\n");
    }
}

