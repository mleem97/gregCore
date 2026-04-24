using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using MelonLoader;
using gregCore.API;
using gregCore.Core.Models;

namespace gregCore.Bridge.RustFFI;

public static class RustFFIBridge
{
    private static readonly List<IntPtr> _loadedLibraries = new();
    private static readonly List<RustPlugin> _plugins = new();
    private static GregCoreAPI _apiTable;
    private static IntPtr _apiTablePtr;

    // Delegates for the function table
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void LogDelegate(IntPtr msg);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate double GetDoubleDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void SetDoubleDelegate(double val);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate uint GetUintDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int DispatchDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate float GetFloatDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void SetFloatDelegate(float val);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate IntPtr GetStringDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void SetStringDelegate(IntPtr str);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate int GetIntDelegate();
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void SetIntDelegate(int val);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void GetPlayerPosDelegate(out float x, out float y, out float z, out float ry);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void EventActionDelegate(uint eventId, ulong data);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void SubscribeDelegate(uint eventId, IntPtr callbackPtr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void OnHookDelegate(IntPtr hookName, IntPtr callbackPtr);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void FireHookDelegate(IntPtr hookName, IntPtr jsonData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void HookActionDelegate(IntPtr hookName, IntPtr trigger, IntPtr jsonData);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate void ConfigSetBoolDelegate(IntPtr modId, IntPtr key, uint val);
    [UnmanagedFunctionPointer(CallingConvention.Cdecl)] delegate uint ConfigGetBoolDelegate(IntPtr modId, IntPtr key, uint def);

    // Keeping delegates alive
    private static readonly List<Delegate> _delegateCache = new();

    public static void Initialize()
    {
        GregAPI.LogInfo("RustFFIBridge initializing...");

        SetupApiTable();
        LoadPlugins();
    }

    private static void SetupApiTable()
    {
        _apiTable = new GregCoreAPI { api_version = 1 };

        // Logging
        _apiTable.log_info = AddDelegate<LogDelegate>(ptr => GregAPI.LogInfo(Marshal.PtrToStringAnsi(ptr) ?? ""));
        _apiTable.log_warning = AddDelegate<LogDelegate>(ptr => GregAPI.LogWarning(Marshal.PtrToStringAnsi(ptr) ?? ""));
        _apiTable.log_error = AddDelegate<LogDelegate>(ptr => GregAPI.LogError(Marshal.PtrToStringAnsi(ptr) ?? ""));

        // Economy
        _apiTable.get_player_money = AddDelegate<GetDoubleDelegate>(() => GregAPI.GetPlayerMoney());
        _apiTable.set_player_money = AddDelegate<SetDoubleDelegate>(val => GregAPI.SetPlayerMoney(val));
        _apiTable.get_player_xp = AddDelegate<GetDoubleDelegate>(() => GregAPI.GetPlayerXp());
        _apiTable.set_player_xp = AddDelegate<SetDoubleDelegate>(val => GregAPI.SetPlayerXp(val));
        _apiTable.get_player_reputation = AddDelegate<GetDoubleDelegate>(() => GregAPI.GetPlayerReputation());
        _apiTable.set_player_reputation = AddDelegate<SetDoubleDelegate>(val => GregAPI.SetPlayerReputation(val));

        // World
        _apiTable.get_server_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetServerCount());
        _apiTable.get_rack_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetRackCount());
        _apiTable.get_switch_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetSwitchCount());
        _apiTable.get_broken_server_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetBrokenServerCount());
        _apiTable.get_broken_switch_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetBrokenSwitchCount());

        // Technicians
        _apiTable.get_free_technician_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetFreeTechnicianCount());
        _apiTable.get_total_technician_count = AddDelegate<GetUintDelegate>(() => GregAPI.GetTotalTechnicianCount());
        _apiTable.dispatch_repair_server = AddDelegate<DispatchDelegate>(() => GregAPI.DispatchRepairServer());
        _apiTable.dispatch_repair_switch = AddDelegate<DispatchDelegate>(() => GregAPI.DispatchRepairSwitch());

        // Time
        _apiTable.get_time_of_day = AddDelegate<GetFloatDelegate>(() => GregAPI.GetTimeOfDay());
        _apiTable.get_day = AddDelegate<GetUintDelegate>(() => GregAPI.GetDay());
        _apiTable.get_seconds_in_full_day = AddDelegate<GetFloatDelegate>(() => GregAPI.GetSecondsInFullDay());
        _apiTable.set_seconds_in_full_day = AddDelegate<SetFloatDelegate>(val => GregAPI.SetSecondsInFullDay(val));

        // Game
        _apiTable.get_current_scene = AddDelegate<GetStringDelegate>(() => Marshal.StringToHGlobalAnsi(GregAPI.GetCurrentScene()));
        _apiTable.is_game_paused = AddDelegate<GetUintDelegate>(() => GregAPI.IsGamePaused() ? 1u : 0u);
        _apiTable.set_game_paused = AddDelegate<SetDoubleDelegate>(val => GregAPI.SetGamePaused(val > 0));
        _apiTable.get_time_scale = AddDelegate<GetFloatDelegate>(() => GregAPI.GetTimeScale());
        _apiTable.set_time_scale = AddDelegate<SetFloatDelegate>(val => GregAPI.SetTimeScale(val));
        _apiTable.trigger_save = AddDelegate<DispatchDelegate>(() => { GregAPI.TriggerSave(); return 0; });
        _apiTable.get_difficulty = AddDelegate<GetIntDelegate>(() => GregAPI.GetDifficulty());

        _apiTable.get_player_position = AddDelegate<GetPlayerPosDelegate>((out float x, out float y, out float z, out float ry) => {
            var pos = GregAPI.GetPlayerPosition();
            x = pos.x; y = pos.y; z = pos.z; ry = pos.y;
        });

        // UI
        _apiTable.show_notification = AddDelegate<LogDelegate>(ptr => GregAPI.ShowNotification(Marshal.PtrToStringAnsi(ptr) ?? ""));

        // Events
        _apiTable.subscribe_event = AddDelegate<SubscribeDelegate>((eventId, cbPtr) => {
            var callback = Marshal.GetDelegateForFunctionPointer<EventActionDelegate>(cbPtr);
            GregAPI.Subscribe(((GregEventId)eventId).ToString(), data => callback(eventId, (ulong)data));
        });
        _apiTable.fire_event = AddDelegate<EventActionDelegate>((id, data) => GregAPI.FireEvent(((GregEventId)id).ToString(), data));

        // Hook API (New)
        _apiTable.on_hook = AddDelegate<OnHookDelegate>((hookPtr, cbPtr) => {
            string hookName = Marshal.PtrToStringAnsi(hookPtr) ?? "";
            var callback = Marshal.GetDelegateForFunctionPointer<HookActionDelegate>(cbPtr);
            GregAPI.Hooks.On(hookName, payloadObj => {
                var payload = (gregCore.Sdk.Models.GregPayload)payloadObj;
                string json = Newtonsoft.Json.JsonConvert.SerializeObject(payload.Data);
                IntPtr hPtr = Marshal.StringToHGlobalAnsi(payload.HookName);
                IntPtr tPtr = Marshal.StringToHGlobalAnsi(payload.Trigger);
                IntPtr jPtr = Marshal.StringToHGlobalAnsi(json);
                callback(hPtr, tPtr, jPtr);
                Marshal.FreeHGlobal(hPtr);
                Marshal.FreeHGlobal(tPtr);
                Marshal.FreeHGlobal(jPtr);
            });
        });
        _apiTable.fire_hook = AddDelegate<FireHookDelegate>((hookPtr, jsonPtr) => {
            string hookName = Marshal.PtrToStringAnsi(hookPtr) ?? "";
            string json = Marshal.PtrToStringAnsi(jsonPtr) ?? "{}";
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? new();
            var payload = new gregCore.Sdk.Models.GregPayload(hookName, "RustMod") { Data = data };
            GregAPI.Hooks.Fire(hookName, payload);
        });

        // Config
        _apiTable.config_set_bool = AddDelegate<ConfigSetBoolDelegate>((modId, key, val) =>
            GregAPI.ConfigSetBool(Marshal.PtrToStringAnsi(modId) ?? "unknown", Marshal.PtrToStringAnsi(key) ?? "unknown", val > 0));
        _apiTable.config_get_bool = AddDelegate<ConfigGetBoolDelegate>((modId, key, def) =>
            GregAPI.ConfigGetBool(Marshal.PtrToStringAnsi(modId) ?? "unknown", Marshal.PtrToStringAnsi(key) ?? "unknown", def > 0) ? 1u : 0u);

        // Alloc and store pointer
        _apiTablePtr = Marshal.AllocHGlobal(Marshal.SizeOf<GregCoreAPI>());
        Marshal.StructureToPtr(_apiTable, _apiTablePtr, false);
    }

    private static IntPtr AddDelegate<T>(T del) where T : Delegate
    {
        _delegateCache.Add(del);
        return Marshal.GetFunctionPointerForDelegate(del);
    }

    private static void LoadPlugins()
    {
        string gameRoot = global::MelonLoader.Utils.MelonEnvironment.GameRootDirectory;
        string rustDir = Path.Combine(gameRoot, "Plugins", "Rust");
        if (!Directory.Exists(rustDir)) Directory.CreateDirectory(rustDir);

        foreach (string file in Directory.GetFiles(rustDir, "*.dll"))
        {
            try
            {
                IntPtr lib = System.Runtime.InteropServices.NativeLibrary.Load(file);
                if (lib == IntPtr.Zero) continue;

                _loadedLibraries.Add(lib);

                IntPtr infoFunc = System.Runtime.InteropServices.NativeLibrary.GetExport(lib, "greg_mod_info");
                IntPtr initFunc = System.Runtime.InteropServices.NativeLibrary.GetExport(lib, "greg_mod_init");

                if (infoFunc == IntPtr.Zero || initFunc == IntPtr.Zero)
                {
                    GregAPI.LogWarning($"Plugin {Path.GetFileName(file)} does not export core functions.");
                    continue;
                }

                var getInfo = Marshal.GetDelegateForFunctionPointer<Func<GregModInfo>>(infoFunc);
                var info = getInfo();

                var init = Marshal.GetDelegateForFunctionPointer<Func<IntPtr, bool>>(initFunc);
                if (init(_apiTablePtr))
                {
                    var plugin = new RustPlugin
                    {
                        Id = Marshal.PtrToStringAnsi(info.id) ?? "Unknown",
                        Handle = lib,
                        Update = GetOptionalExport<Action<float>>(lib, "greg_mod_update"),
                        OnEvent = GetOptionalExport<Action<uint, ulong>>(lib, "greg_mod_event"),
                        OnSceneLoaded = GetOptionalExport<Action<IntPtr>>(lib, "greg_mod_scene_loaded"),
                        Shutdown = GetOptionalExport<Action>(lib, "greg_mod_shutdown")
                    };
                    _plugins.Add(plugin);
                    GregAPI.LogInfo($"Rust Plugin loaded: {plugin.Id}");
                }
            }
            catch (Exception ex)
            {
                GregAPI.LogError($"Error loading {Path.GetFileName(file)}: {ex.Message}");
            }
        }
    }

    private static T? GetOptionalExport<T>(IntPtr lib, string name) where T : Delegate
    {
        if (System.Runtime.InteropServices.NativeLibrary.TryGetExport(lib, name, out IntPtr ptr))
            return Marshal.GetDelegateForFunctionPointer<T>(ptr);
        return null;
    }

    public static void OnUpdate(float dt)
    {
        foreach (var p in _plugins) p.Update?.Invoke(dt);
    }

    public static void OnSceneLoaded(string name)
    {
        IntPtr namePtr = Marshal.StringToHGlobalAnsi(name);
        foreach (var p in _plugins) p.OnSceneLoaded?.Invoke(namePtr);
        Marshal.FreeHGlobal(namePtr);
    }

    public static void Shutdown()
    {
        foreach (var p in _plugins) p.Shutdown?.Invoke();
        foreach (var lib in _loadedLibraries) System.Runtime.InteropServices.NativeLibrary.Free(lib);
        if (_apiTablePtr != IntPtr.Zero) Marshal.FreeHGlobal(_apiTablePtr);
    }

    [StructLayout(LayoutKind.Sequential)]
    struct GregModInfo
    {
        public IntPtr id;
        public IntPtr name;
        public IntPtr version;
        public IntPtr author;
        public IntPtr description;
        public uint api_version;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct GregCoreAPI
    {
        public uint api_version;
        public IntPtr log_info, log_warning, log_error;
        public IntPtr get_player_money, set_player_money, get_player_xp, set_player_xp, get_player_reputation, set_player_reputation;
        public IntPtr get_server_count, get_rack_count, get_switch_count, get_broken_server_count, get_broken_switch_count;
        public IntPtr get_free_technician_count, get_total_technician_count, dispatch_repair_server, dispatch_repair_switch;
        public IntPtr get_time_of_day, get_day, get_seconds_in_full_day, set_seconds_in_full_day;
        public IntPtr get_current_scene, is_game_paused, set_game_paused, get_time_scale, set_time_scale, trigger_save, get_difficulty;
        public IntPtr get_player_position, show_notification;
        public IntPtr subscribe_event, unsubscribe_event, fire_event;
        public IntPtr on_hook, fire_hook;
        public IntPtr config_set_bool, config_get_bool, config_set_int, config_get_int, config_set_float, config_get_float, config_set_string, config_get_string;
    }

    class RustPlugin
    {
        public string Id = "";
        public IntPtr Handle;
        public Action<float>? Update;
        public Action<uint, ulong>? OnEvent;
        public Action<IntPtr>? OnSceneLoaded;
        public Action? Shutdown;
    }
}
