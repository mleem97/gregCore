using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregServerRegistry : GregContentRegistry<ServerDefinition>
{
    public GregServerRegistry() : base(x => x.Id) { }
}
