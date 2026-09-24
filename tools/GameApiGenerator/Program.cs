using Mono.Cecil;

// Generates one gregCore GameApi module per top-level game type, alphabetical.
// Usage: gameapi-gen <Assembly-CSharp.dll> <refs-dir> <out-dir>
var asmPath = args[0];
var refsDir = args[1];
var outDir = args[2];
var genDir = Path.Combine(outDir, "Generated");

var resolver = new DefaultAssemblyResolver();
resolver.AddSearchDirectory(Path.GetDirectoryName(asmPath)!);
if (Directory.Exists(refsDir)) resolver.AddSearchDirectory(refsDir);
var asm = AssemblyDefinition.ReadAssembly(asmPath, new ReaderParameters { AssemblyResolver = resolver });

var keywords = new HashSet<string>(StringComparer.Ordinal)
{
    "abstract","as","base","bool","break","byte","case","catch","char","checked","class","const",
    "continue","decimal","default","delegate","do","double","else","enum","event","explicit",
    "extern","false","finally","fixed","float","for","foreach","goto","if","implicit","in","int",
    "interface","internal","is","lock","long","namespace","new","null","object","operator","out",
    "override","params","private","protected","public","readonly","ref","return","sbyte","sealed",
    "short","sizeof","stackalloc","static","string","struct","switch","this","throw","true","try",
    "typeof","uint","ulong","unchecked","unsafe","ushort","using","virtual","void","volatile","while"
};

static string Sanitize(string name, HashSet<string> kw)
{
    var sb = new System.Text.StringBuilder(name.Length);
    foreach (var c in name)
        sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
    var s = sb.ToString();
    if (s.Length == 0) s = "_";
    if (char.IsDigit(s[0])) s = "_" + s;
    if (kw.Contains(s)) s = "@" + s;
    return s;
}

bool IsComponent(TypeDefinition t)
{
    try
    {
        var b = t.BaseType;
        var depth = 0;
        while (b != null && depth++ < 12)
        {
            var n = b.FullName;
            if (n == "UnityEngine.MonoBehaviour" || n == "UnityEngine.Component" ||
                n == "UnityEngine.Behaviour" || n == "UnityEngine.ScriptableObject")
                return true;
            if (n == "System.Object" || n == "System.ValueType" || n == "System.Enum") return false;
            try { b = b.Resolve()?.BaseType; } catch { return false; }
        }
    }
    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    return false;
}

var types = asm.MainModule.Types
    .Where(t => t.Name != "<Module>")
    .OrderBy(t => t.FullName, StringComparer.OrdinalIgnoreCase)
    .ToList();

var ambiguousTypeRefs = new HashSet<string>(StringComparer.Ordinal)
{
    "Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry",
    "Il2Cpp._PrivateImplementationDetails_",
    "Il2Cpp.UnitySourceGeneratedAssemblyMonoScriptTypes_v1",
};

var sourceSha = "";
var registry = new List<string>();
var generated = 0;

