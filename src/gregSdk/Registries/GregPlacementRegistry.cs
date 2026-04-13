using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregPlacementRegistry : GregContentRegistry<PlacementRuleDefinition>
{
    public GregPlacementRegistry() : base(x => x.Id) { }
}
