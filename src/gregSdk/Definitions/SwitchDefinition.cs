namespace greg.Sdk.Definitions;

public class SwitchDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int FrontPorts { get; set; }
    public int RearPorts { get; set; }
    public int ThroughputGbps { get; set; }
    public bool IsManaged { get; set; }
    public string[] SupportedSfpProfiles { get; set; } = System.Array.Empty<string>();
    public int PowerUsageWatts { get; set; }
    public float Price { get; set; }
    public string ModelOverridePath { get; set; }
}

