using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregSwitchRegistry : GregContentRegistry<SwitchDefinition>
{
    public GregSwitchRegistry() : base(x => x.Id) { }
}
