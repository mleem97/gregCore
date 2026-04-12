namespace greg.Sdk.Definitions;

public class CustomerDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Segment { get; set; } = "SmallBusiness";
    public float Budget { get; set; }
    public string[] RequirementRules { get; set; } = System.Array.Empty<string>();
    public string[] OwnershipRules { get; set; } = System.Array.Empty<string>();
}
