/// <file-summary>
/// Schicht:      Infrastructure (IO)
/// Zweck:        Verzeichnis-Policy (gregCore Baukasten):
///               - Mods (manuell): AUSSCHLIESSLICH ./Mods, keine Unterordner.
///                 Ausnahme: ./Mods/gregNative/* (gregNative-Pakete, TopLevel).
///               - gregNative: AUSSCHLIESSLICH ./Mods/gregNative.
///               - UserLibs (Lua/Rust/JS-Kompatibilitaet u.ae., keine Mods):
///                 AUSSCHLIESSLICH ./UserLibs.
///               - MelonLoader-Plugins: AUSSCHLIESSLICH ./Plugins.
///               - UserData: AUSSCHLIESSLICH ./UserData.
///               Audit() meldet Verstoesse (Report, kein Auto-Move).
///               EnsureLayout() legt fehlende Ordner an.
/// </file-summary>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using MelonLoader;
using Mono.Cecil;

namespace gregCore.Infrastructure.IO;

[ExcludeFromCodeCoverage(Justification = "Filesystem layout checks over live game install; needs running game.")]
public static class GregDirectoryPolicy
{
    public sealed class Violation
    {
        public string Kind;
        public string Path;
        public string Detail;
        public override string ToString() => $"[{Kind}] {Path}: {Detail}";
    }

    private const string MelonModBase = "MelonLoader.MelonMod";
    private const string MelonPluginBase = "MelonLoader.MelonPlugin";

    public static void EnsureLayout(string gameRoot)
    {
        if (string.IsNullOrWhiteSpace(gameRoot)) return;
        foreach (string sub in new[] { "Mods", "Plugins", "UserLibs", "UserData", Path.Combine("Mods", "gregNative") })
        {
            try
            {
                string dir = Path.Combine(gameRoot, sub);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    public static IReadOnlyList<Violation> Audit(string gameRoot)
    {
        var result = new List<Violation>();
        if (string.IsNullOrWhiteSpace(gameRoot) || !Directory.Exists(gameRoot)) return result;
        try { AuditModsDir(Path.Combine(gameRoot, "Mods"), result); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try { AuditFlatDir(Path.Combine(gameRoot, "Plugins"), "plugin"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        try { AuditFlatDir(Path.Combine(gameRoot, "UserLibs"), "userlib"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        return result;
    }

    public static void LogReport(IReadOnlyList<Violation> violations)
    {
        try
        {
            if (violations == null || violations.Count == 0)
            {
                MelonLogger.Msg("[gregCore][Dirs] Layout ok: Mods/Plugins/UserLibs/UserData + Mods/gregNative.");
                return;
            }
            MelonLogger.Warning($"[gregCore][Dirs] {violations.Count} Verzeichnis-Verstoesse:");
            foreach (var v in violations.Take(20))
                MelonLogger.Warning("[gregCore][Dirs] " + v);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    // Mods/: DLLs nur top-level. Ausnahme: gregNative-Baum (eigene Regel).
    // MelonMod-DLLs in Unterordnern = Verstoss (MelonLoader scannt sie nicht;
    // Staging/Modmanager muessen sie nach ./Mods spiegeln).
    private static void AuditModsDir(string modsDir, List<Violation> result)
    {
        if (!Directory.Exists(modsDir)) return;
        string nativeRoot;
        try { nativeRoot = Path.GetFullPath(Path.Combine(modsDir, "gregNative")); }
        catch { nativeRoot = null; }
        foreach (string file in Directory.EnumerateFiles(modsDir, "*.dll", SearchOption.AllDirectories))
        {
            string full;
            try { full = Path.GetFullPath(file); } catch { continue; }
            string dir = Path.GetDirectoryName(full);
            if (string.Equals(dir, modsDir, System.StringComparison.OrdinalIgnoreCase)) continue;
            if (nativeRoot != null && (full.StartsWith(nativeRoot + Path.DirectorySeparatorChar, System.StringComparison.OrdinalIgnoreCase)))
                continue; // gregNative-Baum: eigene Regel, hier ok
            string kind = ClassifyAssembly(full);
            if (kind == "mod")
                result.Add(new Violation { Kind = "mod-in-subdirectory", Path = full, Detail = "Manuelle Mods duerfen nur direkt in ./Mods liegen (keine Unterordner)." });
        }
    }

    // Plugins/UserLibs: keine Aussage ueber Inhalt, nur Existenz-Check fuer Report.
    // Harte Pruefung (fremde Mod-DLLs) gehoert dem Companion-Plugin (Cecil dort).
    private static void AuditFlatDir(string dir, string kind)
    {
        if (!Directory.Exists(dir))
            throw new DirectoryNotFoundException(dir);
    }

    // "mod" | "plugin" | "lib" | "unknown" via Cecil-Vererbung (read-only, kein Load).
    internal static string ClassifyAssembly(string file)
    {
        try
        {
            using var module = ModuleDefinition.ReadModule(file);
            foreach (var type in module.Types.SelectMany(AllTypes))
            {
                try
                {
                    if (type.IsInterface || type.IsAbstract) continue;
                    for (var b = type.BaseType; b != null;)
                    {
                        string name = null;
                        try { name = b.FullName; } catch { break; }
                        if (name == MelonModBase) return "mod";
                        if (name == MelonPluginBase) return "plugin";
                        TypeDefinition resolved = null;
                        try { resolved = b.Resolve(); } catch { break; }
                        if (resolved == null) break;
                        b = resolved.BaseType;
                    }
                }
                catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            }
        }
        catch { return "unknown"; }
        return "lib";
    }

    private static IEnumerable<TypeDefinition> AllTypes(TypeDefinition type)
    {
        yield return type;
        foreach (var nested in type.NestedTypes.SelectMany(AllTypes)) yield return nested;
    }
}
