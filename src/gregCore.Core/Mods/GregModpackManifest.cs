/// <file-summary>
/// Layer:       Core (Mods)
/// Purpose:     Optional mod-pack manifest (Mods/manifest.json):
///              { Name, Mods[], Library[], Plugins[] } with paths relative
///              to Mods/. Validates entries (must exist, .dll only, no
///              traversal, never .deactivated) and exposes Library folders
///              for dependency probing. Absent manifest = no behavior change.
/// Maintainer:  Read/validate only; never loads assemblies here.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace gregCore.Core.Mods;

public sealed class GregModpackManifest
{
    public string Name { get; set; } = "";
    public List<string> Mods { get; set; } = new();
    public List<string> Library { get; set; } = new();
    public List<string> Plugins { get; set; } = new();

    public sealed class Validation
    {
        public GregModpackManifest Manifest { get; init; }
        public List<string> ValidMods { get; } = new();
        public List<string> ValidLibrary { get; } = new();
        public List<string> ValidPlugins { get; } = new();
        public List<string> LibraryFolders { get; } = new();
        public List<string> Warnings { get; } = new();
        public bool Found => Manifest != null;
    }

    public static string ManifestPath(string modsDir) => Path.Combine(modsDir ?? "", "manifest.json");

    /// <summary>Loads + validates Mods/manifest.json. Never throws.</summary>
    public static Validation TryLoad(string modsDir)
    {
        var result = new Validation { Manifest = null };
        try
        {
            if (string.IsNullOrWhiteSpace(modsDir) || !Directory.Exists(modsDir))
            {
                result.Warnings.Add("Mods directory missing.");
                return result;
            }
            string path = ManifestPath(modsDir);
            if (!File.Exists(path)) return result;
            GregModpackManifest manifest;
            try
            {
                manifest = JsonSerializer.Deserialize<GregModpackManifest>(
                    File.ReadAllText(path),
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                result.Warnings.Add($"manifest.json invalid: {ex.GetBaseException().Message}");
                return result;
            }
            if (manifest == null) { result.Warnings.Add("manifest.json empty."); return result; }
            return Validate(manifest, modsDir);
        }
        catch (Exception ex)
        {
            result.Warnings.Add($"manifest load failed: {ex.GetBaseException().Message}");
            return result;
        }
    }

    internal static Validation Validate(GregModpackManifest manifest, string modsDir)
    {
        var result = new Validation { Manifest = manifest };
        string root;
        try { root = Path.GetFullPath(modsDir); }
        catch { result.Warnings.Add("Mods directory invalid."); return result; }

        foreach (var e in manifest.Mods ?? Enumerable.Empty<string>())
            CheckEntry(e, root, "Mods", result.ValidMods, result.Warnings);
        foreach (var e in manifest.Library ?? Enumerable.Empty<string>())
            CheckEntry(e, root, "Library", result.ValidLibrary, result.Warnings);
        foreach (var e in manifest.Plugins ?? Enumerable.Empty<string>())
            CheckEntry(e, root, "Plugins", result.ValidPlugins, result.Warnings);

        try
        {
            foreach (var lib in result.ValidLibrary)
            {
                string dir = Path.GetDirectoryName(Path.Combine(root, lib));
                if (!string.IsNullOrEmpty(dir) && !result.LibraryFolders.Contains(dir, StringComparer.OrdinalIgnoreCase))
                    result.LibraryFolders.Add(dir);
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return result;
    }

    private static void CheckEntry(string entry, string root, string section, List<string> valid, List<string> warnings)
    {
        if (string.IsNullOrWhiteSpace(entry))
        {
            warnings.Add($"{section}: empty entry skipped.");
            return;
        }
        string rel = entry.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
        string full;
        try { full = Path.GetFullPath(Path.Combine(root, rel)); }
        catch { warnings.Add($"{section}: invalid path '{entry}'."); return; }
        if (!full.Equals(root, StringComparison.OrdinalIgnoreCase) &&
            !full.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
        {
            warnings.Add($"{section}: '{entry}' escapes Mods/ — skipped.");
            return;
        }
        if (Infrastructure.IO.GregDeactivatedGuard.IsDeactivatedPath(full))
        {
            warnings.Add($"{section}: '{entry}' is deactivated — skipped (activate via gregCore first).");
            return;
        }
        if (!full.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            warnings.Add($"{section}: '{entry}' is not a .dll — skipped.");
            return;
        }
        if (!File.Exists(full))
        {
            warnings.Add($"{section}: '{entry}' not found.");
            return;
        }
        valid.Add(entry);
    }
}
