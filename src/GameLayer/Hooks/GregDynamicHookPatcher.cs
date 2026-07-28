using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using gregCore.Core.Abstractions;
using gregCore.Core.Events;
using gregCore.Core.Models;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Applies profile-aware Harmony patches from the v2 hook manifest.
/// Legacy array manifests remain supported as a migration path, but all
/// parameters must resolve and all overloads must be unambiguous.
/// </summary>
public sealed class GregDynamicHookPatcher
{
    private readonly HarmonyLib.Harmony _harmony;
    private readonly GregEventBus _eventBus;
    private readonly IGregLogger _logger;
    private readonly Dictionary<string, HookDefinitionV2> _definitions = new(StringComparer.Ordinal);
    private readonly HashSet<string> _installedHookIds = new(StringComparer.Ordinal);
    private int _installedCount;
    private int _failedCount;

    public int InstalledCount => _installedCount;
    public int FailedCount => _failedCount;
    public int TotalHooks { get; private set; }
    public string? ManifestProfileId { get; private set; }

    public GregDynamicHookPatcher(HarmonyLib.Harmony harmony, GregEventBus eventBus, IGregLogger logger)
    {
        _harmony = harmony ?? throw new ArgumentNullException(nameof(harmony));
        _eventBus = eventBus ?? throw new ArgumentNullException(nameof(eventBus));
        _logger = (logger ?? throw new ArgumentNullException(nameof(logger))).ForContext("DynamicHookPatcher");
    }

    public void InstallFromFile(
        string hooksFilePath,
        string? activeProfileId = null,
        IReadOnlyCollection<string>? enabledGroups = null)
    {
        if (!File.Exists(hooksFilePath))
        {
            _logger.Warning($"Hooks file not found: {hooksFilePath}");
            return;
        }

        try
        {
            HookManifestV2 manifest = LoadManifest(hooksFilePath);
            if (!string.IsNullOrWhiteSpace(manifest.ProfileId))
                ManifestProfileId ??= manifest.ProfileId;
            TotalHooks += manifest.Hooks.Count;

            bool profileMismatch =
                !string.IsNullOrWhiteSpace(activeProfileId) &&
                !string.IsNullOrWhiteSpace(manifest.ProfileId) &&
                !manifest.ProfileId.Equals(activeProfileId, StringComparison.OrdinalIgnoreCase);

            if (profileMismatch)
            {
                _logger.Warning(
                    $"Hook manifest profile '{manifest.ProfileId}' does not match active profile '{activeProfileId}'. Required hooks from this manifest are disabled.");
            }

            foreach (HookDefinitionV2 incomingDefinition in manifest.Hooks)
            {
                if (string.IsNullOrWhiteSpace(incomingDefinition.Id))
                {
                    _failedCount++;
                    continue;
                }

                // The first definition wins. This preserves v2 definitions when
                // the legacy fallback manifest is loaded afterwards.
                if (!_definitions.TryGetValue(incomingDefinition.Id, out HookDefinitionV2? definition))
                {
                    definition = incomingDefinition;
                    _definitions.Add(definition.Id, definition);
                }

                if (profileMismatch && definition.Required)
                    continue;

                if (enabledGroups != null && enabledGroups.Count > 0 &&
                    !enabledGroups.Contains(definition.Group, StringComparer.OrdinalIgnoreCase))
                    continue;

                // High-frequency hooks are activated only when a consumer exists.
                if (definition.HighFrequency && !_eventBus.HasSubscribers(definition.Id))
                    continue;

                InstallDefinition(definition, activeProfileId);
            }

            _logger.Info(
                $"[DynamicPatcher] Manifest v{manifest.SchemaVersion}, profile={manifest.ProfileId ?? "legacy"}, " +
                $"installed={_installedCount}, failed={_failedCount}, declared={TotalHooks}.");
        }
        catch (Exception ex)
        {
            _logger.Error("Failed to install dynamic hooks", ex);
        }
    }

    /// <summary>
    /// Activates a deferred/high-frequency hook after a consumer subscribes.
    /// </summary>
    public bool InstallById(string hookId, string? activeProfileId = null)
    {
        if (_installedHookIds.Contains(hookId)) return true;
        if (!_definitions.TryGetValue(hookId, out HookDefinitionV2? definition)) return false;
        return InstallDefinition(definition, activeProfileId);
    }

