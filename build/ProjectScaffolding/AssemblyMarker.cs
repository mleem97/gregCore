namespace GregCore.Build.ProjectScaffolding;

/// <summary>
/// Marks migration assemblies until production types are moved behind their
/// final project boundaries. Keeping the projects buildable prevents the
/// solution from advertising modules that do not exist on disk.
/// </summary>
internal static class AssemblyMarker
{
}
