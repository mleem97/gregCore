using System;
using System.Reflection;
using gregCore.Core.Abstractions;

namespace gregCore.GameLayer.Hooks;

/// <summary>
/// Prüft die Existenz von Methoden vor dem Harmony-Patching (Harmony Layer).
/// Schützt vor Game-Version-Drift und IL2CPP-Inkompatibilitäten.
/// </summary>
public sealed class GregCompatBridge
{
    private readonly IGregLogger _logger;

    public GregCompatBridge(IGregLogger logger)
    {
        _logger = logger.ForContext("CompatBridge");
    }

    /// <summary>
    /// Prüft, ob eine Methode in der Assembly existiert.
    /// </summary>
    public bool VerifyMethod(string ns, string className, string methodName)
    {
        try
        {
            var fullName = $"{ns}.{className}";
            var type = Type.GetType(fullName);
            if (type == null)
            {
                // Versuche über Assembly-CSharp zu finden
                var assembly = Assembly.Load("Assembly-CSharp");
                type = assembly?.GetType(fullName);
            }

            if (type == null)
            {
                _logger.Warning($"Klasse nicht gefunden: {fullName}");
                return false;
            }

            var method = type.GetMethod(methodName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (method == null)
            {
                _logger.Warning($"Methode nicht gefunden: {fullName}.{methodName}");
                return false;
            }

            _logger.Success($"Methode verifiziert: {fullName}.{methodName}");
            return true;
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler bei der Verifizierung von {ns}.{className}.{methodName}: {ex.Message}");
            return false;
        }
    }
}
