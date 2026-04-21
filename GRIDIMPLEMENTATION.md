## GREGCORE — GREG.GRID PLACEMENT SYSTEM + GREG.SAVE ENGINE — MASTER-PROMPT
> Version: 0.1.0 | Ziel-Modul: greg.GridPlacement + greg.SaveEngine

ROLLE
Du bist Lead Framework-Architekt, IL2CPP-Reverse-Engineer und Senior
Technical Writer für gregCore — dem MelonLoader-basierten Modding-Stack
für "Data Center" (Waseku, Steam, Unity 6000.4.2f1, Il2CPP, .NET 6, x64).
Du arbeitest direkt gegen die IL2CPP-Assemblies aus dem Workspace
(gregReferences / Assembly-CSharp.dll) und gegen das bestehende
gregCore-Framework (GregEventDispatcher, GregPayload, GregNativeEventHooks,
GregCompatBridge, GregLanguageRegistry, GregHarmonyService, GregUIManager).

ZIEL DIESES PROMPTS
Implementiere zwei neue gregCore-Subsysteme vollständig:
  1. greg.GridPlacement  — ersetzt das RackHolder-Plate-System durch
                           ein Sims-artiges Grid-Placement-System
  2. greg.SaveEngine     — ersetzt das Vanilla-Save-System durch eine
                           eigenständige, hochperformante Datenbank
Beide Subsysteme werden über das F8-Menü aktiviert, konfiguriert und
gesteuert. Alle Mod-Einstellungen im Framework laufen künftig über F8.

═══════════════════════════════════════════════════════════
SCHRITT 0 — PRE-ANALYSIS (PFLICHT VOR JEDER IMPLEMENTIERUNG)
═══════════════════════════════════════════════════════════

LIES in dieser Reihenfolge — niemals überspringen:
  1. Assembly-CSharp.dll (via IL2CPP-Workspace / gregReferences)
  2. Suche nach allen Typen, die enthalten:
       RackHolder, RackPlate, RackPlaceholder, RackBase,
       FloorTile, FloorGrid, GridManager, PlacementManager,
       SaveManager, SaveData, GameSave, SerializationManager,
       NetworkSaveData, SwitchSaveData, ServerSaveData
  3. Für jeden gefundenen Typ dokumentiere:
       - Exakter Klassenname + Namespace
       - Parent-Klasse (MonoBehaviour, Il2CppObjectBase, ...)
       - Key-Fields (Positionen, IDs, Größen, State-Flags)
       - Key-Methods (Place, Remove, Load, Save, Serialize, Init)
       - Bekannte IL2CPP-Interop-Probleme aus dem Projektverlauf
  4. Bestimme:
       - Exakte Größe einer FloorTile in Unity-World-Units (Vector3)
       - Exakte Größe eines Racks in Unity-World-Units
       - Wie RackHolder-Plates aktuell instanziiert werden (Prefab? Code?)
       - Welches Koordinatensystem das Spiel nutzt (Y-up, Z-forward?)
       - Wo SaveManager.SaveGame() / LoadGame() aufgerufen wird
       - Ob Save-Dateien binary oder JSON sind (aus bisheriger Analyse:
         Binary IL2CPP-serialized — bestätige und dokumentiere)

OUTPUT von Schritt 0:
  PRE-ANALYSIS REPORT:
    - FloorTile World-Size: [X]u × [Z]u
    - Rack World-Size: [X]u × [Z]u × [Y]u
    - Grid-Cell Conclusion: 1 Cell = Rack-Footprint = [X]u × [Z]u
    - Sub-Grid: 4 Sub-Cells pro Grid-Cell (2×2 Unterteilung)
    - RackHolder-Placement-Methode: [gefundene Methode]
    - Save-Format: Binary IL2CPP [bestätigt / abweichend]
    - Vanilla-Kompatibilität: NICHT angestrebt (by design)

═══════════════════════════════════════════════════════════
TEIL A — GREG.GRIDPLACEMENT SYSTEM
═══════════════════════════════════════════════════════════

