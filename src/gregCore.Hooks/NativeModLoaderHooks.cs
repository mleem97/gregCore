using System;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Observes the game's own mod loader (prefix/postfix only, no behavior change).
/// Reports LoadAllMods/LoadModPack/LoadDll as greg.native.mod.* events.
/// </summary>
public static class NativeModLoaderHooks
{
    public static string LoadAllModsStarted { get; } = "greg.native.mod.loadAllModsStarted";
    public static string LoadAllModsFinished { get; } = "greg.native.mod.loadAllModsFinished";
    public static string LoadModPackStarted { get; } = "greg.native.mod.loadModPackStarted";
    public static string LoadModPackFinished { get; } = "greg.native.mod.loadModPackFinished";
    public static string LoadDllStarted { get; } = "greg.native.mod.loadDllStarted";
    public static string LoadDllFinished { get; } = "greg.native.mod.loadDllFinished";

    private static bool _installed;
    private static IGregLogger? _logger;
    private static GregEventBus? _eventBus;

    public static void Install(IGregLogger logger, GregEventBus eventBus, HarmonyLib.Harmony harmony)
    {
        if (_installed) return;
        _logger = logger.ForContext("NativeModLoaderHooks");
        _eventBus = eventBus;
        try
        {
            harmony.PatchAll(typeof(LoadAllModsObserver));
            harmony.PatchAll(typeof(LoadModPackObserver));
            harmony.PatchAll(typeof(LoadDllObserver));
            _installed = true;
            _logger.Info("NativeModLoaderHooks installiert (Observer only).");
        }
        catch (Exception ex)
        {
            _logger.Error("NativeModLoaderHooks Installation failed", ex);
        }
    }

    private static void Emit(string hookName)
    {
        try
        {
            _eventBus?.Publish(hookName, new EventPayload
            {
                HookName = hookName,
                OccurredAtUtc = DateTime.UtcNow,
                Data = new Dictionary<string, object>()
            });
        }
        catch (Exception ex)
        {
            _logger?.Error($"Event {hookName} failed", ex);
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ModLoader), nameof(global::Il2Cpp.ModLoader.LoadAllMods))]
    private static class LoadAllModsObserver
    {
        private static void Prefix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadAllModsStarted);
            }
            catch (Exception ex) { _logger?.Error("LoadAllMods Prefix failed", ex); }
        }

        private static void Postfix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadAllModsFinished);
            }
            catch (Exception ex) { _logger?.Error("LoadAllMods Postfix failed", ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ModLoader), nameof(global::Il2Cpp.ModLoader.LoadModPack))]
    private static class LoadModPackObserver
    {
        private static void Prefix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadModPackStarted);
            }
            catch (Exception ex) { _logger?.Error("LoadModPack Prefix failed", ex); }
        }

        private static void Postfix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadModPackFinished);
            }
            catch (Exception ex) { _logger?.Error("LoadModPack Postfix failed", ex); }
        }
    }

    [HarmonyPatch(typeof(global::Il2Cpp.ModLoader), nameof(global::Il2Cpp.ModLoader.LoadDll))]
    private static class LoadDllObserver
    {
        private static void Prefix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadDllStarted);
            }
            catch (Exception ex) { _logger?.Error("LoadDll Prefix failed", ex); }
        }

        private static void Postfix(global::Il2Cpp.ModLoader __instance)
        {
            try
            {
                if (__instance == null || __instance.Pointer == IntPtr.Zero) return;
                Emit(LoadDllFinished);
            }
            catch (Exception ex) { _logger?.Error("LoadDll Postfix failed", ex); }
        }
    }
}
