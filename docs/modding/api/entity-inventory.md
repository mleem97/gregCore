# EntityInventory (`GregEntityInventory`)

Audience: C# mod authors. Namespace `gregCore.Infrastructure.Persistence`
(assembly `gregCore.Core`, no additional reference required).

On load, gregCore inventories everything present in the save — servers,
switches, routers, firewalls, patch panels, cables, SFP modules, LACP groups —
and assigns each entry a stable UID that is invisible to the player.
This allows addressing things directly instead of scanning lists.

## Display separation (vanilla label, persistent ID)

Devices **look vanilla** but are persistently mapped in the background:

- `ServerID` / `switchId` / `patchPanelId` carry the `gregID:…`
  (save + cable references + inventory key — stable across reloads).
- `gameObject.name` stays vanilla (`Server.Yellow1…`), no rename.
- If a screen text still contains a `gregID:` token, a postfix replaces
  only the token with the vanilla label
  (`ScrubGregIds`); vanilla texts are no-ops.

## UID rules

| Kind | UID | Stability |
|---|---|---|
| Server / Switch / PatchPanel | their `gregID:…` | stable (hardware ID persistence) |
| Cable / LACP | `gregUID:Cable:<id>` / `gregUID:Lacp:<groupId>` | deterministic from vanilla ID |
| Router / Firewall / SFP | `gregUID:<Kind>:<12hex>` | persisted via sidecar (`greg_inventory.<save>.tsv`), index/shift repair via hint |

Exception: when a pre-greg save loads for the very first time,
server/switch/patch panel get deterministic fallback UIDs; once healing +
save have completed, the `gregID`s apply permanently (one-time change,
then stable).

UIDs never appear in UI or object names (except the existing `gregID`s) —
only in the sidecar and in memory. Live resolution exists for kinds with
a readable live ID (server/switch/patch panel); cable/router/firewall/SFP/LACP
are inventoried on the save side.

## API (all calls try/catch-guarded, safe defaults)

```csharp
// Ready? (false e.g. in the main menu)
bool ready = GregEntityInventory.IsReady;

// Count / enumerate
int n = GregEntityInventory.Count(GregEntityInventory.InventoryKind.Server);
IReadOnlyList<GregEntityInventory.Entry> all =
    GregEntityInventory.GetAll(GregEntityInventory.InventoryKind.Switch);
// Entry: Kind, NativeKey, Uid, Hint

// Native key -> UID (e.g. Kind.Server, "gregID:Server:…")
if (GregEntityInventory.TryGetUid(kind, nativeKey, out string uid)) { … }

// UID -> live game object (server/switch/patch panel only)
if (GregEntityInventory.TryFindLive(uid, out GameObject go)) { … }

// Cache refresh after (re)build
GregEntityInventory.Rebuilt += () => { /* re-read */ };
```

`Summary()` returns a single-line count for the log (`Server=12 Switch=3 …`).

## Control (prefs, dump, verify)

MelonPreferences category `gregCore.EntityInventory`:

| Entry | Default | Effect |
|---|---|---|
| `Enabled` | true | false = no rebuild, inventory empty/inactive |
| `VerboseLogging` | false | verbose inventory logs |
| `DumpOnRebuild` | false | log full inventory after each rebuild |

```csharp
// Full inventory as text (UID, native key, hint, live status)
string dump = GregEntityInventory.Dump();

// Self-check: duplicate UIDs, empty keys, live resolvability.
// Returns report + logs warnings on findings.
string report = GregEntityInventory.Verify();
```
