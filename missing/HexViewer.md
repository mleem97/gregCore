# HexViewer — Framework Gap Analysis

> Date: 2026-04-30  
> Plugin: gregMod.HexViewer v2.0.0  
> Framework: gregCore  

---

## 1. Real-Time Config Sync (F8 Menu)

**Feature-Name:** Hot-Reloadable MelonPreferences → UI Toolkit bindings

**Problembeschreibung:**  
HexViewer nutzt `MelonPreferences` für Toggle, Mode und Anchor. Wenn der Spieler Werte im F8-Menü ändert, werden sie zwar auf Disk persistiert, aber das HUD aktualisiert seine Position/Visibilität erst beim nächsten `OnUpdate`-Zyklus oder nach Scene-Reload. Es gibt kein Framework-seitiges Event wie `OnPreferenceChanged(string category, string key)`.

**Warum wird es benötigt?**  
Accessibility-Features müssen sofort reagieren. Ein Spieler, der im Menü den Mode von "Developer" auf "Standard" wechselt, sollte nicht 5 Sekunden warten, bis das HUD aktualisiert.

**Vorgeschlagene Framework-Lösung:**  
1. `GregConfigService.Subscribe(string category, string key, Action<T> onChanged)`  
2. Intern polling von `MelonPreferences.HasCategoryChanged()` oder Reflection-Tracking der Entry-Objekte.  
3. Alternativ: `GregEventBus.Publish("greg.config.Changed", payload)` bei jedem F8-Save.

---

## 2. Sub-Element Recognition (Parent-Child Relationships)

**Feature-Name:** Hierarchical Hardware Query API

**Problembeschreibung:**  
Ein `Rack` GameObject enthält mehrere `Server`- oder `NetworkSwitch`-Children. Aktuell muss HexViewer `GetComponent<Il2Cpp.Rack>()` auf dem getroffenen Collider aufrufen. Wenn der Spieler auf einen einzelnen Server-Einschub zielt, der ein Child des Racks ist, erkennt das Raycast nur den Collider des Racks (je nach Mesh-Aufbau) oder den Server. Es gibt keine Framework-API, die sagt: "Dieser Server gehört zu Rack #3".

**Warum wird es benötigt?**  
Die Spec fordert "Unified Objects": Racks sollen als Einheit dargestellt werden, Einzelschübe nur bei primärem Fokus. Ohne Parent-Child-API kann HexViewer nicht unterscheiden, ob der Spieler das Rack als Ganzes oder einen einzelnen Slot anvisiert.

**Vorgeschlagene Framework-Lösung:**  
1. `GregHardwareQuery.GetParentRack(GameObject child) -> Il2Cpp.Rack?`  
2. `GregHardwareQuery.GetSlots(Il2Cpp.Rack rack) -> Il2Cpp.Server[]`  
3. `GregHardwareQuery.GetSlotIndex(Il2Cpp.Server server) -> int`  
4. Intern: Harmony-Patch auf `Rack.Awake()` oder `Rack.Start()`, das eine Dictionary `serverInstanceId -> rackRef` füllt.

---

## 3. Localization Layer (Code-Name → Display Name)

**Feature-Name:** `GregLocalization.Resolve(string internalName) -> string`

**Problembeschreibung:**  
`GameObject.name` liefert interne Unity-Namen wie "Server_Rack_01_Prototype(Clone)" oder "CableSpinner_Blue_3m". Die Spec verbietet explizit die Anzeige von Code-Variablen. Aktuell muss HexViewer `name` direkt anzeigen oder manuelle Mappings pflegen.

**Warum wird es benötigt?**  
Barrierefreiheit: Spieler sollen lesbare Namen wie "19\" Server Rack" oder "3 m Fiber Cable (Blue)" sehen, keine internen Identifikatoren.

**Vorgeschlagene Framework-Lösung:**  
1. JSON-basierte Locale-Dateien unter `UserData/gregCore/Locales/{lang}.json`  
2. `GregLocalization.Resolve(name)` sucht nach Regex-Patterns oder exakten Matches.  
3. Fallback-Kette: Locale → `GregDisplayNameAttribute` (per Reflection auf IL2CPP-Typen) → `name`  
4. Expose via `GregAPI.GetDisplayName(GameObject go)`.

---

## 4. CableSpinner Hex-Color API

