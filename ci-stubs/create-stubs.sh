#!/bin/bash
# Erzeugt leere Stub-DLLs für CI-Build ohne echtes MelonLoader
# Diese enthalten nur die nötigen Type-Definitionen

mkdir -p ci-stubs
cd ci-stubs

ASSEMBLIES=(
    "MelonLoader"
    "Il2CppInterop.Runtime"
    "Il2CppInterop.Common"
    "0Harmony"
    "Assembly-CSharp"
    "Il2Cppmscorlib"
    "UnityEngine.CoreModule"
    "UnityEngine.IMGUIModule"
    "UnityEngine.UIModule"
    "UnityEngine.InputLegacyModule"
)

for asm in "${ASSEMBLIES[@]}"; do
    if [ ! -f "$asm.dll" ]; then
        echo "Creating stub for $asm..."
        dotnet new classlib -n "$asm" -f net6.0 --force
        dotnet build "$asm/$asm.csproj" -c Release -o .
        rm -rf "$asm"
    fi
done

echo "Stubs created in $(pwd)"
