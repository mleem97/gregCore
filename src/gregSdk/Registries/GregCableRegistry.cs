using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregCableRegistry : GregContentRegistry<CableDefinition>
{
    public GregCableRegistry() : base(x => x.Id) { }
}
