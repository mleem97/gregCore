/// <file-summary>
/// Layer:       Infrastructure (Persistence)
/// Purpose:     Third part of GregEntityInventory (partial): config,
///               sidecar serialization and pure helpers (UID/keys/TSV,
///               display scrub). Split due to codeline limits.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using MelonLoader;

namespace gregCore.Infrastructure.Persistence;

public static partial class GregEntityInventory
{
    // ------------------------------------------------------------ config

    /// <summary>
    /// Control surface of the inventory (MelonPreferences, category
    /// gregCore.EntityInventory). Enabled=false switches rebuilds off.
    /// </summary>
    public static class EntityInventoryConfig
    {
        private static bool _loaded;
        private static readonly object _loadGate = new object();

        public static bool Enabled { get; internal set; } = true;
        public static bool VerboseLogging { get; internal set; } = false;
        public static bool DumpOnRebuild { get; internal set; } = false;

        public static void Load()
        {
            try
            {
                lock (_loadGate)
                {
                    if (_loaded) return;
                    _loaded = true;
                }
                var category = MelonPreferences.CreateCategory("gregCore.EntityInventory", "EntityInventory (save inventory)");
                var enabled = category.CreateEntry("Enabled", true, "Build inventory on load.");
                var verbose = category.CreateEntry("VerboseLogging", false, "Verbose inventory logs.");
                var dump = category.CreateEntry("DumpOnRebuild", false, "Log full inventory after each rebuild.");
                try
                {
                    Enabled = enabled.Value;
                    VerboseLogging = verbose.Value;
                    DumpOnRebuild = dump.Value;
                }
                catch { /* ignored: optional persistence, game continues without sidecar */ }
                try { category.SaveToFile(false); } catch { /* ignored: optional persistence, game continues without sidecar */ }
            }
            catch { /* ignored: optional persistence, game continues without sidecar */ }
        }

        // Tests/tools only: runtime switch without file.
        public static void OverrideForTesting(bool? enabled, bool? verbose, bool? dump)
        {
            try
            {
                lock (_loadGate) { _loaded = true; }
                if (enabled.HasValue) Enabled = enabled.Value;
                if (verbose.HasValue) VerboseLogging = verbose.Value;
                if (dump.HasValue) DumpOnRebuild = dump.Value;
            }
            catch { /* ignored: optional persistence, game continues without sidecar */ }
        }
    }

    private static int PersistedCount()
    {
        try { lock (_gate) { return _persistedUid.Count; } }
        catch { return -1; }
    }

    // ------------------------------------------------------------ sidecar

    private static void EnsureSidecarRegistered()
    {
        try
        {
            lock (_gate)
            {
                if (_sidecarRegistered) return;
                _sidecarRegistered = true;
            }
            GregSaveGuard.RegisterSidecar(SidecarModId, SerializeMap, ParseMap);
        }
        catch { /* ignored: optional persistence, game continues without sidecar */ }
    }

    public static string SerializeMap()
    {
        try
        {
            var sb = new StringBuilder();
            sb.AppendLine("# gregUID inventory: kind\tnativeKey\tuid\thint (unsichtbar, nur gregCore)");
            lock (_gate)
            {
                foreach (var kv in _persistedUid)
                {
                    int sep = kv.Key.IndexOf('\n');
                    if (sep <= 0) continue;
                    string kind = kv.Key.Substring(0, sep);
                    string nativeKey = kv.Key.Substring(sep + 1);
                    string hint = "";
                    _persistedHint.TryGetValue(kv.Key, out hint);
                    sb.Append(SanitizeField(kind)).Append('\t')
                      .Append(SanitizeField(nativeKey)).Append('\t')
                      .Append(SanitizeField(kv.Value)).Append('\t')
                      .Append(SanitizeField(hint)).AppendLine();
                }
            }
            return sb.ToString();
        }
        catch { return null; }
    }

    public static void ParseMap(string content)
    {
        try
        {
            if (string.IsNullOrEmpty(content)) return;
            var uids = ParseUidMap(content, out var hints);
            if (uids.Count == 0 && hints.Count == 0) return;
            lock (_gate)
            {
                foreach (var kv in uids) _persistedUid[kv.Key] = kv.Value;
                foreach (var kv in hints) _persistedHint[kv.Key] = kv.Value;
            }
        }
        catch { /* ignored: optional persistence, game continues without sidecar */ }
    }

