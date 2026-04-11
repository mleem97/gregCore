using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregCustomerRegistry : GregContentRegistry<CustomerDefinition>
{
    public GregCustomerRegistry() : base(x => x.Id) { }
}