KONZEPT
Das Vanilla-System platziert Racks via vorab platzierten "RackHolder"-
Floor-Plates. Dieses System wird durch ein dynamisches Grid-Placement
ersetzt — vergleichbar mit The Sims, Planet Coaster oder ähnlichen
Bausimulationen:
  - Der Raumboden ist ein unsichtbares Grid
  - 1 Grid-Cell = 1 Rack-Footprint
  - Jede Grid-Cell ist intern in 4 Sub-Cells (2×2) unterteilt für
    präzises Snapping und spätere Erweiterungen (Kabeltrassen, Licht)
  - Racks werden direkt per Drag/Click auf Grid-Cells platziert
  - Kein Vorab-Platzieren von Plates mehr nötig

A-1: CORE CLASSES (Namespace: greg.GridPlacement)

  GregGridManager
    Verwaltet das gesamte Grid im Speicher.
    Felder:
      Dictionary<Vector2Int, GregGridCell> cells
      float cellSizeX, cellSizeZ  // aus Pre-Analysis ermittelt
      Vector3 gridOrigin          // Weltkoordinaten-Ursprung des Grids
    Methoden:
      void Initialize(Vector3 origin, int width, int depth)
      GregGridCell GetCell(Vector2Int coord)
      GregGridCell GetCellAtWorldPos(Vector3 worldPos)
      bool IsCellOccupied(Vector2Int coord)
      bool PlaceRack(Vector2Int coord, GregPlaceableRack rack)
      bool RemoveRack(Vector2Int coord)
      Vector3 SnapToGrid(Vector3 worldPos)
      void DrawDebugGrid()  // OnGUI-basiert, nur im Debug-Modus

  GregGridCell
    Repräsentiert eine einzelne Grid-Zelle.
    Felder:
      Vector2Int coord
      bool isOccupied
      GregPlaceableRack? occupant
      GregSubCell[4] subCells  // 2×2 Sub-Grid
      bool isBlocked           // Wand, Hindernis etc.

  GregSubCell
    Felder:
      Vector2Int subCoord  // 0–3
      bool isOccupied
      string occupantType  // "cable", "light", "reserved", null

  GregPlaceableRack
    Wraps ein bestehendes IL2CPP-Rack-Objekt.
    Felder:
      string rackId
      Vector2Int gridCoord
      GameObject? unityGameObject
      Il2CppObjectBase? vanillaRackRef  // Referenz auf Vanilla-Objekt
    Methoden:
      void PlaceAt(Vector2Int coord, Vector3 worldPos)
      void Remove()
      void Highlight(bool active)

  GregPlacementController
    Steuert den Build-Mode (Eingabe, Preview, Snapping).
    Felder:
      bool buildModeActive
      GregPlaceableRack? previewRack
      GregGridManager grid
    Methoden:
      void ActivateBuildMode()
      void DeactivateBuildMode()
      void OnUpdate()   // Raycast + Snapping + Preview
      void OnGUI()      // Cursor-Overlay, Zell-Highlight
      void TryPlace(Vector3 worldPos)
      void TryRemove(Vector3 worldPos)

A-2: HARMONY-PATCHES (via GregHarmonyService)

  PATCH 1 — RackHolder-Spawn unterdrücken
    Ziel:    [ExaktKlassennameAusAnalyse].PlaceRackHolder (o.ä.)
    Typ:     Prefix → return false wenn GridPlacement aktiv
    Zweck:   Vanilla-RackHolder-Plates werden nicht mehr gespawnt.
    Flag:    FeatureFlags.GRID_PLACEMENT_ACTIVE

  PATCH 2 — Rack-Placement umleiten
    Ziel:    [ExaktKlassennameAusAnalyse] — Methode die Rack an Position setzt
    Typ:     Prefix → umleiten zu GregPlacementController.TryPlace()
    Zweck:   Alle Rack-Placements laufen durch das Grid-System.

  PATCH 3 — Rack-Removal umleiten
    Ziel:    [ExaktKlassennameAusAnalyse] — Methode die Rack entfernt
    Typ:     Postfix → GregGridManager.RemoveRack() aufrufen

  WENN ZIELKLASSE NICHT GEFUNDEN:
    → Erstelle MISSING.md mit:
        Welche Klasse wird gesucht, warum, vorgeschlagene Signatur,
        Workaround bis Klasse identifiziert ist.
    → Implementiere GregPlacementController als standalone (kein Patch)
      mit eigenem Raycast + GameObject-Instantiate für Preview-Mesh.

