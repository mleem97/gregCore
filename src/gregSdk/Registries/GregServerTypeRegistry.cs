using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregServerTypeRegistry : GregContentRegistry<ServerTypeDefinition>
{
    public GregServerTypeRegistry() : base(x => x.Id) { }
}
