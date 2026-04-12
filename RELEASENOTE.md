:rotating_light: **Major Update: gregCore v1.0.0.7 is LIVE! (gregUI Framework & HexViewer Integration)**

Hey everyone,

We've just reached another massive milestone! Today we are dropping **gregCore v1.0.0.7**, which introduces the brand-new **gregUI Framework** and deep integration with the **HexViewer** for advanced developer tooling.

---

:wrench: **What's new in v1.0.0.7?**

This update focuses on UI extensibility and modularity.

:paintbrush: **gregUI Framework** — A complete UI manipulation layer for UGUI.
    *   **GregUIBuilder**: Fluent API for modders to create panels and components with 5 lines of code.
    *   **Luminescent Architect Design System**: Standardized tokens for colors, spacing, and glow effects.
    *   **Live UI Replacements**: Users can now override any game UI element (position, scale, color) via `ui_overrides.json`.
:joystick: **Advanced Hooking** — New `greg.UI.*` hook category. Subscribe to MainMenu, PauseMenu, and HUD events.
:mag: **Developer Tooling** — HexViewer integration with F1 (UI Tree), F2 (Hook Monitor), and F3 (Element Inspector).

---

:package: **Installation**

1.  **Requirement**: Ensure you have [MelonLoader v0.6.1](https://github.com/LavaGang/MelonLoader/releases) installed for *Data Center*.
2.  **Files**: 
    *   Copy `gregCore.dll` to your game's `Mods/` folder.
    *   Copy `Jint.dll` and `Esprima.dll` to your game's `MelonLoader/Libs/` folder.
    *   Copy `greg_hooks.json` to your game's root directory (next to the `.exe`).
3.  **Launch**: Start the game. You should see "gregCore v1.0.0.7 initialized" in the console.

---

:point_right: **[Download gregCore v1.0.0.7 on GitHub](https://github.com/mleem97/gregCore/releases/tag/v1.0.0.7)**

Thank you for being part of this journey! — Your **teamGreg**
