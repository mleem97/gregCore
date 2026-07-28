using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Hooks;

public enum MethodCompatibilityStatus
{
    Compatible,
    AssemblyMissing,
    TypeMissing,
    ParameterTypeMissing,
    MethodMissing,
    Ambiguous,
    ReturnTypeMismatch,
    Error
}

public sealed record MethodCompatibilityRequest(
    string Assembly,
    string Type,
    string Method,
    IReadOnlyList<string> ParameterTypes,
    string? ReturnType = null,
    bool? Static = null,
    int GenericArity = 0);

public sealed record MethodCompatibilityResult(
    MethodCompatibilityStatus Status,
    MethodCompatibilityRequest Request,
    Type? ResolvedType = null,
    MethodBase? ResolvedMethod = null,
    string? ActualSignature = null,
    string? Reason = null)
{
    public bool IsCompatible => Status == MethodCompatibilityStatus.Compatible;
}

/// <summary>
/// Performs non-patching compatibility checks against complete signatures.
/// This is used by diagnostics and CI smoke tests before Harmony is invoked.
/// </summary>
public sealed class GregCompatBridge
{
    private readonly IGregLogger _logger;

    public GregCompatBridge(IGregLogger logger)
    {
        _logger = (logger ?? throw new ArgumentNullException(nameof(logger))).ForContext("CompatBridge");
    }

    public MethodCompatibilityResult VerifyMethod(MethodCompatibilityRequest request)
    {
        try
        {
            Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name?.Equals(
                    request.Assembly, StringComparison.OrdinalIgnoreCase) == true);
            if (assembly == null)
            {
                return Report(new MethodCompatibilityResult(
                    MethodCompatibilityStatus.AssemblyMissing,
                    request,
                    Reason: $"Assembly not loaded: {request.Assembly}"));
            }

            Type? type = assembly.GetType(request.Type, throwOnError: false, ignoreCase: false);
            if (type == null)
            {
                return Report(new MethodCompatibilityResult(
                    MethodCompatibilityStatus.TypeMissing,
                    request,
                    Reason: $"Type not found: {request.Type}"));
            }

            var parameterTypes = new Type[request.ParameterTypes.Count];
            for (int index = 0; index < request.ParameterTypes.Count; index++)
            {
                Type? resolved = ResolveType(request.ParameterTypes[index]);
                if (resolved == null)
                {
                    return Report(new MethodCompatibilityResult(
                        MethodCompatibilityStatus.ParameterTypeMissing,
                        request,
                        type,
                        Reason: $"Parameter {index} type not found: {request.ParameterTypes[index]}"));
                }
                parameterTypes[index] = resolved;
            }

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Instance | BindingFlags.Static |
                                       BindingFlags.FlattenHierarchy;
            MethodInfo[] matches = type.GetMethods(flags)
                .Where(method => method.Name.Equals(request.Method, StringComparison.Ordinal))
                .Where(method => !request.Static.HasValue || method.IsStatic == request.Static.Value)
                .Where(method => GenericArity(method) == request.GenericArity)
                .Where(method => ParametersEqual(method.GetParameters(), parameterTypes))
                .ToArray();

            if (matches.Length == 0)
            {
                return Report(new MethodCompatibilityResult(
                    MethodCompatibilityStatus.MethodMissing,
                    request,
                    type,
                    Reason: "No method matched the complete signature."));
            }

            if (matches.Length > 1)
            {
                return Report(new MethodCompatibilityResult(
                    MethodCompatibilityStatus.Ambiguous,
                    request,
                    type,
                    Reason: $"{matches.Length} methods matched the requested signature."));
            }

            MethodInfo method = matches[0];
            if (!string.IsNullOrWhiteSpace(request.ReturnType))
            {
                Type? expectedReturnType = ResolveType(request.ReturnType);
                if (expectedReturnType == null || expectedReturnType != method.ReturnType)
                {
                    return Report(new MethodCompatibilityResult(
                        MethodCompatibilityStatus.ReturnTypeMismatch,
                        request,
                        type,
                        method,
                        Format(method),
                        $"Expected return type {request.ReturnType}, found {method.ReturnType.FullName}."));
                }
            }

            return Report(new MethodCompatibilityResult(
                MethodCompatibilityStatus.Compatible,
                request,
                type,
                method,
                Format(method)));
        }
        catch (Exception ex)
        {
            return Report(new MethodCompatibilityResult(
                MethodCompatibilityStatus.Error,
                request,
                Reason: ex.Message));
        }
    }

    /// <summary>
    /// Backward-compatible name-only probe. New code should use the structured overload.
    /// </summary>
    [Obsolete("Use VerifyMethod(MethodCompatibilityRequest) with a complete signature.")]
    public bool VerifyMethod(string ns, string className, string methodName)
    {
        string typeName = string.IsNullOrWhiteSpace(ns) ? className : $"{ns}.{className}";
        try
        {
            Assembly? assembly = AppDomain.CurrentDomain.GetAssemblies()
                .FirstOrDefault(candidate => candidate.GetName().Name == "Assembly-CSharp");
            Type? type = assembly?.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (type == null) return false;

            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.Instance | BindingFlags.Static;
            return type.GetMethods(flags).Any(method => method.Name == methodName);
        }
        catch
        {
            return false;
        }
    }

    private MethodCompatibilityResult Report(MethodCompatibilityResult result)
    {
        if (result.IsCompatible)
            _logger.Success($"Compatible method: {result.ActualSignature}");
        else
            _logger.Warning($"Compatibility check failed ({result.Status}): {result.Reason}");
        return result;
    }

    private static int GenericArity(MethodInfo method) =>
        method.IsGenericMethod ? method.GetGenericArguments().Length : 0;

    private static bool ParametersEqual(ParameterInfo[] actual, Type[] expected)
    {
        if (actual.Length != expected.Length) return false;
        for (int index = 0; index < actual.Length; index++)
        {
            if (actual[index].ParameterType != expected[index]) return false;
        }
        return true;
    }

    private static Type? ResolveType(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return null;
        string text = name.Trim();
        bool byRef = text.EndsWith("&", StringComparison.Ordinal);
        if (byRef) text = text[..^1];
        bool array = text.EndsWith("[]", StringComparison.Ordinal);
        if (array) text = text[..^2];

        Type? type = text switch
        {
            "void" or "Void" or "System.Void" => typeof(void),
            "bool" or "Boolean" or "System.Boolean" => typeof(bool),
            "int" or "Int32" or "System.Int32" => typeof(int),
            "long" or "Int64" or "System.Int64" => typeof(long),
            "float" or "Single" or "System.Single" => typeof(float),
            "double" or "Double" or "System.Double" => typeof(double),
            "string" or "String" or "System.String" => typeof(string),
            "object" or "Object" or "System.Object" => typeof(object),
            _ => Type.GetType(text, throwOnError: false, ignoreCase: false)
        };

        if (type == null)
        {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(text, throwOnError: false, ignoreCase: false)
                    ?? assembly.GetType($"Il2Cpp.{text}", throwOnError: false, ignoreCase: false)
                    ?? assembly.GetType($"UnityEngine.{text}", throwOnError: false, ignoreCase: false);
                if (type != null) break;
            }
        }

        if (type == null) return null;
        if (array) type = type.MakeArrayType();
        if (byRef) type = type.MakeByRefType();
        return type;
    }

    private static string Format(MethodBase method) =>
        $"{method.DeclaringType?.FullName}.{method.Name}(" +
        string.Join(", ", method.GetParameters().Select(parameter => parameter.ParameterType.FullName)) + ")";
}
