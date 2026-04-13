namespace gregCoreSDK.Sdk.Definitions;

public class PlacementRuleDefinition
{
    public string Id { get; set; } = string.Empty;
    public string TargetCategoryId { get; set; } = string.Empty;
    public string[] AllowedSurfaceTags { get; set; } = System.Array.Empty<string>();
    public bool RequiresPower { get; set; }
    public bool RequiresNetwork { get; set; }
}
