/// <file-summary>
/// Layer:       Bridge
/// Purpose:     Central orchestrator for the Lua modding environment.
/// Maintainer:   Initializes loader, scheduler, hot-reload and dev tools.
///               Connects C# hooks to the Lua VM.
/// </file-summary>

using System;
using System.Collections.Generic;
using System.IO;
using MoonSharp.Interpreter;
using MelonLoader;
using gregCore.API;
using gregCore.Infrastructure.Scripting.Lua;
using gregCore.Infrastructure.Scripting.Lua.Modules;
using gregCore.Infrastructure.Scripting.Lua.Dev;

namespace gregCore.Bridge.LuaFFI;

public sealed class LuaFFIBridge
{
    private static readonly List<LuaPlugin> _plugins = new();
    private static LuaHotReload? _hotReload;
    private static LuaHookBindingGenerator? _hookGenerator;
    private static LuaRepl? _repl;
    private static LuaProfiler? _profiler;
    private static LuaErrorOverlay? _errorOverlay;
    private static bool _initialized;

    public static void Initialize()
    {
        if (_initialized) return;

        MelonLogger.Msg("[LuaFFI] Initializing modernized Lua environment...");

        UserData.RegisterType<gregCore.UI.GregUIBuilder>();

        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string luaDir = Path.Combine(gameRoot, "UserData", "gregCore", "Mods", "Lua");
        string sharedDir = Path.Combine(luaDir, "@shared");
        string hooksFile = Path.Combine(gameRoot, "UserData", "gregCore", "game_hooks.json");

        if (!Directory.Exists(luaDir)) Directory.CreateDirectory(luaDir);
        if (!Directory.Exists(sharedDir)) Directory.CreateDirectory(sharedDir);

        // Infrastructure
        _profiler = new LuaProfiler(2.0f); // 2ms per frame budget
        _errorOverlay = new LuaErrorOverlay();
        _repl = new LuaRepl();
        _repl.Initialize();

        // Hook Generator
        _hookGenerator = new LuaHookBindingGenerator(API.GregAPI.EventBus!, hooksFile);
        _hookGenerator.LoadHooks();

        // Hot Reload
        _hotReload = new LuaHotReload(luaDir, OnPluginNeedsReload);
        _hotReload.Start();

        LoadPlugins(luaDir);
        _initialized = true;
    }

    private static void LoadPlugins(string luaDir)
    {
        var dirs = global::gregCore.Infrastructure.IO.GregDeactivatedGuard.ExcludeDeactivated(
            Directory.GetDirectories(luaDir));
        var files = global::gregCore.Infrastructure.IO.GregFileSystem.EnumerateFilesByExtension(luaDir, ".lua");
        foreach (string source in dirs.Concat(files))
        {
            try
            {
                TryLoadSource(source, luaDir);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[LuaFFI] Error loading mod {source}: {ex.Message}");
            }
        }
    }

    private static void TryLoadSource(string source, string luaDir)
    {
        try
        {
            if (!TryResolveSource(source, out string dir, out string mainFile, out bool isLegacyFile)) return;
            LoadSinglePlugin(source, dir, mainFile, luaDir, isLegacyFile);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Error loading mod {source}: {ex.Message}");
            try { _errorOverlay?.ReportError(Path.GetFileName(source), ex.Message); } catch { }
        }
    }

    private static bool TryResolveSource(string source, out string dir, out string mainFile, out bool isLegacyFile)
    {
        dir = "";
        mainFile = "";
        isLegacyFile = false;
        try
        {
            if (global::gregCore.Infrastructure.IO.GregDeactivatedGuard.IsDeactivatedPath(source)) return false;
            isLegacyFile = File.Exists(source);
            dir = isLegacyFile ? Path.GetDirectoryName(source)! : source;
            if (IsSkippedDir(dir, isLegacyFile)) return false;
            mainFile = isLegacyFile ? source : Path.Combine(dir, "main.lua");
            if (!isLegacyFile && !File.Exists(mainFile)) return false;
            return true;
        }
        catch { return false; }
    }

    private static bool IsSkippedDir(string dir, bool isLegacyFile)
    {
        try
        {
            if (isLegacyFile) return false;
            return Path.GetFileName(dir).StartsWith("@");
        }
        catch { return true; }
    }

