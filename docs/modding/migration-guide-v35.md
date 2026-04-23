# Modding Migration Guide: gregCore v1.0.0.35

## The Unity 6 IL2CPP Problem
In Unity 6 (Data Center version), the **IMGUI** system (`GUI.Window`, `GUILayout`) is heavily stripped during the IL2CPP build process. Using managed delegates (like `Action<int>`) for UI windows often fails with:
`[Il2CppInterop] Exception in IL2CPP-to-Managed trampoline: System.NotSupportedException: Method unstripping failed`.

This causes infinite log spam and crashes the rendering bridge.

## The Solution: GregUI (UGUI Framework)
Starting with version 1.0.0.35, `gregCore` provides a safe, native UGUI backend that bypasses the broken IMGUI trampolines.

### 1. Legacy Fixes (Automatic)
- **`greg.Sdk.gregEventDispatcher`**: Restored for compatibility with older mods.
- **`gregNativeEventHooks`**: Stabilized signatures for better IL2CPP linking.

### 2. Migrating to GregUI (Manual Action Required)
Mods using `OnGUI()` or `GUI.Window()` should migrate to the **GregUI Builder API**.

#### Old Code (Prone to Crashes):
```csharp
void OnGUI() {
    GUI.Window(id, rect, DrawWindow, "Title");
}
void DrawWindow(int id) {
    if (GUILayout.Button("Click Me")) { /* logic */ }
}
```

#### New Safe Code (v1.0.0.35+):
```csharp
using gregCore.PublicApi;

public void BuildUI() {
    greg.UI.CreateBuilder("Economy Hud")
        .SetSize(400, 300)
        .AddLabel("Current Funds: $1500")
        .AddButton("Add Money", () => {
            // Your logic here - safely wrapped for IL2CPP
        })
        .AddButton("Close", () => {
            gregCore.UI.GregUIManager.SetPanelActive("Economy Hud", false);
        })
        .Build();
}

// Toggle visibility via hotkey
if (Input.GetKeyDown(KeyCode.F9)) {
    gregCore.UI.GregUIManager.TogglePanel("Economy Hud");
}
```

## Important: Clear Cache
After updating `gregCore.dll`, you **MUST** delete the following folder to force metadata regeneration:
`[GamePath]/MelonLoader/Il2CppAssemblies/`

Failure to do so will cause `TypeLoadExceptions` even if the code is correct.
