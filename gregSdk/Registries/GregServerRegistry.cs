using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregServerRegistry : GregContentRegistry<ServerDefinition>
{
    public GregServerRegistry() : base(x => x.Id) { }
}
