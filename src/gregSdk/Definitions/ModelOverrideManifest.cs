namespace greg.Sdk.Definitions;

public class ModelOverrideManifest
{
    public string TargetContentId { get; set; } = string.Empty;
    public string ReplacementPath { get; set; } = string.Empty;
    public string FallbackPath { get; set; } = string.Empty;
    public int SchemaVersion { get; set; }
    public int Priority { get; set; }
    public string Author { get; set; } = "Unknown";
}
