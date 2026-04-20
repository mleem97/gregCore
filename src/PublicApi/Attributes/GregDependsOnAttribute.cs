/// <file-summary>
/// Schicht:      PublicApi
/// Zweck:        Attribut für Mod-Abhängigkeiten.
/// Maintainer:   Wird vom DependencyResolver ausgewertet.
/// </file-summary>

namespace gregCore.PublicApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class GregDependsOnAttribute : Attribute
{
    public string DependencyId { get; }
    public string MinimumVersion { get; }

    public GregDependsOnAttribute(string dependencyId, string minimumVersion = "1.0.0")
    {
        DependencyId = dependencyId;
        MinimumVersion = minimumVersion;
    }
}
