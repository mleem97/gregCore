using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Diagnostics;
using Mono.Cecil;

// Deterministic, offline scanner for a Data Center installation.
internal static class Program
{
public static int Main(string[] args)
{
var options = Args.Parse(args);
if (options.GameRoot is null) { Console.Error.WriteLine("Usage: --game-root <path> --output <directory>"); return 2; }
var root = Path.GetFullPath(options.GameRoot);
var output = Path.GetFullPath(options.Output ?? Path.Combine("coverage", "build-UNKNOWN"));
Directory.CreateDirectory(output);

var files = new[] {
    Find(root, "Assembly-CSharp.dll", "MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll"),
    Find(root, "GameAssembly.dll", "GameAssembly.dll"),
    Find(root, "global-metadata.dat", "Data/Metadata/global-metadata.dat")
};
var fingerprint = new Fingerprint(root, files);
var inventory = new List<MemberRow>();
foreach (var assemblyPath in files.Where(p => p.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) && File.Exists(p)))
{
    try { ScanAssembly(assemblyPath, inventory); }
    catch (Exception ex) { Console.Error.WriteLine($"warning: {assemblyPath}: {ex.Message}"); }
}
inventory = inventory.OrderBy(x => x.Assembly, StringComparer.Ordinal).ThenBy(x => x.Type, StringComparer.Ordinal)
    .ThenBy(x => x.Kind, StringComparer.Ordinal).ThenBy(x => x.Name, StringComparer.Ordinal).ThenBy(x => x.Signature, StringComparer.Ordinal).ToList();
var relevant = inventory.Where(x => x.ModdingRelevant).ToList();
var hooks = relevant.Where(x => x.Kind == "method").Select((x, i) => new HookRow {
    Id = "scanner." + StableId(x), Name = "gregExt.scanned." + SafeName(x.Domain) + "." + SafeName(x.Name),
    Assembly = x.Assembly, Namespace = x.Namespace, Type = x.Type, Member = x.Name, Signature = x.Signature,
    Domain = x.Domain, Risk = x.Risk, Status = "review", SupportedLanguages = new[] { "CSharp", "Lua" },
    ApprovalReason = "Discovered as modding-relevant; requires maintainer review before implementation."
}).ToList();
var excluded = inventory.Where(x => !x.ModdingRelevant).Select(x => new ExcludedRow { Assembly=x.Assembly, Type=x.Type, Member=x.Name, Reason=x.ExclusionReason }).ToList();

WriteJson(Path.Combine(output, "fingerprint.json"), fingerprint);
WriteJson(Path.Combine(output, "assembly-inventory.json"), inventory);
WriteJson(Path.Combine(output, "modding-manifest.json"), new Manifest {
    ManifestVersion=2, SchemaVersion="2.0.0", GameBuild=fingerprint.GameBuild, AssemblyFingerprint=fingerprint.CombinedSha256,
    UnityVersion=fingerprint.UnityVersion, MelonLoaderVersion=fingerprint.MelonLoaderVersion, Il2CppInteropVersion=fingerprint.Il2CppInteropVersion,
    Hooks=hooks, ExcludedMembers=excluded
});
WriteCsv(Path.Combine(output, "coverage.csv"), inventory);
WriteJson(Path.Combine(output, "coverage-diff.json"), new { added = relevant.Select(StableId).OrderBy(x=>x).ToArray(), removed = Array.Empty<string>(), changed = Array.Empty<string>() });
Console.WriteLine($"scanned {inventory.Count} members, {relevant.Count} modding-relevant; fingerprint {fingerprint.CombinedSha256}");
return 0;
}

