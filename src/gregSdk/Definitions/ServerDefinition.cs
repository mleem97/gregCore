namespace gregCoreSDK.Sdk.Definitions;

public class ServerDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int RackUnits { get; set; } = 1;
    public int PowerUsageWatts { get; set; }
    public int MaxIOPS { get; set; }
    public string ServerTypeId { get; set; } = string.Empty;
    public float Price { get; set; }
    public string[] Tags { get; set; } = System.Array.Empty<string>();
    public string ModelOverridePath { get; set; }
}
