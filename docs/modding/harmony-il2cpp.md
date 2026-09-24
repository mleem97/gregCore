# Harmony-Stil + IL2CPP-Fallstricke

> Patch-Regeln der Team-Mods: kleinste Fläche, defensive Methoden, keine Kosten
> pro Frame. Alles hier steht so im Code (Backplanes, MoreModules, MoreSpools, HexViewer).

## 1. Patch-Fläche minimal halten

- `harmony.PatchAll(typeof(Patches))` (explizit) oder `HarmonyInstance.PatchAll()`.
- Pro Patch **eine** Aufgabe; schwere Arbeit in eigene Klassen auslagern
  (Backplanes: `Patches` dünn, `CatalogInjector` dick).
- Jede Patch-Methode: `try/catch` + Null-Checks, Mod darf den Game-Thread **nie**
  runterziehen. Fehlende Zielmethoden (nach Spiel-Update) schlägt Harmony still
  fehl — toten Code (`ShopCartItem.AddSpawnedItem`-Hook v1.x) aktiv entfernen.
- Prefix-Rückgabe: `true` = Original läuft, `false` = schlucken (nur mit Grund,
  z. B. Custom-IDs im `GetPrefabForItem`-Prefix mit `ref __result`).

## 2. Kein per-frame Reflection

- `Type.GetType`-Probes **einmalig cachen** (`bool? _hasCore`).
- Scans/Sweeps per Timestamp-Gate (1/s-Repair-Fenster, 2-s-Panel-Refresh,
  0,1-s-HUD-Throttle, 30-s-Rescans) statt pro Frame.
- `FindObjectsOfType` nur in Fenstern/auf Knopfdruck (Verify/Repair), nie in `OnUpdate`.

## 3. IL2CPP-Typen korrekt benutzen

- Arrays sind fix: `new Il2CppReferenceArray<ShopItem>(old.Length + 1)` kopieren
  statt Resize (Shop-, Slot-Arrays).
- `Il2CppSystem.Nullable<Color>` für optionale Farben
  (`cartItem.Initialize(shop, name, id, price, type, noCustomColor)`).
- Objekte nicht cachen und ewig halten: Il2Cpp-GC invalidiert native Pointer —
  lieber on-demand neu bauen (MoreModules-Template-Regel) bzw. Pointer-Keys
  (`RepairGuard` per nativem `IntPtr`, **nicht** per geboxtem Wrapper —
  sonst Stack-Overflow durch verfehlte Re-Entrancy-Guards).
- Inaktive Template-Halter (`SetActive(false)` + `DontDestroyOnLoad`), damit der
  `UsableObject`-Tracker Templates ignoriert; `activeInHierarchy`-Checks vor Zugriff.
- Delegates ans Spiel via `DelegateSupport.ConvertDelegate<T>`; manche Events
  existieren nur als `add_/remove_`-Methoden (z. B. Unity-Log-Feed).
- `renderer.materials` klont (Namen kriegen ` (Instance)`); geteilte Materials
  nie direkt umfärben.

## 4. Nach Spiel-Update: Kompatibilitäts-Routine

1. Nur generierte Caches löschen, Interop-Assemblies neu erzeugen lassen.
2. Einmal ohne optionale Mods starten, dann kontrolliert dazu schalten.
3. Hook-Signaturen gegen das Live-Assembly verifizieren (Felder/Methoden aus
   Abschnitt 3), tote Hooks entfernen, UserData-Config und Saves behalten.
