using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregCustomerRegistry : GregContentRegistry<CustomerDefinition>
{
    public GregCustomerRegistry() : base(x => x.Id) { }
}
