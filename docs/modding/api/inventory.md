# Save-Inventar (`GregSaveInventory`)

Audience: C#-Mod-Autoren. Namespace `gregCore.Infrastructure.Persistence`
(Assembly `gregCore.Core`, keine Zusatzreferenz nötig).

Beim Laden inventarisiert gregCore alles, was im Save existiert — Server,
Switches, Router, Firewalls, PatchPanels, Kabel, SFP-Module, LACP-Gruppen —
und versieht jeden Eintrag mit einer stabilen, für den Spieler unsichtbaren
UID. Darüber lassen sich Dinge direkt ansteuern, statt Listen zu scannen.

## UID-Regeln

| Kind | UID | Stabilität |
|---|---|---|
| Server / Switch / PatchPanel | ihre `gregID:…` | stabil (HardwareId-Persistence) |
| Kabel / LACP | `gregUID:Cable:<id>` / `gregUID:Lacp:<groupId>` | deterministisch aus Vanilla-ID |
| Router / Firewall / SFP | `gregUID:<Kind>:<12hex>` | per Sidecar persistiert (`greg_inventory.<save>.tsv`), Index+Shift-reparatur via Hint |

Ausnahme: Lädt ein Pre-greg-Save zum allerersten Mal, bekommen
Server/Switch/PatchPanel deterministische Fallback-UIDs; sobald Healing +
Save durch sind, gelten dauerhaft die `gregID`s (einmaliger Wechsel,
danach stabil).

UIDs stehen nie in UI oder Objektnamen (außer den bestehenden `gregID`s) —
nur im Sidecar und im Speicher. Live-Auflösung gibt es für Kinds mit
lesbarer Live-ID (Server/Switch/PatchPanel); Kabel/Router/Firewall/SFP/LACP
sind save-seitig inventarisiert.

## API (alle Aufrufe try/catch-gesichert, Safe-Defaults)

```csharp
// Bereit? (false z.B. im Hauptmenü)
bool ready = GregSaveInventory.IsReady;

// Zählen / aufzählen
int n = GregSaveInventory.Count(GregSaveInventory.InventoryKind.Server);
IReadOnlyList<GregSaveInventory.Entry> all =
    GregSaveInventory.GetAll(GregSaveInventory.InventoryKind.Switch);
// Entry: Kind, NativeKey, Uid, Hint

// Native-Key -> UID (z.B. Kind.Server, "gregID:Server:…")
if (GregSaveInventory.TryGetUid(kind, nativeKey, out string uid)) { … }

// UID -> Live-GameObject (nur Server/Switch/PatchPanel)
if (GregSaveInventory.TryFindLive(uid, out GameObject go)) { … }

// Cache-Refresh nach (Re-)Build
GregSaveInventory.Rebuilt += () => { /* neu einlesen */ };
```

`Summary()` liefert eine einzeilige Zählung fürs Log (`Server=12 Switch=3 …`).