    private bool InstallDefinition(HookDefinitionV2 definition, string? activeProfileId)
    {
        if (_installedHookIds.Contains(definition.Id)) return true;

        MethodBase? resolved = null;
        string? failure = null;

        foreach (HookCandidateV2 candidate in definition.Candidates)
        {
            if (!CandidateSupportsProfile(candidate, activeProfileId))
                continue;

            MethodResolution resolution = ResolveCandidate(candidate);
            if (resolution.Method != null)
            {
                resolved = resolution.Method;
                break;
            }

            failure = resolution.Reason;
        }

        if (resolved == null)
        {
            _failedCount++;
            string severity = definition.Required ? "required" : "optional";
            _logger.Warning($"Unable to resolve {severity} hook {definition.Id}: {failure ?? "no matching candidate"}");
            return false;
        }

        string patchKind = NormalizePatchKind(definition.PatchKind);
        var binding = new HookRuntimeBinding(
            definition.Id,
            definition.CaptureArguments,
            definition.HighFrequency);
        bool bindingAdded = false;
        bool patchReserved = false;

        try
        {
            lock (RuntimeSync)
            {
                if (!RuntimeBindings.TryGetValue(resolved, out List<HookRuntimeBinding>? bindings))
                {
                    bindings = new List<HookRuntimeBinding>();
                    RuntimeBindings[resolved] = bindings;
                    ParameterCache[resolved] = resolved.GetParameters();
                }

                if (!bindings.Any(existing => existing.HookId.Equals(definition.Id, StringComparison.Ordinal)))
                {
                    bindings.Add(binding);
                    bindingAdded = true;
                }

                if (!RuntimePatchKinds.TryGetValue(resolved, out HashSet<string>? patchKinds))
                {
                    patchKinds = new HashSet<string>(StringComparer.Ordinal);
                    RuntimePatchKinds[resolved] = patchKinds;
                }

                // Several stable greg hook IDs may map to one game method. Harmony
                // is installed only once per method and patch kind; dispatch fans
                // out to all runtime bindings.
                patchReserved = patchKinds.Add(patchKind);
            }

            if (patchReserved)
            {
                var patchMethod = new HarmonyMethod(
                    typeof(GregDynamicHookPatcher),
                    patchKind == "prefix" ? nameof(GenericPrefix) : nameof(GenericPostfix));

                if (patchKind == "prefix")
                    _harmony.Patch(resolved, prefix: patchMethod);
                else
                    _harmony.Patch(resolved, postfix: patchMethod);
            }

            _installedHookIds.Add(definition.Id);
            _installedCount++;
            return true;
        }
        catch (Exception ex)
        {
            lock (RuntimeSync)
            {
                if (bindingAdded && RuntimeBindings.TryGetValue(resolved, out List<HookRuntimeBinding>? bindings))
                {
                    bindings.RemoveAll(existing => existing.HookId.Equals(definition.Id, StringComparison.Ordinal));
                    if (bindings.Count == 0)
                    {
                        RuntimeBindings.Remove(resolved);
                        ParameterCache.Remove(resolved);
                    }
                }

                if (patchReserved && RuntimePatchKinds.TryGetValue(resolved, out HashSet<string>? patchKinds))
                {
                    patchKinds.Remove(patchKind);
                    if (patchKinds.Count == 0)
                        RuntimePatchKinds.Remove(resolved);
                }
            }

            _failedCount++;
            _logger.Warning($"Failed to patch {FormatMethod(resolved)} for {definition.Id}: {ex.Message}");
            return false;
        }
    }

    private HookManifestV2 LoadManifest(string hooksFilePath)
    {
        string json = File.ReadAllText(hooksFilePath);
        JToken root = JToken.Parse(json);

        if (root.Type == JTokenType.Array)
        {
            List<GameHookJsonDef> legacy = root.ToObject<List<GameHookJsonDef>>() ?? new();
            return ConvertLegacy(legacy);
        }

        HookManifestV2 manifest = root.ToObject<HookManifestV2>()
            ?? throw new InvalidDataException("Hook manifest is empty.");

        if (manifest.SchemaVersion != 2)
            throw new InvalidDataException($"Unsupported hook manifest schema {manifest.SchemaVersion}.");

        foreach (HookDefinitionV2 hook in manifest.Hooks)
        {
            if (hook.Candidates.Count == 0)
                throw new InvalidDataException($"Hook {hook.Id} has no candidates.");
        }

        return manifest;
    }

