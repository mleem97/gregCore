# gregCore Templates

This directory contains starter templates to help you build mods and plugins on top of **gregCore**.

## Available Templates

- `greg.BasedModTemplate/`: Recommended MelonLoader mod with **`ProjectReference`** to **`framework/gregCore.csproj`** and **`gregFramework.Core`** API surface.
- `StandaloneModTemplate/`: Minimal class library; add a framework reference as needed.
- `greg.PluginTemplate/`: **`MelonPlugin`** template for code that runs before mods.
- `UiTemplate/`: React/Vite UI bridged to the game via gregCore web bridge patterns.

## How to use the Framework

gregCore is loaded as a MelonLoader Plugin. This means it initializes early and provides foundational event hooks and routing for your mods.

To consume the framework in your mod:
1. Add a **`<ProjectReference>`** to **`../framework/gregCore.csproj`** (from a template under `Templates/`) — output assembly **`gregCore.dll`**.
2. Use **`gregFramework.Core`**: `GregEventDispatcher`, `GregHookName`, `GregNativeEventHooks`, etc. Subscribe to canonical **`greg.*`** strings (see **`greg_hooks.json`** and [greg hooks catalog](https://github.com/mleem97/gregWiki/blob/main/docs/reference/greg-hooks-catalog.md)).
3. (Optional) Rust/native mods use the FFI bridge in the **`framework/ModLoader/`** layer.

For a ready-to-run Sysadmin sample in C#/Lua/TS/Rust, use `Templates/mod/sysadmin/`.

## Creating a Plugin to Extend Framework Functions

If you want to add new framework-level capabilities (like a new global networking stack or a custom asset exporter) rather than a gameplay mod, you should create a Plugin:

1. Copy the `greg.PluginTemplate/` to your workspace.
2. In your main entry class, inherit from `MelonPlugin` (instead of `MelonMod`).
3. This ensures your extension runs early in the MelonLoader lifecycle, allowing it to register its own global event handlers or modify engine settings before gameplay mods execute.
4. Distribute your compiled `.dll` to users, instructing them to place it in the `Plugins/` folder (or `Mods/` if explicitly supported as a late-binding plugin).

## Using the UI Template

The `UiTemplate` uses Vite and React.
1. Navigate to `Templates/UiTemplate/react-ui`.
2. Run `npm install` and `npm run dev`.
3. The UI uses `window.gregBridge.invoke(action, payload)` to communicate with the C# layer (`DC2WebBridge.cs`).
4. To export your UI for mod distribution, run `npm run export:mod`. This will package the HTML/CSS/JS assets into a folder that can be bundled with your C# mod.