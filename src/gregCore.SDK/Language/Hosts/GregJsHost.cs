/// <file-summary>
/// Layer:       SDK (Language host: javascript)
/// Purpose:     Per-mod Jint runtime for JS script mods.
///              Dirs: UserData/gregCore/Mods/JS/<modId>/*.js (+ compiled output).
///              Each mod gets an isolated engine with the full UI-focused
///              `greg` API (log, toasts, panels, menus, settings, hooks).
///              HotLoad: file watcher + debounce, applied in the main menu
///              only (never mid-game). Raw .ts files are skipped with a
///              warning (compile with tsc first, see templates/js).
///              English only.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jint;
using Jint.Native;
using MelonLoader;

namespace gregCore.Sdk.Language.Hosts;

public sealed class GregJsHost : IGregLanguageHost
{
    public string HostId => "javascript";
    public string HostName => nameof(GregJsHost);
    public bool IsActive { get; private set; }
    public string[] FileExtensions => new[] { ".js", ".ts" };

    private sealed class ModRuntime
    {
        public string Id = "";
        public string Directory = "";
        public Engine Engine;
        public JsValue UpdateFn;
        public JsValue SceneFn;
        public bool UpdateValid = true;
        public bool SceneValid = true;
    }

    private readonly List<ModRuntime> _mods = new();
    private string _jsDir = "";
    private string _currentScene = "";
    private FileSystemWatcher _watcher;
    private System.Timers.Timer _debounce;
    private readonly HashSet<string> _pendingReloads = new(StringComparer.OrdinalIgnoreCase);
    private bool _pendingLogged;

    public (bool available, string detail) IsDependencyAvailable()
    {
        try { return (typeof(Engine) != null, "JS runtime binding (Jint)"); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return (false, "JS runtime binding (Jint)"); }
    }

    public void Activate(string modsScriptsDir)
    {
        if (IsActive) return;
        try
        {
            string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
            _jsDir = Path.Combine(gameRoot, "UserData", "gregCore", "Mods", "JS");
            if (!Directory.Exists(_jsDir)) Directory.CreateDirectory(_jsDir);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  _jsDir = ""; }
        if (string.IsNullOrEmpty(_jsDir)) return;

        LoadAll();
        StartWatching();
        IsActive = true;
    }

    public void OnUpdate(float dt)
    {
        if (!IsActive) return;
        List<ModRuntime> snapshot;
        lock (_mods) { snapshot = _mods.ToList(); }
        foreach (var mod in snapshot)
        {
            if (mod?.Engine == null || !mod.UpdateValid) continue;
            try
            {
                var fn = mod.UpdateFn;
                if (fn == null || fn.IsNull() || fn.IsUndefined()) continue;
                fn.Call(JsValue.Undefined, new[] { JsValue.FromObject(mod.Engine, dt) });
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  mod.UpdateValid = false; }
        }
    }

