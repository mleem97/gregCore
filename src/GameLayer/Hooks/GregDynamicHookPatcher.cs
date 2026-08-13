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
        private readonly HookInstallReport _report = new();

        public int InstalledCount => _installedCount;
        public int FailedCount => _failedCount;
        public int TotalHooks { get; private set; }
        public HookInstallReport InstallReport => _report;

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
                // The release manifest is deliberately an object. Refuse the legacy
                // unbound inventory here: installing every discovered member is unsafe.
                if (!json.TrimStart().StartsWith("{", StringComparison.Ordinal))
                {
                    _report.ManifestVersion = "legacy-rejected";
                    _report.Skipped.Add(new HookInstallEntry { HookId = "legacy-inventory", Status = "skipped", ErrorClass = "ManifestNotBoundToBuild", TargetMember = hooksFilePath });
                    _logger.Warning("Rejected legacy unbound hook inventory; use framework/greg_hooks.json.");
                    return;
                }
                var manifest = JsonConvert.DeserializeObject<GregHooksManifest>(json);
                InstallFromManifest(manifest, Path.GetDirectoryName(hooksFilePath) ?? Directory.GetCurrentDirectory());
                return;

#pragma warning disable CS0162
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

        public void InstallFromManifest(GregHooksManifest? manifest, string manifestDirectory)
        {
            if (manifest == null) { _report.Skipped.Add(new HookInstallEntry { Status="skipped", ErrorClass="InvalidManifest" }); return; }
            _report.ManifestVersion = manifest.ManifestVersion > 0 ? manifest.ManifestVersion.ToString() : manifest.Version.ToString();
            TotalHooks = manifest.Hooks.Count;
            var gameRoot = Directory.GetParent(manifestDirectory)?.Parent?.FullName ?? manifestDirectory;
            var fingerprint = Core.Diagnostics.GameFingerprint.Capture(gameRoot);
            var fingerprintKnown = !string.IsNullOrWhiteSpace(manifest.AssemblyFingerprint) && manifest.AssemblyFingerprint != "UNKNOWN";
            var fingerprintMatches = fingerprintKnown && string.Equals(manifest.AssemblyFingerprint, fingerprint.CombinedSha256, StringComparison.OrdinalIgnoreCase);
            _report.FingerprintMatch = fingerprintMatches ? "match" : fingerprintKnown ? "mismatch" : "unknown";
            if (!fingerprintMatches)
            {
                _report.SafeMode = true;
                foreach (var hook in manifest.Hooks)
                    _report.Disabled.Add(Entry(hook, "disabled", "UnknownOrMismatchedBuild", null));
                _logger.Warning($"Hook manifest fingerprint {_report.FingerprintMatch}; risky hooks disabled.");
                return;
            }

            foreach (var hook in manifest.Hooks)
            {
                if (!string.Equals(hook.Status, "implemented", StringComparison.OrdinalIgnoreCase)) { _report.Skipped.Add(Entry(hook, "skipped", "NotImplemented", null)); continue; }
                try
                {
                    var method = ResolveManifestMethod(hook);
                    if (method == null) { _report.Failed.Add(Entry(hook, "failed", "TargetNotFound", null)); continue; }
                    lock (_globalMethodToHookNames) _globalMethodToHookNames[method] = new List<string> { hook.Name };
                    _harmony.Patch(method, postfix: new HarmonyMethod(typeof(GregDynamicHookPatcher), nameof(GenericPostfix)));
                    _installedCount++; _report.Installed.Add(Entry(hook, "installed", "", method));
                }
                catch (Exception ex) { _failedCount++; _report.Failed.Add(Entry(hook, "failed", ex.GetType().Name, null, ex)); _logger.Warning($"Hook {hook.Id} failed: {ex.Message}"); }
            }
        }

        private MethodBase? ResolveManifestMethod(GregHookDef hook)
        {
            var typeName = string.IsNullOrWhiteSpace(hook.Type) ? hook.PatchTarget : (string.IsNullOrWhiteSpace(hook.Namespace) ? hook.Type : hook.Namespace + "." + hook.Type);
            var type = SafeTypeByName(typeName) ?? SafeTypeByName(hook.Type);
            return type == null ? null : SafeGetMethod(type, string.IsNullOrWhiteSpace(hook.Member) ? hook.MethodName : hook.Member, null);
        }

        private static HookInstallEntry Entry(GregHookDef hook, string status, string error, MethodBase? method, Exception? ex = null) => new() {
            HookId=hook.Id, Status=status, ErrorClass=error, Exception=ex?.ToString() ?? "", TargetMember=method == null ? hook.Member : method.DeclaringType?.FullName + "." + method.Name
        };

        private static string GetHookName(GameHookJsonDef hook)
        {
            return $"greg.{hook.Group}.{hook.MethodName}";
        }

        private MethodBase? ResolveMethod(GameHookJsonDef hook)
        {
            var fullTypeName = $"{hook.Namespace}.{hook.ClassName}";
            var type = SafeTypeByName(fullTypeName);

            if (type == null)
            {
                // Try without namespace prefix for Il2Cpp types
                type = SafeTypeByName(hook.ClassName);
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

            return SafeGetMethod(type, hook.MethodName, paramTypes);
        }

        private static MethodBase? SafeGetMethod(Type type, string methodName, Type[]? paramTypes)
        {
            try
            {
                const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy;

                if (paramTypes == null || paramTypes.Length == 0)
                {
                    // GetMethod(name, flags) is safe and doesn't log warnings
                    return type.GetMethod(methodName, flags);
                }

                // GetMethod(name, flags, binder, types, modifiers) is also safe
                return type.GetMethod(methodName, flags, null, paramTypes, null);
            }
            catch
            {
                return null;
            }
        }

        private static readonly Dictionary<string, Type?> _typeCache = new();

        private static Type? SafeTypeByName(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) return null;

            if (_typeCache.TryGetValue(typeName, out var cached)) return cached;

            // Fast path: fully qualified or simple type in current/mscorlib
            var t = Type.GetType(typeName);
            if (t != null) 
            {
                _typeCache[typeName] = t;
                return t;
            }

            // Manual search to avoid Assembly.GetTypes() which throws TypeLoadException on dummy DLLs
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                t = asm.GetType(typeName, throwOnError: false, ignoreCase: false);
                if (t != null) 
                {
                    _typeCache[typeName] = t;
                    return t;
                }
            }

            _typeCache[typeName] = null;
            return null;
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

            // Try direct type resolution safely
            result = SafeTypeByName(typeName);
            if (result != null) return result;

            // Try Il2Cpp prefix for game types
            result = SafeTypeByName($"Il2Cpp.{typeName}");
            if (result != null) return result;

            // Try UnityEngine prefix for Unity types
            result = SafeTypeByName($"UnityEngine.{typeName}");
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

    public sealed class HookInstallReport
    {
        public string ManifestVersion { get; set; } = "UNKNOWN";
        public string FingerprintMatch { get; set; } = "unknown";
        public bool SafeMode { get; set; }
        public List<HookInstallEntry> Installed { get; } = new();
        public List<HookInstallEntry> Failed { get; } = new();
        public List<HookInstallEntry> Skipped { get; } = new();
        public List<HookInstallEntry> Disabled { get; } = new();
    }

    public sealed class HookInstallEntry
    {
        public string HookId { get; set; } = "";
        public string Status { get; set; } = "";
        public string ErrorClass { get; set; } = "";
        public string Exception { get; set; } = "";
        public string TargetMember { get; set; } = "";
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
