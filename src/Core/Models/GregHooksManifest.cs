namespace gregCore.Core.Models;

public class GregHookPayloadSchema
{
    public string TargetType { get; set; } = string.Empty;
    public bool IsStatic { get; set; }
    public string HookSubject { get; set; } = string.Empty;
    public string ReturnType { get; set; } = string.Empty;
    public string Parameters { get; set; } = string.Empty;
}

public class GregHookDef
{
    public string Name { get; set; } = string.Empty;
    public string Legacy { get; set; } = string.Empty;
    public string PatchTarget { get; set; } = string.Empty;
    public string Strategy { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string HookSubject { get; set; } = string.Empty;
    public bool HotLoop { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
    public string FriendlyAlias { get; set; } = string.Empty;
    
    public GregHookPayloadSchema? PayloadSchema { get; set; }
}

public class GregHooksManifest
{
    public int Version { get; set; }
    public string Description { get; set; } = string.Empty;
    public string GeneratedFrom { get; set; } = string.Empty;
    public List<GregHookDef> Hooks { get; set; } = new();
}
