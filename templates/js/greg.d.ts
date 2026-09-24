/**
 * greg.d.ts — Type definitions for gregCore JS script mods.
 *
 * Workflow: author in TypeScript against these types, compile with tsc,
 * drop the emitted .js into UserData/gregCore/Mods/JS/<modId>/.
 * The runtime executes .js only (raw .ts is skipped with a warning).
 *
 * Engine: Jint. Only the `greg` object below is guaranteed; direct .NET
 * access is NOT part of the contract (use the greg API).
 */

declare const greg: {
  /** Log lines (mod-tagged). */
  log(msg: string): void;
  warn(msg: string): void;
  error(msg: string): void;

  /** Transient toast (auto-expires). Duration in seconds. */
  toast(msg: string, durationSec?: number): void;
  /** Rich toast: small header, title, sub line. */
  toastRich(top: string, title: string, sub: string, durationSec?: number): void;
  /** Notification service (title + message). */
  notify(title: string, msg: string, durationSec?: number): void;

  /**
   * Create a Toolkit panel (chainable builder). Methods mirror
   * GregPanelBuilder: AddHeadline/AddLabel/AddButton/AddSecondaryButton/
   * AddToggle/AddSlider/AddSeparator/AddSpacer/ClearContent/Show/Hide/Toggle.
   */
  createPanel(title: string): any;

  /** F1-hub wiring for toggle menus (one call). */
  bindMenu(menuId: string, toggle: () => void, isOpen: () => boolean): void;
  /** Report live open state (e.g. from a hotkey path). */
  reportMenu(menuId: string, open: boolean): void;

  /** Settings + keybinds (shown in core settings UI). */
  registerToggle(settingId: string, label: string, def: boolean): void;
  registerSlider(settingId: string, label: string, def: number): void;
  registerKey(actionId: string, label: string, onPress: () => void): void;

  /** Hook bus (same names as the Lua SDK). */
  on(hookName: string, callback: (data: any) => void): void;
};

/** Optional per-mod lifecycle (called when defined). */
declare function onUpdate(dt: number): void;
declare function onSceneLoaded(sceneName: string): void;
