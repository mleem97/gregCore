using System;
using HarmonyLib;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Beobachtet den spiel-eigenen ModLoader (nur Prefix/Postfix, kein Verhaltenswechsel).
/// Meldet LoadAllMods/LoadModPack/LoadDll als greg.native.mod.* Events.
/// </summary>
public static class NativeModLoaderHooks
{
    public const string LoadAllModsStarted = "greg.native.mod.loadAllModsStarted";
    public const string LoadAllModsFinished = "greg.native.mod.loadAllModsFinished";
    public const string LoadModPackStarted = "greg.native.mod.loadModPackStarted";
    public const string LoadModPackFinished = "greg.native.mod.loadModPackFinished";
    public const string LoadDllStarted = "greg.native.mod.loadDllStarted";
    public const string LoadDllFinished = "greg.native.mod.loadDllFinished";

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
            _logger.Error("NativeModLoaderHooks Installation fehlgeschlagen", ex);
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
            _logger?.Error($"Event {hookName} fehlgeschlagen", ex);
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
