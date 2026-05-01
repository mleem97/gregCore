using System;
using System.Reflection;

class Program
{
    static void Main()
    {
        try
        {
            var asm = Assembly.LoadFrom(@"C:\Users\marvi\GitRepos\GameFramework-Monorepo\GregCore\lib\references\MelonLoader\Il2CppAssemblies\UnityEngine.UIElementsModule.dll");
            var t = asm.GetType("UnityEngine.UIElements.EventCallback`1");
            if (t == null)
            {
                Console.WriteLine("Type not found");
                return;
            }
            Console.WriteLine("Base: " + t.BaseType);
            Console.WriteLine("IsClass: " + t.IsClass);
            Console.WriteLine("IsValueType: " + t.IsValueType);
            Console.WriteLine("IsSealed: " + t.IsSealed);
            foreach (var ctor in t.GetConstructors())
                Console.WriteLine("Ctor: " + ctor);
            var invoke = t.GetMethod("Invoke");
            Console.WriteLine("Invoke: " + invoke);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
