# Hardware-IDs — eigenes GregCore-System (`gregID:`)

> Eigene GregCore-Implementierung. Code:
> `src/gregCore.Patches/Hardware/HardwareIdPersistencePatch.cs`,
> Kompatibilitaetswache: `src/gregCore.Patches/Hardware/IncompatibleModGuard.cs`.

## Problem

Vanilla vergibt Geraete-IDs mit Unity-`GetInstanceID`-Suffixen
(z.B. `Switch_123456`). Diese Suffixe sind pro Session anders — nach
Save/Load zeigen Kabel-Endpunkte auf tote IDs, Netzwerk-Topologien brechen.

## Design (Single-Scheme)

Es gibt genau **ein** stabiles Schema: `gregID:<Typ>:<12 HEX>`, z.B.
`gregID:Switch:A3F9C41B2E77`. Jede ID ohne `gregID:`-Praefix — leer,
vanilla-generiert oder fremd — wird exakt einmal ueberfuehrt:

- **Live-Objekte** bei `Start`/`Awake` (`GregSwitchIdAssignPatch`,
  `GregPatchPanelIdAssignPatch`, `GregServerIdAssignPatch`).
  `gameObject.name` bleibt Vanilla (Display-Trennung); Screens werden per
  Scrub-Patches (`GregServerScreenScrubPatch`, `GregSwitchScreenScrubPatch`)
  von ID-Tokens freigehalten.
- **Save-Daten** beim Laden (`GregNetworkIdHealing` auf
  `WaypointInitializationSystem.LoadNetworkState`): Legacy-IDs in
  `NetworkSaveData` (Switches, PatchPanels, Server) werden umgeschrieben,
  Kabel-Endpunkte wandern mit, danach `RequestRouteEvaluation()`.
- **Suffix-Bereinigung** (`Greg*CleanPatch` auf `GenerateUnique*`):
  Nur numerische Suffixe werden gestrippt (`Switch_123` → `Switch`);
  Nutzer-Benennung mit Buchstaben (`Core_Switch_A`) bleibt erhalten.
- **Persistenz der IDs** ueber `SaveSystem.displayToRawMap`.

Alles best-effort (Null-/Pointer-Checks, pro-Eintrag-Guards im Healing,
`HookIntegration.LogPatchError`) — das ID-System kann nie Save oder Start
reissen. Status im Log: `[gregCore][HwId]`.

## Inkompatible Fremd-Mods

Ein zweites ID-System darf niemals gleichzeitig laufen (ID-Churn,
Kabelbrueche). `IncompatibleModGuard` erkennt den alten separaten
404-PersistentID-Mod (Mod-/Assembly-Name) und entpatcht ihn per
`HarmonyInstance.UnpatchSelf()` — bei Mod-Init und erneut beim
Szenen-Laden (einmal pro Sitzung, Warnung in Log + Toast).
Danach ist `gregID` garantiert das einzige ID-System.
