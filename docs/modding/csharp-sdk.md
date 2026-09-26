# C# SDK for script mods

> Entry: `GregCoreMod.PublicAPI` (`IGregAPI`). Same power as the Lua SDK,
> for C# scripts in `UserData/gregCore/Mods/CSharp/<modId>/*.cs`
> (implement `gregCore.Bridge.CSharpScript.IGregCSharpMod`).

## API surface (via `PublicAPI`)

```csharp
var api = GregCoreMod.PublicAPI;
if (api == null) return; // core not ready

api.RegisterMod("mymod", "My Mod", "1.0.0");
api.Log("hello"); api.Warn("careful"); api.Error("broken");
api.Toast("Done.", 3f);
api.ToastRich("MYMOD", "Title", "sub line", 5f);
api.BindMenuToggle("mymod", Toggle, () => _open);
api.ReportMenu("mymod", _open);
api.RegisterShopPrefab(itemId, baseItemId, () => BuildPrefab());
api.TryResolveShopPrefab(itemId, out var prefab);
api.RegisterSaveSidecar("mymod", Serialize, Load);
api.RegisterToggle("mymod", "enabled", "Enabled", true, v => _on = v);
api.RegisterSlider("mymod", "rate", "Rate", 1f, v => _rate = v);
api.RegisterKeybind("mymod", "toggle", "Toggle", KeyCode.F8, Toggle, "Controls", "");
api.On("scene-loaded", p => { /* GregPayload.Data */ });
api.Fire("my-event", new GregPayload { HookName = "my-event" });
```

Hooks, settings, keybinds and notifications behave exactly like the Lua
SDK equivalents. Direct namespace access (`gregCore.Core.Networking.*`,
`Il2Cpp.*`, Unity) also works — scripts compile against all loaded
assemblies — but prefer the `IGregAPI` surface (stable across versions).

## HotLoad (main menu only)

C# scripts HotLoad like Lua scripts, with one hard rule: **reloads apply
in the main menu, never mid-game** (live objects would dangle).

- Edit any `*.cs` under your mod dir → change is queued (500 ms debounce).
- Entering the main menu applies it: old instance gets `OnShutdown()`,
  sources recompile, new instance gets `OnInit()`.
- Manual: `api.TryReloadScriptsNow()` returns true when applied now
  (false = not in main menu, stays queued).

Limits (by design, same as Lua): old assemblies stay loaded (no CLR
unload) — statics and Harmony patches from the previous revision
survive. Scripts must clean GameObjects/timers in `OnShutdown()`.
Scripts should not Harmony-patch; use hooks + `IGregAPI` instead.
No compiler on the machine (`Microsoft.CodeAnalysis` missing) means
no C# scripts at all — check the bridge warning in the log.
