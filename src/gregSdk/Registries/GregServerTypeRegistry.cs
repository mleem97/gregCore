using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregServerTypeRegistry : GregContentRegistry<ServerTypeDefinition>
{
    public GregServerTypeRegistry() : base(x => x.Id) { }
}

