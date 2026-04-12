using greg.Sdk.Definitions;

namespace greg.Sdk.Registries;

public class GregEmployeeRegistry : GregContentRegistry<EmployeeDefinition>
{
    public GregEmployeeRegistry() : base(x => x.Id) { }
}
