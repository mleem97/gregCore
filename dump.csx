using System;
using System.Reflection;

try {
    Assembly asm = Assembly.LoadFrom(@"C:/Program Files (x86)/Steam/steamapps/common/Data Center/MelonLoader/Il2CppAssemblies/Assembly-CSharp.dll");
    foreach (Type t in asm.GetTypes()) {
        string name = t.Name.ToLower();
        if (name.Contains("rack") || name.Contains("save") || name.Contains("grid") || name.Contains("floor") || name.Contains("place")) {
            Console.WriteLine(t.FullName);
        }
    }
} catch (Exception e) {
    Console.WriteLine(e);
}