    private static void LoadSinglePlugin(string source, string dir, string mainFile, string luaDir, bool isLegacyFile)
    {
        try
        {
            string manifestFile = Path.Combine(dir, "mod.json");
            var manifest = isLegacyFile
                ? new gregCore.Core.Models.ModManifest { Id = Path.GetFileNameWithoutExtension(source), Name = Path.GetFileNameWithoutExtension(source), Entrypoint = Path.GetFileName(source), Loader = "Lua" }
                : ReadManifest(manifestFile, Path.GetFileName(dir));
            string id = manifest.Id;
            mainFile = Path.Combine(dir, string.IsNullOrWhiteSpace(manifest.Entrypoint) ? "main.lua" : manifest.Entrypoint);
            var script = new Script(CoreModules.Preset_SoftSandbox);
            var gregTable = SetupScript(script, dir, luaDir, id);
            var scheduler = SetupScheduler(script, gregTable);
            RunMainFile(script, mainFile);
            var plugin = BuildPlugin(id, manifest, script, mainFile, scheduler);
            SafeCall(plugin, plugin.OnInit);
            _plugins.Add(plugin);
            RegisterInModRegistry(id, manifest);
            _hotReload?.RegisterPlugin(id, script, mainFile);
            MelonLogger.Msg($"[LuaFFI] Mod loaded: {id} ({_hookGenerator?.TotalHookCount} hooks available)");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Error loading mod {dir}: {ex.Message}");
            _errorOverlay?.ReportError(Path.GetFileName(dir), ex.Message);
        }
    }

    private static Table SetupScript(Script script, string dir, string luaDir, string id)
    {
        var loader = new LuaModuleLoader(script, dir, Path.Combine(luaDir, "@shared"));
        loader.Register();
        var gregTable = new Table(script);
        script.Globals["greg"] = gregTable;
        RegisterCoreModules(gregTable, script, id, dir);
        RegisterDomainModules(gregTable, script, id, dir);
        _hookGenerator?.RegisterInScript(script, gregTable, id);
        return gregTable;
    }

    private static void RegisterCoreModules(Table gregTable, Script script, string id, string dir)
    {
        try
        {
            GregEventLuaModule.Register(gregTable, script, API.GregAPI.EventBus!, id);
            GregIoLuaModule.Register(gregTable, script, id, dir);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Core modules failed for {id}: {ex.Message}");
        }
    }

    private static void RegisterDomainModules(Table gregTable, Script script, string id, string dir)
    {
        try
        {
            RegisterDomainBatchA(gregTable, script, id, dir);
            RegisterDomainBatchB(gregTable, script, id, dir);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Domain modules failed for {id}: {ex.Message}");
        }
    }

    private static void RegisterDomainBatchA(Table gregTable, Script script, string id, string dir)
    {
        try
        {
            LuaPlayerModule.Register(gregTable, script, id);
            LuaWorldModule.Register(gregTable, script, id);
            LuaRackModule.Register(gregTable, script, id);
            LuaServerModule.Register(gregTable, script, id);
            LuaSwitchModule.Register(gregTable, script, id);
            LuaPatchModule.Register(gregTable, script, id);
            LuaTechModule.Register(gregTable, script, id);
            LuaCableModule.Register(gregTable, script, id);
            LuaNetModule.Register(gregTable, script, id);
            LuaCustomerModule.Register(gregTable, script, id);
            LuaEconomyModule.Register(gregTable, script, id);
            LuaShopModule.Register(gregTable, script, id);
            LuaRequestsModule.Register(gregTable, script, id);
            LuaSubnetModule.Register(gregTable, script, id);
            LuaUiModule.Register(gregTable, script, id);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Domain batch A failed for {id}: {ex.Message}");
        }
    }

