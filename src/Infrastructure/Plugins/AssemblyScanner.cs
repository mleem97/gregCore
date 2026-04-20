/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Scannt Assemblies nach Mod-Klassen via Mono.Cecil.
/// Maintainer:   Nutzt Mono.Cecil für statische Analyse. Assembly.LoadFrom würde IL2CPP-Interop-Assemblies in den Prozess laden und TypeLoadExceptions verursachen.
/// </file-summary>

using Mono.Cecil;

namespace gregCore.Infrastructure.Plugins;

internal sealed class AssemblyScanner : IAssemblyScanner
{
    public IReadOnlyList<PluginInfo> ScanDirectory(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var plugins = new List<PluginInfo>();
        if (!Directory.Exists(path)) return plugins;

        foreach (var file in Directory.GetFiles(path, "*.dll"))
        {
            try
            {
                using var module = ModuleDefinition.ReadModule(file);
                plugins.Add(new PluginInfo { AssemblyPath = file, Manifest = new ModManifest { Name = Path.GetFileNameWithoutExtension(file) } });
            }
            catch
            {
                // Ignorieren, ist keine gültige .NET Assembly
            }
        }

        return plugins;
    }
}
