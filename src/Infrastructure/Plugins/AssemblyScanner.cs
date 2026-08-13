/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Scannt Assemblies nach Mod-Klassen via Mono.Cecil.
/// Maintainer:   Nutzt Mono.Cecil für statische Analyse. Assembly.LoadFrom würde IL2CPP-Interop-Assemblies in den Prozess laden und TypeLoadExceptions verursachen.
/// </file-summary>

using Mono.Cecil;
using System.Security.Cryptography;
using gregCore.PublicApi.Attributes;

namespace gregCore.Infrastructure.Plugins;

public sealed class AssemblyScanner : IAssemblyScanner
{
    public IReadOnlyList<PluginInfo> ScanDirectory(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var plugins = new List<PluginInfo>();
        if (!Directory.Exists(path)) return plugins;

        foreach (var file in Directory.GetFiles(path, "*.dll").OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            try
            {
                using var module = ModuleDefinition.ReadModule(file);
                var modType = module.Types.SelectMany(AllTypes)
                    .FirstOrDefault(t => t.CustomAttributes.Any(a => a.AttributeType.FullName == typeof(GregModAttribute).FullName));
                var modAttribute = modType?.CustomAttributes.FirstOrDefault(a => a.AttributeType.FullName == typeof(GregModAttribute).FullName);
                var id = GetString(modAttribute, 0);
                var name = GetString(modAttribute, 1);
                var version = GetString(modAttribute, 2);
                var dependencies = modType?.CustomAttributes
                    .Where(a => a.AttributeType.FullName == typeof(GregDependsOnAttribute).FullName)
                    .Select(a => GetString(a, 0)).Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.Ordinal).OrderBy(x => x, StringComparer.Ordinal).ToArray() ?? Array.Empty<string>();
                plugins.Add(new PluginInfo
                {
                    AssemblyPath = file,
                    AssemblyName = module.Assembly?.Name.Name ?? Path.GetFileNameWithoutExtension(file),
                    AssemblyVersion = module.Assembly?.Name.Version?.ToString() ?? string.Empty,
                    ModTypeName = modType?.FullName ?? string.Empty,
                    Sha256 = ComputeSha256(file),
                    DeclaredDependencies = dependencies,
                    ScanStatus = modType == null ? "NO_MOD_ATTRIBUTE" : "SCANNED",
                    Manifest = new ModManifest
                    {
                        Id = id,
                        Name = string.IsNullOrWhiteSpace(name) ? Path.GetFileNameWithoutExtension(file) : name,
                        Version = string.IsNullOrWhiteSpace(version) ? (module.Assembly?.Name.Version?.ToString() ?? "0.0.0") : version,
                        Dependencies = dependencies
                    }
                });
            }
            catch
            {
                // Ignorieren, ist keine gültige .NET Assembly
            }
        }

        return plugins;
    }

    private static IEnumerable<TypeDefinition> AllTypes(TypeDefinition type)
    {
        yield return type;
        foreach (var nested in type.NestedTypes.SelectMany(AllTypes)) yield return nested;
    }

    private static string GetString(CustomAttribute? attribute, int index) =>
        attribute != null && attribute.ConstructorArguments.Count > index
            ? attribute.ConstructorArguments[index].Value?.ToString() ?? string.Empty : string.Empty;

    private static string ComputeSha256(string file)
    {
        using var stream = File.OpenRead(file);
        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(stream)).ToLowerInvariant();
    }
}