    private static void RegisterDomainBatchB(Table gregTable, Script script, string id, string dir)
    {
        try
        {
            LuaTabletModule.Register(gregTable, script, id);
            LuaComputerModule.Register(gregTable, script, id);
            LuaModsModule.Register(gregTable, script, id);
            LuaModSaveModule.Register(gregTable, script, id);
            LuaInternetModule.Register(gregTable, script, id);
            LuaSettingsModule.Register(gregTable, script, id);
            LuaObjectivesModule.Register(gregTable, script, id);
            LuaTooltipModule.Register(gregTable, script, id);
            LuaCoopModule.Register(gregTable, script, id);
            LuaMiscModule.Register(gregTable, script, id);
            LuaItemsModule.Register(gregTable, script, id, dir);
            LuaJsonModule.Register(gregTable, script, id);
            LuaConfigModule.Register(gregTable, script, id, dir);
            LuaSaveModule.Register(gregTable, script, id, dir);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Domain batch B failed for {id}: {ex.Message}");
        }
    }

    private static LuaCoroutineScheduler SetupScheduler(Script script, Table gregTable)
    {
        var scheduler = new LuaCoroutineScheduler(script);
        scheduler.Register(gregTable);
        return scheduler;
    }

    private static void RunMainFile(Script script, string mainFile)
    {
        if (!File.Exists(mainFile)) throw new FileNotFoundException($"Lua entrypoint not found: {mainFile}");
        script.DoFile(mainFile);
    }

    private static LuaPlugin BuildPlugin(string id, gregCore.Core.Models.ModManifest manifest, Script script, string mainFile, LuaCoroutineScheduler scheduler)
    {
        return new LuaPlugin
        {
            Id = id,
            Manifest = manifest,
            Script = script,
            MainFile = mainFile,
            Scheduler = scheduler,
            OnInit = script.Globals.Get("on_init").Type == DataType.Function ? script.Globals.Get("on_init").Function : null,
            OnUpdate = script.Globals.Get("on_update").Type == DataType.Function ? script.Globals.Get("on_update").Function : null,
            OnSceneLoaded = script.Globals.Get("on_scene_loaded").Type == DataType.Function ? script.Globals.Get("on_scene_loaded").Function : null,
            OnShutdown = script.Globals.Get("on_shutdown").Type == DataType.Function ? script.Globals.Get("on_shutdown").Function : null,
            OnReload = script.Globals.Get("on_reload").Type == DataType.Function ? script.Globals.Get("on_reload").Function : null
        };
    }

    private static void RegisterInModRegistry(string id, gregCore.Core.Models.ModManifest manifest)
    {
        try
        {
            gregCore.Core.Mods.GregModRegistry.Register(
                id,
                manifest.Name ?? id,
                manifest.Version ?? "0.0.0",
                new string[0]);
        }
        catch { /* ignored: registry best-effort */ }
    }

    public static void OnUpdate(float dt)
    {
        if (!_initialized) return;

        _repl?.Update();

        foreach (var plugin in _plugins)
        {
            using (_profiler?.BeginScope(plugin.Id))
            {
                try
                {
                    plugin.Scheduler.OnUpdate(dt);
                    if (plugin.OnUpdate != null)
                    {
                        plugin.OnUpdate.Call(dt);
                    }
                }
                catch (Exception ex)
                {
                    _errorOverlay?.ReportError(plugin.Id, ex.Message);
                }
            }
        }

        _profiler?.EndFrame();
    }

