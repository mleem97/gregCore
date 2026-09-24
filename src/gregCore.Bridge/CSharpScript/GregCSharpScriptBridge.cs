using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;
using gregCore.API;

namespace gregCore.Bridge.CSharpScript;

/// <summary>
/// Orchestrates discovery, compilation, and lifecycle of C# script mods.
/// </summary>
public sealed class GregCSharpScriptBridge
{
    private static readonly List<GregCSharpModContext> _mods = new();
    private static bool _initialized;
    private static string _csharpDir = "";
    private static string _currentScene = "";
    private static FileSystemWatcher _watcher;
    private static System.Timers.Timer _debounce;
    private static readonly HashSet<string> _pendingReloads = new(StringComparer.OrdinalIgnoreCase);
    private static bool _pendingLogged;

    public static void Initialize()
    {
        if (_initialized) return;

        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string csharpDir = Path.Combine(gameRoot, "UserData", "gregCore", "Mods", "CSharp");

        if (!Directory.Exists(csharpDir))
            Directory.CreateDirectory(csharpDir);
        _csharpDir = csharpDir;

        if (!GregCSharpCompiler.IsAvailable)
        {
            MelonLogger.Warning("[CSharpScriptBridge] Roslyn not available — C# script mods will not be loaded.");
            _initialized = true;
            return;
        }

        LoadMods(csharpDir);
        StartWatching();
        _initialized = true;
    }

    public static bool IsMainMenu()
    {
        try { return string.Equals(_currentScene, "MainMenu", StringComparison.OrdinalIgnoreCase); }
        catch { return false; }
    }

    /// <summary>
    /// Applies pending script reloads — main menu only (never mid-game:
    /// live objects would dangle). Returns true when applied now.
    /// Queued reloads auto-apply on next main-menu entry.
    /// </summary>
    public static bool TryReloadNow()
    {
        try
        {
            if (!_initialized) return false;
            lock (_pendingReloads)
            {
                if (_pendingReloads.Count == 0) return true;
                if (!IsMainMenu()) return false;
                var dirs = _pendingReloads.ToList();
                _pendingReloads.Clear();
                _pendingLogged = false;
                foreach (var dir in dirs) ReloadModDir(dir);
                return true;
            }
        }
        catch { return false; }
    }

    private static void StartWatching()
    {
        try
        {
            if (_watcher != null || string.IsNullOrEmpty(_csharpDir)) return;
            _watcher = new FileSystemWatcher(_csharpDir, "*.cs");
            _watcher.IncludeSubdirectories = true;
            _watcher.NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            _watcher.Changed += (_, e) => QueueReload(e.FullPath);
            _watcher.Created += (_, e) => QueueReload(e.FullPath);
            _watcher.Renamed += (_, e) => QueueReload(e.FullPath);
            _watcher.Deleted += (_, e) => QueueReload(e.FullPath);
            _watcher.EnableRaisingEvents = true;
            _debounce = new System.Timers.Timer(500);
            _debounce.AutoReset = false;
            _debounce.Elapsed += (_, __) => ProcessPending();
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[CSharpScriptBridge] Watcher failed: {ex.Message}");
        }
    }

