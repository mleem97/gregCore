using greg.Sdk.Definitions;
using greg.Sdk.Interfaces;

namespace greg.Sdk.Validators;

public class ServerValidator : IContentValidator<ServerDefinition>
{
    public bool Validate(ServerDefinition definition, out string error)
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(definition.Id)) { error = "ID is required."; return false; }
        if (definition.RackUnits < 1 || definition.RackUnits > 42) { error = "RackUnits must be between 1 and 42."; return false; }
        if (definition.PowerUsageWatts < 0) { error = "PowerUsageWatts cannot be negative."; return false; }
        return true;
    }
}
