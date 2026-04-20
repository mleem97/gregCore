# modding_core_architecture_summary

> Zweck: Single Source of Truth für AI-/Bridge-Entwicklung (Lua, Rust, Go, Python, TS/JS) gegen gregCore.
>
> Scope: Laufzeit im Spielprozess (MelonLoader + Unity IL2CPP), inklusive FFI, IPC, Eventing und Sicherheitsgrenzen.

## 1) Runtime Lifecycle, Bootstrap & Schichten

### Schichtzuordnung (verbindlich)
- **Unity Spiel / IL2CPP Assembly (Game Layer):** gepatchte Spieltypen/Methoden (z. B. `Player.UpdateCoin`, `ServerPowerButton`, UI/Save-Methoden).
- **GregFramework Core SDK (Core Layer):** `src/gregModLoader/gregCore.cs`, `gregHarmonyPatches.cs`, `gregFfiBridge.cs`, `gregGameApi.cs`, `gregEventDispatcher.cs`.
- **Plugin Layer:** `src/gregModLoader/Plugins/*` (`gregPluginBase`, `gregRegistry`, Dependency-Resolver).
- **Language Bridges:** `src/Scripting/*` (`iGregLanguageBridge`, `gregLanguageBridgeHost`, Lua/JS/Rust/Go-Bridges).
- **Mod Layer:** User-Mods, native DLL-Mods (FFI), Script-Mods.

### Haupt-Entry und Aufrufreihenfolge
- **Core-Layer Entry:** `gregCoreLoader : MelonMod` in `src/gregModLoader/gregCore.cs`.
- Relevante Lifecycle-Methoden:
  - `OnInitializeMelon()`
  - `OnSceneWasLoaded(int buildIndex, string sceneName)`
  - `OnUpdate()`
  - `OnFixedUpdate()`
  - `OnGUI()`
  - `OnApplicationQuit()`

### Effektive Initialisierungssequenz
1. Core initialisiert Konfiguration, Aktivierung/Flags, Logging.
2. Core erstellt/initialisiert FFI (`gregFfiBridge`) und API-Tabelle (`gregGameApi`).
3. Core initialisiert Script-Host (`gregLanguageBridgeHost`) und lädt Bridges.
4. Core installiert Harmony-Patches (`gregHarmonyPatches`).
5. Plugins werden registriert/aufgelöst (`gregRegistry`, `gregDependencyResolver`) und über Ready-Callbacks aktiviert.
6. Runtime-Loop verteilt Update-Ticks an Core, Bridges und FFI-Module.

### Shutdown-Semantik
- `OnApplicationQuit()` triggert geordnetes Stoppen:
  - Script-Host/Bridges herunterfahren,
  - native FFI-Module via `mod_shutdown` und `FreeLibrary` freigeben,
  - Pointer/Handles im `gregGameApi.Dispose()` bereinigen.

---

## 2) IL2CPP Hooking-Modell & Ausführungsfluss

### Hook-Ebene
- **Core Layer:** `src/gregModLoader/gregHarmonyPatches.cs` patcht IL2CPP-Spielmethoden mit Harmony Prefix/Postfix.
- Ziel ist **Event-Proxying**, nicht direkter unkontrollierter Mod-Zugriff auf Unity-Typen.

### Hook-zu-Event Pipeline
1. IL2CPP-Methode wird gepatcht (Prefix/Postfix).
2. Patch extrahiert primitive/struct-basierte Daten.
3. Dispatch über `EventDispatcher`/`gregEventDispatcher` mit numerischer `EventIds`-ID.
4. `GregHookIntegration` mappt `eventId -> greg.*` Hookname (`gregNativeEventHooks`).
5. Hook-Payload wird normalisiert (`BuildPayload(...)`) und an Bus/FFI weitergereicht.

### Canonical Hook-Namen
- Mapping zentral in `src/gregSdk/gregNativeEventHooks.cs`.
- Primärquelle für Namen: `greg_hooks.json` + framework-only Ergänzungen via `gregHookName.Create(...)`.
- Fallback bei unbekannten IDs: `greg.SYSTEM.UnmappedNativeEvent`.

