using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregItemRegistry : GregContentRegistry<ItemDefinition>
{
    public GregItemRegistry() : base(x => x.Id) { }
}

