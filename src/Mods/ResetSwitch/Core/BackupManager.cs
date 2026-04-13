using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Il2Cpp;
using MelonLoader.Utils;

namespace greg.Mods.ResetSwitch.Core;

public sealed class SwitchBackupEntry
{
    public string SwitchId { get; set; }
    public string SwitchName { get; set; }
    public string RackId { get; set; }
    public bool IsOn { get; set; }
    public bool IsBroken { get; set; }
    public int CableCount { get; set; }
    public float TotalConnectionSpeed { get; set; }
    public List<int> CustomerIds { get; set; } = new();
    public DateTime TimestampUtc { get; set; }
}

public sealed class SwitchBackupDocument
{
    public DateTime CreatedAtUtc { get; set; }
    public int SwitchCount { get; set; }
    public List<SwitchBackupEntry> Switches { get; set; } = new();
}

public static class BackupManager
{
    public static string CreateBackup(IEnumerable<SwitchInfo> switches)
    {
        var list = switches?.Where(s => s?.Instance != null).ToList() ?? new List<SwitchInfo>();
        var doc = new SwitchBackupDocument
        {
            CreatedAtUtc = DateTime.UtcNow,
            SwitchCount = list.Count
        };

        foreach (var item in list)
        {
            var sw = item.Instance;
            var entry = new SwitchBackupEntry
            {
                SwitchId = sw.switchId,
                SwitchName = sw.label,
                RackId = item.RackId,
                IsOn = sw.isOn,
                IsBroken = sw.isBroken,
                TimestampUtc = DateTime.UtcNow
            };

            if (sw.cableLinkSwitchPorts != null)
            {
                entry.CableCount = sw.cableLinkSwitchPorts.Count;
                for (var i = 0; i < sw.cableLinkSwitchPorts.Count; i++)
                {
                    var cable = sw.cableLinkSwitchPorts[i];
                    if (cable == null)
                    {
                        continue;
                    }

                    entry.TotalConnectionSpeed += cable.connectionSpeed;
                    if (cable.CustomerID != 0 && !entry.CustomerIds.Contains(cable.CustomerID))
                    {
                        entry.CustomerIds.Add(cable.CustomerID);
                    }
                }
            }

            doc.Switches.Add(entry);
        }

        var directory = Path.Combine(MelonEnvironment.UserDataDirectory, "greg.Mods.ResetSwitch");
        Directory.CreateDirectory(directory);

        var filePath = Path.Combine(directory, $"backup_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json");
        var json = JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(filePath, json);
        return filePath;
    }
}