    private static HookManifestV2 ConvertLegacy(List<GameHookJsonDef> hooks)
    {
        return new HookManifestV2
        {
            SchemaVersion = 1,
            ProfileId = null,
            Hooks = hooks.Select(hook => new HookDefinitionV2
            {
                Id = $"greg.{hook.Group}.{hook.MethodName}",
                Group = hook.Group,
                Required = false,
                HighFrequency = IsHighFrequencyMethod(hook.MethodName),
                CaptureArguments = true,
                PatchKind = "postfix",
                Candidates = new List<HookCandidateV2>
                {
                    new()
                    {
                        Assembly = "Assembly-CSharp",
                        Type = string.IsNullOrWhiteSpace(hook.Namespace)
                            ? hook.ClassName
                            : $"{hook.Namespace}.{hook.ClassName}",
                        Method = hook.MethodName,
                        GenericArity = 0,
                        Static = null,
                        ReturnType = NormalizeLegacyTypeName(hook.ReturnType),
                        ParameterTypes = hook.Parameters.Select(parameter =>
                            NormalizeLegacyTypeName(parameter.Type)).ToList()
                    }
                }
            }).ToList()
        };
    }

    private static bool CandidateSupportsProfile(HookCandidateV2 candidate, string? activeProfileId)
    {
        if (candidate.Profiles.Count == 0 || string.IsNullOrWhiteSpace(activeProfileId))
            return true;

        return candidate.Profiles.Contains(activeProfileId, StringComparer.OrdinalIgnoreCase);
    }

    private static MethodResolution ResolveCandidate(HookCandidateV2 candidate)
    {
        Type? type = ResolveType(candidate.Type, candidate.Assembly);
        if (type == null)
            return MethodResolution.Fail($"type not found: {candidate.Type} in {candidate.Assembly}");

        var parameterTypes = new Type[candidate.ParameterTypes.Count];
        for (int index = 0; index < candidate.ParameterTypes.Count; index++)
        {
            Type? parameterType = ResolveSignatureType(candidate.ParameterTypes[index]);
            if (parameterType == null)
            {
                return MethodResolution.Fail(
                    $"parameter {index} type not found: {candidate.ParameterTypes[index]}");
            }
            parameterTypes[index] = parameterType;
        }

        const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                   BindingFlags.Instance | BindingFlags.Static |
                                   BindingFlags.FlattenHierarchy;

        MethodInfo[] namedMethods;
        try
        {
            namedMethods = type.GetMethods(flags)
                .Where(method => method.Name.Equals(candidate.Method, StringComparison.Ordinal))
                .ToArray();
        }
        catch (Exception ex)
        {
            return MethodResolution.Fail($"unable to enumerate methods: {ex.Message}");
        }

        MethodBase[] matching = namedMethods
            .Where(method => !candidate.Static.HasValue || method.IsStatic == candidate.Static.Value)
            .Where(method => GetGenericArity(method) == candidate.GenericArity)
            .Where(method => ParametersMatch(method.GetParameters(), parameterTypes))
            .Where(method => ReturnTypeMatches(method.ReturnType, candidate.ReturnType))
            .Cast<MethodBase>()
            .ToArray();

        return matching.Length switch
        {
            1 => MethodResolution.Success(matching[0]),
            0 => MethodResolution.Fail($"signature not found: {FormatCandidate(candidate)}"),
            _ => MethodResolution.Fail($"ambiguous signature ({matching.Length} matches): {FormatCandidate(candidate)}")
        };
    }

    private static int GetGenericArity(MethodInfo method) =>
        method.IsGenericMethodDefinition || method.IsGenericMethod
            ? method.GetGenericArguments().Length
            : 0;

    private static bool ParametersMatch(ParameterInfo[] actual, Type[] expected)
    {
        if (actual.Length != expected.Length) return false;
        for (int index = 0; index < actual.Length; index++)
        {
            if (actual[index].ParameterType != expected[index])
                return false;
        }
        return true;
    }

