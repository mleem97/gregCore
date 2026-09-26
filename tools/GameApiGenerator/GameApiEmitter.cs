using Mono.Cecil;

// Shared emission logic for GameApiGenerator (keeps Program.cs small for lizard).
// Behavior-preserving: pure code motion from Program.cs top-level statements.
static partial class GameApiEmitter
{
    // Sanitizes identifiers and escapes C# keywords.
    public static string Sanitize(string name, HashSet<string> kw)
    {
        try
        {
            return SanitizeCore(name, kw);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return "_"; }
    }

    // Core sanitizing without outer guard.
    private static string SanitizeCore(string name, HashSet<string> kw)
    {
        var sb = new System.Text.StringBuilder(name.Length);
        foreach (var c in name)
            sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
        return FinishSanitized(sb.ToString(), kw);
    }

    // Applies empty, digit-prefix and keyword rules.
    private static string FinishSanitized(string s, HashSet<string> kw)
    {
        try
        {
            if (s.Length == 0) s = "_";
            if (char.IsDigit(s[0])) s = "_" + s;
            if (kw.Contains(s)) s = "@" + s;
            return s;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return "_"; }
    }

    // Checks whether a type derives from Unity component bases.
    public static bool IsComponent(TypeDefinition t)
    {
        try { return WalkBases(t.BaseType); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Walks base types up to a fixed depth.
    private static bool WalkBases(TypeReference? start)
    {
        try
        {
            var b = start;
            var depth = 0;
            while (b != null && depth++ < 12)
            {
                var n = b.FullName;
                if (IsComponentName(n)) return true;
                if (IsTerminalName(n)) return false;
                if (!TryAdvanceBase(ref b)) return false;
            }
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  }
        return false;
    }

    // Matches Unity component base names.
    private static bool IsComponentName(string n)
    {
        try
        {
            if (n == "UnityEngine.MonoBehaviour") return true;
            if (n == "UnityEngine.Component") return true;
            return IsBehaviourKind(n);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Matches behaviour and scriptable object bases.
    private static bool IsBehaviourKind(string n)
    {
        try
        {
            if (n == "UnityEngine.Behaviour") return true;
            return n == "UnityEngine.ScriptableObject";
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Matches hierarchy roots that stop the walk.
    private static bool IsTerminalName(string n)
    {
        try
        {
            if (n == "System.Object") return true;
            if (n == "System.ValueType") return true;
            return n == "System.Enum";
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Advances to the resolved base type.
    private static bool TryAdvanceBase(ref TypeReference? b)
    {
        try
        {
            var cur = b;
            if (cur == null) return false;
            b = cur.Resolve()?.BaseType;
            return true;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Holds per-type naming info.
    internal sealed record TypeNames(string Ns, string ShortName, bool IsGeneric, string Arity);

    // Extracts namespace, short name and generic arity.
    public static TypeNames GetTypeNames(TypeDefinition t)
    {
        try
        {
            var ns = string.IsNullOrEmpty(t.Namespace) ? "Global" : t.Namespace;
            var shortName = t.Name.Contains('`') ? t.Name[..t.Name.IndexOf('`')] : t.Name;
            var isGeneric = t.Name.Contains('`') || t.HasGenericParameters;
            return new TypeNames(ns, shortName, isGeneric, GetArity(t.Name));
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return new TypeNames("Global", t.Name, false, ""); }
    }

    // Extracts the `N generic arity suffix.
    private static string GetArity(string typeName)
    {
        try
        {
            if (!typeName.Contains('`')) return "";
            var a = typeName[(typeName.IndexOf('`') + 1)..];
            if (a.All(char.IsDigit)) return "_" + a;
            return "";
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return ""; }
    }

    // Classifies the Cecil type kind.
    public static string GetKind(TypeDefinition t)
    {
        try
        {
            if (t.IsEnum) return "Enum";
            if (t.IsInterface) return "Interface";
            return GetKindCore(t);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return "Class"; }
    }

    // Classifies non-enum, non-interface kinds.
    private static string GetKindCore(TypeDefinition t)
    {
        try
        {
            if (t.IsValueType) return "Struct";
            if (t.BaseType?.FullName == "System.MulticastDelegate") return "Delegate";
            if (IsComponent(t)) return "Component";
            return "Class";
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return "Class"; }
    }

    // Checks for a static class shape.
    public static bool IsStaticClass(TypeDefinition t)
    {
        try { return IsStaticShape(t.IsAbstract, t.IsSealed, t.IsEnum, t.IsValueType, t.IsInterface); }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Evaluates static-class flags.
    private static bool IsStaticShape(bool abs, bool sealed_, bool isEnum, bool isValue, bool isIface)
    {
        try
        {
            if (!abs) return false;
            if (!sealed_) return false;
            return IsNonStaticExclusion(isEnum, isValue, isIface);
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }

    // Excludes enum, struct and interface shapes.
    private static bool IsNonStaticExclusion(bool isEnum, bool isValue, bool isIface)
    {
        try
        {
            if (isEnum) return false;
            if (isValue) return false;
            return !isIface;
        }
        catch { /* ignored: defensive best-effort (CONVENTIONS.md) */  return false; }
    }
}
