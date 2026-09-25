// example-mod.ts — minimal JS/TS script mod (UI-focused).
// Compile: tsc example-mod.ts --target es2020 --lib es2020
// Output example-mod.js goes to UserData/gregCore/Mods/JS/example-mod/.
// HotLoad: edit + save, changes apply on next main-menu entry.

// (Removed in favor of tsconfig "include" in real projects; kept here so the single file compiles standalone.)
// eslint-disable-next-line @typescript-eslint/no-triple-slash-reference -- greg.d.ts is a global script (no modules in Jint); triple-slash is the correct way to reference it.
/// <reference path="../greg.d.ts" />

let open = false;

interface ExamplePanel {
  ClearContent(): void;
  AddHeadline(text: string): void;
  AddLabel(text: string): void;
  AddButton(label: string, onClick: () => void): void;
  AddSecondaryButton(label: string, onClick: () => void): void;
  Show(): void;
  Hide(): void;
}

let panel: ExamplePanel | null = null;

function toggle(): void {
  open = !open;
  if (open) {
    if (panel === null) panel = greg.createPanel("Example") as ExamplePanel;
    panel.ClearContent();
    panel.AddHeadline("Hello from JS");
    panel.AddLabel("This panel is fully script-driven.");
    panel.AddButton("Say hi", () => { greg.toast("Hi from example-mod!", 3); });
    panel.AddSecondaryButton("Close", toggle);
    panel.Show();
  } else if (panel !== null) {
    panel.Hide();
  }
  greg.reportMenu("example-mod", open);
}

greg.registerToggle("enabled", "Enabled", true);
greg.registerKey("toggle", "Toggle panel", toggle);
greg.bindMenu("example-mod", toggle, () => open);
greg.toast("example-mod loaded.", 3);

// Lifecycle entry point, invoked by the greg host by name (see js-sdk.md).
onSceneLoaded = (sceneName: string): void => {
  if (open) greg.reportMenu("example-mod", true);
};
