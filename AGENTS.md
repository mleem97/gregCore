# AGENTS.md - IL2CPP Blackbox Modding Guidelines (2026)

## STATUS: MANDATORY SYSTEM RULES
**Kontext:** Unity 6.4+ (Unity 6000.4+) | IL2CPP Backend | MelonLoader 0.7+ / BepInEx 6 | Il2CppInterop
**Umgebung:** Blackbox Modding (Kein Quellcode-Zugriff). Alle Logik basiert auf Reverse Engineering.

---

## 1. REVERSE ENGINEERING & LOGIC INTEGRITÄT

* **HALLUZINATIONS-VERBOT:** Generiere niemals Logik, die auf Vermutungen über den internen C#-Code basiert. Analysiere die Dummy-DLLs mit `Cpp2IL`, `Il2CppDumper` oder `Ghidra`. Wenn die Methode existiert, aber die Logik unklar ist, fordere eine vollständige Analyse an.
* **DUMMY-DLL REFERENZEN:** Nutze ausschließlich die durch `Il2CppDumper` aus der `global-metadata.dat` extrahierten Dummy-DLLs als Typreferenz. Keine Annahmen über nicht extrahierte Typen.
* **RUNTIME INSPEKTION:** Verlasse dich bei der Objekthierarchie nicht auf statische Analysen. Nutze **UnityExplorer** oder **BetterMonoInspector**, um GameObjects, Komponenten und deren aktiven State zur Laufzeit zu validieren.
* **SYMBOLS & OFFSETS:** Bei nativen Detours müssen Offsets über `ghidra_with_struct.py` verifiziert werden. Vertraue niemals ungeprüften Memory-Adressen aus Foren, Discord oder GitHub Gists.
* **VERSIONSKONTROLLE:** Beachte, dass sich Offsets, VTable-Indizes und Method-Signaturen zwischen Game-Versionen ändern können. Prüfe immer die Zielversion spezifisch.

---

## 2. IL2CPP INTEROP & TYPE SYSTEM (2026)

* **CASTING:** Nutze niemals Standard C# Casts `(T)obj` oder `obj as T`. Verwende zwingend:
    * `obj.Cast<T>()` für explizite Umwandlungen.
    * `obj.TryCast<T>()` für sichere Typprüfungen mit Nullable-Return.
* **TYPE OF:** Ersetze `typeof(T)` durch `Il2CppType.Of<T>()`, wenn Interaktion mit der IL2CPP-Domain erforderlich ist.
* **GENERICS & SPECIALIZATION:** IL2CPP spezialisiert Generics zur Kompilierzeit. Der Aufruf von generischen Methoden über Interop erfordert oft spezifische `MethodInfo`-Suchen über das Metadaten-System. Nutze `AccessTools.Method` mit vollständiger Signatur.
* **CUSTOM CLASSES:** Jede Klasse, die von Unity-Typen erbt, MUSS:
    1. Ein `RuntimeType` sein (via `ClassInjector.RegisterTypeInIl2Cpp<T>()`)
    2. Einen Konstruktor besitzen, der `IntPtr` akzeptiert: `public MyClass(IntPtr ptr) : base(ptr) {}`
    3. Alle managed Felder sollten `readonly` oder `private` sein, mit öffentlichen Properties für Access
* **NATIVE HANDLES:** Nutze `new Il2CppObjectBase.NativePtr` für Referenzen auf native Objekte. Halte niemals direkte Pointer alive – nutze die managed Wrapper.
* **DUMMY-ASSEMBLY-KOMPILIERUNG:** Kompiliere ausschließlich gegen die Il2CppInterop Dummy-Assemblies (aus `Il2CppDumper`/`Cpp2IL`). **NIE** gegen originale Game-Assemblies kompilieren — generische Value-Typen (z.B. `UnityEngine.UIElements.StyleEnum<T>`) können zwischen Original und Dummy unterschiedliche Layouts haben und verursachen zur Laufzeit `TypeLoadException: value type mismatch`.

---

## 3. MEMORY MANAGEMENT & STABILITY

* **OBJECT SURVIVAL:** Native Unity-Objekte können vom GC zerstört werden, während der managed Proxy noch existiert.
    * Prüfe IMMER: `Object.NativePointer != IntPtr.Zero`
    * Nutze `Object.TryGetComponent<T>()` statt direkter Casts