static void ScanAssembly(string path, List<MemberRow> rows) {
    using var asm = AssemblyDefinition.ReadAssembly(path, new ReaderParameters { ReadSymbols = false });
    var name = Path.GetFileName(path);
    foreach (var type in asm.MainModule.Types.SelectMany(AllTypes).OrderBy(t=>t.FullName, StringComparer.Ordinal)) {
        foreach (var m in type.Methods.OrderBy(x=>x.Name, StringComparer.Ordinal).ThenBy(x=>x.FullName, StringComparer.Ordinal))
            rows.Add(Row(name, type, "method", m.Name, m.FullName, m.IsStatic, m.IsPublic));
        foreach (var p in type.Properties.OrderBy(x=>x.Name, StringComparer.Ordinal)) rows.Add(Row(name, type, "property", p.Name, p.FullName, false, p.GetMethod?.IsPublic == true));
        foreach (var f in type.Fields.OrderBy(x=>x.Name, StringComparer.Ordinal)) rows.Add(Row(name, type, "field", f.Name, f.FullName, f.IsStatic, f.IsPublic));
    }
}
static IEnumerable<TypeDefinition> AllTypes(TypeDefinition t) => new[] { t }.Concat(t.NestedTypes.SelectMany(AllTypes));
static MemberRow Row(string assembly, TypeDefinition type, string kind, string name, string signature, bool isStatic, bool isPublic) {
    var relevant = IsRelevant(type, name);
    return new MemberRow { Assembly=assembly, Namespace=type.Namespace, Type=type.FullName, Kind=kind, Name=name, Signature=signature,
        Static=isStatic, Visibility=isPublic ? "public" : "non-public", Domain=Domain(type.FullName), ModdingRelevant=relevant,
        Risk=kind == "method" && name is "Update" or "LateUpdate" or "FixedUpdate" ? "high" : relevant ? "medium" : "low",
        ExclusionReason=relevant ? "" : "Unity/third-party internals or compiler-generated member" };
}
static bool IsRelevant(TypeDefinition t, string member) => !t.FullName.Contains("UnityEngine", StringComparison.OrdinalIgnoreCase)
    && !t.FullName.Contains("System.", StringComparison.OrdinalIgnoreCase) && !member.StartsWith("<", StringComparison.Ordinal)
    && (t.Namespace.StartsWith("Il2Cpp", StringComparison.OrdinalIgnoreCase) || t.Namespace.StartsWith("DataCenter", StringComparison.OrdinalIgnoreCase));
static string Domain(string type) { var s=type.ToLowerInvariant(); return s.Contains("player") ? "Player" : s.Contains("network") || s.Contains("server") ? "Network" : s.Contains("save") ? "Save" : s.Contains("ui") ? "UI" : s.Contains("shop") || s.Contains("coin") ? "Economy" : "Gameplay"; }
static string StableId(MemberRow x) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(string.Join("|", x.Assembly,x.Type,x.Kind,x.Name,x.Signature)))).ToLowerInvariant()[..16];
static string SafeName(string s) => new string(s.Where(char.IsLetterOrDigit).ToArray()).ToLowerInvariant() is { Length: > 0 } v ? v : "unknown";
static string Find(string root, string file, string preferred) { var p=Path.Combine(root, preferred); if(File.Exists(p)) return p; return Directory.Exists(root) ? Directory.GetFiles(root,file,SearchOption.AllDirectories).OrderBy(x=>x,StringComparer.Ordinal).FirstOrDefault() ?? p : p; }
static void WriteJson<T>(string path,T value) => File.WriteAllText(path, JsonSerializer.Serialize(value, new JsonSerializerOptions { WriteIndented=true }), new UTF8Encoding(false));
static void WriteCsv(string path,List<MemberRow> rows) { using var w=new StreamWriter(path,false,new UTF8Encoding(false)); w.WriteLine("assembly,type,kind,name,signature,static,visibility,domain,moddingRelevant,risk,exclusionReason"); foreach(var x in rows) w.WriteLine(string.Join(",", new[]{x.Assembly,x.Type,x.Kind,x.Name,x.Signature,x.Static.ToString().ToLowerInvariant(),x.Visibility,x.Domain,x.ModdingRelevant.ToString().ToLowerInvariant(),x.Risk,x.ExclusionReason}.Select(Csv))); static string Csv(string x)=>"\""+x.Replace("\"","\"\"")+"\""; }

