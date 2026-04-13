using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregSwitchRegistry : GregContentRegistry<SwitchDefinition>
{
    public GregSwitchRegistry() : base(x => x.Id) { }
}