* **REFERENCE LOOPS:** Vermeide zirkuläre Referenzen zwischen Managed Code und IL2CPP-Typen. Dies führt unweigerlich zu `ObjectCollectedException` oder Memory Leaks.
* **STRING HANDLING:** IL2CPP-Strings sind native C++ Strings. Bei direkten API-Calls nutze `Il2CppSystem.String` – konvertiere explizit mit `string` / `new Il2CppSystem.String()`.
* **GARBAGE COLLECTION:** Rufe `GC.Collect()` nur explizit bei Memory-kritischen Operationen auf. Nutze `using` blocks für temporäre native Wrappers.
* **DISPOSABLE PATTERN:** Implementiere ` IDisposable` für alle Custom-Klassen, die native Resourcen halten. Nutze `Destroy()` für Unity-Objekte, `Free()` für native Pointer.

---

## 4. INPUT SYSTEM MIGRATION (UNITY 6000.4+ / IL2CPP)

* **LEGACY INPUT VERBOT:** Wenn das Spiel im Player Settings auf "Input System package" umgestellt ist (erkennbar an `InvalidOperationException: You are trying to read Input using the UnityEngine.Input class`), ist **jeder** Zugriff auf `UnityEngine.Input` (Legacy Input Manager) zur Laufzeit ungültig.
    * `Input.GetKeyDown(KeyCode)` → `Keyboard.current?[Key.F1].wasPressedThisFrame` (mit Null-Check!)
    * `Input.mousePosition` → `Mouse.current?.position.ReadValue()`
    * `Input.GetMouseButtonDown(0)` → `Mouse.current?.leftButton.wasPressedThisFrame`
    * `Input.GetAxis(...)` → Entsprechende `InputAction` aus dem Game referenzieren oder über `Keyboard`/`Mouse` direkt abfragen
* **KEYCODE MAPPING:** `Keyboard.current` verwendet den `Key`-Enum, nicht `KeyCode`. Für dynamische Keybinds muss eine explizite `KeyCode → Key` Mapping-Tabelle existieren (siehe `GregInputBindingService`).
* **NULL-SAFETY:** `Keyboard.current` und `Mouse.current` können in IL2CPP-Kontexten `null` sein. Immer null-guarden: `if (Keyboard.current?.f1Key.wasPressedThisFrame == true)`.

## 5. HARMONY PATCHING (NATIVE HOOKS) - 2026 REVISED

* **TRANSPILER-VERBOT:** In IL2CPP-Umgebungen sind IL-Transpiler funktionslos oder führen zu Inkonsistenzen. Nutze ausschließlich **Prefix** oder **Postfix**.
* **DEFENSIVE HOOKING:** Jeder Patch MUSS in einem `try-catch`-Block gekapselt sein. Eine unbehandelte Exception in einem Hook bringt den nativen Game-Thread zum sofortigen Absturz (Hard Crash).
    ```csharp
    [HarmonyPrefix]
    [HarmonyPatch(typeof(TargetClass), nameof(TargetClass.TargetMethod))]
    private static void PatchPrefix(TargetClass __instance)
    {
        try
        {
            if (__instance == null) return;
            // Logik
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"Hook failed: {ex.Message}");
        }
    }
    ```
* **NULL-CHECKS:** Führe in jedem Prefix einen Null-Check für `__instance` und ALLE Parameter durch, bevor Logik ausgeführt wird.
* **METHOD SIGNATURES:** Stelle sicher, dass die Patch-Signatur exakt den Dummy-DLLs entspricht. Bei Discrepanzen nutze `HarmonyLib.Traverse` für flexible Accessors.
* **IL2CPP FIELD ACCESSORS:** Auto-generierte Property-Getter für Felder (z.B. `switchId`, `patchPanelId`) haben in IL2CPP **keinen managed Method-Body** und sind daher mit Harmony **nicht patchbar**. Verwende stattdessen Postfix auf die aufrufende Methode oder intercepte auf höherer Ebene.
* **NICHT-EXISTIERENDE METHODEN:** Methoden, die in den Original-Assemblies existieren, können in den Dummy-DLLs fehlen (z.B. `PositionIndicator.OnDestroy`, `InputController.Move/Look/Interact`). Vor dem Patchen IMMER gegen die Dummy-DLL verifizieren.
* **MULTI-PATCH ORDNUNG:** Nutze explizite Priorities bei mehreren Patches auf dieselbe Methode: `[HarmonyPriority(int)]`.
* **STACK TRACE HANDLING:** Fange in Postfix-Blocks keine Exceptions – lasse sie zum original Call durchpropagieren, aber logge sie vorher.
* **ORIGINAL CALL ERHALTEN:** Rufe `original()` NIE aus Prefix-Blocks auf – dies führt zu undefiniertem Verhalten. Nutze ausschließlich Postfix für Ergebnis-Modifikationen.

