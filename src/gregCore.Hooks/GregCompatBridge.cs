using System;
using System.Reflection;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Checks that methods exist before Harmony patching (Harmony layer).
/// Protects against game version drift and IL2CPP incompatibilities.
/// </summary>
public sealed class GregCompatBridge
{
    private readonly IGregLogger _logger;

    public GregCompatBridge(IGregLogger logger)
    {
        _logger = logger.ForContext("CompatBridge");
    }

    /// <summary>
    /// Checks whether a method exists in the assembly.
    /// </summary>
    public bool VerifyMethod(string ns, string className, string methodName)
    {
        try
        {
            var fullName = $"{ns}.{className}";
            var type = Type.GetType(fullName);
            if (type == null)
            {
                // Try to find via Assembly-CSharp
                var assembly = Assembly.Load("Assembly-CSharp");
                type = assembly?.GetType(fullName);
            }

            if (type == null)
            {
                _logger.Warning($"Class not found: {fullName}");
                return false;
            }

            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method == null)
            {
                _logger.Warning($"Method not found: {fullName}.{methodName}");
                return false;
            }

            _logger.Success($"Method verified: {fullName}.{methodName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Error verifying {ns}.{className}.{methodName}: {ex.Message}");
            return false;
        }
    }
}
