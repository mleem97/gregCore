using greg.Sdk.Definitions;

namespace gregCoreSDK.Sdk.Registries;

public class GregEmployeeRegistry : GregContentRegistry<EmployeeDefinition>
{
    public GregEmployeeRegistry() : base(x => x.Id) { }
}
