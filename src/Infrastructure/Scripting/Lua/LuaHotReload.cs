using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using MoonSharp.Interpreter;
using MelonLoader;
using Timer = System.Timers.Timer;

namespace gregCore.Infrastructure.Scripting.Lua;

/// <summary>
/// Watches Lua files for changes and hot-reloads them.
/// Supports on_reload() callback in mods.
/// </summary>
public class LuaHotReload : IDisposable
{
    private readonly string _watchRoot;
    private readonly Action<LuaPluginReloadInfo> _onReload;
    private FileSystemWatcher? _watcher;
    private readonly System.Timers.Timer _debounceTimer;
    private readonly HashSet<string> _pendingFiles = new();
    private readonly Dictionary<string, LuaPluginEntry> _pluginMap = new();

    private class LuaPluginEntry
    {
        public string Id = "";
        public Script Script = null!;
        public string MainFilePath = "";
        public Closure? OnInit;
        public Closure? OnUpdate;
        public Closure? OnReload;
        public Closure? OnShutdown;
    }

    public LuaHotReload(string watchRoot, Action<LuaPluginReloadInfo> onReload)
    {
        _watchRoot = Path.GetFullPath(watchRoot);
        _onReload = onReload;
        _debounceTimer = new Timer(500);
        _debounceTimer.Elapsed += (s, e) =>
        {
            _debounceTimer.Stop();
            ProcessPendingReloads();
        };
    }

    public void RegisterPlugin(string modId, Script script, string mainFilePath)
    {
        var entry = new LuaPluginEntry
        {
            Id = modId,
            Script = script,
            MainFilePath = mainFilePath,
            OnInit = script.Globals.Get("on_init").Type == DataType.Function
                ? script.Globals.Get("on_init").Function : null,
            OnUpdate = script.Globals.Get("on_update").Type == DataType.Function
                ? script.Globals.Get("on_update").Function : null,
            OnReload = script.Globals.Get("on_reload").Type == DataType.Function
                ? script.Globals.Get("on_reload").Function : null,
            OnShutdown = script.Globals.Get("on_shutdown").Type == DataType.Function
                ? script.Globals.Get("on_shutdown").Function : null,
        };
        _pluginMap[modId] = entry;
    }

    public void Start()
    {
        if (!Directory.Exists(_watchRoot)) return;

        _watcher = new FileSystemWatcher(_watchRoot, "*.lua")
        {
            IncludeSubdirectories = true,
            EnableRaisingEvents = true,
            NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName
        };

        _watcher.Changed += OnFileChanged;
        _watcher.Created += OnFileChanged;
        _watcher.Deleted += OnFileChanged;
        _watcher.Renamed += OnFileChanged;

        MelonLogger.Msg("[LuaHotReload] Watching: " + _watchRoot);
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_pendingFiles)
        {
            _pendingFiles.Add(e.FullPath);
            _debounceTimer.Stop();
            _debounceTimer.Start();
        }
    }

    private void ProcessPendingReloads()
    {
        string[] files;
        lock (_pendingFiles)
        {
            files = _pendingFiles.ToArray();
            _pendingFiles.Clear();
        }

        foreach (var file in files)
        {
            if (!File.Exists(file)) continue;

            string? modId = FindModForFile(file);
            if (modId == null) continue;

            if (_pluginMap.TryGetValue(modId, out var entry))
                ReloadPlugin(entry, file);
        }
    }

    private string? FindModForFile(string filePath)
    {
        string fullPath = Path.GetFullPath(filePath);
        string rootPath = _watchRoot;

        if (!IsPathWithin(fullPath, rootPath))
            return null;

        string? current = Path.GetDirectoryName(fullPath);
        while (current != null && IsPathWithin(current, rootPath))
        {
            if (File.Exists(Path.Combine(current, "main.lua")))
                return Path.GetFileName(current);

            if (current.Equals(rootPath, PathComparison))
                break;

            current = Path.GetDirectoryName(current);
        }

        return null;
    }

    private static StringComparison PathComparison => OperatingSystem.IsWindows()
        ? StringComparison.OrdinalIgnoreCase
        : StringComparison.Ordinal;

    private static bool IsPathWithin(string candidatePath, string rootPath)
    {
        if (candidatePath.Equals(rootPath, PathComparison))
            return true;

        string rootWithSeparator = Path.EndsInDirectorySeparator(rootPath)
            ? rootPath
            : rootPath + Path.DirectorySeparatorChar;

        return candidatePath.StartsWith(rootWithSeparator, PathComparison);
    }

    private void ReloadPlugin(LuaPluginEntry entry, string changedFile)
    {
        try
        {
            if (entry.OnReload != null)
            {
                try { entry.OnReload.Call(); }
                catch (Exception ex) { MelonLogger.Error($"[LuaHotReload] {entry.Id} on_reload error: {ex.Message}"); }
            }

            if (entry.OnShutdown != null)
            {
                try { entry.OnShutdown.Call(); }
                catch (Exception ex) { MelonLogger.Error($"[LuaHotReload] {entry.Id} on_shutdown error: {ex.Message}"); }
            }

            string mainFile = entry.MainFilePath;
            if (!File.Exists(mainFile)) return;

            var newScript = new Script(CoreModules.Preset_SoftSandbox);

            _onReload(new LuaPluginReloadInfo
            {
                ModId = entry.Id,
                OldScript = entry.Script,
                NewScript = newScript,
                MainFilePath = mainFile
            });
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaHotReload] Reload failed for {entry.Id}: {ex.Message}");
        }
    }

    public void Stop() => Dispose();

    public void Dispose()
    {
        _watcher?.Dispose();
        _debounceTimer.Dispose();
    }
}

public struct LuaPluginReloadInfo
{
    public string ModId;
    public Script OldScript;
    public Script NewScript;
    public string MainFilePath;
}
