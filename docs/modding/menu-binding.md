# F1 menu binding (one call)

> API: `gregCore.UI.GregMenuBinding` (`src/gregCore.UI/GregMenuBinding.cs`).

Replaces the hand-rolled opener + closer + `SetOpen` block (~10 lines)
with one call. Behavior is identical: opener toggles and reports the live
state, closer hides (toggles only if open) and reports closed.

```csharp
// Own isolated method (JIT split — only runs behind GregHost.HasCore):
private void RegisterCoreExtras()
{
    gregCore.UI.GregMenuBinding.BindToggle("mymod",
        MyOverlay.Toggle, () => MyOverlay.IsVisible);
}
```

Hotkey toggle paths keep reporting via one line:

```csharp
gregCore.UI.GregMenuBinding.Report("mymod", MyOverlay.IsVisible);
```

Rules: call only when `GregHost.HasCore` is true, from an isolated method
(gregCore types must never load without the DLL). No unregister needed —
bindings live for the session. Copy-paste probe: `templates/csharp/GregHost.cs`.
