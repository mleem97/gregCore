using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregCableRegistry : GregContentRegistry<CableDefinition>
{
    public GregCableRegistry() : base(x => x.Id) { }
}
