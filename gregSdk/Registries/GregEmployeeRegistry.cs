using gregSdk.Definitions;

namespace gregSdk.Registries;

public class GregEmployeeRegistry : GregContentRegistry<EmployeeDefinition>
{
    public GregEmployeeRegistry() : base(x => x.Id) { }
}
