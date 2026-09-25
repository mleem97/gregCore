using Mono.Cecil;

// Member collection for GameApiGenerator (partial, behavior-preserving).
static partial class GameApiEmitter
{
    // Finds a singleton member named instance/Instance of own type.
    public static string? FindSingletonMember(TypeDefinition t)
    {
        try
        {
            var p = FindSingletonProperty(t);
            if (p != null) return p;
            return FindSingletonField(t);
        }
        catch { return null; }
    }

    // Searches static properties for a singleton.
    private static string? FindSingletonProperty(TypeDefinition t)
    {
        try
        {
            foreach (var p in t.Properties)
            {
                try
                {
                    if (IsSingletonProperty(p, t.FullName)) return p.Name;
                }
                catch { }
            }
        }
        catch { }
        return null;
    }

    // Checks one property for singleton shape.
    private static bool IsSingletonProperty(PropertyDefinition p, string fullName)
    {
        try
        {
            if (!(p.GetMethod?.IsPublic == true && p.GetMethod.IsStatic)) return false;
            if (!(p.Name == "instance" || p.Name == "Instance")) return false;
            return p.PropertyType.FullName == fullName;
        }
        catch { return false; }
    }

    // Searches static fields for a singleton.
    private static string? FindSingletonField(TypeDefinition t)
    {
        try
        {
            foreach (var f in t.Fields)
            {
                try
                {
                    if (IsSingletonField(f, t.FullName)) return f.Name;
                }
                catch { }
            }
        }
        catch { }
        return null;
    }

    // Checks one field for singleton shape.
    private static bool IsSingletonField(FieldDefinition f, string fullName)
    {
        try
        {
            if (!(f.IsPublic && f.IsStatic)) return false;
            if (!(f.Name == "instance" || f.Name == "Instance")) return false;
            return f.FieldType.FullName == fullName;
        }
        catch { return false; }
    }

    // Collects public method names with static flags.
    public static List<(string Name, bool Static)> CollectMethods(TypeDefinition t)
    {
        var methods = new List<(string Name, bool Static)>();
        try
        {
            if (t.IsEnum) return methods;
            CollectMethodsCore(t, methods);
            methods.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
        }
        catch { }
        return methods;
    }

    // Core method enumeration.
    private static void CollectMethodsCore(TypeDefinition t, List<(string Name, bool Static)> methods)
    {
        try
        {
            var seen = new HashSet<string>(StringComparer.Ordinal);
            foreach (var m in t.Methods)
            {
                try
                {
                    if (IsSkippedMethod(m)) continue;
                    if (seen.Add(m.Name)) methods.Add((m.Name, m.IsStatic));
                }
                catch { }
            }
        }
        catch { }
    }

    // Filters constructors, special and compiler-generated methods.
    private static bool IsSkippedMethod(MethodDefinition m)
    {
        try
        {
            if (!m.IsPublic) return true;
            if (m.IsConstructor) return true;
            if (m.IsSpecialName) return true;
            return m.Name.StartsWith("<", StringComparison.Ordinal);
        }
        catch { return true; }
    }

    // Collects public property names.
    public static List<string> CollectProps(TypeDefinition t)
    {
        var props = new List<string>();
        try
        {
            foreach (var p in t.Properties)
            {
                try
                {
                    if (p.Name.StartsWith("<", StringComparison.Ordinal)) continue;
                    if (p.GetMethod?.IsPublic == true || p.SetMethod?.IsPublic == true) props.Add(p.Name);
                }
                catch { }
            }
        }
        catch { }
        return props;
    }

    // Collects public field names.
    public static List<string> CollectFields(TypeDefinition t)
    {
        var fields = new List<string>();
        try
        {
            foreach (var f in t.Fields)
            {
                try
                {
                    if (!f.IsPublic) continue;
                    if (f.IsSpecialName) continue;
                    if (f.Name.StartsWith("<", StringComparison.Ordinal)) continue;
                    fields.Add(f.Name);
                }
                catch { }
            }
        }
        catch { }
        return fields;
    }

    // Collects nested type paths.
    public static List<string> CollectNested(TypeDefinition t)
    {
        var nested = new List<string>();
        try
        {
            CollectNestedCore(t, nested);
            nested.Sort(StringComparer.OrdinalIgnoreCase);
        }
        catch { }
        return nested;
    }

    // Walks nested types with an explicit stack.
    private static void CollectNestedCore(TypeDefinition t, List<string> nested)
    {
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
        catch { }
    }

    // Collects enum value names.
    public static List<(string Name, string Value)> CollectEnumValues(TypeDefinition t)
    {
        var vals = new List<(string Name, string Value)>();
        try
        {
            if (!t.IsEnum) return vals;
            foreach (var f in t.Fields)
            {
                try
                {
                    if (!f.IsPublic) continue;
                    if (f.IsSpecialName) continue;
                    if (f.Name == "value__") continue;
                    vals.Add((f.Name, f.Constant?.ToString() ?? "?"));
                }
                catch { }
            }
        }
        catch { }
        return vals;
    }

    // Holds all collected per-type data.
    internal sealed record TypeData(
        TypeDefinition Def,
        TypeNames Names,
        string Kind,
        string? SingletonMember,
        List<(string Name, bool Static)> Methods,
        List<string> Props,
        List<string> Fields,
        List<string> Nested,
        List<(string Name, string Value)> EnumValues);

    // Collects every member list for one type.
    public static TypeData CollectAll(TypeDefinition t)
    {
        var names = GetTypeNames(t);
        var kind = GetKind(t);
        return new TypeData(
            t,
            names,
            kind,
            FindSingletonMember(t),
            CollectMethods(t),
            CollectProps(t),
            CollectFields(t),
            CollectNested(t),
            CollectEnumValues(t));
    }
}