    public static void OnSceneLoaded(string name)
    {
        if (!_initialized) return;
        foreach (var plugin in _plugins)
        {
            try { plugin.OnSceneLoaded?.Call(name); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
    }

    public static void Shutdown()
    {
        if (!_initialized) return;
        foreach (var plugin in _plugins)
        {
            GregEventLuaModule.UnregisterAll(plugin.Id, API.GregAPI.EventBus!);
            LuaComputerModule.UnregisterAll(plugin.Id);
            try { plugin.OnShutdown?.Call(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        }
        _plugins.Clear();
        _hotReload?.Stop();
        _initialized = false;
    }

    private static void OnPluginNeedsReload(LuaPluginReloadInfo info)
    {
        MelonLogger.Msg($"[LuaFFI] Hot-reloading mod: {info.ModId}");

        // Find existing plugin
        var existing = _plugins.Find(p => p.Id == info.ModId);
        if (existing != null)
        {
            GregEventLuaModule.UnregisterAll(existing.Id, API.GregAPI.EventBus!);
            LuaComputerModule.UnregisterAll(existing.Id);
            try { existing.OnShutdown?.Call(); } catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
            _plugins.Remove(existing);
        }

        // Re-load using the provided new Script instance from LuaHotReload.
        try
        {
            LoadSpecificPlugin(info);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Hot-reload failed for {info.ModId}: {ex.Message}");
        }
    }

    private static void LoadSpecificPlugin(LuaPluginReloadInfo info)
    {
        // Use the NewScript provided by the hot-reload infrastructure and wire up
        // the same modules / scheduler / hooks as in initial LoadPlugins.
        var newScript = info.NewScript;
        string id = info.ModId;
        string mainFile = info.MainFilePath;

        try
        {
            ReloadSinglePlugin(newScript, id, mainFile);
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Error reloading mod {info.ModId}: {ex.Message}");
            _errorOverlay?.ReportError(info.ModId, ex.Message);
        }
    }

    private static void ReloadSinglePlugin(Script newScript, string id, string mainFile)
    {
        try
        {
            string modDir = Path.GetDirectoryName(mainFile)!;
            string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
            string luaDir = Path.Combine(gameRoot, "UserData", "gregCore", "Mods", "Lua");
            var gregTable = SetupReloadScript(newScript, modDir, luaDir, id);
            var scheduler = SetupScheduler(newScript, gregTable);
            newScript.DoFile(mainFile);
            var plugin = BuildPlugin(id, ReadManifest(Path.Combine(modDir, "mod.json"), id), newScript, mainFile, scheduler);
            SafeCall(plugin, plugin.OnInit);
            _plugins.Add(plugin);
            _hotReload?.RegisterPlugin(id, newScript, mainFile);
            MelonLogger.Msg($"[LuaFFI] Mod reloaded: {id} ({_hookGenerator?.TotalHookCount} hooks available)");
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Error reloading mod {id}: {ex.Message}");
            _errorOverlay?.ReportError(id, ex.Message);
        }
    }

    private static Table SetupReloadScript(Script newScript, string modDir, string luaDir, string id)
    {
        try
        {
            string sharedDir = Path.Combine(luaDir, "@shared");
            var loader = new LuaModuleLoader(newScript, modDir, sharedDir);
            loader.Register();
            var gregTable = new Table(newScript);
            newScript.Globals["greg"] = gregTable;
            RegisterCoreModules(gregTable, newScript, id, modDir);
            RegisterDomainModules(gregTable, newScript, id, modDir);
            _hookGenerator?.RegisterInScript(newScript, gregTable, id);
            return gregTable;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaFFI] Reload setup failed for {id}: {ex.Message}");
            var fallback = new Table(newScript);
            newScript.Globals["greg"] = fallback;
            return fallback;
        }
    }

    private static void SafeCall(LuaPlugin plugin, Closure? closure, params object[] args)
    {
        if (closure == null) return;
        try { closure.Call(args); }
        catch (Exception ex)
        {
            MelonLogger.Error($"[LuaMod:{plugin.Id}] Runtime error: {ex.Message}");
            _errorOverlay?.ReportError(plugin.Id, ex.Message);
        }
    }

    private static gregCore.Core.Models.ModManifest ReadManifest(string path, string fallbackId)
    {
        if (!File.Exists(path))
            return new gregCore.Core.Models.ModManifest { Id = fallbackId, Name = fallbackId, Entrypoint = "main.lua", Loader = "Lua" };
        var manifest = JsonSerializer.Deserialize<gregCore.Core.Models.ModManifest>(File.ReadAllText(path));
        if (manifest == null || string.IsNullOrWhiteSpace(manifest.Id))
            throw new InvalidDataException($"Lua manifest '{path}' has no id.");
        if (!string.IsNullOrWhiteSpace(manifest.Entrypoint) && !File.Exists(Path.Combine(Path.GetDirectoryName(path)!, manifest.Entrypoint)))
            throw new FileNotFoundException($"Lua entrypoint not found: {manifest.Entrypoint}");
        return manifest;
    }
}

public class LuaPlugin
{
    public string Id = "";
    public Script Script = null!;
    public string MainFile = "";
    public gregCore.Core.Models.ModManifest Manifest = new();
    public LuaCoroutineScheduler Scheduler = null!;
    public Closure? OnInit;
    public Closure? OnUpdate;
    public Closure? OnSceneLoaded;
    public Closure? OnShutdown;
    public Closure? OnReload;
}
