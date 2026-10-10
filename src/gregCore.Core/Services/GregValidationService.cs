using System;
using gregCore.Core.Abstractions;

namespace gregCore.Core.Services;

/// <summary>
/// Validates API inputs and framework states (Core Layer).
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
            _logger.Error("Validation error: ModId must not be empty.");
            return false;
        }
        return true;
    }

    public bool ValidateHookName(string hookName)
    {
        if (string.IsNullOrWhiteSpace(hookName) || !hookName.StartsWith("greg."))
        {
            _logger.Error($"Validation error: Invalid hook name '{hookName}'. Must start with 'greg.'.");
            return false;
        }
        return true;
    }
}
