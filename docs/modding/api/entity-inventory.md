# EntityInventory (`GregEntityInventory`)

Audience: C#-Mod-Autoren. Namespace `gregCore.Infrastructure.Persistence`
(Assembly `gregCore.Core`, keine Zusatzreferenz nötig).

Beim Laden inventarisiert gregCore alles, was im Save existiert — Server,
Switches, Router, Firewalls, PatchPanels, Kabel, SFP-Module, LACP-Gruppen —
und versieht jeden Eintrag mit einer stabilen, für den Spieler unsichtbaren
UID. Darüber lassen sich Dinge direkt ansteuern, statt Listen zu scannen.

## Display-Trennung (Vanilla-Bezeichnung, persistente ID)

Geräte **sehen vanilla aus**, sind aber im Hintergrund persistent zugeordnet:

- `ServerID` / `switchId` / `patchPanelId` tragen die `gregID:…`
  (Save + Kabel-Referenzen + Inventar-Key — stabil über Reloads).
- `gameObject.name` bleibt Vanilla (`Server.Yellow1…`), kein Rename.
- Falls ein Screen-Text trotzdem ein `gregID:`-Token enthält, ersetzt ein
  Postfix nur das Token durch die Vanilla-Bezeichnung
  (`ScrubGregIds`); Vanilla-Texte sind No-Op.

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
bool ready = GregEntityInventory.IsReady;

// Zählen / aufzählen
int n = GregEntityInventory.Count(GregEntityInventory.InventoryKind.Server);
IReadOnlyList<GregEntityInventory.Entry> all =
    GregEntityInventory.GetAll(GregEntityInventory.InventoryKind.Switch);
// Entry: Kind, NativeKey, Uid, Hint

// Native-Key -> UID (z.B. Kind.Server, "gregID:Server:…")
if (GregEntityInventory.TryGetUid(kind, nativeKey, out string uid)) { … }

// UID -> Live-GameObject (nur Server/Switch/PatchPanel)
if (GregEntityInventory.TryFindLive(uid, out GameObject go)) { … }

// Cache-Refresh nach (Re-)Build
GregEntityInventory.Rebuilt += () => { /* neu einlesen */ };
```

`Summary()` liefert eine einzeilige Zählung fürs Log (`Server=12 Switch=3 …`).

## Kontrolle (Prefs, Dump, Verify)

MelonPreferences-Kategorie `gregCore.EntityInventory`:

| Entry | Default | Wirkung |
|---|---|---|
| `Enabled` | true | false = kein Rebuild, Inventar leer/inaktiv |
| `VerboseLogging` | false | ausführliche Inventar-Logs |
| `DumpOnRebuild` | false | volles Inventar nach jedem Rebuild loggen |

```csharp
// Vollständiges Inventar als Text (UID, NativeKey, Hint, Live-Status)
string dump = GregEntityInventory.Dump();

// Selbstprüfung: Duplikat-UIDs, leere Keys, Live-Auflösbarkeit.
// Gibt Report zurück + loggt Warnungen bei Befund.
string report = GregEntityInventory.Verify();
```
