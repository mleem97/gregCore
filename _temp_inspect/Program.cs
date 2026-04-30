using System;
using System.Linq;
using Mono.Cecil;

class Program
{
    static void Main()
    {
        var basePath = @"C:\Users\marvi\GitRepos\GameFramework-Monorepo\GregCore\lib\references\MelonLoader\Il2CppAssemblies";
        using (var asm = AssemblyDefinition.ReadAssembly(System.IO.Path.Combine(basePath, "UnityEngine.UIElementsModule.dll")))
        {
            var ec = asm.MainModule.Types.FirstOrDefault(t => t.Name.StartsWith("EventCallback"));
            if (ec != null)
            {
                Console.WriteLine($"EventCallback: {ec.FullName}");
                foreach (var m in ec.Methods.Where(m => m.Name == "op_Implicit"))
                {
                    Console.WriteLine($"  op_Implicit RETURN: {m.ReturnType.FullName}");
                    foreach (var p in m.Parameters)
                        Console.WriteLine($"    Param: {p.ParameterType.FullName}");
                }
                foreach (var m in ec.Methods.Where(m => m.Name == "op_Explicit"))
                {
                    Console.WriteLine($"  op_Explicit RETURN: {m.ReturnType.FullName}");
                    foreach (var p in m.Parameters)
                        Console.WriteLine($"    Param: {p.ParameterType.FullName}");
                }
            }
        }
    }
}
