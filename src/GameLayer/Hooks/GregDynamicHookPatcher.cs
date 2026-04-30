using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using MelonLoader;
using Newtonsoft.Json;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Hooks
{
    /// <summary>
    /// Dynamically applies Harmony patches for all hooks defined in game_hooks.json.
    /// Uses a generic postfix to dispatch events to GregEventBus.
    /// </summary>
    public sealed class GregDynamicHookPatcher
    {
        private readonly HarmonyLib.Harmony _harmony;
        private readonly GregEventBus _eventBus;
        private readonly IGregLogger _logger;
        private int _installedCount;
        private int _failedCount;

        public int InstalledCount => _installedCount;
        public int FailedCount => _failedCount;
        public int TotalHooks { get; private set; }

        public GregDynamicHookPatcher(HarmonyLib.Harmony harmony, GregEventBus eventBus, IGregLogger logger)
        {
            _harmony = harmony;
            _eventBus = eventBus;
            _logger = logger.ForContext("DynamicHookPatcher");
        }

        /// <summary>
        /// Loads game_hooks.json and applies patches for all resolvable methods.
        /// </summary>
        public void InstallFromFile(string hooksFilePath)
        {
            if (!File.Exists(hooksFilePath))
            {
                _logger.Warning($"Hooks file not found: {hooksFilePath}");
                return;
            }

            try
            {
                var json = File.ReadAllText(hooksFilePath);
                var hooks = JsonConvert.DeserializeObject<List<GameHookJsonDef>>(json);

                if (hooks == null || hooks.Count == 0)
                {
                    _logger.Warning("No hooks found in hooks file.");
                    return;
                }

                TotalHooks = hooks.Count;
                _logger.Info($"[DynamicPatcher] Loaded {hooks.Count} hook definitions from manifest.");

                // Group by unique method to avoid duplicate patches
                var methodGroups = new Dictionary<MethodBase, List<GameHookJsonDef>>();

                foreach (var hook in hooks)
                {
                    try
                    {
                        var method = ResolveMethod(hook);
                        if (method == null)
                        {
                            _failedCount++;
                            continue;
                        }

                        if (!methodGroups.TryGetValue(method, out var list))
                        {
                            list = new List<GameHookJsonDef>();
                            methodGroups[method] = list;
                        }
                        list.Add(hook);
                    }
                    catch (Exception ex)
                    {
                        _logger.Debug($"Failed to resolve hook {hook.ClassName}.{hook.MethodName}: {ex.Message}");
                        _failedCount++;
                    }
                }

                // Apply patches
                foreach (var kvp in methodGroups)
                {
                    try
                    {
                        var method = kvp.Key;
                        var hookNames = kvp.Value.Select(h => GetHookName(h)).ToList();

                        lock (_globalMethodToHookNames)
                        {
                            _globalMethodToHookNames[method] = hookNames;
                        }

                        _harmony.Patch(method, postfix: new HarmonyMethod(typeof(GregDynamicHookPatcher), nameof(GenericPostfix)));
                        _installedCount++;
                    }
                    catch (Exception ex)
                    {
                        _logger.Warning($"Failed to patch {kvp.Key.DeclaringType?.Name}.{kvp.Key.Name}: {ex.Message}");
                        _failedCount++;
                    }
                }

                _logger.Info($"[DynamicPatcher] Installed {_installedCount} patches, failed {_failedCount} hooks.");
            }
            catch (Exception ex)
            {
                _logger.Error("Failed to install dynamic hooks", ex);
            }
        }

        private static string GetHookName(GameHookJsonDef hook)
        {
            return $"greg.{hook.Group}.{hook.MethodName}";
        }

        private MethodBase? ResolveMethod(GameHookJsonDef hook)
        {
            var fullTypeName = $"{hook.Namespace}.{hook.ClassName}";
            var type = AccessTools.TypeByName(fullTypeName);

            if (type == null)
            {
                // Try without namespace prefix for Il2Cpp types
                type = AccessTools.TypeByName(hook.ClassName);
            }

            if (type == null) return null;

            Type[]? paramTypes = null;
            if (hook.Parameters != null && hook.Parameters.Count > 0)
            {
                paramTypes = hook.Parameters
                    .Select(p => ParseParameterType(p.Type))
                    .Where(t => t != null)
                    .ToArray()!;
            }

            return AccessTools.Method(type, hook.MethodName, paramTypes);
        }

        private static Type? ParseParameterType(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            // Primitive type mapping
            var result = typeName switch
            {
                "Void" => typeof(void),
                "Boolean" or "Bool" => typeof(bool),
                "Int32" or "Int" => typeof(int),
                "Int64" or "Long" => typeof(long),
                "UInt32" or "UInt" => typeof(uint),
                "UInt64" or "ULong" => typeof(ulong),
                "Single" or "Float" => typeof(float),
                "Double" => typeof(double),
                "String" => typeof(string),
                "Object" => typeof(object),
                _ => null
            };

            if (result != null) return result;

            // Try direct type resolution via AccessTools
            result = AccessTools.TypeByName(typeName);
            if (result != null) return result;

            // Try Il2Cpp prefix for game types
            result = AccessTools.TypeByName($"Il2Cpp.{typeName}");
            if (result != null) return result;

            // Try UnityEngine prefix for Unity types
            result = AccessTools.TypeByName($"UnityEngine.{typeName}");
            if (result != null) return result;

            return null;
        }

        // ─── Static state for Harmony Postfix ────────────────────────────

        private static readonly Dictionary<MethodBase, List<string>> _globalMethodToHookNames = new();
        private static GregEventBus? _globalEventBus;
        private static IGregLogger? _globalLogger;

        public static void SetGlobalBus(GregEventBus bus) => _globalEventBus = bus;
        public static void SetGlobalLogger(IGregLogger logger) => _globalLogger = logger;

        // ─── Harmony Postfix ─────────────────────────────────────────────

        public static void GenericPostfix(MethodBase __originalMethod, object[] __args)
        {
            if (_globalEventBus == null) return;

            List<string>? hookNames;
            lock (_globalMethodToHookNames)
            {
                if (!_globalMethodToHookNames.TryGetValue(__originalMethod, out hookNames)) return;
            }

            var payloadData = new Dictionary<string, object>
            {
                { "method", __originalMethod.Name },
                { "type", __originalMethod.DeclaringType?.Name ?? "Unknown" }
            };

            if (__args != null && __args.Length > 0)
            {
                var parameters = __originalMethod.GetParameters();
                for (int i = 0; i < Math.Min(__args.Length, parameters.Length); i++)
                {
                    try
                    {
                        payloadData[$"arg_{parameters[i].Name}"] = __args[i] ?? "null";
                    }
                    catch
                    {
                        payloadData[$"arg_{i}"] = "<unavailable>";
                    }
                }
            }

            var payload = new EventPayload
            {
                HookName = "",
                OccurredAtUtc = DateTime.UtcNow,
                Data = payloadData,
                IsCancelable = false,
                IsCancelled = false
            };

            foreach (var hookName in hookNames)
            {
                try
                {
                    _globalEventBus.Publish(hookName, payload with { HookName = hookName });
                }
                catch (Exception ex)
                {
                    _globalLogger?.Error($"Hook dispatch failed for {hookName}", ex);
                }
            }
        }
    }

    // ─── JSON Models ───────────────────────────────────────────────────

    public class GameHookJsonDef
    {
        public string Group { get; set; } = "";
        public string Namespace { get; set; } = "";
        public string ClassName { get; set; } = "";
        public string MethodName { get; set; } = "";
        public string ReturnType { get; set; } = "";
        public bool IsVoid { get; set; }
        public List<GameHookParameterDef> Parameters { get; set; } = new();
    }

    public class GameHookParameterDef
    {
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
    }
}