### Cancelable vs Non-cancelable
- `gregEventDispatcher` unterstützt normale und cancelable Listener (`Func<string, object, bool>`).
- Cancel-Pfad wird in Patchpunkten genutzt, wo Spielaktion blockierbar ist.

---

## 3) Interop, FFI & IPC (ABI, Ports, Protokolle)

### Native FFI (Core ↔ Native Mod)
- **Core Layer Datei:** `src/gregModLoader/gregFfiBridge.cs`.
- Win32 Loader-API:
  - `LoadLibrary`
  - `GetProcAddress`
  - `FreeLibrary`
- Erwartete native Exports (C-ABI):
  - `mod_info`
  - `mod_init`
  - `mod_update`
  - `mod_fixed_update`
  - `mod_on_scene_loaded`
  - `mod_on_gui`
  - `mod_shutdown`
  - `mod_on_event`

### API-Table für native Module
- **Core Layer Datei:** `src/gregModLoader/gregGameApi.cs`.
- Versioniertes Struct: `GameAPITable` (aktuell auf v12 erweitert).
- Delegates werden via `Marshal.GetFunctionPointerForDelegate(...)` als Funktionszeiger exportiert.

### IPC/Netzwerkflächen
- **MCP HTTP Server (Core/Tooling-Grenze):** `GregMCPServer` via `HttpListener` (localhost).
- **Multiplayer Transport (Plugin/Core):** WebSocket-Client in `GregMultiplayerService`.
- **Plugin-Sync (Core/Service):** `HttpClient` in `gregPluginSyncService`.
- **Native Plattform-Interop:** Steam-P2P via `steam_api64`-Imports in `gregGameApi.cs`.

### Bridge-Host für Sprachen
- **Language Bridge Layer:** `iGregLanguageBridge` + `gregLanguageBridgeHost`.
- Lua: MoonSharp; JS/TS: Jint; Rust/Go über Bridge-Adapter.
- Host ist der Isolations- und Lifecycle-Knoten für alle Script-Runtimes.

---

## 4) Memory Boundaries, Ownership & Lifetime

### Ownership-Regeln an der FFI-Grenze
- **Unmanaged Allokation durch Core:** `Marshal.AllocHGlobal`.
- **Freigabe durch Core nach Callback:** `Marshal.FreeHGlobal` (symmetrisch im Dispatch-Pfad).
- **String-Marshalling:** `Marshal.StringToHGlobalAnsi` / `Marshal.PtrToStringAnsi`.
- **Blittable Struct Transfer:** `[StructLayout(LayoutKind.Sequential)]` in Event-Payloads.

### Delegate-/Function-Pointer-Lifetime
- Delegates für API-Funktionen werden als Felder gehalten, damit der GC keine Funktionszeiger invalidiert.
- `GameAPITable` speichert `IntPtr` auf Delegate-Stubs; Freigabe zentral im `Dispose()`.

### GCHandle-Verwendung
- Für Event-/Payload-Weitergabe werden `GCHandle.Alloc(...)` Handles erzeugt.
- Handles werden nach Verwendung explizit gelöst (`GCHandle.Free()`), um Leaks/Pinning-Druck zu verhindern.

### Fehlerresilienz an Grenzstellen
- FFI-Aufrufe sind einzeln in `try/catch` eingefasst.
- Fehler in einem Modul dürfen den Core-Loop nicht terminieren (Fail-isolated execution).

### Kritische ABI-Regeln für externe Bindings
- Struct-Reihenfolge/Field-Typen sind ABI-kritisch; keine Reorder/Pack-Änderung ohne Version-Bump.
- Auf der Bridge-Seite nur stabile primitive Typen (`int`, `uint`, `float`, `byte[]`, `IntPtr`) übergeben.
- Keine Ownership-Mehrdeutigkeit: jeder Pointer braucht eindeutige „who frees“-Regel.

---

## 5) DTOs, Manifeste & Serialisierung

### Typfamilien
- **Runtime Event DTOs (Core):** `src/gregModLoader/Events/*.cs` (`iModEvent` mit `DateTime OccurredAtUtc`).
- **Native Event Struct DTOs (Core):** `src/gregModLoader/gregEventDispatcher.cs` (z. B. `ValueChangedData`, `DayEndedData`, `ShopItemAddedData`).
- **SDK/Config DTOs (Core/SDK):**
  - `GregUiReplacementManifest`
  - `ModelOverrideManifest`
  - `ServerDefinition`
  - `ItemDefinition`
  - `PluginSyncConfig`, `PluginSyncManifest`