    public void OnSceneLoaded(string sceneName)
    {
        if (!IsActive) return;
        try { _currentScene = sceneName ?? ""; } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        List<ModRuntime> snapshot;
        lock (_mods) { snapshot = _mods.ToList(); }
        foreach (var mod in snapshot)
            DispatchScene(mod, sceneName);
        try { if (IsMainMenu()) TryReloadNow(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void DispatchScene(ModRuntime mod, string sceneName)
    {
        if (mod?.Engine == null || !mod.SceneValid) return;
        try
        {
            var fn = mod.SceneFn;
            if (fn == null || fn.IsNull() || fn.IsUndefined()) return;
            fn.Call(JsValue.Undefined, new[] { JsValue.FromObject(mod.Engine, sceneName ?? "") });
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  mod.SceneValid = false; }
    }

    public void Shutdown()
    {
        try { if (_watcher != null) { _watcher.EnableRaisingEvents = false; _watcher.Dispose(); _watcher = null; } } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        try { if (_debounce != null) { _debounce.Stop(); _debounce.Dispose(); _debounce = null; } } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        lock (_mods) { _mods.Clear(); }
        IsActive = false;
    }

    private static bool IsMainMenuStatic(string scene)
    {
        try { return string.Equals(scene, "MainMenu", StringComparison.OrdinalIgnoreCase); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private bool IsMainMenu() => IsMainMenuStatic(_currentScene);

    /// <summary>Applies pending reloads now (main menu only). True = applied (or nothing pending).</summary>
    public bool TryReloadNow()
    {
        try
        {
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
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    private void LoadAll()
    {
        try
        {
            if (!Directory.Exists(_jsDir)) return;
            foreach (var modDir in Directory.GetDirectories(_jsDir))
            {
                var name = Path.GetFileName(modDir);
                if (name.StartsWith("@") || name.StartsWith(".")) continue;
                LoadOne(modDir);
            }
            WarnRawTypeScript(_jsDir);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void LoadOne(string modDir)
    {
        try
        {
            string modId = Path.GetFileName(modDir);
            var files = Directory.GetFiles(modDir, "*.js", SearchOption.TopDirectoryOnly);
            if (files.Length == 0) return;

            var engine = new Engine(cfg => cfg.AllowClr().LimitMemory(16_000_000));
            var rt = new ModRuntime { Id = modId, Directory = modDir, Engine = engine };
            RegisterApi(engine, modId);

            foreach (var file in files.OrderBy(f => f))
            {
                try { engine.Execute(File.ReadAllText(file), Path.GetFileName(file)); }
                catch (Exception ex)
                {
                    MelonLogger.Error($"[gregCore][JS] {modId}/{Path.GetFileName(file)}: {ex.GetBaseException().Message}");
                }
            }

            try { rt.UpdateFn = engine.GetValue("onUpdate"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            try { rt.SceneFn = engine.GetValue("onSceneLoaded"); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
            lock (_mods)
            {
                _mods.RemoveAll(m => string.Equals(m?.Directory, modDir, StringComparison.OrdinalIgnoreCase));
                _mods.Add(rt);
            }
            MelonLogger.Msg($"[gregCore][JS] Loaded mod '{modId}' ({files.Length} file(s)).");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[gregCore][JS] Failed to load '{modDir}': {ex.GetBaseException().Message}");
        }
    }

    private static void WarnRawTypeScript(string dir)
    {
        try
        {
            var ts = Directory.GetFiles(dir, "*.ts", SearchOption.AllDirectories);
            foreach (var f in ts)
                MelonLogger.Warning($"[gregCore][JS] Skipped raw TypeScript (compile with tsc first, see templates/js): {f}");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void ReloadModDir(string modDir)
    {
        try
        {
            lock (_mods) { _mods.RemoveAll(m => string.Equals(m?.Directory, modDir, StringComparison.OrdinalIgnoreCase)); }
            if (Directory.Exists(modDir)) LoadOne(modDir);
            else MelonLogger.Msg($"[gregCore][JS] Unloaded removed mod '{Path.GetFileName(modDir)}'.");
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void StartWatching()
    {
        try
        {
            if (_watcher != null || string.IsNullOrEmpty(_jsDir)) return;
            _watcher = new FileSystemWatcher(_jsDir, "*.js");
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
            MelonLogger.Warning($"[gregCore][JS] Watcher failed: {ex.Message}");
        }
    }

    private void QueueReload(string fullPath)
    {
        try
        {
            if (string.IsNullOrEmpty(fullPath)) return;
            var dir = Path.GetDirectoryName(fullPath);
            if (string.IsNullOrEmpty(dir)) return;
            lock (_pendingReloads) { _pendingReloads.Add(dir); }
            try
            {
                if (_debounce != null) { _debounce.Stop(); _debounce.Start(); }
                else ProcessPending();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  ProcessPending(); }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void ProcessPending()
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
                        MelonLogger.Msg("[gregCore][JS] Script changes queued — applied on next main-menu entry.");
                    }
                    return;
                }
                _pendingReloads.Clear();
                _pendingLogged = false;
            }
            ReloadAllEngines();
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private void ReloadAllEngines()
    {
        List<string> dirs;
        lock (_mods) { dirs = _mods.Select(m => m.Directory).Distinct(StringComparer.OrdinalIgnoreCase).ToList(); _mods.Clear(); }
        foreach (var dir in dirs)
        {
            if (Directory.Exists(dir)) LoadOne(dir);
        }
        MelonLogger.Msg("[gregCore][JS] Scripts reloaded (main menu).");
    }

    // ── greg API (UI-focused) ────────────────────────────────────────────────

    private static void RegisterApi(Engine engine, string modId)
    {
        var api = new Dictionary<string, object>();
        RegisterLogApi(api, modId);
        RegisterNotifyApi(api, engine, modId);
        RegisterMenuApi(api, engine, modId);
        RegisterSettingsApi(api, engine, modId);
        RegisterHookApi(api, engine);
        engine.SetValue("greg", api);
    }

    private static void RegisterLogApi(Dictionary<string, object> api, string modId)
    {
        api["log"] = (Action<string>)(msg => SafeLog($"[{modId}] {msg}"));
        api["warn"] = (Action<string>)(msg => SafeWarn($"[{modId}] {msg}"));
        api["error"] = (Action<string>)(msg => SafeError($"[{modId}] {msg}"));
    }

    private static void RegisterNotifyApi(Dictionary<string, object> api, Engine engine, string modId)
    {
        api["toast"] = (Action<string, double>)((msg, dur) =>
        {
            try { gregCore.UI.GregNotificationManager.Show(msg ?? "", (float)dur); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
        api["toastRich"] = (Action<string, string, string, double>)((top, title, sub, dur) =>
        {
            try { gregCore.UI.GregNotificationManager.ShowRich(top ?? "", title ?? "", sub ?? "", null, null, (float)dur); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
        api["notify"] = (Action<string, string, double>)((title, msg, dur) =>
        {
            try
            {
                var api2 = gregCore.Core.GregCoreMod.PublicAPI;
                if (api2 != null) api2.ShowNotification(title ?? modId, msg ?? "", (float)dur);
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
    }

    private static void RegisterMenuApi(Dictionary<string, object> api, Engine engine, string modId)
    {
        api["createPanel"] = (Func<string, object>)(title =>
        {
            try { return gregCore.UI.GregPanelBuilder.Create(title ?? modId).Build(); }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return null; }
        });
        api["bindMenu"] = (Action<string, JsValue, JsValue>)((menuId, toggleFn, isOpenFn) =>
        {
            try
            {
                gregCore.UI.GregMenuBinding.BindToggle(menuId,
                    ToAction(toggleFn),
                    ToFuncBool(engine, isOpenFn));
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
        api["reportMenu"] = (Action<string, bool>)((menuId, open) =>
        {
            try { gregCore.UI.GregMenuBinding.Report(menuId, open); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
    }

    private static void RegisterSettingsApi(Dictionary<string, object> api, Engine engine, string modId)
    {
        api["registerToggle"] = (Action<string, string, bool>)((settingId, label, def) =>
        {
            try { gregCore.Core.GregCoreMod.PublicAPI?.RegisterToggle(modId, settingId, label, def); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
        api["registerSlider"] = (Action<string, string, double>)((settingId, label, def) =>
        {
            try { gregCore.Core.GregCoreMod.PublicAPI?.RegisterSlider(modId, settingId, label, (float)def); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
        api["registerKey"] = (Action<string, string, JsValue>)((actionId, label, fn) =>
        {
            try
            {
                gregCore.Core.GregCoreMod.PublicAPI?.RegisterKeybind(modId, actionId, label,
                    UnityEngine.KeyCode.None, ToAction(fn));
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
    }

    private static void RegisterHookApi(Dictionary<string, object> api, Engine engine)
    {
        api["on"] = (Action<string, JsValue>)((hookName, fn) =>
        {
            try
            {
                var bus = gregCore.Core.GregCoreMod.PublicAPI;
                if (bus == null) return;
                var cb = fn;
                bus.On(hookName, payload =>
                {
                    try { cb.Call(JsValue.Undefined, new[] { JsValue.FromObject(engine, payload?.Data) }); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
                });
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        });
    }

    private static Action ToAction(JsValue fn)
    {
        return () =>
        {
            try
            {
                if (fn == null || fn.IsNull() || fn.IsUndefined()) return;
                fn.Call(JsValue.Undefined, Array.Empty<JsValue>());
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        };
    }

    private static Func<bool> ToFuncBool(Engine engine, JsValue fn)
    {
        return () =>
        {
            try
            {
                if (fn == null || fn.IsNull() || fn.IsUndefined()) return false;
                var r = fn.Call(JsValue.Undefined, Array.Empty<JsValue>());
                return r.IsBoolean() && r.AsBoolean();
            }
            catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
        };
    }

    private static void SafeLog(string msg)
    {
        try { MelonLoader.MelonLogger.Msg("[gregCore][JS] " + msg); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void SafeWarn(string msg)
    {
        try { MelonLoader.MelonLogger.Warning("[gregCore][JS] " + msg); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }

    private static void SafeError(string msg)
    {
        try { MelonLoader.MelonLogger.Error("[gregCore][JS] " + msg); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
    }
}
