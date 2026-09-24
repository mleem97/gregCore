# Eigene Shop-Items (fortgeschritten)

> Muster aus MoreSpools (Kabelrollen) und MoreModules (SFP-Module): eigene IDs,
> Prefab-Routing, Shop-Buttons, Cart-Auslieferung. Nur für Mods, die wirklich
> kaufbare Items brauchen.

## 1. Eigene ID-Ranges (Kollisionen vermeiden)

Vanilla-IDs gehören dem Spiel. Eigene Ranges **weit oben**, dokumentiert und stabil
(Saves referenzieren sie!):

- MoreSpools: `MOD_ID_BASE = 100` (Kabel, `itemType == 6` = CableSpinner).
- MoreModules: `MOD_ID_BASE = 1000`, `BULK_ID_BASE = 2000`, `TRAY_ID_BASE = 3000`
  (SFPBox Typ 9 / SFPModule Typ 8). Keine Kollision mit Backplanes (9001+).
- `IsCustomItemID(id)`-Range-Checks an jeder Abzweigung.

## 2. Prefab-Routing (`GetPrefabForItem`-Prefix)

```csharp
[HarmonyPatch(typeof(ComputerShop), nameof(ComputerShop.GetPrefabForItem))]
private static bool Prefix(int itemID, PlayerManager.ObjectInHand itemType, ref GameObject __result)
{
    if ((int)itemType != 6) return true;                    // nur eigene Typen
    if (!Core.Registry.TryGetValue(itemID, out var entry)) return true;
    __result = Core.BuildSpinnerPrefab(mgm, itemID, entry); // frischer Klon
    return false;                                          // Original schlucken
}
```

Klone aus dem Vanilla-Base-Prefab bauen (Cache `prefabID → GameObject`), Custom-Werte
(Länge, `prefabID` am `UsableObject`) setzen. Templates **inaktiv** unter eigenem
Holder parken (siehe [Harmony + IL2CPP](harmony-il2cpp.md)).

## 3. Shop-Buttons (Sektion „HL Mods")

- Template: beliebiges Vanilla-`ShopItem` klonen (`Instantiate(source.gameObject, parent)`).
- Neues `ShopItemSO` (`CreateInstance`): `itemName`, `price`, `xpToUnlock`,
  **eigene** `itemID`, `itemType`, `sprite`, `isCustomColor`-Flag, `eol` übernehmen.
- `guid` eindeutig, Texte (`txtName/txtPrice/txtXpToUnlock`) setzen, `SetActive(true)`.
- Achtung: `shopItems` ist IL2CPP-fix — per `Il2CppReferenceArray`-Kopie erweitern.
- Custom-Color-Items brauchen den Vanilla-Farbpicker-Flow (nicht umgehen).

## 4. Cart → Auslieferung

- Custom-IDs brauchen oft eigene Cart-Logik (`ButtonBuyShopItem`-Prefix): Zeile per
  `ShopCartItem.Initialize(shop, name, id, price, itemType, noCustomColor)` anlegen,
  Menge via `BuyAnotherItem`, Summe via `UpdateCartTotal`.
- Auslieferung am Checkout über `GetPrefabForItem` (eine Instanz pro Kauf, keine
  Extra-Spawns — sonst Geister-Boxen).
- **Bulk-Käufe pro Einheit zuordnen** (Vorbild Backplanes-Checkout-Snapshot):
  Beim `SpawnAll`-Beginn eine Spec **pro Einheit** in Cart-Reihenfolge ablegen
  (Quantity expandieren), Prefab-Familie gegen Cart-Drift prüfen (Preis-Peek nur
  als Korrektur, nie als Primärschlüssel). So kriegt bei 30+ Einheiten aus
  mehreren Familien zum selben Preis jede Spawn-Instanz ihre exakte Spec.
  Pending-Queues dabei groß genug dimensionieren (Backplanes: Cap 12 → 200,
  10-Minuten-Expiry) und nach Checkout verifizieren (Mismatch → Log + Notification).
- Exklusivität: Wer dieselben ID-Ranges besitzt (MoreModules vs. MoreServers vs.
  RealisticModules), weicht per `RegisteredMelons`-Check zurück (`s_disabledBySibling`,
  Fehler ins Log) — doppelte Buttons/Käufe sind schlimmer als ein inaktiver Mod.

## 5. Custom-Farben bei Mengen (stale UIDs)

Vanilla ruft `ApplyColorToSpawnedItem` pro Spawn gern mit stale UID (z. B. immer 1),
und `spawnedItems` wird nie geleert (stale Keys!). Bewährt (Backplanes):

- Im Prefix auf den **frischesten Checkout-Spawn** umleiten (eigene
  `SpawnPhysicalItem`-Postfix-Liste), nicht auf ±1-Heuristik verlassen.
- Im `SpawnAllPurchasedItems`-Postfix Zeilen-Quantity auf aufeinanderfolgende
  Spawns verteilen (Unit-Offsets = kumulierte Mengen aller Zeilen davor),
  ungefärbte per Force nachziehen. Ergebnis: 6× Custom-Rack → 6× Farbe.

## 5. Save-Kompatibilität

PrefabIDs landen in Saves. Umbenennen/Recyceln bricht alte Spielstände → IDs und
`ShopGuid`s **nie** wiederverwenden. Nachfolger-Mods halten Aliase lesbar
(RealisticModules liest MoreModules-`1000–1006`).

## 6. Shortcut: GregShopItems API (EN)

> API: `gregCore.Core.Networking.GregShopItems`. Implements steps 2–3 above
> as reusable primitives — no more per-mod registry dicts, template scans,
> or button builders.

```csharp
// Once (e.g. in SetupRegistry):
gregCore.Core.Networking.GregShopItems.RegisterPrefab(itemId, baseItemId,
    () => BuildMyPrefab(itemId));

// In the GetPrefabForItem prefix (replaces Registry dict + fallback scan):
if (gregCore.Core.Networking.GregShopItems.TryResolvePrefab(itemID, out var prefab))
{
    __result = prefab;   // inactive holder child (prefab semantics)
    return false;
}
if (gregCore.Core.Networking.GregShopItems.TryGetBaseId(itemID, out var baseId))
{
    itemID = baseId;     // remap-only path (Backplanes style)
}
return true;

// Shop buttons (replaces AddShopButton + dedup flag):
var template = gregCore.Core.Networking.GregShopItems.FindTemplate(shop, itemType);
var parent = gregCore.Core.Networking.GregShopItems.FindSection(shop, "HL Mods");
gregCore.Core.Networking.GregShopItems.AddButton(template, parent,
    itemId, label, price, xp, guid, isCustomColor, sprite); // skips existing guid
```

Rules: prefab results must be inactive holder children (never live scene
objects — orphans pollute saves, see MoreSpools v1.2.1). Item IDs and shop
guids are never reused (section 5).

### Adoption note: standalone vs hard dependency

The API above replaces vanilla code paths (prefab routing, button
creation), so — unlike additive APIs (`GregMenuBinding`, toasts,
sidecars) — it cannot sit behind a `GregHost.HasCore` guard with a
vanilla fallback without duplicating the logic. Adopting it means a
hard dependency: add `references/gregCore.dll` + csproj `Reference`
(`Private=false`), keep the DLL in `Mods/` next to yours, and fail
fast with a clear error when it is missing. Recommended for new shop
mods; existing standalone shop mods (MoreSpools, MoreModules,
MoreServers) stay on their local implementation until they opt in.