foreach (var t in types)
{
    var ns = string.IsNullOrEmpty(t.Namespace) ? "Global" : t.Namespace;
    var shortName = t.Name.Contains('`') ? t.Name[..t.Name.IndexOf('`')] : t.Name;
    var isGeneric = t.Name.Contains('`') || t.HasGenericParameters;
    var arity = "";
    if (t.Name.Contains('`'))
    {
        var a = t.Name[(t.Name.IndexOf('`') + 1)..];
        if (a.All(char.IsDigit)) arity = "_" + a;
    }

    string kind;
    if (t.IsEnum) kind = "Enum";
    else if (t.IsInterface) kind = "Interface";
    else if (t.IsValueType) kind = "Struct";
    else if (t.BaseType?.FullName == "System.MulticastDelegate") kind = "Delegate";
    else if (IsComponent(t)) kind = "Component";
    else kind = "Class";

    var isStaticClass = t.IsAbstract && t.IsSealed && !t.IsEnum && !t.IsValueType && !t.IsInterface;

    // singleton: static property/field named instance|Instance of own type
    string? singletonMember = null;
    try
    {
        foreach (var p in t.Properties)
        {
            if ((p.GetMethod?.IsPublic == true && p.GetMethod.IsStatic) &&
                (p.Name == "instance" || p.Name == "Instance") &&
                p.PropertyType.FullName == t.FullName) { singletonMember = p.Name; break; }
        }
        if (singletonMember == null)
        {
            foreach (var f in t.Fields)
            {
                if (f.IsPublic && f.IsStatic &&
                    (f.Name == "instance" || f.Name == "Instance") &&
                    f.FieldType.FullName == t.FullName) { singletonMember = f.Name; break; }
            }
        }
    }
    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

    var methods = new List<(string Name, bool Static)>();
    var seenMethods = new HashSet<string>(StringComparer.Ordinal);
    if (!t.IsEnum)
    {
        try
        {
            foreach (var m in t.Methods)
            {
                if (!m.IsPublic || m.IsConstructor || m.IsSpecialName) continue;
                if (m.Name.StartsWith("<", StringComparison.Ordinal)) continue;
                if (seenMethods.Add(m.Name)) methods.Add((m.Name, m.IsStatic));
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
        methods.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
    }

    var props = new List<string>();
    try
    {
        foreach (var p in t.Properties)
        {
            if (p.Name.StartsWith("<", StringComparison.Ordinal)) continue;
            if (p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true) props.Add(p.Name);
        }
    }
    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

    var fields = new List<string>();
    try
    {
        foreach (var f in t.Fields)
        {
            if (!f.IsPublic || f.IsSpecialName) continue;
            if (f.Name.StartsWith("<", StringComparison.Ordinal)) continue;
            fields.Add(f.Name);
        }
    }
    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }

    var nested = new List<string>();
    try
    {
        var stack = new Stack<(TypeDefinition Def, string Prefix)>();
        foreach (var n in t.NestedTypes) stack.Push((n, ""));
        while (stack.Count > 0)
        {
            var (def, prefix) = stack.Pop();
            var full = string.IsNullOrEmpty(prefix) ? def.Name : prefix + "/" + def.Name;
            nested.Add(full);
            foreach (var child in def.NestedTypes) stack.Push((child, full));
        }
    }
    catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    nested.Sort(StringComparer.OrdinalIgnoreCase);

    var enumValues = new List<(string Name, string Value)>();
    if (t.IsEnum)
    {
        try
        {
            foreach (var f in t.Fields)
            {
                if (!f.IsPublic || f.IsSpecialName || f.Name == "value__") continue;
                enumValues.Add((f.Name, f.Constant?.ToString() ?? "?"));
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */ }
    }

    var moduleName = Sanitize(shortName, keywords) + arity + "Module";
    var nsPath = ns.Replace('.', '/');
    var dir = Path.Combine(genDir, nsPath);
    Directory.CreateDirectory(dir);
    var file = Path.Combine(dir, Sanitize(shortName, keywords) + arity + ".g.cs");

    var sb = new System.Text.StringBuilder();
    sb.AppendLine("// <auto-generated>GameApiGenerator: ilspycmd/Cecil-Dump von Assembly-CSharp. Nicht von Hand editieren.</auto-generated>");
    sb.AppendLine("using System;");
    sb.AppendLine("using HarmonyLib;");
    sb.AppendLine("using gregCore.Core.Abstractions;");
    sb.AppendLine("using gregCore.Core.Events;");
    sb.AppendLine();
    sb.AppendLine($"namespace gregCore.GameApi.{ns};");
    sb.AppendLine();
    sb.AppendLine($"/// <summary>Modder-Modul für den Spiel-Typ <c>{t.FullName}</c> ({kind}).</summary>");
    sb.AppendLine($"public static class {moduleName}");
    sb.AppendLine("{");
    sb.AppendLine($"    public const string GameTypeName = \"{t.FullName}\";");
    sb.AppendLine($"    public const string GameNamespace = \"{t.Namespace}\";");
    sb.AppendLine($"    public const string Kind = \"{kind}\";");
    sb.AppendLine($"    public const bool IsSingleton = {(singletonMember != null ? "true" : "false")};");
    sb.AppendLine();

    var typeRef = $"global::{t.FullName.Replace('/', '+').Replace('+', '.')}";
    // nested separator in C# is '.', FullName uses '/'
    typeRef = "global::" + t.FullName.Replace('/', '.');
    var canRef = !isGeneric && kind is "Class" or "Component" && !t.Name.StartsWith("_", StringComparison.Ordinal) && !ambiguousTypeRefs.Contains(t.FullName);

    if (singletonMember != null && canRef)
    {
        sb.AppendLine("    public static " + typeRef + "? TryGetSingleton()");
        sb.AppendLine("    {");
        sb.AppendLine("        try");
        sb.AppendLine("        {");
        sb.AppendLine($"            var inst = {typeRef}.{singletonMember};");
        sb.AppendLine("            if (inst == null || inst.Pointer == System.IntPtr.Zero) return null;");
        sb.AppendLine("            return inst;");
        sb.AppendLine("        }");
        sb.AppendLine("        catch { return null; }");
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    if (kind == "Component" && canRef)
    {
        sb.AppendLine("    public static " + typeRef + "? TryFindFirst() =>");
        sb.AppendLine($"        GregGameModuleHost.FindFirst<{typeRef}>();");
        sb.AppendLine();
    }

    if (methods.Count > 0)
    {
        sb.AppendLine("    public static class Methods");
        sb.AppendLine("    {");
        foreach (var (name, _) in methods)
            sb.AppendLine($"        public const string {Sanitize(name, keywords)} = \"{name}\";");
        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static class Events");
        sb.AppendLine("    {");
        foreach (var (name, _) in methods)
            sb.AppendLine($"        public const string {Sanitize(name, keywords)} = \"greg.game.{t.FullName}.{name}\";");
        sb.AppendLine("    }");
        sb.AppendLine();
        if (canRef)
        {
            sb.AppendLine("    public static bool Hook(global::HarmonyLib.Harmony harmony, GregEventBus eventBus, IGregLogger logger, string methodName) =>");
            sb.AppendLine($"        GregGameModuleHost.HookMethod(harmony, typeof({typeRef}), methodName, eventBus, logger);");
            sb.AppendLine();
        }
    }

    if (props.Count > 0)
    {
        sb.AppendLine("    public static class Properties");
        sb.AppendLine("    {");
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var p in props)
        {
            var id = Sanitize(p, keywords);
            if (!seen.Add(id)) continue;
            sb.AppendLine($"        public const string {id} = \"{p}\";");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    if (fields.Count > 0)
    {
        sb.AppendLine("    public static class Fields");
        sb.AppendLine("    {");
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var f in fields)
        {
            var id = Sanitize(f, keywords);
            if (!seen.Add(id)) continue;
            sb.AppendLine($"        public const string {id} = \"{f}\";");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    if (nested.Count > 0)
    {
        sb.AppendLine("    public static class NestedTypes");
        sb.AppendLine("    {");
        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (var i = 0; i < nested.Count; i++)
        {
            var id = Sanitize(nested[i].Replace('/', '_'), keywords);
            if (string.IsNullOrEmpty(id)) id = $"Nested{i}";
            var baseId = id; var n = 1;
            while (!seen.Add(id)) id = $"{baseId}_{n++}";
            sb.AppendLine($"        public const string {id} = \"{nested[i]}\";");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    if (enumValues.Count > 0)
    {
        sb.AppendLine("    public static class Values");
        sb.AppendLine("    {");
        var seen = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (name, value) in enumValues)
        {
            var id = Sanitize(name, keywords);
            if (!seen.Add(id)) continue;
            sb.AppendLine($"        public const string {id} = \"{name}\"; // = {value}");
        }
        sb.AppendLine("    }");
        sb.AppendLine();
    }

    sb.AppendLine("}");
    File.WriteAllText(file, sb.ToString());
    generated++;

    registry.Add($"        new GameApiModuleDescriptor {{ GameTypeName = \"{t.FullName}\", ModuleTypeName = \"gregCore.GameApi.{ns}.{moduleName}\", Kind = \"{kind}\", IsSingleton = {(singletonMember != null ? "true" : "false")}, IsGeneric = {(isGeneric ? "true" : "false")}, MethodCount = {methods.Count} }},");
}

using (var stream = File.OpenRead(asmPath))
{
    var hash = System.Security.Cryptography.SHA256.HashData(stream);
    sourceSha = Convert.ToHexString(hash).ToLowerInvariant();
}

var regPath = Path.Combine(genDir, "GregGameApiRegistry.g.cs");
var reg = new System.Text.StringBuilder();
reg.AppendLine("// <auto-generated>GameApiGenerator Registry. Nicht von Hand editieren.</auto-generated>");
reg.AppendLine("using System;");
reg.AppendLine("using System.Collections.Generic;");
reg.AppendLine();
reg.AppendLine("namespace gregCore.GameApi;");
reg.AppendLine();
reg.AppendLine("public sealed record GameApiModuleDescriptor");
reg.AppendLine("{");
reg.AppendLine("    public string GameTypeName { get; init; } = string.Empty;");
reg.AppendLine("    public string ModuleTypeName { get; init; } = string.Empty;");
reg.AppendLine("    public string Kind { get; init; } = string.Empty;");
reg.AppendLine("    public bool IsSingleton { get; init; }");
reg.AppendLine("    public bool IsGeneric { get; init; }");
reg.AppendLine("    public int MethodCount { get; init; }");
reg.AppendLine("}");
reg.AppendLine();
reg.AppendLine("public static class GregGameApiRegistry");
reg.AppendLine("{");
reg.AppendLine("    public const string GeneratorVersion = \"1.1.0\";");
reg.AppendLine($"    public const string SourceAssembly = \"Assembly-CSharp.dll\";");
reg.AppendLine($"    public const string SourceSha256 = \"{sourceSha}\";");
reg.AppendLine($"    public const string GeneratedUtc = \"{DateTime.UtcNow:yyyy-MM-ddTHH:mm:ssZ}\";");
reg.AppendLine($"    public const int ModuleCount = {registry.Count};");
reg.AppendLine("    public static readonly IReadOnlyList<GameApiModuleDescriptor> Modules = new List<GameApiModuleDescriptor>");
reg.AppendLine("    {");
foreach (var line in registry) reg.AppendLine(line);
reg.AppendLine("    }.AsReadOnly();");
reg.AppendLine();
reg.AppendLine("    public static GameApiModuleDescriptor? TryGetModule(string gameTypeName)");
reg.AppendLine("    {");
reg.AppendLine("        if (string.IsNullOrWhiteSpace(gameTypeName)) return null;");
reg.AppendLine("        foreach (var m in Modules)");
reg.AppendLine("            if (string.Equals(m.GameTypeName, gameTypeName, StringComparison.Ordinal)) return m;");
reg.AppendLine("        return null;");
reg.AppendLine("    }");
reg.AppendLine("}");
File.WriteAllText(regPath, reg.ToString());

