using gregSdk.Definitions;
using gregSdk.Interfaces;

namespace gregSdk.Validators;

public class SwitchValidator : IContentValidator<SwitchDefinition>
{
    public bool Validate(SwitchDefinition definition, out string error)
    {
        error = string.Empty;
        if (string.IsNullOrEmpty(definition.Id)) { error = "ID is required."; return false; }
        if (definition.FrontPorts < 0) { error = "FrontPorts cannot be negative."; return false; }
        if (definition.RearPorts < 0) { error = "RearPorts cannot be negative."; return false; }
        return true;
    }
}