    private static void QueueReload(string fullPath)
    {
        try
        {
            if (string.IsNullOrEmpty(fullPath)) return;
            string dir = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(dir)) return;
            lock (_pendingReloads) { _pendingReloads.Add(dir); }
            try
            {
                if (_debounce != null) { _debounce.Stop(); _debounce.Start(); }
                else ProcessPending();
            }
            catch { ProcessPending(); }
        }
        catch { }
    }

    private static void ProcessPending()
    {
        try
        {
            lock (_pendingReloads)
            {
                if (_pendingReloads.Count == 0) return;
                if (!IsMainMenu())
                {
                    if (!_pendingLogged)
                    {
                        _pendingLogged = true;
                        MelonLogger.Msg("[CSharpScriptBridge] Script changes queued — applied on next main-menu entry.");
                    }
                    return;
                }
                _pendingReloads.Clear();
                _pendingLogged = false;
            }
            ReloadAll();
        }
        catch { }
    }

    private static void ReloadAll()
    {
        List<GregCSharpModContext> snapshot;
        lock (_mods) { snapshot = _mods.ToList(); _mods.Clear(); }
        foreach (var ctx in snapshot)
        {
            try { ctx.SafeCall(() => ctx.Instance.OnShutdown(), "OnShutdown"); } catch { }
        }
        if (!string.IsNullOrEmpty(_csharpDir))
        {
            try { LoadMods(_csharpDir); } catch { }
        }
        MelonLogger.Msg("[CSharpScriptBridge] Scripts reloaded (main menu).");
    }

    private static void ReloadModDir(string modDir)
    {
        try
        {
            if (string.IsNullOrEmpty(modDir)) return;
            GregCSharpModContext old = null;
            lock (_mods)
            {
                for (int i = _mods.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(_mods[i]?.Directory, modDir, StringComparison.OrdinalIgnoreCase))
                    {
                        old = _mods[i];
                        _mods.RemoveAt(i);
                    }
                }
            }
            if (old != null)
            {
                try { old.SafeCall(() => old.Instance.OnShutdown(), "OnShutdown"); } catch { }
            }
            if (Directory.Exists(modDir)) LoadOne(modDir);
            MelonLogger.Msg($"[CSharpScriptBridge] Reloaded '{Path.GetFileName(modDir)}'.");
        }
        catch { }
    }

    private static void LoadMods(string csharpDir)
    {
        foreach (string modDir in Directory.GetDirectories(csharpDir))
        {
            LoadOne(modDir);
        }
    }

    private static void LoadOne(string modDir)
    {
        string modId = "";
        try
        {
            modId = Path.GetFileName(modDir);
            if (modId.StartsWith("@") || modId.StartsWith(".")) return;

            string[] csFiles = global::gregCore.Infrastructure.IO.GregFileSystem.EnumerateFilesByExtension(modDir, ".cs", SearchOption.TopDirectoryOnly).ToArray();
            if (csFiles.Length == 0) return;

            Assembly assembly = GregCSharpCompiler.Compile(modId, csFiles);
            if (assembly == null) return;

            var modType = assembly.GetTypes()
                .FirstOrDefault(t => typeof(IGregCSharpMod).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);
            if (modType == null)
            {
                MelonLogger.Warning("[CSharpScriptBridge] Mod has no class implementing IGregCSharpMod: " + modId);
                return;
            }

            var instance = (IGregCSharpMod)Activator.CreateInstance(modType);
            if (instance == null)
            {
                MelonLogger.Warning("[CSharpScriptBridge] Failed to instantiate mod: " + modId);
                return;
            }

            var context = new GregCSharpModContext
            {
                Id = instance.ModId,
                Directory = modDir,
                Assembly = assembly,
                Instance = instance
            };

            context.SafeCall(() => context.Instance.OnInit(), "OnInit");
            context.Initialized = true;
            lock (_mods) { _mods.Add(context); }

            MelonLogger.Msg("[CSharpScriptBridge] Loaded mod: " + instance.ModName + " v" + instance.Version);
        }
        catch (ReflectionTypeLoadException ex)
        {
            MelonLogger.Error("[CSharpScriptBridge] Type load error in mod '" + modId + "': " + ex.Message);
        }
        catch (Exception ex)
        {
            MelonLogger.Error("[CSharpScriptBridge] Failed to load mod '" + modId + "': " + ex.Message);
        }
    }

    public static void OnUpdate(float dt)
    {
        if (!_initialized) return;
        List<GregCSharpModContext> snapshot;
        lock (_mods) { snapshot = _mods.ToList(); }
        foreach (var mod in snapshot)
        {
            if (!mod.Initialized) continue;
            mod.SafeCall(() => mod.Instance.OnUpdate(dt), "OnUpdate");
        }
    }

    public static void OnSceneLoaded(string sceneName)
    {
        if (!_initialized) return;
        try { _currentScene = sceneName ?? ""; } catch { }
        List<GregCSharpModContext> snapshot;
        lock (_mods) { snapshot = _mods.ToList(); }
        foreach (var mod in snapshot)
        {
            if (!mod.Initialized) continue;
            mod.SafeCall(() => mod.Instance.OnSceneLoaded(sceneName), "OnSceneLoaded");
        }
        // Queued script changes apply on main-menu entry (never mid-game).
        try { if (IsMainMenu()) TryReloadNow(); } catch { }
    }

    public static void Shutdown()
    {
        if (!_initialized) return;
        foreach (var mod in _mods)
        {
            mod.SafeCall(() => mod.Instance.OnShutdown(), "OnShutdown");
        }
        _mods.Clear();
        _initialized = false;
    }
}
