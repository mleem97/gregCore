using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using greg.Sdk.Services;
using UnityEngine;
using Il2Cpp;
using MelonLoader;
using greg.Mods.ResetSwitch.Integration;
using greg.Sdk.Services;


namespace greg.Mods.ResetSwitch.Core;

public enum FlowStatus { OK, Degraded, Dead }

public class SwitchInfo
{
    public NetworkSwitch Instance;
    public string Name;
    public string SwitchId;
    public string RackId;
    public string SlotId;
    public string Row;
    public string Position;
    public string MountPos;
    public FlowStatus Status;
    public DeepFlowStatus DeepStatus;
    public bool Selected;
}

public static class SwitchScanner
{
    public static List<SwitchInfo> ScanAll()
    {
        MelonLogger.Msg("[SwitchScanner] Starting scan (label-mode, no hierarchy walk)...");
        var result = new List<SwitchInfo>();
        var scanItems = GregResetSwitchService.ScanAllSwitches();
        var switches = new List<NetworkSwitch>();
        foreach (var item in scanItems)
        {
            if (item?.Instance != null)
            {
                switches.Add(item.Instance);
            }
        }

        MelonLogger.Msg($"[SwitchScanner] Found {switches.Count} NetworkSwitch components.");

        foreach (var sw in switches)
        {
            if (sw == null)
            {
                continue;
            }

            if (string.IsNullOrEmpty(sw.switchId))
            {
                continue;
            }

            var deepStatus = FlowSimulator.Evaluate(sw);
            var status = DeepToFlow(deepStatus);
            var displayName = string.IsNullOrWhiteSpace(sw.label) ? sw.name : sw.label;

            ParseRackAndSlotFromLabel(displayName, sw.switchId, out var labelRackId, out var labelSlotId);
            BuildRackFromDetectedPlacement(sw, out var detectedRackId, out var detectedSlotId, out var row, out var position, out var mountPos);

            var rackId = string.IsNullOrWhiteSpace(detectedRackId) ? labelRackId : detectedRackId;
            var slotId = string.IsNullOrWhiteSpace(detectedSlotId) || detectedSlotId == "UNKNOWN" ? labelSlotId : detectedSlotId;

            result.Add(new SwitchInfo
            {
                Instance = sw,
                Name = displayName,
                SwitchId = sw.switchId,
                RackId = rackId,
                SlotId = slotId,
                Row = row,
                Position = position,
                MountPos = mountPos,
                Status = status,
                DeepStatus = deepStatus,
                Selected = false
            });
        }

        var dead = 0;
        var degraded = 0;
        var ok = 0;
        foreach (var item in result)
        {
            switch (item.Status)
            {
                case FlowStatus.Dead:
                    dead++;
                    break;
                case FlowStatus.Degraded:
                    degraded++;
                    break;
                default:
                    ok++;
                    break;
            }
        }

        GregCoreIntegration.FireScanComplete(result.Count, dead, degraded, ok);
        MelonLogger.Msg($"[SwitchScanner] Scan complete: {result.Count} switches listed.");
        return result;
    }

    private static void ParseRackAndSlotFromLabel(string label, string fallback, out string rackId, out string slotId)
    {
        if (string.IsNullOrEmpty(label))
        {
            rackId = fallback;
            slotId = "UNKNOWN";
            return;
        }

        var parts = label.Trim().Split(' ');
        if (parts.Length == 0)
        {
            rackId = fallback;
            slotId = "UNKNOWN";
            return;
        }

        var slotPart = parts[parts.Length - 1];
        var match = Regex.Match(slotPart, @"^(.+)-([A-Z]-\d+)$");
        if (match.Success)
        {
            rackId = match.Groups[1].Value;
            slotId = match.Groups[2].Value;
            return;
        }

        rackId = slotPart;
        slotId = "UNKNOWN";
    }

    private static void BuildRackFromDetectedPlacement(NetworkSwitch sw, out string rackId, out string slotId, out string row, out string position, out string mountPos)
    {
        row = ReadMemberAsString(sw, "row", "rackRow", "rowIndex");
        position = ReadMemberAsString(sw, "position", "rackPosition", "positionIndex");
        mountPos = ReadMemberAsString(sw, "mountPos", "mountPosition", "mountPoint", "slot", "slotId");

        var parts = new List<string>();
        if (!string.IsNullOrWhiteSpace(row))
        {
            parts.Add($"ROW {row}");
        }

        if (!string.IsNullOrWhiteSpace(position))
        {
            parts.Add($"POS {position}");
        }

        if (!string.IsNullOrWhiteSpace(mountPos))
        {
            parts.Add($"MOUNT {mountPos}");
        }

        rackId = parts.Count > 0 ? string.Join(" | ", parts) : string.Empty;
        slotId = !string.IsNullOrWhiteSpace(mountPos) ? $"MOUNT-{mountPos}" : "UNKNOWN";
    }

    private static string ReadMemberAsString(object target, params string[] memberNames)
    {
        if (target == null || memberNames == null || memberNames.Length == 0)
        {
            return null;
        }

        var type = target.GetType();
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase;

        foreach (var memberName in memberNames)
        {
            if (string.IsNullOrWhiteSpace(memberName))
            {
                continue;
            }

            try
            {
                var field = type.GetField(memberName, flags);
                if (field != null)
                {
                    var fieldValue = field.GetValue(target);
                    var fieldText = ToMeaningfulText(fieldValue);
                    if (!string.IsNullOrWhiteSpace(fieldText))
                    {
                        return fieldText;
                    }
                }

                var prop = type.GetProperty(memberName, flags);
                if (prop != null)
                {
                    var propValue = prop.GetValue(target, null);
                    var propText = ToMeaningfulText(propValue);
                    if (!string.IsNullOrWhiteSpace(propText))
                    {
                        return propText;
                    }
                }
            }
            catch
            {
            }
        }

        return null;
    }

    private static string ToMeaningfulText(object value)
    {
        if (value == null)
        {
            return null;
        }

        var text = value.ToString()?.Trim();
        if (string.IsNullOrWhiteSpace(text))
        {
            return null;
        }

        return text == "0" ? null : text;
    }

    private static FlowStatus DeepToFlow(DeepFlowStatus deepStatus)
    {
        return deepStatus switch
        {
            DeepFlowStatus.Active => FlowStatus.OK,
            DeepFlowStatus.Idle => FlowStatus.Degraded,
            DeepFlowStatus.PoweredOff => FlowStatus.Dead,
            DeepFlowStatus.Broken => FlowStatus.Dead,
            DeepFlowStatus.Isolated => FlowStatus.Dead,
            _ => FlowStatus.Dead
        };
    }
}
