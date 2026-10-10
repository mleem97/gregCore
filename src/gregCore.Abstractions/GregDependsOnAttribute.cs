/// <file-summary>
/// Layer:       PublicApi
/// Purpose:      Attribute for mod dependencies.
/// Maintainer:   Evaluated by the DependencyResolver.
/// </file-summary>

namespace gregCore.PublicApi.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public sealed class GregDependsOnAttribute : Attribute
{
    public string DependencyId { get; }
    public string MinimumVersion { get; }

    public GregDependsOnAttribute(string dependencyId)
        : this(dependencyId, "1.0.0") { }

    public GregDependsOnAttribute(string dependencyId, string minimumVersion)
    {
        DependencyId = dependencyId;
        MinimumVersion = minimumVersion;
    }
}
