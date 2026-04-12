namespace greg.Sdk.Definitions;

public class SfpDefinition
{
    public string Id { get; set; } = string.Empty;
    public double SpeedGbps { get; set; }
    public string[] CompatibilityTags { get; set; } = System.Array.Empty<string>();
    public decimal Price { get; set; }
}