    private static bool ReturnTypeMatches(Type actual, string expectedName)
    {
        if (string.IsNullOrWhiteSpace(expectedName)) return true;
        Type? expected = ResolveSignatureType(expectedName);
        return expected != null && actual == expected;
    }

    private static Type? ResolveType(string typeName, string? assemblyName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return null;
        string cacheKey = $"{assemblyName}|{typeName}";

        lock (TypeCache)
        {
            if (TypeCache.TryGetValue(cacheKey, out Type? cached))
                return cached;
        }

        Type? resolved = null;
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (!string.IsNullOrWhiteSpace(assemblyName) &&
                !assembly.GetName().Name!.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
                continue;

            resolved = assembly.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (resolved != null) break;
        }

        resolved ??= Type.GetType(typeName, throwOnError: false, ignoreCase: false);

        // Legacy convenience fallbacks. V2 manifests should use fully qualified names.
        resolved ??= FindTypeInLoadedAssemblies($"Il2Cpp.{typeName}");
        resolved ??= FindTypeInLoadedAssemblies($"UnityEngine.{typeName}");
        resolved ??= FindTypeInLoadedAssemblies(typeName);

        lock (TypeCache)
            TypeCache[cacheKey] = resolved;
        return resolved;
    }

    private static Type? FindTypeInLoadedAssemblies(string typeName)
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type? type = assembly.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (type != null) return type;
        }
        return null;
    }

    private static Type? ResolveSignatureType(string typeName)
    {
        if (string.IsNullOrWhiteSpace(typeName)) return null;
        string text = typeName.Trim();

        bool byRef = text.StartsWith("ref ", StringComparison.Ordinal) ||
                     text.StartsWith("out ", StringComparison.Ordinal) ||
                     text.StartsWith("in ", StringComparison.Ordinal) ||
                     text.EndsWith("&", StringComparison.Ordinal);
        if (byRef)
        {
            text = text.TrimEnd('&').Trim();
            foreach (string prefix in new[] { "ref ", "out ", "in " })
            {
                if (text.StartsWith(prefix, StringComparison.Ordinal))
                    text = text[prefix.Length..].Trim();
            }
        }

        bool pointer = text.EndsWith("*", StringComparison.Ordinal);
        if (pointer) text = text[..^1].Trim();

        int arrayDepth = 0;
        while (text.EndsWith("[]", StringComparison.Ordinal))
        {
            arrayDepth++;
            text = text[..^2].Trim();
        }

        Type? resolved = ResolveSimpleOrGenericType(text);
        if (resolved == null) return null;

        for (int index = 0; index < arrayDepth; index++)
            resolved = resolved.MakeArrayType();
        if (pointer) resolved = resolved.MakePointerType();
        if (byRef) resolved = resolved.MakeByRefType();
        return resolved;
    }

    private static Type? ResolveSimpleOrGenericType(string typeName)
    {
        if (PrimitiveTypes.TryGetValue(typeName, out Type? primitive))
            return primitive;

        int open = typeName.IndexOf('<');
        if (open > 0 && typeName.EndsWith(">", StringComparison.Ordinal))
        {
            string genericName = typeName[..open].Trim();
            string argumentText = typeName[(open + 1)..^1];
            List<string> argumentNames = SplitGenericArguments(argumentText);
            Type[] arguments = new Type[argumentNames.Count];
            for (int index = 0; index < argumentNames.Count; index++)
            {
                Type? argument = ResolveSignatureType(argumentNames[index]);
                if (argument == null) return null;
                arguments[index] = argument;
            }

            Type? genericDefinition = ResolveType($"{genericName}`{arguments.Length}", null);
            if (genericDefinition == null && !genericName.Contains('.'))
            {
                genericDefinition = ResolveType(
                    $"Il2CppSystem.Collections.Generic.{genericName}`{arguments.Length}", null)
                    ?? ResolveType($"System.Collections.Generic.{genericName}`{arguments.Length}", null);
            }

            try
            {
                return genericDefinition?.MakeGenericType(arguments);
            }
            catch
            {
                return null;
            }
        }

        return ResolveType(typeName, null);
    }

    private static List<string> SplitGenericArguments(string text)
    {
        var result = new List<string>();
        int depth = 0;
        int start = 0;
        for (int index = 0; index < text.Length; index++)
        {
            char character = text[index];
            if (character == '<') depth++;
            else if (character == '>') depth--;
            else if (character == ',' && depth == 0)
            {
                result.Add(text[start..index].Trim());
                start = index + 1;
            }
        }
        result.Add(text[start..].Trim());
        return result;
    }

    private static string NormalizeLegacyTypeName(string typeName) => typeName switch
    {
        "Void" => "System.Void",
        "Boolean" or "Bool" => "System.Boolean",
        "Int32" or "Int" => "System.Int32",
        "Int64" or "Long" => "System.Int64",
        "UInt32" or "UInt" => "System.UInt32",
        "UInt64" or "ULong" => "System.UInt64",
        "Single" or "Float" => "System.Single",
        "Double" => "System.Double",
        "String" => "System.String",
        "Object" => "System.Object",
        _ => typeName
    };

    private static bool IsHighFrequencyMethod(string methodName) =>
        methodName is "Update" or "FixedUpdate" or "LateUpdate" or "OnUpdate";

    private static string NormalizePatchKind(string patchKind) =>
        patchKind.Equals("prefix", StringComparison.OrdinalIgnoreCase) ? "prefix" : "postfix";

    private static string FormatCandidate(HookCandidateV2 candidate) =>
        $"{candidate.Type}.{candidate.Method}({string.Join(", ", candidate.ParameterTypes)})";

    private static string FormatMethod(MethodBase method) =>
        $"{method.DeclaringType?.FullName}.{method.Name}({string.Join(", ", method.GetParameters().Select(parameter => parameter.ParameterType.FullName))})";

    private static readonly object RuntimeSync = new();
    private static readonly Dictionary<MethodBase, List<HookRuntimeBinding>> RuntimeBindings = new();
    private static readonly Dictionary<MethodBase, ParameterInfo[]> ParameterCache = new();
    private static readonly Dictionary<MethodBase, HashSet<string>> RuntimePatchKinds = new();
    private static readonly Dictionary<string, Type?> TypeCache = new(StringComparer.Ordinal);
    private static GregEventBus? GlobalEventBus;
    private static IGregLogger? GlobalLogger;

    private static readonly Dictionary<string, Type> PrimitiveTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        ["void"] = typeof(void), ["System.Void"] = typeof(void),
        ["bool"] = typeof(bool), ["Boolean"] = typeof(bool), ["System.Boolean"] = typeof(bool),
        ["byte"] = typeof(byte), ["System.Byte"] = typeof(byte),
        ["sbyte"] = typeof(sbyte), ["System.SByte"] = typeof(sbyte),
        ["short"] = typeof(short), ["Int16"] = typeof(short), ["System.Int16"] = typeof(short),
        ["ushort"] = typeof(ushort), ["UInt16"] = typeof(ushort), ["System.UInt16"] = typeof(ushort),
        ["int"] = typeof(int), ["Int32"] = typeof(int), ["System.Int32"] = typeof(int),
        ["uint"] = typeof(uint), ["UInt32"] = typeof(uint), ["System.UInt32"] = typeof(uint),
        ["long"] = typeof(long), ["Int64"] = typeof(long), ["System.Int64"] = typeof(long),
        ["ulong"] = typeof(ulong), ["UInt64"] = typeof(ulong), ["System.UInt64"] = typeof(ulong),
        ["float"] = typeof(float), ["Single"] = typeof(float), ["System.Single"] = typeof(float),
        ["double"] = typeof(double), ["System.Double"] = typeof(double),
        ["char"] = typeof(char), ["System.Char"] = typeof(char),
        ["string"] = typeof(string), ["String"] = typeof(string), ["System.String"] = typeof(string),
        ["object"] = typeof(object), ["Object"] = typeof(object), ["System.Object"] = typeof(object),
        ["IntPtr"] = typeof(IntPtr), ["System.IntPtr"] = typeof(IntPtr),
        ["UIntPtr"] = typeof(UIntPtr), ["System.UIntPtr"] = typeof(UIntPtr)
    };

    public static void SetGlobalBus(GregEventBus bus) => GlobalEventBus = bus;
    public static void SetGlobalLogger(IGregLogger logger) => GlobalLogger = logger;

    public static void GenericPrefix(MethodBase __originalMethod, object[] __args) =>
        Dispatch(__originalMethod, __args);

    public static void GenericPostfix(MethodBase __originalMethod, object[] __args) =>
        Dispatch(__originalMethod, __args);

    private static void Dispatch(MethodBase originalMethod, object[]? arguments)
    {
        GregEventBus? eventBus = GlobalEventBus;
        if (eventBus == null) return;

        HookRuntimeBinding[] bindings;
        ParameterInfo[] parameters;
        lock (RuntimeSync)
        {
            if (!RuntimeBindings.TryGetValue(originalMethod, out List<HookRuntimeBinding>? registered))
                return;
            bindings = registered.ToArray();
            parameters = ParameterCache.TryGetValue(originalMethod, out ParameterInfo[]? cached)
                ? cached
                : Array.Empty<ParameterInfo>();
        }

        HookRuntimeBinding[] subscribed = bindings
            .Where(binding => eventBus.HasSubscribers(binding.HookId))
            .ToArray();
        if (subscribed.Length == 0) return;

        bool captureArguments = subscribed.Any(binding => binding.CaptureArguments);
        var payloadData = new Dictionary<string, object>(captureArguments ? parameters.Length + 2 : 2)
        {
            ["method"] = originalMethod.Name,
            ["type"] = originalMethod.DeclaringType?.FullName ?? "Unknown"
        };

        if (captureArguments && arguments != null)
        {
            int count = Math.Min(arguments.Length, parameters.Length);
            for (int index = 0; index < count; index++)
            {
                string name = parameters[index].Name ?? index.ToString();
                payloadData[$"arg_{name}"] = arguments[index] ?? "null";
            }
        }

        var payload = new EventPayload
        {
            HookName = string.Empty,
            OccurredAtUtc = DateTime.UtcNow,
            Data = payloadData,
            IsCancelable = false,
            IsCancelled = false
        };

        foreach (HookRuntimeBinding binding in subscribed)
        {
            try
            {
                eventBus.Publish(binding.HookId, payload with { HookName = binding.HookId });
            }
            catch (Exception ex)
            {
                GlobalLogger?.Error($"Hook dispatch failed for {binding.HookId}", ex);
            }
        }
    }

    private sealed record HookRuntimeBinding(string HookId, bool CaptureArguments, bool HighFrequency);

    private sealed record MethodResolution(MethodBase? Method, string? Reason)
    {
        public static MethodResolution Success(MethodBase method) => new(method, null);
        public static MethodResolution Fail(string reason) => new(null, reason);
    }
}

