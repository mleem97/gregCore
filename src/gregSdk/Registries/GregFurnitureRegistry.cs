using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregFurnitureRegistry : GregContentRegistry<FurnitureDefinition>
{
    public GregFurnitureRegistry() : base(x => x.Id) { }
}
