using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregServerRegistry : GregContentRegistry<ServerDefinition>
{
    public GregServerRegistry() : base(x => x.Id) { }
}

