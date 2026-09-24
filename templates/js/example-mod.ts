// example-mod.ts — minimal JS/TS script mod (UI-focused).
// Compile: tsc example-mod.ts --target es2020 --lib es2020
// Output example-mod.js goes to UserData/gregCore/Mods/JS/example-mod/.
// HotLoad: edit + save, changes apply on next main-menu entry.

/// <reference path="../greg.d.ts" />

let open = false;
let panel: any = null;

function toggle(): void {
  open = !open;
  if (open) {
    if (!panel) panel = greg.createPanel("Example");
    panel.ClearContent();
    panel.AddHeadline("Hello from JS");
    panel.AddLabel("This panel is fully script-driven.");
    panel.AddButton("Say hi", () => greg.toast("Hi from example-mod!", 3));
    panel.AddSecondaryButton("Close", toggle);
    panel.Show();
  } else if (panel) {
    panel.Hide();
  }
  greg.reportMenu("example-mod", open);
}

greg.registerToggle("enabled", "Enabled", true);
greg.registerKey("toggle", "Toggle panel", toggle);
greg.bindMenu("example-mod", toggle, () => open);
greg.toast("example-mod loaded.", 3);

function onSceneLoaded(sceneName: string): void {
  if (open) greg.reportMenu("example-mod", true);
}
