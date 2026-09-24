// Rule contracts. Assembly rules run per managed DLL (Cecil, no load).
// Tree rules run once per analyzed root (file layout, manifests).

using Mono.Cecil;

namespace GregLint.Rules;

public interface IAssemblyRule
{
    string Id { get; }
    string Title { get; }
    IReadOnlyList<Finding> Check(AssemblyContext ctx);
}

public interface ITreeRule
{
    string Id { get; }
    string Title { get; }
    IReadOnlyList<Finding> Check(LintContext ctx);
}

public sealed class AssemblyContext
{
    public string File { get; init; } = "";
    public ModuleDefinition Module { get; init; } = null!;
    public string AssemblyName { get; init; } = "";
}
