using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregCableRegistry : GregContentRegistry<CableDefinition>
{
    public GregCableRegistry() : base(x => x.Id) { }
}
