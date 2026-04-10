# Layout: `gregCore/framework/`

| Path | Inhalt |
|------|--------|
| **`FrikaMF.csproj`** | Ein MelonLoader-Plugin-Projekt; Ausgabe **`FrikaModdingFramework.dll`**. |
| **`Main.cs`**, **`Main.CI.cs`** | Plugin-Einstieg (MelonMod). |
| **`Sdk/`** | Öffentliche **gregFramework.Core**-API (`GregEventDispatcher`, `GregHookName`, `GregDomain`, `GregPayload`, `GregCompatBridge`) für Mods. |
| **`ModLoader/`** | FFI, `EventDispatcher`, `HarmonyPatches`, Game-/Plugin-Services (`DataCenterModLoader`, `FrikaMF.*`). |
| **`harmony/`** | Optional: per Generator erzeugte Domain-Harmony-Klassen (`Generate-GregHooksFromIl2CppDump.ps1`). |

`greg_hooks.json` wird bei Build aus dem **Monorepo-Root** (`../../greg_hooks.json`) neben die DLL kopiert, sofern die Datei existiert.
