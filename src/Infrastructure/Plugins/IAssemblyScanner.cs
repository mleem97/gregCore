/// <file-summary>
/// Schicht:      Infrastructure (Internal)
/// Zweck:        Interface für den Assembly-Scanner.
/// Maintainer:   Wird intern von der Plugin-Registry genutzt.
/// </file-summary>

namespace gregCore.Infrastructure.Plugins;

public interface IAssemblyScanner
{
    IReadOnlyList<PluginInfo> ScanDirectory(string path);
}
