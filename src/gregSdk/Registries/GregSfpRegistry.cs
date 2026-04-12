using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregSfpRegistry : GregContentRegistry<SfpDefinition>
{
    public GregSfpRegistry() : base(x => x.Id) { }
}
