# GregCore events

Audience: all supported mod languages.

Canonical names use `gregMod.<domain>.<event>`. `greg.*` names are legacy compatibility aliases and are deprecated. Hook payloads are dictionaries with the fields declared in `framework/greg_hooks.json`; callbacks run on the documented thread.

Subscriptions must be disposed during unload. C# mods can use the disposable returned by `context.Events.On(...)` or the protected `On(...)` helper on `GregMod`. A callback exception is logged and isolated from other subscribers.

The committed manifest is the source of truth for IDs, payload fields, cancellation, and threading. An empty or unknown manifest entry is not an API guarantee.