public sealed class HookManifestV2
{
    [JsonProperty("schemaVersion")]
    public int SchemaVersion { get; set; }

    [JsonProperty("profileId")]
    public string? ProfileId { get; set; }

    [JsonProperty("hooks")]
    public List<HookDefinitionV2> Hooks { get; set; } = new();
}

public sealed class HookDefinitionV2
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("group")]
    public string Group { get; set; } = string.Empty;

    [JsonProperty("required")]
    public bool Required { get; set; }

    [JsonProperty("highFrequency")]
    public bool HighFrequency { get; set; }

    [JsonProperty("captureArguments")]
    public bool CaptureArguments { get; set; } = true;

    [JsonProperty("patchKind")]
    public string PatchKind { get; set; } = "postfix";

    [JsonProperty("candidates")]
    public List<HookCandidateV2> Candidates { get; set; } = new();
}

public sealed class HookCandidateV2
{
    [JsonProperty("profiles")]
    public List<string> Profiles { get; set; } = new();

    [JsonProperty("assembly")]
    public string Assembly { get; set; } = "Assembly-CSharp";

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("method")]
    public string Method { get; set; } = string.Empty;

    [JsonProperty("genericArity")]
    public int GenericArity { get; set; }

    [JsonProperty("static")]
    public bool? Static { get; set; }

    [JsonProperty("returnType")]
    public string ReturnType { get; set; } = "System.Void";

    [JsonProperty("parameterTypes")]
    public List<string> ParameterTypes { get; set; } = new();
}

public sealed class GameHookJsonDef
{
    public string Group { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public bool IsVoid { get; set; }
    public List<GameHookParameterDef> Parameters { get; set; } = new();
}

public sealed class GameHookParameterDef
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
}
