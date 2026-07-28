using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Il2CppInterop.Runtime.Injection;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Interop;

public enum Il2CppRegistrationStatus
{
    NotAttempted,
    Registered,
    AlreadyRegistered,
    UnsupportedType,
    Failed,
    SkippedByCompatibilityProfile
}

public sealed record Il2CppRegistrationResult(
    Type ManagedType,
    Il2CppRegistrationStatus Status,
    bool Required,
    string? Message = null,
    Exception? Exception = null)
{
    public bool Succeeded => Status is Il2CppRegistrationStatus.Registered or Il2CppRegistrationStatus.AlreadyRegistered;
}

/// <summary>
/// Single registration boundary for managed components injected into IL2CPP.
/// It validates constructor shape, is idempotent and produces per-type results
/// instead of allowing one failure to abort every registration.
/// </summary>
public sealed class Il2CppTypeRegistry
{
    private readonly IGregLogger _logger;
    private readonly ConcurrentDictionary<Type, Il2CppRegistrationResult> _results = new();

    public Il2CppTypeRegistry(IGregLogger logger)
    {
        _logger = (logger ?? throw new ArgumentNullException(nameof(logger))).ForContext("Il2CppTypeRegistry");
    }

    public IReadOnlyCollection<Il2CppRegistrationResult> Results => _results.Values.ToArray();

    public Il2CppRegistrationResult Register<T>(bool required = false, bool profileAllowsInjection = true)
        where T : class
    {
        Type managedType = typeof(T);
        if (_results.TryGetValue(managedType, out Il2CppRegistrationResult? cached))
        {
            return cached with
            {
                Status = cached.Succeeded
                    ? Il2CppRegistrationStatus.AlreadyRegistered
                    : cached.Status
            };
        }

        if (!profileAllowsInjection)
        {
            return Store(new Il2CppRegistrationResult(
                managedType,
                Il2CppRegistrationStatus.SkippedByCompatibilityProfile,
                required,
                "The active compatibility profile disabled class injection."));
        }

        if (!HasSupportedConstructionPath(managedType, out string? validationMessage))
        {
            return Store(new Il2CppRegistrationResult(
                managedType,
                Il2CppRegistrationStatus.UnsupportedType,
                required,
                validationMessage));
        }

        try
        {
            // The non-generic overload is present across the supported
            // Il2CppInterop 1.x line and avoids coupling this wrapper to
            // changing generic constraints.
            ClassInjector.RegisterTypeInIl2Cpp(managedType);
            return Store(new Il2CppRegistrationResult(
                managedType,
                Il2CppRegistrationStatus.Registered,
                required));
        }
        catch (Exception ex)
        {
            return Store(new Il2CppRegistrationResult(
                managedType,
                Il2CppRegistrationStatus.Failed,
                required,
                ex.Message,
                ex));
        }
    }

    public bool RequiredRegistrationsSucceeded() =>
        _results.Values.Where(result => result.Required).All(result => result.Succeeded);

    private Il2CppRegistrationResult Store(Il2CppRegistrationResult result)
    {
        _results[result.ManagedType] = result;

        if (result.Succeeded)
        {
            _logger.Info($"IL2CPP type registered: {result.ManagedType.FullName}");
        }
        else if (result.Required)
        {
            _logger.Error(
                $"Required IL2CPP type registration failed: {result.ManagedType.FullName}: {result.Message}",
                result.Exception);
        }
        else
        {
            _logger.Warning(
                $"Optional IL2CPP type registration skipped/failed: {result.ManagedType.FullName}: {result.Message}");
        }

        return result;
    }

    private static bool HasSupportedConstructionPath(Type type, out string? message)
    {
        if (type.IsAbstract || type.IsInterface || type.ContainsGenericParameters)
        {
            message = "Injected types must be closed, non-abstract classes.";
            return false;
        }

        const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        bool hasPointerConstructor = type.GetConstructor(
            flags,
            binder: null,
            types: new[] { typeof(IntPtr) },
            modifiers: null) != null;

        if (!hasPointerConstructor)
        {
            message = "Missing required constructor with a single System.IntPtr parameter.";
            return false;
        }

        message = null;
        return true;
    }
}
