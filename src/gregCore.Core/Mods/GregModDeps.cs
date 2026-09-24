/// <file-summary>
/// Schicht:      Core (Mods)
/// Zweck:        Mod-Abhängigkeiten: deklarieren (Declare), Ladezustand
///               prüfen (IsLoaded/EnsureLoaded/CheckAll mit Versionen) und
///               Manifeste für ModSync/Coop vergleichen (GetLocalManifest,
///               DiffManifests). Alles best-effort, nie Exceptions.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MelonLoader;

namespace gregCore.Core.Mods;

[ExcludeFromCodeCoverage(Justification = "Runtime MelonLoader registry access; needs running game.")]
public static class GregModDeps
{
    // ── Deklaration ──────────────────────────────────────────────────────────

    public sealed class Dependency
    {
        public string ModId { get; set; } = "";
        public string MinVersion { get; set; } = "";
        public bool Required { get; set; } = true;
    }

    public sealed class Problem
    {
        public string OwnerModId = "";
        public string ModId = "";
        public string Detail = "";
        public bool Required = true;

        public override string ToString()
        {
            return $"[{OwnerModId}] braucht '{ModId}': {Detail}";
        }
    }

    public sealed class ModEntry
    {
        public string Id = "";
        public string Version = "";
    }

    public sealed class ManifestDiff
    {
        public List<ModEntry> MissingOnRemote = new List<ModEntry>();
        public List<ModEntry> MissingLocally = new List<ModEntry>();
        public List<string> VersionMismatches = new List<string>();
        public bool Compatible => MissingOnRemote.Count == 0 && MissingLocally.Count == 0 && VersionMismatches.Count == 0;
    }

    private static readonly Dictionary<string, List<Dependency>> _declared =
        new Dictionary<string, List<Dependency>>(StringComparer.OrdinalIgnoreCase);

    private static readonly object _lock = new object();

    public static void Declare(string ownerModId, params Dependency[] deps)
    {
        if (string.IsNullOrWhiteSpace(ownerModId) || deps == null) return;
        lock (_lock)
        {
            if (!_declared.TryGetValue(ownerModId, out var list))
            {
                list = new List<Dependency>();
                _declared[ownerModId] = list;
            }
            foreach (var d in deps)
            {
                if (d == null || string.IsNullOrWhiteSpace(d.ModId)) continue;
                list.RemoveAll(x => string.Equals(x.ModId, d.ModId, StringComparison.OrdinalIgnoreCase));
                list.Add(d);
            }
        }
    }

    // ── Ladezustand (MelonLoader-Registry) ───────────────────────────────────