A-3: GRID VISUAL (OnGUI + Unity)

  Grid-Darstellung (nur wenn Build-Mode aktiv):
    - Unsichtbares Grid wird als dünnes Linien-Overlay gerendert
    - Aktive Zelle: Highlight in GregUITheme.Primary (61F4D8) mit 40% Alpha
    - Belegte Zelle: Highlight in GregUITheme.Error (ED4245) mit 30% Alpha
    - Freie Zelle: kein Overlay (Grid-Linien in GregUITheme.GhostBorder)
    - Sub-Grid-Linien: sichtbar bei Zoom-In > [konfigurierbarer Threshold]
    - Preview-Rack: halbtransparentes Ghost-Mesh auf Snap-Position

  Rendering:
    - Grid-Linien: GL.Lines in OnGUI (nicht OnRenderObject — IL2CPP-safe)
    - Alternativ: Plane-Mesh mit transparentem Material wenn GL nicht
      verfügbar → MISSING.md mit Grund

A-4: NEUE HOOKS (in GregNativeEventHooks registrieren)

  public const string WorldRackPlaced   = "greg.WORLD.RackPlaced";
  public const string WorldRackRemoved  = "greg.WORLD.RackRemoved";
  public const string WorldRackMoved    = "greg.WORLD.RackMoved";
  public const string WorldGridReady    = "greg.WORLD.GridReady";
  public const string WorldBuildMode    = "greg.WORLD.BuildModeToggled";

  Payload-Felder (GregPayload):
    RackPlaced/Removed/Moved:
      "rackId"    → string
      "gridCoord" → string (z.B. "4,7")
      "worldPos"  → string (z.B. "12.5,0,8.0")
      "modId"     → string (aufrufender Mod)

═══════════════════════════════════════════════════════════
TEIL B — GREG.SAVEENGINE
═══════════════════════════════════════════════════════════

KONZEPT
Das Vanilla-Save-System ist Binary-IL2CPP-serialized und nicht
modder-freundlich (aus bisheriger Analyse bestätigt: portVlanFilters
nur Runtime-only, kein direkter Save-Zugriff möglich).
greg.SaveEngine ersetzt es durch eine eigenständige, embedded Datenbank:

  Datenbank: LiteDB 5.x (embedded, serverless, document-oriented)
  Grund: LiteDB ist eine BSON-basierte embedded NoSQL-DB für .NET —
         schneller als SQLite für Document-Reads weil kein Schema-Overhead,
         kein Server, kein Connection-Pool. Alles in einer einzigen .db-Datei.
         NuGet: LiteDB, Version 5.0.21
  WICHTIG: LiteDB wird via ILRepack in gregCore.dll eingebettet —
           keine externe DLL, keine UserLibs-Abhängigkeit.
  Vanilla-Kompatibilität: NICHT vorhanden — by design.

B-1: VANILLA SAVE DETECTION (PFLICHT)

  Beim Spielstart und bei jedem SaveManager.LoadGame():
    → Prüfe ob der geladene Spielstand ein Vanilla-Save ist.
    → Erkennungskriterien (alle aus Pre-Analysis zu bestätigen):
        a) Kein greg.SaveEngine-Header in der Save-Datei
        b) Bekannte Vanilla-Binary-Signatur (Magic Bytes / Header-Pattern)
        c) Dateiname-Pattern falls bekannt
    → Wenn Vanilla-Save erkannt:
        [gregCore] ⚠ Vanilla save detected — greg.GridPlacement DISABLED
        [gregCore] ⚠ Vanilla save detected — greg.SaveEngine in READ-ONLY mode
        → GregFeatureGuard.DisableFeature("GridPlacement")
        → GregFeatureGuard.DisableFeature("SaveEngine.Write")
        → F8-Menü zeigt Banner: "Vanilla Save — Modded features disabled"
        → Alle Game-Breaking-Funktionen (Rack-Placement-Override,
          SaveManager-Patch) werden NICHT aktiviert.
    → Wenn greg.SaveEngine-Save erkannt:
        → Normal-Modus: alle Features aktiv.

  GregFeatureGuard (neue Klasse in frameworkSdk/):
    public static class GregFeatureGuard
      public static void DisableFeature(string featureKey)
      public static void EnableFeature(string featureKey)
      public static bool IsEnabled(string featureKey)
      public static bool IsVanillaSave { get; private set; }
      public static event Action<string> OnFeatureStateChanged

  Modder-API (für Wiki dokumentieren):
    Modder-Code kann prüfen:
      if (GregFeatureGuard.IsVanillaSave) { ... }
      if (!GregFeatureGuard.IsEnabled("GridPlacement")) { ... }
    Event subscriben:
      GregFeatureGuard.OnFeatureStateChanged += (key) => { ... }

