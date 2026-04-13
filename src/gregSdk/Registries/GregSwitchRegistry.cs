using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregSwitchRegistry : GregContentRegistry<SwitchDefinition>
{
    public GregSwitchRegistry() : base(x => x.Id) { }
}