record Args(string? GameRoot,string? Output) { public static Args Parse(string[] a) => new(a.SkipWhile(x=>x!="--game-root").Skip(1).FirstOrDefault(), a.SkipWhile(x=>x!="--output").Skip(1).FirstOrDefault()); }
record Fingerprint { public string GameBuild{get;init;}="UNKNOWN"; public string UnityVersion{get;init;}="UNKNOWN"; public string MelonLoaderVersion{get;init;}="UNKNOWN"; public string Il2CppInteropVersion{get;init;}="UNKNOWN"; public string AssemblyCSharpSha256{get;init;}=""; public string GameAssemblySha256{get;init;}=""; public string MetadataSha256{get;init;}=""; public string CombinedSha256{get;init;}=""; public Fingerprint(string root,string[] files) { GameBuild=ReadVersion(root); UnityVersion=ProductVersion(root,"UnityPlayer.dll"); MelonLoaderVersion=AssemblyVersion(root,"MelonLoader.dll"); Il2CppInteropVersion=AssemblyVersion(root,"Il2CppInterop.Runtime.dll"); AssemblyCSharpSha256=Hash(files[0]); GameAssemblySha256=Hash(files[1]); MetadataSha256=Hash(files[2]); CombinedSha256=HashText(string.Join("\n",GameBuild,AssemblyCSharpSha256,GameAssemblySha256,MetadataSha256,UnityVersion,MelonLoaderVersion,Il2CppInteropVersion)); } }
record MemberRow { public string Assembly{get;init;}=""; public string Namespace{get;init;}=""; public string Type{get;init;}=""; public string Kind{get;init;}=""; public string Name{get;init;}=""; public string Signature{get;init;}=""; public bool Static{get;init;} public string Visibility{get;init;}=""; public string Domain{get;init;}=""; public bool ModdingRelevant{get;init;} public string Risk{get;init;}=""; public string ExclusionReason{get;init;}=""; }
record HookRow { public string Id{get;init;}=""; public string Name{get;init;}=""; public string Assembly{get;init;}=""; public string Namespace{get;init;}=""; public string Type{get;init;}=""; public string Member{get;init;}=""; public string Signature{get;init;}=""; public string Domain{get;init;}=""; public string Risk{get;init;}=""; public string Status{get;init;}=""; public string[] SupportedLanguages{get;init;}=Array.Empty<string>(); public string ApprovalReason{get;init;}=""; }
record ExcludedRow { public string Assembly{get;init;}=""; public string Type{get;init;}=""; public string Member{get;init;}=""; public string Reason{get;init;}=""; }
record Manifest { public int ManifestVersion{get;init;} public string SchemaVersion{get;init;}=""; public string GameBuild{get;init;}=""; public string AssemblyFingerprint{get;init;}=""; public string UnityVersion{get;init;}=""; public string MelonLoaderVersion{get;init;}=""; public string Il2CppInteropVersion{get;init;}=""; public List<HookRow> Hooks{get;init;}=new(); public List<ExcludedRow> ExcludedMembers{get;init;}=new(); }
static string Hash(string path) => File.Exists(path) ? Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant() : "";
static string HashText(string text) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();
static string ReadVersion(string root) => File.Exists(Path.Combine(root,"version.txt")) ? File.ReadAllText(Path.Combine(root,"version.txt")).Trim() : "UNKNOWN";
static string ProductVersion(string root,string file) { var p=Find(root,file,file); return File.Exists(p) ? FileVersionInfo.GetVersionInfo(p).ProductVersion ?? "UNKNOWN" : "UNKNOWN"; }
static string AssemblyVersion(string root,string file) { var p=Find(root,file,file); try { return File.Exists(p) ? System.Reflection.AssemblyName.GetAssemblyName(p).Version?.ToString() ?? "UNKNOWN" : "UNKNOWN"; } catch { return "UNKNOWN"; } }
}
