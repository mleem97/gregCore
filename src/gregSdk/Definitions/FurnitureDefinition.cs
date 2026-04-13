namespace greg.Sdk.Definitions;

public class FurnitureDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ModelPath { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Category { get; set; } = string.Empty;
    public string[] InteractionEffects { get; set; } = System.Array.Empty<string>();
}

