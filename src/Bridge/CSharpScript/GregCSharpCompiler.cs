using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using MelonLoader;

namespace gregCore.Bridge.CSharpScript;

/// <summary>
/// Reflection-based Roslyn compiler for in-memory C# mod compilation.
/// Does not introduce a compile-time dependency on Roslyn.
/// </summary>
internal static class GregCSharpCompiler
{
    private static bool _probed;
    private static bool _available;
    private static Type? _compilationType;
    private static Type? _csharpSyntaxTreeType;
    private static Type? _metadataReferenceType;

    internal static bool IsAvailable
    {
        get
        {
            if (!_probed) Probe();
            return _available;
        }
    }

    private static void Probe()
    {
        _probed = true;
        try
        {
            var csharpAsm = Assembly.Load("Microsoft.CodeAnalysis.CSharp");
            var codeAnalysisAsm = Assembly.Load("Microsoft.CodeAnalysis");

            _compilationType = csharpAsm.GetType("Microsoft.CodeAnalysis.CSharp.CSharpCompilation");
            _csharpSyntaxTreeType = csharpAsm.GetType("Microsoft.CodeAnalysis.CSharp.CSharpSyntaxTree");
            _metadataReferenceType = codeAnalysisAsm.GetType("Microsoft.CodeAnalysis.MetadataReference");

            _available = _compilationType != null
                      && _csharpSyntaxTreeType != null
                      && _metadataReferenceType != null;
        }
        catch (Exception ex)
        {
            MelonLogger.Warning($"[CSharpCompiler] Roslyn probe failed: {ex.Message}");
            _available = false;
        }
    }

    /// <summary>
    /// Compiles all <paramref name="sourceFiles"/> into a single in-memory assembly.
    /// </summary>
    internal static Assembly? Compile(string modId, string[] sourceFiles)
    {
        if (!IsAvailable)
        {
            MelonLogger.Warning($"[CSharpCompiler] Cannot compile '{modId}': Roslyn not available.");
            return null;
        }

        try
        {
            var parseText = _csharpSyntaxTreeType!.GetMethod("ParseText", new[] { typeof(string) });
            if (parseText == null)
            {
                MelonLogger.Error("[CSharpCompiler] ParseText method not found.");
                return null;
            }

            var syntaxTrees = new List<object>();
            foreach (var file in sourceFiles)
            {
                if (!File.Exists(file)) continue;
                string code = File.ReadAllText(file);
                dynamic tree = parseText.Invoke(null, new object[] { code })!;
                syntaxTrees.Add(tree);
            }

            if (syntaxTrees.Count == 0)
            {
                MelonLogger.Warning($"[CSharpCompiler:{modId}] No valid source files to compile.");
                return null;
            }

            var createFromFile = _metadataReferenceType!.GetMethod("CreateFromFile", new[] { typeof(string) });
            if (createFromFile == null)
            {
                MelonLogger.Error("[CSharpCompiler] CreateFromFile method not found.");
                return null;
            }

            var references = new List<object>();
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic || string.IsNullOrEmpty(asm.Location)) continue;
                try
                {
                    dynamic reference = createFromFile.Invoke(null, new object[] { asm.Location })!;
                    references.Add(reference);
                }
                catch { /* ignore unloadable refs */ }
            }

            dynamic compilation = ((dynamic)_compilationType!).Create(modId + "_dyn");

            foreach (var tree in syntaxTrees)
                compilation = compilation.AddSyntaxTrees(tree);

            foreach (var reference in references)
                compilation = compilation.AddReferences(reference);

            using var ms = new MemoryStream();
            dynamic emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                foreach (var diagnostic in emitResult.Diagnostics)
                {
                    string msg = diagnostic.GetMessage();
                    int severity = (int)diagnostic.Severity;
                    // Roslyn DiagnosticSeverity: Hidden=0, Info=1, Warning=2, Error=3
                    if (severity == 3)
                        MelonLogger.Error($"[CSharpCompiler:{modId}] {msg}");
                    else
                        MelonLogger.Warning($"[CSharpCompiler:{modId}] {msg}");
                }
                return null;
            }

            ms.Seek(0, SeekOrigin.Begin);
            return Assembly.Load(ms.ToArray());
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"[CSharpCompiler] Compilation failed for '{modId}': {ex.Message}");
            return null;
        }
    }
}