B-2: LITEDB SCHEMA

  Collection: "grid_state"
    {
      _id: ObjectId,
      sessionId: string (GUID),
      savedAt: DateTime,
      gregSaveVersion: string ("1.0.0"),
      gridWidth: int,
      gridDepth: int,
      gridOrigin: { x: float, y: float, z: float },
      cellSizeX: float,
      cellSizeZ: float,
      placedRacks: [
        {
          rackId: string,
          gridCoord: { x: int, z: int },
          worldPos: { x: float, y: float, z: float },
          rackType: string,
          label: string,
          placedAt: DateTime
        }
      ]
    }

  Collection: "server_state"
    { serverId, rackId, serverType, isOn, isBroken, eolTime,
      customerId, appId, label, gregSavedAt }

  Collection: "network_state"
    { switchId, switchType, rackId, isOn, isBroken, eolTime,
      portVlanFilters: [ { portIndex, vlanId, mode } ],
      gregSavedAt }

  Collection: "cable_state"
    { cableId, fromPort, toPort, cableType, color, length, gregSavedAt }

  Collection: "greg_meta"
    { key: "header", value: "greg.SaveEngine.v1",
      gameVersion: string, gregCoreVersion: string,
      createdAt: DateTime, lastSavedAt: DateTime,
      isVanillaSave: false }

B-3: SAVEENGINE KLASSEN

  GregSaveEngine (Namespace: greg.SaveEngine)
    Felder:
      LiteDatabase db
      string dbPath  // GameDir/Saves/gregSave_{sessionId}.greg.db
    Methoden:
      void Initialize(string saveDir)
      void SaveAll()             // Kompletter State-Dump
      void LoadAll()             // Kompletter State-Restore
      void SaveGridState(GregGridManager grid)
      void LoadGridState(GregGridManager grid)
      void SaveServerState(IEnumerable<ServerData> servers)
      void LoadServerState()
      bool IsGregSave(string filePath)  // Header-Prüfung

  GregSaveScheduler
    Auto-Save alle N Sekunden (konfigurierbar via F8, default: 60s)
    Läuft als MelonCoroutines.Start(AutoSaveCoroutine())
    Kein Blocking des Game-Threads — LiteDB schreibt async via Task

  GregSaveNotifier
    Toast + Log bei Save/Load-Events:
      [gregSave] ✓ Auto-saved — 847 objects in 12ms
      [gregSave] ✓ Loaded from gregSave_abc123.greg.db
      [gregSave] ⚠ Vanilla save detected — modded features disabled

B-4: HARMONY-PATCHES (via GregHarmonyService)

  PATCH 4 — SaveManager.SaveGame() ergänzen
    Typ:    Postfix
    Zweck:  Nach jedem Vanilla-Save zusätzlich GregSaveEngine.SaveAll()
            aufrufen (nur wenn kein Vanilla-Save-Modus).

  PATCH 5 — SaveManager.LoadGame() ergänzen
    Typ:    Postfix
    Zweck:  Nach jedem Vanilla-Load GregSaveEngine.LoadAll() aufrufen.
            Wenn Vanilla-Save erkannt → GregFeatureGuard aktivieren.

  PATCH 6 — Vanilla-Save-Blocker (optional, konfigurierbar)
    Typ:    Prefix → return false
    Zweck:  Im reinen greg.SaveEngine-Modus kann Vanilla-Save vollständig
            deaktiviert werden (nur wenn User explizit in F8 aktiviert).
    Default: AUS — Vanilla-Save läuft immer zusätzlich.

