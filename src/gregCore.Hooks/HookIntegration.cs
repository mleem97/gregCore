using System;
using System.Reflection;
using HarmonyLib;
using UnityEngine;
using MelonLoader;
using greg.Sdk;

namespace gregCore.GameLayer.Hooks
{
    public static class HookIntegration
    {
        public static void Install(object mod, bool auto) { }
        public static void LogPatchError(string mod, Exception ex) => MelonLogger.Error($"[{mod}] Patch Error: {ex.Message}");
        public static void Emit(string id, object? data = null) { }

        public static void Apply(HarmonyLib.Harmony harmony)
        {
            try 
            {
                var playerType = SafeTypeByName("Player") ?? SafeTypeByName("Il2Cpp.Player");
                if (playerType != null)
                {
                    var m = SafeGetMethod(playerType, "UpdateCoin");
                    if (m != null) harmony.Patch(m, postfix: new HarmonyMethod(typeof(HookIntegration), nameof(Postfix_Generic)));
                }

                var saveManagerType = SafeTypeByName("SaveManager") ?? SafeTypeByName("Il2Cpp.SaveManager");
                if (saveManagerType != null)
                {
                    var m = SafeGetMethod(saveManagerType, "SaveGame");
                    if (m != null) harmony.Patch(m, postfix: new HarmonyMethod(typeof(HookIntegration), nameof(Postfix_Generic)));
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[gC-Hooks] Dynamic patch failed: {ex.Message}");
            }
        }

        private static MethodBase? SafeGetMethod(Type type, string methodName)
        {
            const System.Reflection.BindingFlags flags = System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy;
            return type.GetMethod(methodName, flags);
        }

        private static readonly System.Collections.Generic.Dictionary<string, Type?> _typeCache = new();

        private static Type? SafeTypeByName(string typeName)
        {
            if (_typeCache.TryGetValue(typeName, out var cached)) return cached;

            var t = Type.GetType(typeName);
            if (t != null) 
            {
                _typeCache[typeName] = t;
                return t;
            }

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(typeName, false, false);
                if (t != null) 
                {
                    _typeCache[typeName] = t;
                    return t;
                }
            }
            _typeCache[typeName] = null;
            return null;
        }

        public static void Postfix_Generic()
        {
            gregNativeEventHooks.OnCoinsChanged?.Invoke(null!);
            gregNativeEventHooks.GameLoaded?.Invoke(null!);
        }
    }
}
