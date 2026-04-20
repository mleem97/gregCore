/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Attribut zur Markierung einer Mod-Klasse.
/// Maintainer:   Wird vom AssemblyScanner via Mono.Cecil erkannt.
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