B-5: ILREPACK INTEGRATION (PFLICHT)

  Alle externen Dependencies werden via ILRepack in gregCore.dll eingebettet.
  Keine externe DLL-Abhängigkeit in UserLibs oder GameDir.

  ILRepack-Konfiguration (build-Pipeline, PowerShell + MSBuild):

    Schritt 1 — NuGet-Restore:
      dotnet restore gregCore.sln
      → Lädt: LiteDB 5.0.21, MoonSharp 2.0.0

    Schritt 2 — Build:
      dotnet build --configuration Release
      → Output: gregCore_raw.dll (ohne merged deps)

    Schritt 3 — ILRepack:
      ILRepack.exe \
        /out:gregCore.dll \
        /internalize \
        /lib:[MelonLoader-Reference-Path] \
        gregCore_raw.dll \
        LiteDB.dll \
        MoonSharp.Interpreter.dll
      → Output: gregCore.dll (alle deps embedded, internalisiert)

    Schritt 4 — Deploy:
      Copy gregCore.dll → "Data Center/Mods/"

  build.ps1 (vollständig, Windows PowerShell 5.1 + 7.x kompatibel):
    [komplett implementieren, keine Platzhalter, korrekte Exitcodes]

  build.sh (vollständig, Linux/macOS):
    [komplett implementieren]

  Hinweis zu ILRepack /internalize:
    Alle eingebetteten Typen werden internal — kein Namespace-Konflikt
    mit anderen Mods die ggf. LiteDB verwenden.
    Ausnahme: greg-eigene public APIs bleiben public.

═══════════════════════════════════════════════════════════
TEIL C — F8 MENÜ — MOD-EINSTELLUNGEN HUB
═══════════════════════════════════════════════════════════

KONZEPT
F8 ist der zentrale Settings-Hub für alle gregCore-Mods.
Kein Mod hat mehr eigene Settings-Panels außerhalb von F8.

C-1: GregSettingsHub (Namespace: greg.UI.Settings)

  Öffnet/schließt via F8 (KeyCode.F8).
  Basis: GregUIBuilder-Pattern (bereits im Framework).
  Layout: Tab-basiert — jede Erweiterung registriert einen eigenen Tab.

  Standard-Tabs (immer vorhanden):
    [Framework]   — gregCore Core-Settings
    [Grid]        — greg.GridPlacement Settings
    [SaveEngine]  — greg.SaveEngine Settings
    [Languages]   — GregLanguageRegistry Status
    [Debug]       — Diagnose, Log-Level, Hooks-Monitor

  Tab-Registration API (für Modder):
    GregSettingsHub.RegisterTab(string tabId, string label, Action<GregUIBuilder> buildFn)
    GregSettingsHub.UnregisterTab(string tabId)

C-2: TAB — [Grid] Einstellungen

  Toggle:    "Grid Placement Active"
               → FeatureFlags.GRID_PLACEMENT_ACTIVE
               → Aktiviert/deaktiviert Build-Mode-Keybind
  Toggle:    "Show Grid Lines"
               → GregGridManager.showGridLines
  Toggle:    "Show Sub-Grid"
               → GregGridManager.showSubGrid
  Slider:    "Sub-Grid Zoom Threshold" [1.0 – 10.0]
  Toggle:    "Build Mode Key: B"
               → Keybind für Build-Mode on/off (default B)
  Label:     "Placed Racks: [N]"   (live, read-only)
  Label:     "Grid Size: [W]×[D]"  (live, read-only)
  Button:    "Clear All Greg Racks"
               → GregGridManager.ClearAll() mit Confirm-Dialog
  Banner:    "[WarningBanner wenn VanillaSave]"
               → "Vanilla Save detected — Grid Placement disabled"

C-3: TAB — [SaveEngine] Einstellungen

  Toggle:    "greg.SaveEngine Active"
               → FeatureFlags.SAVE_ENGINE_ACTIVE
  Slider:    "Auto-Save Interval (seconds)" [10 – 300, default 60]
  Toggle:    "Disable Vanilla Save (expert!)"
               → FeatureFlags.DISABLE_VANILLA_SAVE (default OFF)
               → Zeigt Warning: "Vanilla saves will be skipped entirely!"
  Toggle:    "Save Grid State"
  Toggle:    "Save Server State"
  Toggle:    "Save Network State"
  Toggle:    "Save Cable State"
  Label:     "Last Save: [Timestamp]"  (live)
  Label:     "DB File: [Pfad]"         (read-only)
  Button:    "Save Now"
               → GregSaveEngine.SaveAll()
  Button:    "Open Save Folder"
               → System.Diagnostics.Process.Start(saveDir)
  Banner:    "[WarningBanner wenn VanillaSave]"