**Feature-Name:** `Il2Cpp.CableSpinner.GetHexColor() -> string`

**Problembeschreibung:**  
CableSpinner-Objekte haben visuelle Farben (Kabelrollen in verschiedenen Farben). Aktuell muss HexViewer über `MeshRenderer.material.color` reverse-engineern, was instabil ist (Material-Instanzen, LOD, Batchings). Es gibt keine garantierte API, die den logischen Farb-Code des Kabels liefert.

**Warum wird es benötigt?**  
Die Spec fordert: "Identifiziere bei Kabelrollen und Racks den Hex-Farbwert". Wenn der Farbwert nur aus dem Mesh-Material gelesen wird, kann er bei Lighting-Changes oder Asset-Updates falsch sein.

**Vorgeschlagene Framework-Lösung:**  
1. Harmony-Patch auf `CableSpinner.Start()` oder `CableSpinner.SetColor()`  
2. Speichere `hexColor` in einem `Dictionary<int, string>` (InstanceID → Hex).  
3. Expose via `GregCableAPI.GetHexColor(Il2Cpp.CableSpinner spinner)`.

---

## 5. Cable / Switch Port Type & Medium API

**Feature-Name:** `GregCableAPI.GetPortType(GameObject go) -> (string type, string medium)`

**Problembeschreibung:**  
Switches und Kabel haben physische Port-Typen (RJ45, SFP, QSFP) und Medien (COPPER, FIBER). Aktuell muss HexViewer heuristisch aus `name` oder `ports.Count` ableiten. Dies ist fehleranfällig und bricht bei neuen DLC-Hardwaretypen.

**Warum wird es benötigt?**  
Die Spec fordert: "Identifiziere bei Kabeln und Switch-Inserts den Typ (RJ, SFP, QSFP) und das Medium (Markierung FIBER, falls zutreffend)". Heuristiken sind nicht zukunftssicher.

**Vorgeschlagene Framework-Lösung:**  
1. Cpp2IL-Analyse der `Cable`-, `CablePort`- und `NetworkSwitch`-Klassen.  
2. Expose der echten Felder (z.B. `Cable.portType`, `Cable.medium`, `NetworkSwitch.fiberPorts`) via `GregCableAPI`.  
3. Falls Felder intern sind: Harmony-Patch + Runtime-Cache.

---

## 6. IL2CPP-Safe Component Enumeration

**Feature-Name:** `GregInterop.GetComponentsSafe<T>(GameObject go) -> T[]`

**Problembeschreibung:**  
`go.GetComponents<Component>()` auf IL2CPP-Objekten kann `ObjectCollectedException` werfen, wenn das native Objekt zwischenzeitlich vom GC freigegeben wurde. Der aktuelle HexViewer-Inspector nutzt `GetComponents<Component>()` im try-catch, aber das ist langsam und unzuverlässig.

**Warum wird es benötigt?**  
Stabilität. Ein Accessibility-Plugin darf unter keinen Umständen das Spiel crashen.

**Vorgeschlagene Framework-Lösung:**  
1. Extension-Method `GameObject.GetComponentsSafe<T>()`  
2. Intern: Prüfe `go.Pointer != IntPtr.Zero && go != null` vor jedem Zugriff.  
3. Nutze `Il2CppType.Of<T>()` statt `typeof(T)` für IL2CPP-Generics.  
4. Cache `MethodInfo` für `GetComponents` via `AccessTools`.

---

## Zusammenfassung

| # | Feature | Kritikalität | Workaround im Plugin |
|---|---------|-------------|----------------------|
| 1 | Config Sync | Medium | Polling in `OnUpdate` |
| 2 | Sub-Element Recognition | Hoch | Raycast nur auf getroffenes GO, kein Rack-Grouping |
| 3 | Localization | Hoch | Zeigt `go.name` direkt an |
| 4 | CableSpinner Hex API | Medium | `MeshRenderer.material.color` Heuristik |
| 5 | Port Type / Medium API | Hoch | `name.Contains()` + `ports.Count` Heuristik |
| 6 | Safe Component Enum | Mittel | try-catch um `GetComponents()` |

---

**Anmerkung:**  
Sämtliche Heuristiken im HexViewer sind mit `try-catch` abgesichert und loggen Warnungen. Sie dienen als Übergangslösung bis die oben genannten Framework-Features implementiert sind.
