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
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Legacy { get; set; } = string.Empty;
    public string Assembly { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Member { get; set; } = string.Empty;
    public string Signature { get; set; } = string.Empty;
    public string Domain { get; set; } = string.Empty;
    public string Threading { get; set; } = string.Empty;
    public bool Cancellable { get; set; }
    public string Risk { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<string> SupportedLanguages { get; set; } = new();
    public string ApprovalReason { get; set; } = string.Empty;
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
    public int ManifestVersion { get; set; }
    public string SchemaVersion { get; set; } = string.Empty;
    public string GameBuild { get; set; } = string.Empty;
    public string AssemblyFingerprint { get; set; } = string.Empty;
    public string UnityVersion { get; set; } = string.Empty;
    public string MelonLoaderVersion { get; set; } = string.Empty;
    public string Il2CppInteropVersion { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Description { get; set; } = string.Empty;
    public string GeneratedFrom { get; set; } = string.Empty;
    public List<GregHookDef> Hooks { get; set; } = new();
    public List<GregExcludedMember> ExcludedMembers { get; set; } = new();
}

public class GregExcludedMember
{
    public string Assembly { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Member { get; set; } = string.Empty;
    public string Reason { get; set; } = string.Empty;
}
