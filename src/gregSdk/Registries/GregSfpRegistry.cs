using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregSfpRegistry : GregContentRegistry<SfpDefinition>
{
    public GregSfpRegistry() : base(x => x.Id) { }
}