---

## 6. UI & ASSETS (UNITY 6000.4+ / 2026)

* **LEGACY-VERBOT:** Die Nutzung von `OnGUI()`, `GUILayout`, `Editor`-Klassen und IMGUI ist strikt untersagt.
* **MODERN UI STACK:** Nutze primär **UI Toolkit** (UXML/USS).
    * Lade UXML dynamisch via `AssetBundle` oder `Resources.Load()`
    * Nutze `UIElements.VisualElement` als Basisklasse
    * Setze Styles via USS oder C#-Properties
* **UI TOOLKIT CALLBACKS IN IL2CPP:** `RegisterCallback<TEvent>()` akzeptiert in IL2CPP-Umgebungen keine impliziten Lambda-Konvertierungen, weil `EventCallback<T>` von `Il2CppSystem.MulticastDelegate` erbt. Verwende explizite `new Action<TEvent>(...)`-Wrapper:
    ```csharp
    var btn = new Button();
    btn.RegisterCallback<ClickEvent>(new Action<ClickEvent>(_ => OnClick()));
    ```
* **UGUI FALLBACK:** Wenn UI Toolkit nicht möglich ist, nutze UGUI ausschließlich via Code-Generierung:
    * Erstelle `GameObject` hierarchies via `AddComponent<GameObject>()`
    * Nutze `Canvas`/`CanvasRenderer` nur wenn unvermeidbar
* **ASSET LOADING:**
    1. `AssetBundle.LoadFromFile()` – bevorzugt für Mod-Bundles
    2. `Addressables` – nur wenn Game-spezifische Keys bekannt sind
    3. `Resources.Load()` – nur wenn keine Alternative existiert
* **YAML-DIREKT-EDITIERUNG:** Bearbeite niemals `.prefab` oder `.unity` Dateien direkt. Erzeuge oder modifiziere UI-Elemente ausschließlich zur Laufzeit.
* **THEME KONSISTENZ:** Mache UI-Elemente konfigurierbar (Colors, Sizes). Greife nicht auf private Theme-Variablen zu.

---

## 7. MELONLOADER 0.7+ SPEZIFISCH

* **MOD LIFECYCLE:**
    * Registriere Typen in `OnPreLoad()` oder `OnApplicationStart()`
    * Initialisiere alle Systems in `OnLateLoad()` für Load-Order Safety
    * Nutze `OnQuitRequest()` für Cleanup
* **LOGGING:**
    ```csharp
    MelonLogger.Msg(ConsoleColor.Cyan, "Message");    // Info
    MelonLogger.Warning("Warning message");            // Warnung
    MelonLogger.Error("Error message");               // Fehler
    MelonLogger.WriteToLogDetailed(exception, true); // Exceptions
    ```
* **CONFIG SYSTEM:** Nutze `MelonPreferences.CreateEntry<T>()` für persistente Einstellungen. Speichere niemals direkt in Dateien.
* **ASSEMBLY RESOLUTION:** Handle fehlende Dependencies via `AppDomain.CurrentDomain.AssemblyResolve`.
* **CROSS-MOD KOMMUNIKATION:** Nutze `MelonHandler.Plugins` für Inter-Mod-Communication. Vermeide direkte Type-Sharing.

---

## 8. BOILERPLATE TEMPLATES (2026)

### Custom Component (MelonLoader)
```csharp
using System;
using Il2CppInterop.Runtime.Injection;
using Il2CppInterop.Runtime.Attributes;
using UnityEngine;

namespace MyMod.Components
{
    [Il2CppRegister]
    public class ModComponent : MonoBehaviour
    {
        public ModComponent(IntPtr ptr) : base(ptr) { }

        [HideInInspector]
        public string Id { get; set; }

        void Awake()
        {
            MelonLogger.Msg("ModComponent awake");
        }

        void Update()
        {
            // Safety first
            if (this == null || !IsValid()) return;
        }

        public bool IsValid() => base.Object != null && base.Object.m_Version != 0;

        public void Cleanup()
        {
            // Managed cleanup
        }

        void OnDestroy()
        {
            Cleanup();
        }
    }
}

// Registrierung in MelonMod:
internal static class ModSetup
{
    internal static void RegisterTypes()
    {
        ClassInjector.RegisterTypeInIl2Cpp<ModComponent>();
    }
}
```