### Serializer-Stack
- `System.Text.Json` für Runtime-/Service-Pfade (`GregPersistenceService`, MCP-Antworten).
- `Newtonsoft.Json` in Teilen des Config-Stacks (`GregConfigService`).
- Konsequenz: DTO-Contracts sind serializer-agnostisch zu halten (öffentliche Properties/Fields stabil).

### Feld- und Versionsstabilität
- Event- und Manifest-Felder sind externe Verträge für Bridges/Mods.
- Änderungen nur additiv und versioniert (insb. API-Table + manifestartige Contracts).
- Entfernen/Umbenennen erzeugt harte Breaking Changes für native und Script-Bindings.

### Datentyp-Praxis für Multi-Language Bindings
- Bevorzugt JSON-kompatible Primitiven und flache Objekte.
- Für binäre Übergaben klar deklarierte Byte-Arrays + Länge.
- Keine impliziten Unity-/IL2CPP-Objektreferenzen in öffentlichen Bridge-DTOs.

---

## 6) Event-System, Hook-API & Sandboxing-Einschränkungen

### Event-Systeme (parallel vorhanden)
1. **String-basierter Hook-Bus (Core):** `gregEventDispatcher` mit Hooknamen `greg.<DOMAIN>.<Event>`.
2. **Type-safe Bus (SDK):** `GregEventBus` (`Subscribe<T>`, `Publish<T>`, `Unsubscribe<T>`).
3. **Native Event-ID Dispatch (Core↔FFI):** `EventIds` + struct-payload + `mod_on_event`.

### Hook-Namenskonvention
- Kanonisch: `greg.<Domain>.<Event>` (bzw. in Altbeständen teils uppercase Domains).
- Mappingquelle für native IDs: `gregNativeEventHooks.ByEventId`.
- Hook-Integration: `GregHookIntegration.EmitForSimple/EmitForStruct`.

### Sandboxing-Realität (wichtig für AI/Bridge-Design)
- **Lua I/O Modul:** `src/Scripting/Lua/LuaModules/gregIoLuaModule.cs` enthält Dateioperationen (`read_file`, `write_file`, `list_files` etc.).
- Kommentar behauptet Sandbox-Scope, Implementierung zeigt aktuell keine harte Pfad-Isolation auf OS-Ebene.
- **ReferenceScanner:** lädt Assemblies via `Assembly.LoadFrom` aus Dateisystemscan; keine AppDomain/Process-Isolation.
- Damit gilt: aktuelle Isolation ist primär **kooperativ/logisch**, nicht sicherheitstechnisch stark.

### Verbindliche Einschränkungen für neue Bindings
- Bridge-Code darf Core/Unity-Aufrufe nur über freigegebene API/Hooks ausführen.
- Keine direkten unsicheren Pointer-Operationen außerhalb definierter FFI-Wrapper.
- Untrusted Script/Native Mods als potenziell fehlerhaft behandeln (guard clauses + timeout/backpressure + exception fences).
- Für echte Sandbox-Anforderungen ist Prozessisolation (separater Host) nötig; im aktuellen In-Process-Modell nicht garantiert.

---

## Appendix: Operative Leitplanken für Bridge-Autoren

- **Schichtdisziplin:**
  - Mod/Bridge → `greg.*` Hook/API,
  - kein direkter Unity-Objektzugriff als öffentliches Contract.
- **ABI-Disziplin:**
  - `StructLayout.Sequential`, feste Feldtypen, versionierte Erweiterung.
- **Fehlerdisziplin:**
  - Jede Grenzstelle (`FFI`, `JSON parse`, `network`) mit `try/catch` + Logging.
- **Lifecycle-Disziplin:**
  - `init -> update/fixed_update/gui -> shutdown` strikt einhalten.
- **Kompatibilität:**
  - Runtime-Ziel bleibt `.NET 6` (IL2CPP/MelonLoader-kompatibel).
