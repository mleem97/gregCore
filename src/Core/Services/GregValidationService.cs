using System;
using gregCore.Core.Abstractions;

namespace gregCore.Core.Services;

/// <summary>
/// Validiert API-Eingaben und Framework-Zustände (Core Layer).
/// </summary>
public sealed class GregValidationService
{
    private readonly IGregLogger _logger;

    public GregValidationService(IGregLogger logger)
    {
        _logger = logger.ForContext("ValidationService");
    }

    public bool ValidateModId(string modId)
    {
        if (string.IsNullOrWhiteSpace(modId))
        {
            _logger.Error("Validierungsfehler: ModId darf nicht leer sein.");
            return false;
        }
        return true;
    }

    public bool ValidateHookName(string hookName)
    {
        if (string.IsNullOrWhiteSpace(hookName) || !hookName.StartsWith("greg."))
        {
            _logger.Error($"Validierungsfehler: Ungültiger Hook-Name '{hookName}'. Muss mit 'greg.' beginnen.");
            return false;
        }
        return true;
    }
}
