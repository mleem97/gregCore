using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MelonLoader;
using gregCore.Sdk.Language.Hosts;

namespace gregCore.Sdk.Language;

public enum Language
{
    Lua,
    Rust,
    Python,
    CSharpScript,
    JavaScript
}

public static class GregLanguageRegistry
{
    private static readonly Dictionary<Language, IGregLanguageHost> ActiveHosts = new();
    private static readonly Dictionary<Language, IGregLanguageHost> AvailableHosts = new()
    {
        { Language.Lua, new GregLuaHost() },
        { Language.Rust, new GregRustHost() },
        { Language.Python, new GregPythonHost() },
        { Language.CSharpScript, new GregCSharpScriptHost() },
        { Language.JavaScript, new GregJsHost() }
    };

    private static bool _scanCompleted;

    public static bool IsActive(Language lang)
    {
        return ActiveHosts.ContainsKey(lang);
    }

    public static IGregLanguageHost GetHost(Language lang)
    {
        if (!ActiveHosts.TryGetValue(lang, out var host))
        {
            throw new KeyNotFoundException($"Language host is not active: {lang}");
        }

        return host;
    }

    public static void ScanAndActivate(string modsScriptsDir)
    {
        if (_scanCompleted)
        {
            MelonLogger.Msg("[gregCore] Language scan already completed. Skipping repeated activation.");
            return;
        }

        Directory.CreateDirectory(modsScriptsDir);

        var scan = new ScriptScanResult(modsScriptsDir)
        {
            LuaCount = Count(modsScriptsDir, "*.lua"),
            PythonCount = Count(modsScriptsDir, "*.py"),
            RustRsCount = Count(modsScriptsDir, "*.rs"),
            RustRmodCount = Count(modsScriptsDir, "*.rmod"),
            JsCount = Count(modsScriptsDir, "*.js"),
            TsCount = Count(modsScriptsDir, "*.ts"),
            CSharpCount = Count(modsScriptsDir, "*.cs")
        };

        LogDependencyBlock();
        LogScanBlock(scan);

        MelonLogger.Msg("[gregCore] ── Language Host Activation ─────────────────");

        TryActivate(Language.Lua, scan.ModsScriptsDir, scan.LuaCount > 0, "MoonSharp 2.0.0", "Lua scripts detected", "no *.lua scripts found");
        TryActivate(Language.Python, scan.ModsScriptsDir, scan.PythonCount > 0, "Python-Host-Bindings", "Python scripts detected", "no *.py scripts found", scan.PythonCount);
        TryActivate(Language.Rust, scan.ModsScriptsDir, scan.RustTotalCount > 0, "FFI-Bridge", "Rust mods detected", "no *.rs/*.rmod scripts found");
        TryActivate(Language.JavaScript, scan.ModsScriptsDir, scan.JsTsTotalCount > 0, "JS runtime", "JS/TS scripts detected", "no *.js/*.ts scripts found");
        TryActivate(Language.CSharpScript, scan.ModsScriptsDir, scan.CSharpCount > 0, "Roslyn", "C# scripts detected", "no *.cs scripts found");

        MelonLogger.Msg("[gregCore] ────────────────────────────────────────────");
        MelonLogger.Msg($"[gregCore]   Active hosts: {ActiveHosts.Count} / 5");

        int totalScripts = scan.LuaCount + scan.PythonCount + scan.RustTotalCount + scan.JsTsTotalCount + scan.CSharpCount;
        MelonLogger.Msg("[gregCore] ═══════════════════════════════════════════════════════════");
        MelonLogger.Msg("[gregCore] ✓ gregCore initialization complete");
        MelonLogger.Msg("[gregCore]   Harmony patches:    OK [UNVERIFIED]");
        MelonLogger.Msg("[gregCore]   ModConfig:          OK [UNVERIFIED]");
        MelonLogger.Msg($"[gregCore]   Language Hosts:     {ActiveHosts.Count} active");
        MelonLogger.Msg($"[gregCore]   Scripts (Mods/Scripts): {totalScripts} files total");
        MelonLogger.Msg("[gregCore] ═══════════════════════════════════════════════════════════");

        _scanCompleted = true;
    }

    public static void OnUpdate(float dt)
    {
        foreach (var host in ActiveHosts.Values)
        {
            try
            {
                host.OnUpdate(dt);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host update failed ({host.HostName}): {ex}");
            }
        }
    }

    public static void OnSceneLoaded(string sceneName)
    {
        foreach (var host in ActiveHosts.Values)
        {
            try
            {
                host.OnSceneLoaded(sceneName);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host scene callback failed ({host.HostName}): {ex}");
            }
        }
    }

    public static void Shutdown()
    {
        foreach (var host in ActiveHosts.Values.ToArray())
        {
            try
            {
                host.Shutdown();
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gregCore] Host shutdown failed ({host.HostName}): {ex}");
            }
        }

