using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregPlacementRegistry : GregContentRegistry<PlacementRuleDefinition>
{
    public GregPlacementRegistry() : base(x => x.Id) { }
}

