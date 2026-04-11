using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregFurnitureRegistry : GregContentRegistry<FurnitureDefinition>
{
    public GregFurnitureRegistry() : base(x => x.Id) { }
}
