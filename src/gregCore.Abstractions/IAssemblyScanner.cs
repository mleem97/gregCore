/// <file-summary>
/// Layer:       Infrastructure (Internal)
/// Purpose:      Interface for the assembly scanner.
/// Maintainer:   Used internally by the plugin registry.
/// </file-summary>

namespace gregCore.Infrastructure.Plugins;

public interface IAssemblyScanner
{
    IReadOnlyList<PluginInfo> ScanDirectory(string path);
}
