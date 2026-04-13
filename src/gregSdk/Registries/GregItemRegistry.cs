using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregItemRegistry : GregContentRegistry<ItemDefinition>
{
    public GregItemRegistry() : base(x => x.Id) { }
}
