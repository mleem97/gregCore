using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Il2Cpp;
using MelonLoader;
using UnityEngine;

namespace greg.Mods.ResetSwitch.Core;

public sealed class SwitchSaveEntry
{
    public int Index { get; set; }
    public string SwitchId { get; set; }
    public int SwitchType { get; set; }
    public int RackPositionUID { get; set; }
    public string Label { get; set; }
    public string RackSlot { get; set; }
    public string RackId { get; set; }
    public string SlotId { get; set; }
    public bool SavedIsOn { get; set; }
    public bool SavedIsBroken { get; set; }
    public int TimeToBrake { get; set; }
    public int EolTime { get; set; }
    public bool FoundInRuntime { get; set; }
    public bool RuntimeIsOn { get; set; }
    public bool RuntimeIsBroken { get; set; }
    public string CorruptionReason { get; set; }
}

public sealed class SaveAnalysisReport
{
    public int TotalSwitchesInSave { get; set; }
    public int TotalRuntimeSwitches { get; set; }
    public int Consistent { get; set; }
    public int RuntimeMismatch { get; set; }
    public int SaveCorrupt { get; set; }
    public int MissingFromRuntime { get; set; }
    public int MissingFromSave { get; set; }
    public int NeverInitialized { get; set; }
    public int PoweredOffCaution { get; set; }
    public int TemplatesUnplaced { get; set; }
    public string SavePath { get; set; }
    public string SaveFormat { get; set; }
    public string AnalysisMode { get; set; }
    public string BugClassification { get; set; }
    public List<SwitchSaveEntry> Entries { get; set; } = new();

    public override string ToString()
    {
        var builder = new StringBuilder();
        builder.AppendLine($"Save Path: {SavePath}");
        builder.AppendLine($"Save Format: {SaveFormat}");
        builder.AppendLine($"Analysis Mode: {AnalysisMode}");
        builder.AppendLine($"Bug Classification: {BugClassification}");
        builder.AppendLine($"Total in save: {TotalSwitchesInSave}");
        builder.AppendLine($"Total runtime: {TotalRuntimeSwitches}");
        builder.AppendLine($"Never initialized: {NeverInitialized}");
        builder.AppendLine($"Powered off (caution): {PoweredOffCaution}");
        builder.AppendLine($"Templates/Unplaced: {TemplatesUnplaced}");

        foreach (var entry in Entries)
        {
            builder.AppendLine($"- {entry.SwitchId} | uid={entry.RackPositionUID} | isOn={entry.SavedIsOn} | eol={entry.EolTime} | reason={entry.CorruptionReason}");
        }

        return builder.ToString();
    }
}

