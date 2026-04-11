using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregPlacementRegistry : GregContentRegistry<PlacementRuleDefinition>
{
    public GregPlacementRegistry() : base(x => x.Id) { }
}