### Harmony Patch Template
```csharp
using HarmonyLib;
using MelonLoader;

[HarmonyPatch]
internal static class TargetClass_Patches
{
    private static readonly HarmonyLib.Harmony Harmony = new("com.mymod.id");

    internal static void Apply() => Harmony.PatchAll(typeof(TargetClass_Patches));
    internal static void Revert() => Harmony.UnpatchSelf();

    [HarmonyPrefix]
    [HarmonyPatch(typeof(TargetClass), nameof(TargetClass.TargetMethod))]
    [HarmonyPriority(Priority.High)]
    private static bool TargetMethodPrefix(TargetClass __instance, ref SomeClass __result)
    {
        try
        {
            if (__instance == null) return true;
            
            // Pre-hook Logik
            
            // return false -> original wird nicht aufgerufen
            // return true -> original wird aufgerufen
            return true;
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"TargetMethodPrefix failed: {ex}");
            return true; // Default: Original aufrufen
        }
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TargetClass), nameof(TargetClass.TargetMethod))]
    private static void TargetMethodPostfix(TargetClass __instance, ref SomeClass __result)
    {
        try
        {
            if (__instance == null) return;
            
            // Post-hook Modifikation von __result
        }
        catch (Exception ex)
        {
            MelonLogger.Error($"TargetMethodPostfix failed: {ex}");
        }
    }
}
```

### MelonMod Main Class
```csharp
using MelonLoader;
using MyMod.Components;

namespace MyMod
{
    public class MyMod : MelonMod
    {
        internal static MyMod Instance { get; private set; }
        internal static MelonLogger Logger => (MelonLogger)LoggerInstance;

        public override void OnPreLoad()
        {
            base.OnPreLoad();
            ModSetup.RegisterTypes();
        }

        public override void OnLateLoad()
        {
            base.OnLateLoad();
            Instance = this;
            
            // Post-Load Initialization
        }

        public override void OnQuitRequest()
        {
            // Cleanup
            base.OnQuitRequest();
        }
    }
}
```

---

## 9. FEHLERBEHANDLUNG & DEBUGGING

* **EXCEPTION HANDLING:** Keine unbehandelten Exceptions. Jeder `try-catch` sollte in `catch` einen `MelonLogger.Error()` oder detaillierter Logging haben.
* **LOGGING LEVELED:** Nutze die korrekten Level:
    * `Msg` – Allgemeine Info
    * `Warning` – Potentiale Probleme
    * `Error` – Kritische Fehler
* **STACK TRACE EXPORT:** Bei unerwarteten Fehlern: `MelonLogger.WriteToLogDetailed(ex, true)` für vollständige Stack Traces.
* **DEBUGGING:** Nutze `MelonConsole` für In-Game-Debug-Commands. Setze `#if DEBUG` für diagnostische Features.
* **STABILITY TESTING:** Teste jeden Patch mit:
    1. Null-Instance
    2. Null-Parameters
    3. Race Conditions (schnelle Aufrufe)
    4. Memory Pressure (modifizierte Szenen)

---

## 10. COMPLIANCE CHECKLIST

Vor jeder Code-Generierung prüfe:

- [ ] Dummy-DLL Referenz vorhanden für alle Typen?
- [ ] Kompiliert ausschließlich gegen Il2CppInterop Dummy-Assemblies (nicht Original-Game-Assemblies)?
- [ ] Keine Standard-Casts `(T)obj` oder `as T` verwendet?
- [ ] Alle Custom-Klassen mit `IntPtr`-Konstruktor und `ClassInjector` registriert?
- [ ] Alle Harmony-Patches in `try-catch`?
- [ ] Patch-Ziele in Dummy-DLL verifiziert (existieren die Methoden wirklich)?
- [ ] Null-Checks für `__instance` und alle Parameter?
- [ ] Keine `OnGUI()` / IMGUI Verwendung?
- [ ] Keine Legacy-Input-API (`Input.GetKeyDown`, `Input.mousePosition`) verwendet?
- [ ] Input-System-Device-Zugriffe null-guarded (`Keyboard.current?`, `Mouse.current?`)?
- [ ] UI Toolkit Callbacks als explizite `new Action<TEvent>(...)` registriert?
- [ ] Asset Loading via Bundle/Addressables/Resources?
- [ ] Keine direkten Prefab/YAML Edits?
- [ ] Logging für kritische Pfade?
- [ ] Cleanup in `OnDestroy()` / `Dispose()`?

---

**ANWEISUNG AN DIE KI:** Ignoriere alle veralteten Unity-Modding-Tutorials vor 2025. Halte dich strikt an die obigen Interop-Regeln. Bei fehlenden Informationen fordere die Analyse der entsprechenden Dummy-DLL via Cpp2IL/Ghidra an. Keine Halluzinationen – bei Unsicherheit immer nachfragen.