namespace gregCoreSDK.Sdk.Definitions;

public class CableDefinition
{
    public string Id { get; set; } = string.Empty;
    public int MaxSpeedGbps { get; set; }
    public string ColorHex { get; set; } = "#FFFFFF";
    public string CableType { get; set; } = "RJ45";
    public float LatencyMs { get; set; } = 0.1f;
    public float Durability { get; set; } = 100.0f;
}
