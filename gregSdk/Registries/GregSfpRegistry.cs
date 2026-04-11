using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregSfpRegistry : GregContentRegistry<SfpDefinition>
{
    public GregSfpRegistry() : base(x => x.Id) { }
}