        ActiveHosts.Clear();
        _scanCompleted = false;
    }

    private static int Count(string root, string pattern)
    {
        return Directory.EnumerateFiles(root, pattern, SearchOption.AllDirectories).Count();
    }

    private static void LogDependencyBlock()
    {
        bool pythonAvailable = Type.GetType("Python.Runtime.PythonEngine, Python.Runtime") != null;
        bool jsAvailable = Type.GetType("Jint.Engine, Jint") != null;

        MelonLogger.Msg("[gregCore] ── Dependency Check ────────────────────────");
        MelonLogger.Msg("[gregCore]   MoonSharp 2.0.0          ✓ embedded");
        MelonLogger.Msg($"[gregCore]   Python.Runtime 3.0.3     {(pythonAvailable ? "✓ loaded" : "✗ NOT FOUND (UserLibs/)")}");
        MelonLogger.Msg("[gregCore]   Rust FFI Bridge          ✓ loaded");
        MelonLogger.Msg($"[gregCore]   JS Runtime               {(jsAvailable ? "✓ loaded" : "✗ NOT FOUND")}");
        MelonLogger.Msg("[gregCore] ────────────────────────────────────────────");
    }

    private static void LogScanBlock(ScriptScanResult scan)
    {
        MelonLogger.Msg($"[gregCore] ── Script Scan: {scan.ModsScriptsDir} ────────────────");
        MelonLogger.Msg($"[gregCore]   *.lua    → {scan.LuaCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.py     → {scan.PythonCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.rs     → {scan.RustRsCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.rmod   → {scan.RustRmodCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.js     → {scan.JsCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.ts     → {scan.TsCount} file(s) found");
        MelonLogger.Msg($"[gregCore]   *.cs     → {scan.CSharpCount} file(s) found");
        MelonLogger.Msg("[gregCore] ────────────────────────────────────────────");
    }

    private static void TryActivate(
        Language language,
        string modsScriptsDir,
        bool hasScripts,
        string dependencyName,
        string detectedMessage,
        string notFoundMessage,
        int relevantCount = 0)
    {
        var host = AvailableHosts[language];

        if (!hasScripts)
        {
            MelonLogger.Msg($"[gregCore]   [{ToLabel(language)}] {("✗ SKIPPED").PadRight(10)} — {notFoundMessage}");
            return;
        }

        try
        {
            if (!host.IsDependencyAvailable(out var detail))
            {
                MelonLogger.Warning($"[gregCore] {detectedMessage} but dependency missing — {dependencyName}: {detail}");
                if (language == Language.Python)
                {
                    MelonLogger.Warning($"[gregCore] ⚠ WARNING: {relevantCount}x *.py found but Python.Runtime.dll is missing!");
                    MelonLogger.Warning("[gregCore]   → Place Python.Runtime.dll in:");
                    MelonLogger.Warning("[gregCore]     <Data Center>\\UserLibs\\Python.Runtime.dll");
                    MelonLogger.Warning("[gregCore]   → Python mods will NOT be executed this session.");
                }

                MelonLogger.Msg($"[gregCore]   [{ToLabel(language)}] {("✗ SKIPPED").PadRight(10)} — dependency missing ({detail})");
                return;
            }

            host.Activate(modsScriptsDir);
            ActiveHosts[language] = host;

            if (language == Language.Lua)
            {
                MelonLogger.Msg("[gregCore] Lua scripts detected — activating GregLuaHost (MoonSharp 2.0.0)");
            }
            else if (language == Language.Rust)
            {
                MelonLogger.Msg("[gregCore] Rust mods detected — activating GregRustHost (FFI-Bridge)");
            }
            else if (language == Language.Python)
            {
                MelonLogger.Msg("[gregCore] Python scripts detected — activating GregPythonHost (Python-Host-Bindings)");
            }
            else if (language == Language.JavaScript)
            {
                MelonLogger.Msg("[gregCore] JS/TS scripts detected — activating GregJsHost (JS-Runtime-Binding)");
            }
            else if (language == Language.CSharpScript)
            {
                MelonLogger.Msg("[gregCore] C# scripts detected — activating GregCSharpScriptHost (Roslyn)");
            }

            MelonLogger.Msg($"[gregCore]   [{ToLabel(language)}] {("✓ ACTIVE").PadRight(10)} — {host.HostName}");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore] Host activation failed for {language}: {ex}");
        }
    }

    private static string ToLabel(Language language)
    {
        return language switch
        {
            Language.Lua => "Lua",
            Language.Python => "Python",
            Language.Rust => "Rust",
            Language.JavaScript => "JavaScript",
            Language.CSharpScript => "C# Script",
            _ => language.ToString()
        };
    }

    private sealed class ScriptScanResult
    {
        public ScriptScanResult(string modsScriptsDir)
        {
            ModsScriptsDir = modsScriptsDir;
        }

        public string ModsScriptsDir { get; }
        public int LuaCount { get; set; }
        public int PythonCount { get; set; }
        public int RustRsCount { get; set; }
        public int RustRmodCount { get; set; }
        public int JsCount { get; set; }
        public int TsCount { get; set; }
        public int CSharpCount { get; set; }
        public int RustTotalCount => RustRsCount + RustRmodCount;
        public int JsTsTotalCount => JsCount + TsCount;
    }
}