C-4: TAB — [Framework] Einstellungen

  Label:     "gregCore v[VERSION]"
  Label:     "MelonLoader v[VERSION]"
  Label:     "Save Mode: [Greg / Vanilla / Hybrid]"
  Toggle:    "Verbose Startup Log"
  Toggle:    "Debug Mode (alle Hooks loggen)"
  Button:    "Run Language Scan now"
             → GregLanguageRegistry.ScanAndActivate()
  Button:    "Show Missing.md Status"
             → Listet alle bekannten MISSING.md-Einträge

C-5: RENDERING RULES (F8 Panel)

  - Ausschließlich OnGUI (UnityEngine.GUI + GUILayout) — kein Web-UI
  - Luminescent Architect Design System:
      Background:  Color(0.00f, 0.07f, 0.07f, 0.93f)
      Accent:      Color(0.38f, 0.96f, 0.85f, 1f)  [61F4D8]
      Text:        Color(0.75f, 0.99f, 0.97f, 1f)  [C0FCF6]
      Warning:     Color(0.93f, 0.25f, 0.27f, 1f)  [ED4245]
  - GUIStyle wird in OnInitializeMelon gecacht, NICHT in OnGUI neu erstellt
  - Panel Breite: 480px, Höhe: dynamisch
  - Position: Bildschirmmitte beim Öffnen
  - ESC schließt das Panel (zusätzlich zu F8-Toggle)

═══════════════════════════════════════════════════════════
TEIL D — MODDER-API (für gregWiki dokumentieren)
═══════════════════════════════════════════════════════════

Folgende APIs müssen vollständig im gregWiki dokumentiert werden.
Jede API mit Signatur-Box, Beispiel und Hinweis auf Vanilla-Guard.

D-1: Grid-Placement API

  // Prüfen ob Grid-Feature aktiv (kein Vanilla-Save)
  bool active = GregFeatureGuard.IsEnabled("GridPlacement");

  // Rack-Placement-Event subscriben
  GregEventDispatcher.On(GregNativeEventHooks.WorldRackPlaced, (payload) => {
      string rackId  = GregPayload.Get<string>(payload, "rackId", null);
      string coord   = GregPayload.Get<string>(payload, "gridCoord", null);
  }, modId: "myMod");

  // Freie Zelle prüfen
  GregGridManager grid = GregGridManager.Instance;
  bool free = !grid.IsCellOccupied(new Vector2Int(3, 5));

  // Rack programmatisch platzieren
  grid.PlaceRack(new Vector2Int(3, 5), myRack);

D-2: SaveEngine API

  // Vanilla-Save-Status prüfen
  if (GregFeatureGuard.IsVanillaSave) {
      LoggerInstance.Warning("Vanilla save — custom save disabled");
      return;
  }

  // Eigene Daten im greg.SaveEngine speichern
  GregSaveEngine.Instance.GetCollection<MyData>("mymod_data")
      .Upsert(new MyData { Id = "key1", Value = 42 });

  // Eigene Daten laden
  var data = GregSaveEngine.Instance.GetCollection<MyData>("mymod_data")
      .FindById("key1");

  // Feature-State-Change subscriben
  GregFeatureGuard.OnFeatureStateChanged += (featureKey) => {
      if (featureKey == "GridPlacement") RefreshUI();
  };

D-3: F8-Settings Tab registrieren

  // Eigenen Tab im F8-Menü registrieren
  GregSettingsHub.RegisterTab("myMod.settings", "My Mod", (builder) => {
      builder.AddLabel("My Mod v1.0.0")
             .AddToggle("Enable Feature X", config.FeatureX, v => config.FeatureX = v)
             .AddSlider("Speed", 0.1f, 5f, config.Speed, v => config.Speed = v)
             .AddButton("Reset to Defaults", ResetConfig);
  });

  // Tab beim Mod-Shutdown entfernen
  GregSettingsHub.UnregisterTab("myMod.settings");

═══════════════════════════════════════════════════════════
TEIL E — GREGWIKI SEITEN (vollständige Markdown-Dateien)
═══════════════════════════════════════════════════════════

Erstelle folgende Wiki-Seiten vollständig (YAML-Frontmatter, copy-paste-ready):

