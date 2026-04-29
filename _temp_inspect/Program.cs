using System;
using System.Linq;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            var asm = Assembly.LoadFrom(@"C:\Users\marvi\GitRepos\GameFramework-Monorepo\GregCore\lib\references\MelonLoader\Il2CppAssemblies\UnityEngine.UIElementsModule.dll");
            var types = asm.GetTypes().Where(t => t.Name.Contains("EventCallback")).Take(20);
            foreach (var t in types)
            {
                Console.WriteLine(t.FullName + " | Base: " + t.BaseType + " | IsClass: " + t.IsClass + " | IsValueType: " + t.IsValueType + " | IsSealed: " + t.IsSealed);
                foreach (var ctor in t.GetConstructors())
                    Console.WriteLine("  Ctor: " + ctor);
                var invoke = t.GetMethod("Invoke");
                if (invoke != null) Console.WriteLine("  Invoke: " + invoke);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
