using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregServerTypeRegistry : GregContentRegistry<ServerTypeDefinition>
{
    public GregServerTypeRegistry() : base(x => x.Id) { }
}
