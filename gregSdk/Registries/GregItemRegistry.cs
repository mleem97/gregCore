using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregItemRegistry : GregContentRegistry<ItemDefinition>
{
    public GregItemRegistry() : base(x => x.Id) { }
}
