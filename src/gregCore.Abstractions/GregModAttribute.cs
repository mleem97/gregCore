/// <file-summary>
/// Layer:       PublicApi
/// Purpose:      Attribute marking a mod class.
/// Maintainer:   Detected by the AssemblyScanner via Mono.Cecil.
/// </file-summary>

namespace gregCore.PublicApi.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class GregModAttribute : Attribute
{
    public string Id { get; }
    public string Name { get; }
    public string Version { get; }

    public GregModAttribute(string id, string name, string version)
    {
        Id = id;
        Name = name;
        Version = version;
    }
}