    /// <summary>Pure TSV logic (kindKey -&gt; uid/hint), unit-testable without the game.</summary>
    public static Dictionary<string, string> ParseUidMap(string content, out Dictionary<string, string> hints)
    {
        hints = new Dictionary<string, string>(StringComparer.Ordinal);
        var uids = new Dictionary<string, string>(StringComparer.Ordinal);
        try
        {
            if (string.IsNullOrEmpty(content)) return uids;
            var lines = content.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var raw in lines)
            {
                string line = raw.Trim();
                if (line.Length == 0 || line[0] == '#') continue;
                var parts = line.Split('\t');
                if (parts.Length < 3) continue;
                string kind = parts[0].Trim();
                string nativeKey = parts[1].Trim();
                string uid = parts[2].Trim();
                string hint = parts.Length > 3 ? parts[3].Trim() : "";
                if (kind.Length == 0 || nativeKey.Length == 0 || uid.Length == 0) continue;
                if (!IsGregUid(uid)) continue;
                string kk = kind + "\n" + nativeKey;
                uids[kk] = uid;
                hints[kk] = hint;
            }
        }
        catch { /* ignored: optional persistence, game continues without sidecar */ }
        return uids;
    }

    private static void LoadPersistedMap(string dir, string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(dir) || string.IsNullOrWhiteSpace(name)) return;
            string path = GregSaveGuard.SidecarPath(dir, name, SidecarModId);
            string content = null;
            try { if (File.Exists(path)) content = File.ReadAllText(path); } catch { return; }
            if (string.IsNullOrEmpty(content)) return;
            var uids = ParseUidMap(content, out var hints);
            lock (_gate)
            {
                _persistedUid.Clear();
                _persistedHint.Clear();
                foreach (var kv in uids) _persistedUid[kv.Key] = kv.Value;
                foreach (var kv in hints) _persistedHint[kv.Key] = kv.Value;
            }
            MelonLogger.Msg("[gregCore][Save] Inventory sidecar loaded: " + uids.Count + " UID(s).");
        }
        catch { /* ignored: optional persistence, game continues without sidecar */ }
    }

    // ------------------------------------------------------------ helpers (pure, testable)

    private static readonly Regex GregIdTokenRegex =
        new Regex(@"gregID:[A-Za-z]+:[0-9A-Za-z_\-]+", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex UnityDuplicateSuffixRegex =
        new Regex(@"^(.*)\s\(\d+\)$", RegexOptions.Compiled);

    /// <summary>
    /// Removes gregID tokens from display texts (screens), replacing them with
    /// the vanilla designation. No-op when no token is present (cheap).
    /// </summary>
    public static string ScrubGregIds(string text, string replacement)
    {
        try
        {
            if (string.IsNullOrEmpty(text)) return text;
            if (text.IndexOf("gregID:", StringComparison.OrdinalIgnoreCase) < 0) return text;
            return GregIdTokenRegex.Replace(text, replacement ?? "");
        }
        catch { return text; }
    }

    /// <summary>
    /// Vanilla designation from object names: strip "(Clone)" and Unity duplicate
    /// suffixes (" (1)"). gregID names yield "" (no vanilla
    /// form present - caller falls back).
    /// </summary>
    public static string CleanDisplayName(string name)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name)) return "";
            string n = name.Trim();
            if (n.StartsWith("gregID:", StringComparison.OrdinalIgnoreCase)) return "";
            int clone = n.IndexOf("(Clone)", StringComparison.OrdinalIgnoreCase);
            if (clone >= 0) n = n.Substring(0, clone).Trim();
            var m = UnityDuplicateSuffixRegex.Match(n);
            if (m.Success && m.Groups.Count > 1) n = m.Groups[1].Value.Trim();
            return n;
        }
        catch { return name ?? ""; }
    }

    public static string KindKey(InventoryKind kind, string nativeKey)
    {
        return kind.ToString() + "\n" + (nativeKey ?? "").Trim();
    }

    public static bool IsGregUid(string uid)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(uid)) return false;
            string u = uid.Trim();
            if (u.StartsWith(UidPrefix, StringComparison.OrdinalIgnoreCase)) return true;
            return IsGregDeviceUid(u, ServerPrefix)
                || IsGregDeviceUid(u, SwitchPrefix)
                || IsGregDeviceUid(u, PatchPanelPrefix);
        }
        catch { return false; }
    }

    public static bool IsGregDeviceUid(string uid, string prefix)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(uid) || string.IsNullOrEmpty(prefix)) return false;
            return uid.Trim().StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
        catch { return false; }
    }

    public static string NewUid(InventoryKind kind)
    {
        try { return UidPrefix + kind.ToString() + ":" + Guid.NewGuid().ToString("N").ToUpperInvariant().Substring(0, 12); }
        catch { return UidPrefix + kind.ToString() + ":FFFFFFFFFFFF"; }
    }

    /// <summary>Deterministic 12-hex seed from arbitrary text (stable fallback UIDs).</summary>
    public static string NewHex(string seed)
    {
        try
        {
            unchecked
            {
                uint h1 = 2166136261u, h2 = 16777619u;
                string s = seed ?? "";
                for (int i = 0; i < s.Length; i++)
                {
                    h1 ^= s[i];
                    h1 *= 16777619u;
                    h2 += s[i];
                    h2 *= 31u;
                }
                return h1.ToString("X8") + h2.ToString("X8").Substring(0, 4);
            }
        }
        catch { return "000000000000"; }
    }

    public static string SanitizeField(string value)
    {
        try
        {
            if (value == null) return "";
            return value.Replace('\t', ' ').Replace('\r', ' ').Replace('\n', ' ');
        }
        catch { return ""; }
    }
}