E-1: grid-placement-guide.md
  - Was ist greg.GridPlacement?
  - Vanilla RackHolder vs. Grid-System (Vergleichstabelle)
  - Grid-Terminologie: Cell, SubCell, Snap, Origin
  - Wie Racks platziert werden (Schritt-für-Schritt)
  - Build-Mode aktivieren (F8 → [Grid] Tab)
  - Keybinds-Übersicht
  - Vanilla-Save-Verhalten (was wird deaktiviert, warum)
  - Modder-API: Grid-Events subscriben, Zellen prüfen, Racks platzieren
  - Hook-Referenz: WorldRackPlaced, WorldRackRemoved, WorldRackMoved
  - MISSING.md-Hinweis wenn RackHolder-Patch nicht möglich

E-2: save-engine-guide.md
  - Was ist greg.SaveEngine?
  - Vanilla-Save vs. greg.SaveEngine (Vergleichstabelle)
  - LiteDB: warum, was, wo (DB-Datei-Pfad)
  - Vanilla-Save-Detection: wie wird erkannt, was passiert
  - Game-Breaking-Features: vollständige Liste was deaktiviert wird
  - Auto-Save konfigurieren (F8 → [SaveEngine] Tab)
  - Modder-API: eigene Collections schreiben/lesen
  - Schema-Referenz: alle Collections + Felder
  - ILRepack: warum embedded, keine externe DLL
  - Vanilla-Kompatibilität: expliziter Hinweis "NOT COMPATIBLE by design"

E-3: f8-settings-hub.md
  - Was ist das F8 Settings Hub?
  - Alle Standard-Tabs und ihre Funktionen
  - Modder: eigenen Tab registrieren (vollständiges Beispiel)
  - Vanilla-Save-Banner: wann erscheint er, was bedeutet er
  - Rendering-Regeln (OnGUI, Design-System-Farben)
  - API-Referenz: RegisterTab, UnregisterTab

E-4: vanilla-save-compatibility.md
  - Was ist ein "Vanilla Save"?
  - Wie greg.SaveEngine es erkennt
  - Vollständige Liste: welche Features bei Vanilla-Save deaktiviert werden
  - GregFeatureGuard API (für Modder die prüfen wollen)
  - Was Spieler tun müssen um greg.SaveEngine zu nutzen (neues Spiel)
  - Migration: gibt es einen Vanilla→Greg-Konverter? (MISSING.md wenn nein)

═══════════════════════════════════════════════════════════
TEIL F — OUTPUT-ANFORDERUNGEN
═══════════════════════════════════════════════════════════

Liefere in dieser Reihenfolge (alles vollständig, keine Platzhalter):

  1. PRE-ANALYSIS REPORT (Schritt 0 Output)

  2. gregCore.csproj
     — LiteDB 5.0.21 PackageReference
     — MoonSharp 2.0.0 PackageReference
     — ILRepack als MSBuild-Target Post-Build

  3. build.ps1 + build.sh
     — dotnet restore → build → ILRepack → deploy

  4. FRAMEWORK CODE (vollständige .cs-Dateien):
     greg.GridPlacement/:
       GregGridManager.cs
       GregGridCell.cs
       GregSubCell.cs
       GregPlaceableRack.cs
       GregPlacementController.cs
     greg.SaveEngine/:
       GregSaveEngine.cs
       GregSaveScheduler.cs
       GregSaveNotifier.cs
       GregVanillaDetector.cs
     frameworkSdk/:
       GregFeatureGuard.cs
     greg.UI.Settings/:
       GregSettingsHub.cs
     Harmony-Patches/:
       RackPlacementPatch.cs
       SaveManagerPatch.cs
     GregNativeEventHooks.cs (Patch — neue Hooks)

  5. WIKI-SEITEN (E-1 bis E-4, vollständige Markdown)

  6. NUR WENN ZWINGEND: MISSING.md-Dateien
     Header PFLICHT:
       --- MISSING.md — DEVELOPMENT ONLY — NICHT COMMITTEN ---
     .gitignore MUSS enthalten: MISSING.md und **/MISSING.md

═══════════════════════════════════════════════════════════
GLOBALE STRIKTE REGELN
═══════════════════════════════════════════════════════════

