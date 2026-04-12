# Layout: `gregCore/framework/`

| Path | Inhalt |
|------|--------|
| **`gregCore.csproj`** | Ein MelonLoader-Plugin-Projekt; Ausgabe **`gregCore.dll`**. |
| **`Main.cs`**, **`Main.CI.cs`** | Plugin-Einstieg (MelonMod). |
| **`Sdk/`** | Öffentliche **gregFramework.Core**-API (`GregEventDispatcher`, `GregHookName`, `GregDomain`, `GregPayload`, `GregCompatBridge`, **`GregNativeEventHooks`**) für Mods. |
| **`ModLoader/`** | FFI, `EventDispatcher`, `HarmonyPatches`, Game-/Plugin-Services (`DataCenterModLoader`, `gregCore.*`) und vollständiges MainMenu-UI-Replace (`DC2WebBridge`, `ModSettingsMenuBridge`). |
| **`harmony/`** | Optional: per Generator erzeugte Domain-Harmony-Klassen (`Generate-GregHooksFromIl2CppDump.ps1`). |

`greg_hooks.json` wird bei Build aus dem **Monorepo-Root** (`../../greg_hooks.json`) neben die DLL kopiert, sofern die Datei existiert.