    public static bool IsMelonLoaded(string modIdOrName)
    {
        if (string.IsNullOrWhiteSpace(modIdOrName)) return false;
        try
        {
            var melons = MelonBase.RegisteredMelons;
            if (melons == null) return false;
            foreach (var melon in melons)
            {
                if (melon == null) continue;
                string name = null;
                try { name = melon.Info?.Name; } catch { continue; }
                if (string.Equals(name, modIdOrName, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return false;
    }

    public static string GetMelonVersion(string modIdOrName)
    {
        if (string.IsNullOrWhiteSpace(modIdOrName)) return "";
        try
        {
            var melons = MelonBase.RegisteredMelons;
            if (melons == null) return "";
            foreach (var melon in melons)
            {
                if (melon == null) continue;
                string name = null;
                try { name = melon.Info?.Name; } catch { continue; }
                if (!string.Equals(name, modIdOrName, StringComparison.OrdinalIgnoreCase)) continue;
                try { return melon.Info?.Version ?? ""; } catch { return ""; }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return "";
    }

    // ── Prüfung ──────────────────────────────────────────────────────────────

    public static bool EnsureLoaded(Dependency dep, out string detail)
    {
        detail = "";
        if (dep == null || string.IsNullOrWhiteSpace(dep.ModId))
        {
            detail = "leere Abhängigkeit";
            return false;
        }
        if (!IsMelonLoaded(dep.ModId))
        {
            detail = dep.Required ? "nicht geladen (erforderlich)" : "nicht geladen (optional)";
            return !dep.Required;
        }
        if (!string.IsNullOrWhiteSpace(dep.MinVersion))
        {
            string have = GetMelonVersion(dep.ModId);
            if (!IsVersionAtLeast(have, dep.MinVersion))
            {
                detail = $"Version zu alt (haben: '{have}', brauchen: >={dep.MinVersion})";
                return !dep.Required;
            }
        }
        detail = "ok";
        return true;
    }

    public static List<Problem> CheckOwner(string ownerModId)
    {
        var problems = new List<Problem>();
        if (string.IsNullOrWhiteSpace(ownerModId)) return problems;
        List<Dependency> deps = null;
        lock (_lock)
        {
            if (_declared.TryGetValue(ownerModId, out var list))
                deps = new List<Dependency>(list);
        }
        if (deps == null) return problems;
        foreach (var dep in deps)
        {
            try
            {
                if (!EnsureLoaded(dep, out string detail) && dep.Required)
                {
                    problems.Add(new Problem
                    {
                        OwnerModId = ownerModId,
                        ModId = dep.ModId,
                        Detail = detail,
                        Required = true,
                    });
                }
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return problems;
    }

    // Prüft alle Deklarationen, loggt und meldet ob alles Erforderliche da ist.
    public static bool CheckAll()
    {
        bool allOk = true;
        List<string> owners;
        lock (_lock) { owners = new List<string>(_declared.Keys); }
        foreach (var owner in owners)
        {
            List<Problem> problems = null;
            try { problems = CheckOwner(owner); } catch { problems = new List<Problem>(); }
            foreach (var p in problems)
            {
                allOk = false;
                try { MelonLogger.Error($"[gregCore][Mods] Fehlende Abhängigkeit: {p}"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        return allOk;
    }

    // ── Manifeste für ModSync/Coop ───────────────────────────────────────────
    // Zwei Peers tauschen Manifeste aus (z.B. Lobby-Data); Diff zeigt, ob
    // ModSync sinnvoll möglich ist (gleiche Mods, kompatible Versionen).

    public static List<ModEntry> GetLocalManifest()
    {
        var result = new List<ModEntry>();
        try
        {
            var melons = MelonBase.RegisteredMelons;
            if (melons == null) return result;
            foreach (var melon in melons)
            {
                if (melon == null) continue;
                try
                {
                    string name = melon.Info?.Name ?? "";
                    if (string.IsNullOrWhiteSpace(name)) continue;
                    result.Add(new ModEntry { Id = name, Version = melon.Info?.Version ?? "" });
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        result.Sort((a, b) => string.Compare(a.Id, b.Id, StringComparison.OrdinalIgnoreCase));
        return result;
    }

    public static ManifestDiff DiffManifests(List<ModEntry> local, List<ModEntry> remote)
    {
        var diff = new ManifestDiff();
        var l = ToMap(local);
        var r = ToMap(remote);
        foreach (var kv in l)
        {
            if (!r.TryGetValue(kv.Key, out string remoteVer))
            {
                diff.MissingOnRemote.Add(new ModEntry { Id = kv.Key, Version = kv.Value });
            }
            else if (!string.IsNullOrWhiteSpace(kv.Value)
                     && !string.IsNullOrWhiteSpace(remoteVer)
                     && !string.Equals(kv.Value.Trim(), remoteVer.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                diff.VersionMismatches.Add($"{kv.Key}: lokal {kv.Value} vs. remote {remoteVer}");
            }
        }
        foreach (var kv in r)
        {
            if (!l.ContainsKey(kv.Key))
                diff.MissingLocally.Add(new ModEntry { Id = kv.Key, Version = kv.Value });
        }
        return diff;
    }

    public static string FormatDiff(ManifestDiff diff)
    {
        if (diff == null) return "kein Diff";
        if (diff.Compatible) return "Manifeste kompatibel";
        var parts = new List<string>();
        foreach (var m in diff.MissingOnRemote)
            parts.Add($"remote fehlt: {m.Id} ({m.Version})");
        foreach (var m in diff.MissingLocally)
            parts.Add($"lokal fehlt: {m.Id} ({m.Version})");
        parts.AddRange(diff.VersionMismatches);
        return string.Join("; ", parts);
    }

    private static Dictionary<string, string> ToMap(List<ModEntry> entries)
    {
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        if (entries == null) return map;
        foreach (var e in entries)
        {
            if (e == null || string.IsNullOrWhiteSpace(e.Id)) continue;
            try { map[e.Id] = e.Version ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        return map;
    }

    // "1.2.3", "1.2", "v1.0-beta" tolerant vergleichen (numerische Prefixe).
    public static bool IsVersionAtLeast(string have, string need)
    {
        try
        {
            int[] h = ParseVersion(have);
            int[] n = ParseVersion(need);
            int len = Math.Max(h.Length, n.Length);
            for (int i = 0; i < len; i++)
            {
                int hv = i < h.Length ? h[i] : 0;
                int nv = i < n.Length ? n[i] : 0;
                if (hv != nv) return hv > nv;
            }
            return true;
        }
        catch { return false; }
    }

    private static int[] ParseVersion(string raw)
    {
        var parts = new List<int>();
        if (string.IsNullOrWhiteSpace(raw)) return parts.ToArray();
        string s = raw.Trim().TrimStart('v', 'V');
        foreach (var chunk in s.Split('.', '-', '+'))
        {
            string digits = "";
            foreach (char c in chunk)
            {
                if (c < '0' || c > '9') break;
                digits += c;
            }
            if (digits.Length == 0) break;
            if (int.TryParse(digits, out int v)) parts.Add(v);
            else break;
        }
        return parts.ToArray();
    }
}
