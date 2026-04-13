namespace greg.Sdk.Definitions;

public class EmployeeDefinition
{
    public string Id { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public float BaseSalary { get; set; }
    public int SkillLevel { get; set; }
    public string Specialization { get; set; } = "Generalist";
    public bool IsAvailable { get; set; } = true;
    public string VisualPrefabId { get; set; }
}