public static class SaveAnalysisService
{
    private static readonly Regex SaveSwitchRegex = new(
        "(\\d+)\\s+switchID\\s+(\\S+),\\s*switchType\\s+(\\d+),.*?rackPositionUID\\s+(\\d+),\\s*isOn\\s+(true|false),\\s*label\\s+(.+?),\\s*isBroken\\s+(true|false),\\s*timeToBrake\\s+(\\d+),\\s*eolTime\\s+([\\-\\d]+)",
        RegexOptions.Singleline | RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex LabelRegex = new(
        "^(.*)-([A-Z]-\\d{1,3}|[A-Z]\\d{2,3})$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static SaveAnalysisReport _cached;

    public static SaveAnalysisReport GetCachedOrAnalyze()
    {
        return _cached ?? Analyze();
    }

    public static SaveAnalysisReport Analyze()
    {
        MelonLogger.Msg("[SaveAnalyzer] === ANALYSIS START ===");
        var report = new SaveAnalysisReport();

        var savePath = ResolveSavePath();
        report.SavePath = savePath ?? "<unknown>";
        MelonLogger.Msg($"[SaveAnalyzer] Save path: {report.SavePath}");

        var runtimeSwitches = UnityEngine.Object.FindObjectsOfType<NetworkSwitch>(true)
            .Where(s => s != null && !string.IsNullOrWhiteSpace(s.switchId))
            .ToList();
        report.TotalRuntimeSwitches = runtimeSwitches.Count;

        var runtimeById = new Dictionary<string, NetworkSwitch>();
        foreach (var runtime in runtimeSwitches)
        {
            if (!runtimeById.ContainsKey(runtime.switchId))
            {
                runtimeById.Add(runtime.switchId, runtime);
            }
        }

        var runtimeSaveEntries = TryReadSwitchEntriesFromLoadedSaveData();
        if (runtimeSaveEntries.Count > 0)
        {
            report.AnalysisMode = "RuntimeSaveDataReflection";
            report.SaveFormat = "Loaded SaveData object (reflection)";
            report.TotalSwitchesInSave = runtimeSaveEntries.Count;

            ClassifyEntries(report, runtimeSaveEntries, runtimeById);

            report.BugClassification = report.SaveCorrupt > 0
                ? "SAVE_CORRUPT"
                : (report.RuntimeMismatch > 0 ? "RUNTIME_MISMATCH" : "HEALTHY_OR_INTENTIONAL_OFF");

            MelonLogger.Msg("[SaveAnalyzer] Runtime SaveData reflection parse succeeded.");
            MelonLogger.Msg($"[SaveAnalyzer] Parsed switches from loaded save: {report.TotalSwitchesInSave}");
            MelonLogger.Msg($"[SaveAnalyzer] BUG CLASSIFICATION: {report.BugClassification}");

            _cached = report;
            return report;
        }

        if (string.IsNullOrWhiteSpace(savePath) || !File.Exists(savePath))
        {
            MelonLogger.Warning("[SaveAnalyzer] WARNING: Could not locate save path — analysis skipped");
            report.SaveFormat = "Unknown";
            report.AnalysisMode = "Unavailable";
            report.MissingFromSave = runtimeById.Count;
            report.BugClassification = "UNKNOWN";
            _cached = report;
            return report;
        }

        var content = File.ReadAllText(savePath);
        report.SaveFormat = "Flat-Text Key-Value (not JSON)";
        report.AnalysisMode = "RawTextRegexFallback";
        MelonLogger.Msg("[SaveAnalyzer] Format detected: Flat-Text Key-Value (not JSON)");

        var switchSection = ExtractSwitchSection(content, out var switchSectionIndex);
        MelonLogger.Msg($"[SaveAnalyzer] Switches section found at char index {switchSectionIndex}");

        var matches = SaveSwitchRegex.Matches(switchSection);
        report.TotalSwitchesInSave = matches.Count;

        var saveById = new Dictionary<string, SwitchSaveEntry>();
        foreach (Match match in matches)
        {
            var entry = BuildEntry(match);
            if (entry == null || string.IsNullOrWhiteSpace(entry.SwitchId) || saveById.ContainsKey(entry.SwitchId))
            {
                continue;
            }

            saveById.Add(entry.SwitchId, entry);
        }

        ClassifyEntries(report, saveById, runtimeById);

        report.BugClassification = report.SaveCorrupt > 0
            ? "SAVE_CORRUPT"
            : (report.RuntimeMismatch > 0 ? "RUNTIME_MISMATCH" : "HEALTHY_OR_INTENTIONAL_OFF");

        MelonLogger.Msg("[SaveAnalyzer] === SAVE HEALTH SUMMARY ===");
        MelonLogger.Msg($"[SaveAnalyzer] Total in save:       {report.TotalSwitchesInSave}");
        MelonLogger.Msg($"[SaveAnalyzer] Never initialized:   {report.NeverInitialized}");
        MelonLogger.Msg($"[SaveAnalyzer] Powered off (intentional?): {report.PoweredOffCaution}");
        MelonLogger.Msg($"[SaveAnalyzer] Templates/Unplaced:  {report.TemplatesUnplaced}");
        MelonLogger.Msg($"[SaveAnalyzer] Healthy (isOn=true): {report.Consistent}");
        MelonLogger.Msg($"[SaveAnalyzer] BUG CLASSIFICATION: {report.BugClassification} — repair is non-persistent until saved");

        _cached = report;
        return report;
    }

    private static void ClassifyEntries(SaveAnalysisReport report, Dictionary<string, SwitchSaveEntry> saveById, Dictionary<string, NetworkSwitch> runtimeById)
    {
        foreach (var entry in saveById.Values)
        {
            if (IsTemplateOrUnplaced(entry))
            {
                entry.CorruptionReason = "TEMPLATE_OR_UNPLACED";
                report.TemplatesUnplaced++;
                MelonLogger.Msg($"[SaveAnalyzer] {entry.SwitchId}: label='{entry.Label}' rackUID={entry.RackPositionUID} → UNPLACED (skip)");
                report.Entries.Add(entry);
                continue;
            }

            if (runtimeById.TryGetValue(entry.SwitchId, out var runtime))
            {
                entry.FoundInRuntime = true;
                entry.RuntimeIsOn = runtime.isOn;
                entry.RuntimeIsBroken = runtime.isBroken;
            }
            else
            {
                entry.CorruptionReason = "MISSING_FROM_RUNTIME";
                report.MissingFromRuntime++;
                report.Entries.Add(entry);
                continue;
            }

            if (!entry.SavedIsOn && entry.RackPositionUID > 0)
            {
                if (entry.EolTime == 14400)
                {
                    entry.CorruptionReason = "NEVER_INITIALIZED";
                    report.NeverInitialized++;
                    report.SaveCorrupt++;
                    MelonLogger.Msg($"[SaveAnalyzer] {entry.SwitchId}: isOn=false eolTime=14400 rackUID={entry.RackPositionUID} → NEVER_INITIALIZED");
                }
                else
                {
                    entry.CorruptionReason = "POWERED_OFF";
                    report.PoweredOffCaution++;
                    MelonLogger.Msg($"[SaveAnalyzer] {entry.SwitchId}: isOn=false eolTime={entry.EolTime} rackUID={entry.RackPositionUID} → POWERED_OFF");
                }
            }
            else if (entry.SavedIsOn == entry.RuntimeIsOn && entry.SavedIsBroken == entry.RuntimeIsBroken && entry.RuntimeIsOn)
            {
                entry.CorruptionReason = "CONSISTENT";
                report.Consistent++;
            }
            else if (entry.SavedIsOn && !entry.RuntimeIsOn)
            {
                entry.CorruptionReason = "RUNTIME_MISMATCH";
                report.RuntimeMismatch++;
            }
            else
            {
                entry.CorruptionReason = "STATE_DIFFERENCE";
                report.RuntimeMismatch++;
            }

            report.Entries.Add(entry);
        }

        foreach (var runtime in runtimeById.Values)
        {
            if (saveById.ContainsKey(runtime.switchId))
            {
                continue;
            }

            report.MissingFromSave++;
        }
    }

    private static Dictionary<string, SwitchSaveEntry> TryReadSwitchEntriesFromLoadedSaveData()
    {
        var result = new Dictionary<string, SwitchSaveEntry>(StringComparer.OrdinalIgnoreCase);

        try
        {
            var saveManagerType = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaveGameManager"))
                .FirstOrDefault(type => type != null);

            if (saveManagerType == null)
            {
                return result;
            }

            var saveManager = GetTypeMemberValue(saveManagerType, null, "instance", "Instance");
            if (saveManager == null)
            {
                return result;
            }

            var saveData = GetMemberValue(saveManager, "currentSaveData", "CurrentSaveData", "saveData", "SaveData", "loadedSaveData", "LoadedSaveData");
            var networkData = GetMemberValue(saveData, "networkData", "NetworkData");
            var switches = GetMemberValue(networkData, "switches", "Switches") as System.Collections.IEnumerable;
            if (switches == null)
            {
                return result;
            }

            foreach (var switchObject in switches)
            {
                if (switchObject == null)
                {
                    continue;
                }

                var switchId = GetMemberValueAsString(switchObject, "switchID", "switchId", "SwitchID", "SwitchId");
                if (string.IsNullOrWhiteSpace(switchId) || result.ContainsKey(switchId))
                {
                    continue;
                }

                var label = GetMemberValueAsString(switchObject, "label", "Label");
                ParseLabel(label, out var rackId, out var slotId);

                var entry = new SwitchSaveEntry
                {
                    Index = GetMemberValueAsInt(switchObject, "index", "Index", "id", "Id"),
                    SwitchId = switchId,
                    SwitchType = GetMemberValueAsInt(switchObject, "switchType", "SwitchType", "type", "Type"),
                    RackPositionUID = GetMemberValueAsInt(switchObject, "rackPositionUID", "RackPositionUID"),
                    SavedIsOn = GetMemberValueAsBool(switchObject, "isOn", "IsOn"),
                    Label = label,
                    RackSlot = slotId,
                    RackId = rackId,
                    SlotId = slotId,
                    SavedIsBroken = GetMemberValueAsBool(switchObject, "isBroken", "IsBroken"),
                    TimeToBrake = GetMemberValueAsInt(switchObject, "timeToBrake", "TimeToBrake"),
                    EolTime = GetMemberValueAsInt(switchObject, "eolTime", "EolTime")
                };

                result.Add(entry.SwitchId, entry);
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SaveAnalyzer] Runtime SaveData reflection failed: {ex.Message}");
        }

        return result;
    }

    private static object GetTypeMemberValue(Type type, object instance, params string[] memberNames)
    {
        if (type == null || memberNames == null)
        {
            return null;
        }

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        foreach (var memberName in memberNames)
        {
            var property = type.GetProperty(memberName, flags);
            if (property != null)
            {
                return property.GetValue(instance);
            }

            var field = type.GetField(memberName, flags);
            if (field != null)
            {
                return field.GetValue(instance);
            }
        }

        return null;
    }

    private static object GetMemberValue(object instance, params string[] memberNames)
    {
        if (instance == null)
        {
            return null;
        }

        return GetTypeMemberValue(instance.GetType(), instance, memberNames);
    }

    private static string GetMemberValueAsString(object instance, params string[] memberNames)
    {
        var value = GetMemberValue(instance, memberNames);
        return value?.ToString()?.Trim() ?? string.Empty;
    }

    private static int GetMemberValueAsInt(object instance, params string[] memberNames)
    {
        var value = GetMemberValue(instance, memberNames);
        if (value == null)
        {
            return 0;
        }

        if (value is int intValue)
        {
            return intValue;
        }

        return int.TryParse(value.ToString(), out var parsed) ? parsed : 0;
    }

    private static bool GetMemberValueAsBool(object instance, params string[] memberNames)
    {
        var value = GetMemberValue(instance, memberNames);
        if (value == null)
        {
            return false;
        }

        if (value is bool boolValue)
        {
            return boolValue;
        }

        if (value is int intValue)
        {
            return intValue != 0;
        }

        return bool.TryParse(value.ToString(), out var parsed) && parsed;
    }

    private static string ExtractSwitchSection(string content, out int startIndex)
    {
        startIndex = content.IndexOf("switches", StringComparison.OrdinalIgnoreCase);
        if (startIndex < 0)
        {
            startIndex = 0;
            return content;
        }

        var endIndex = content.IndexOf("patchPanels", startIndex, StringComparison.OrdinalIgnoreCase);
        if (endIndex < 0)
        {
            endIndex = content.Length;
        }

        return content.Substring(startIndex, endIndex - startIndex);
    }

    private static SwitchSaveEntry BuildEntry(Match match)
    {
        try
        {
            var label = match.Groups[6].Value.Trim();
            ParseLabel(label, out var rackId, out var slotId);

            return new SwitchSaveEntry
            {
                Index = int.Parse(match.Groups[1].Value),
                SwitchId = match.Groups[2].Value.Trim(),
                SwitchType = int.Parse(match.Groups[3].Value),
                RackPositionUID = int.Parse(match.Groups[4].Value),
                SavedIsOn = bool.Parse(match.Groups[5].Value),
                Label = label,
                RackSlot = slotId,
                RackId = rackId,
                SlotId = slotId,
                SavedIsBroken = bool.Parse(match.Groups[7].Value),
                TimeToBrake = int.Parse(match.Groups[8].Value),
                EolTime = int.Parse(match.Groups[9].Value)
            };
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SaveAnalyzer] Failed to parse switch entry: {ex.Message}");
            return null;
        }
    }

    private static bool IsTemplateOrUnplaced(SwitchSaveEntry entry)
    {
        if (entry.RackPositionUID == 0)
        {
            MelonLogger.Msg($"[SaveAnalyzer] {entry.SwitchId}: isOn={entry.SavedIsOn} rackUID=0 → TEMPLATE/UNPLACED (skip)");
            return true;
        }

        if (string.Equals(entry.Label, entry.SwitchId, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (string.Equals(entry.Label, "ESTSETSET", StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return !LabelRegex.IsMatch(entry.Label);
    }

    private static void ParseLabel(string label, out string rackId, out string slotId)
    {
        rackId = string.Empty;
        slotId = string.Empty;
        if (string.IsNullOrWhiteSpace(label))
        {
            return;
        }

        var match = LabelRegex.Match(label);
        if (!match.Success)
        {
            return;
        }

        rackId = match.Groups[1].Value;
        slotId = match.Groups[2].Value;
    }

    private static string ResolveSavePath()
    {
        try
        {
            var type = AppDomain.CurrentDomain.GetAssemblies()
                .Select(assembly => assembly.GetType("SaveGameManager"))
                .FirstOrDefault(t => t != null);

            if (type != null)
            {
                var instanceProperty = type.GetProperty("instance", BindingFlags.Public | BindingFlags.Static);
                var instanceField = type.GetField("instance", BindingFlags.Public | BindingFlags.Static);
                var instance = instanceProperty?.GetValue(null) ?? instanceField?.GetValue(null);

                if (instance != null)
                {
                    var pathProperty = type.GetProperty("currentSavePath", BindingFlags.Public | BindingFlags.Instance)
                                      ?? type.GetProperty("CurrentSavePath", BindingFlags.Public | BindingFlags.Instance);
                    var pathField = type.GetField("currentSavePath", BindingFlags.Public | BindingFlags.Instance)
                                   ?? type.GetField("CurrentSavePath", BindingFlags.Public | BindingFlags.Instance);

                    var value = pathProperty?.GetValue(instance) as string ?? pathField?.GetValue(instance) as string;
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        return value;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SaveAnalyzer] Save path API lookup failed: {ex.Message}");
        }

        try
        {
            var savesDir = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(savesDir))
            {
                return null;
            }

            var latest = new DirectoryInfo(savesDir)
                .GetFiles("*", SearchOption.AllDirectories)
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .FirstOrDefault();

            return latest?.FullName;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[SaveAnalyzer] Save path fallback failed: {ex.Message}");
            return null;
        }
    }
}
