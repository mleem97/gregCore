using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregCustomerRegistry : GregContentRegistry<CustomerDefinition>
{
    public GregCustomerRegistry() : base(x => x.Id) { }
}