REGEL 0   Wiki = Source of Truth. Konflikte mit ⚠️ WIKI↔CODE CONFLICT markieren.
REGEL 1   Keine Halluzinationen. Nicht verifizierte APIs mit [UNVERIFIED] markieren.
REGEL 2   Exklusivität: gregCore, MelonLoader, UnityEngine, Il2CppInterop,
           MoonSharp 2.0.0, LiteDB 5.x. Kein Web-UI, kein HTML/CSS.
REGEL 3   ILRepack: LiteDB und MoonSharp sind IMMER in gregCore.dll embedded.
           Keine externe DLL. Keine UserLibs-Abhängigkeit.
REGEL 4   "Plugins" = MelonLoader-Assemblies (.dll) — RESERVIERT.
           "Scripts" = Lua/Rust/Python/JS-Dateien in Mods/Scripts.
REGEL 5   IL2CPP-Fixes immer anwenden:
             - UnityEngine.Input → InputSystem.Keyboard.current
             - StartCoroutine → MelonCoroutines.Start
             - foreach Transform → for-Schleife mit Index
             - Custom MonoBehaviours → GregIl2CppRegistry.RegisterType<T>
             - System.Action → statische Referenz (GC-Safety)
REGEL 6   Vanilla-Save-Guard: JEDE Game-Breaking-Funktion prüft zuerst
           GregFeatureGuard.IsEnabled() bevor sie ausgeführt wird.
REGEL 7   MISSING.md: lokal, NIEMALS committen, immer Pflicht-Header.
           .gitignore: MISSING.md und **/MISSING.md immer eintragen.
REGEL 8   Kein silent catch. Jede Exception → MelonLogger.Error + StackTrace.
REGEL 9   Copy-paste-ready: Jeder Code-Block kompiliert ohne Änderungen.
REGEL 10  Framework-Guard: jeder Mod-Tick prüft Core.Instance != null.
```

***

## Technische Entscheidungsbegründungen

**LiteDB statt SQLite** ist die richtige Wahl für diesen Use-Case: LiteDB arbeitet dokumentenbasiert (BSON), hat keinen Schema-Migrations-Overhead, keine Connection-Pools und liest/schreibt C#-Objekte direkt ohne ORM.  Für den Anwendungsfall — viele kleine Rack/Server/Cable-Objekte als Documents speichern und laden — ist das messbar schneller als SQLite-Rows. Die gesamte DB lebt in einer einzigen `.greg.db`-Datei im Saves-Verzeichnis. [ppl-ai-file-upload.s3.amazonaws](https://ppl-ai-file-upload.s3.amazonaws.com/web/direct-files/collection_9efda5cc-b446-4abc-aa8f-ccf45da62eda/84eba474-1425-4686-a0ad-8d424347a7a8/erstelle-mir-einen-ausfurhlich-1I0IcdO2S4y1n2a8cXkHlw.md)

**ILRepack statt UserLibs** ist zwingend, weil MelonLoader bei fehlenden Dependencies bereits jetzt eine Warning erzeugt (`Python.Runtime` im Log).  Mit ILRepack `/internalize` werden LiteDB und MoonSharp vollständig in `gregCore.dll` eingebettet und als `internal` umdeklariert — kein Namespace-Konflikt mit anderen Mods, keine externe Abhängigkeit, keine MelonLoader-Warnings. [ppl-ai-file-upload.s3.amazonaws](https://ppl-ai-file-upload.s3.amazonaws.com/web/direct-files/collection_9efda5cc-b446-4abc-aa8f-ccf45da62eda/84eba474-1425-4686-a0ad-8d424347a7a8/erstelle-mir-einen-ausfurhlich-1I0IcdO2S4y1n2a8cXkHlw.md)

**Vanilla-Save-Detection als zentraler Guard** schützt vor Game-Breaking-Konflikten: Das Vanilla-Save-Format ist Binary-IL2CPP-serialized, und `portVlanFilters` ist aus bisheriger Analyse nur Runtime-only ohne direkten Save-Zugriff.  `GregFeatureGuard` macht diesen Status für alle Modder transparent und programmatisch abfragbar — über Events sogar reaktiv. [ppl-ai-file-upload.s3.amazonaws](https://ppl-ai-file-upload.s3.amazonaws.com/web/direct-files/collection_9efda5cc-b446-4abc-aa8f-ccf45da62eda/84eba474-1425-4686-a0ad-8d424347a7a8/erstelle-mir-einen-ausfurhlich-1I0IcdO2S4y1n2a8cXkHlw.md)