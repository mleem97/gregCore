using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregFurnitureRegistry : GregContentRegistry<FurnitureDefinition>
{
    public GregFurnitureRegistry() : base(x => x.Id) { }
}

