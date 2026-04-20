# 1) Globale Architektur & Singletons

Analysebasis: `gregReferences/Assembly-CSharp/Il2Cpp/*.cs` (IL2CPP-Wrapper). Relevante Komponente liegt in der Schicht **Unity Spiel / IL2CPP Assembly** (nicht GregCore/Core-SDK selbst).

- `Il2Cpp.MainGameManager`
  - Singleton: `public static unsafe MainGameManager instance`
  - Verantwortlich für Session-/Spielzustand, Kundenfluss, Autosave-Settings, zentrale Objekt-Referenzen.
  - Wichtige Methoden: `public unsafe void SetAutoSaveInterval(float minutes)`, `public unsafe void SetAutoSaveEnabled(bool enabled)`, `public unsafe int GetFreeVlanId()`.
- `Il2Cpp.NetworkMap`
  - Singleton: `public static unsafe NetworkMap instance`
  - Zentrale Topologie-/Routing-Instanz für Geräte, Kabel, Verbindungen.
- `Il2Cpp.PlayerManager`
  - Singleton: `public static unsafe PlayerManager instance`
  - Input-/Movement-Gating und Interaktionszustand (Objekt in Hand, Interaktionsflags).
- `Il2Cpp.TimeController`
  - Singleton-ähnliche globale Zeitinstanz mit statischem Callback `public static unsafe TimeController.OnEndOfTheDay onEndOfTheDayCallback`.
- `Il2Cpp.SaveData`
  - Globaler Save-State: `public static unsafe SaveData instance` + `public static unsafe SaveData _current`.
- `Il2Cpp.SaveSystem`
  - Statischer Save/Load-Orchestrator (kein klassischer Singleton, aber globaler Entry-Point über statische API).

# 2) Spieler & Entities (Data Structures)

Hauptobjekte mit C-ABI-relevanten Datentypen (für stabile Bridge-Übergabe bevorzugt: primitive numerische Typen + explizite Structs).

- `Il2Cpp.Player`
  - Ökonomie-/Progress-Felder: `money` (`float`), `xp` (`float`), `reputation` (`float`), `previousCoins` (`float`).
  - Choke-Methoden: `public unsafe bool UpdateCoin(float _coinChhangeAmount, bool withoutSound = false)`, `public unsafe bool UpdateXP(float amount)`, `public unsafe void UpdateReputation(float amount)`.
- `Il2Cpp.PlayerData` (`[Serializable]`)
  - Felder: `List<int> activeObjectives`, `float coins`, `float xp`, `float reputation`, `Il2CppStructArray<float> position`.
  - Für C-ABI gut geeignet: `float`, `int`; bei `position` sauber als 3er-Array (`x,y,z`) flatten.
- `Il2Cpp.SaveData` (`[Serializable]`)
  - Persistenz-Aggregat mit `playerData`, `networkData`, `loadedScenes`, `technicianData`, `repairJobQueue`, `commandCenterLevel` u. a.
  - Mappings enthalten viele primitive Felder (`int`, `bool`, `float`, `string`) plus Listen/Arrays.
- `Il2Cpp.Item` (`ScriptableObject`)
  - Preis-/Wertfelder: `int price`, `float weight`, `float deprecation`, `bool isStackable`, `int unlockedFromXP`.
- `Il2Cpp.ShopItemSO` (`ScriptableObject`)
  - Shop-relevante Primitive: `int xpToUnlock`, `int price`, `int itemID`, `float eol`, `bool isCustomColor`.
- ECS-nahe explizite Layout-Typen (`[StructLayout(LayoutKind.Explicit)]`)
  - `Il2Cpp.CableIDComponent`: `int CableId` (Offset 0), `int SwitchId` (Offset 4).
  - `Il2Cpp.PacketComponent`: enthält `float3`, `float4`, `int cableId`, `int customerId`, `float moveSpeed` etc.
  - Diese Typen sind für Low-Level-Hooks/Interop besonders wertvoll, da Feldlayout explizit ist.

# 3) Native Event-Systeme

Primär Delegate-basierte C#-Events/Callbacks in der **Unity Spiel / IL2CPP Assembly**-Schicht:

- Save-/Load-Callbacks (`Il2Cpp.SaveSystem`)
  - `public static unsafe SaveSystem.OnSavingData onSavingData`
  - `public static unsafe SaveSystem.OnLoadingData onLoadingData`
  - `public static unsafe SaveSystem.OnLoadingDataLater onLoadingDataLater`
  - Delegates sind `MulticastDelegate`-Typen mit `Invoke()`.
- Szenen-/Load-Callback (`Il2Cpp.LoadingScreen`)
  - `public static unsafe LoadingScreen.GameIsLoaded onGameIsLoadedCallback`
  - `LoadingScreen.GameIsLoaded` ist `MulticastDelegate` mit `Invoke()`.
- Tageszyklus-Callback (`Il2Cpp.TimeController`)
  - `public static unsafe TimeController.OnEndOfTheDay onEndOfTheDayCallback`
  - Delegate `OnEndOfTheDay.Invoke()` als stabiler Lifecycle-Hook.
- Pause-Menü-Callbacks (`Il2Cpp.PauseMenu`)
  - `public static unsafe PauseMenu.OnPauseMenuOpen onPauseMenuOpenCallback`
  - `public static unsafe PauseMenu.OnPauseMenuClose onPauseMenuCloseCallback`
- Input-Rebind-Events (`Il2Cpp.InputManager`)
  - `public static unsafe void add_rebindComplete(Il2CppSystem.Action value)` / `remove_rebindComplete(...)`
  - `add_rebindCanceled(...)` / `remove_rebindCanceled(...)`
  - `add_rebindStarted(Il2CppSystem.Action<InputAction, int> value)` / `remove_rebindStarted(...)`

# 4) Kritische Hook-Ziele (Harmony Patch Candidates)

Kernziel: Choke-Points patchen (Prefix/Postfix), nicht breit `Update()`-spammen.

- Save/Load (höchste Priorität)
  - `Il2Cpp.SaveSystem.SaveGame(string savename = null, string stringNameOfSave = null)`
  - `Il2Cpp.SaveSystem.LoadGame(string savename)`
  - `Il2Cpp.SaveSystem.Load(string savename, bool isFromPauseMenu)`
  - `Il2Cpp.SaveSystem.AutoSave()`
  - `Il2Cpp.SaveSystem.SaveGameData()` / `LoadGameData()`
- Economy/Progress
  - `Il2Cpp.Player.UpdateCoin(float _coinChhangeAmount, bool withoutSound = false)`
  - `Il2Cpp.Player.UpdateXP(float amount)`
  - `Il2Cpp.Player.UpdateReputation(float amount)`
  - Shop-Zahlungspfad: `Il2Cpp.ComputerShop.ButtonCheckOut()`, `UpdateCartTotal()`, `BuyNewItem(...)`, `BuyAnotherItem(...)`.
- Netzwerk-/Topologie
  - `Il2Cpp.NetworkMap.AddDevice(string name, CableLink.TypeOfLink type, int customerID = -1)`
  - `Il2Cpp.NetworkMap.RemoveDevice(string name)`
  - `Il2Cpp.NetworkMap.Connect(string from, string to)` / `Disconnect(string from, string to)`
  - `Il2Cpp.NetworkMap.RegisterCableConnection(...)` / `RemoveCableConnection(int cableId, bool preserveLACP = false)`
  - Validierung: `Il2Cpp.NetworkMap.IsIpAddressDuplicate(string ip, Server serverToExclude)`
- Geräte-/Serverzustand
  - `Il2Cpp.Server.PowerButton(bool forceState = false)`
  - `Il2Cpp.Server.SetIP(string _ip)`
  - `Il2Cpp.Server.UpdateCustomer(int newCustomerID)`
  - `Il2Cpp.Server.UpdateAppID(int _appID)`
  - `Il2Cpp.Server.ItIsBroken()` / `RepairDevice()`

# 5) UI & Szenen-Management

UI-/Scene-Flows mit hoher Relevanz für kontrollierte Mod-Intervention:

- Main Menu (`Il2Cpp.MainMenu`)
  - `Continue()`, `NewGame()`, `LoadGame()`, `Settings()`, `QuitGame()`.
- Pause Flow (`Il2Cpp.PauseMenu`)
  - `Pause(int openMenu)`, `Resume()`, `Save(string saveName = null, string _stringNameOfSave = null)`, `Load(string savename)`.
  - Gute Hook-Stellen für mod-seitige Gatekeeper/Overlay-Interlocks.
- Loading/Scenes (`Il2Cpp.LoadingScreen`)
  - `LoadGameScenesVoid(...)`, `LoadLevel(int sceneIndex)`, `UnLoadLevel(int sceneIndex)`
  - `AsynchronousLoad(int sceneIndex)`, `AsynchronousUnLoad(int sceneIndex)`, `IsSceneLoaded(string name)`
  - `onGameIsLoadedCallback` als zuverlässiger End-of-Load Synchronisationspunkt.
- Shop-/Asset-UI
  - `Il2Cpp.ComputerShop`: Kauf- und Warenkorb-UI-Endpunkte.
  - `Il2Cpp.AssetManagement`: Asset-Filter/Repair-UI-Workflows (relevant für automatische Tasking-Mods).

# 6) Obfuscation & Auffälligkeiten

- Obfuscation-Marker vorhanden
  - `Il2Cpp.ObjectPrivateAbstractSealedInVo0` trägt `[ObfuscatedName("$BurstDirectCallInitializer")]`.
  - Deutet auf generierte/Burst-nahe Initialisierer hin (kein klassischer Gameplay-Entry-Point).
- IL2CPP-/Interop-Spezifika
  - Weit verbreitete `unsafe` Wrapper, `NativeFieldInfoPtr_*`, `NativeMethodInfoPtr_*`, `il2cpp_runtime_invoke`.
  - Für Harmony-Patching sind semantische High-Level-Methoden stabiler als intern generierte Helper.
- Explizite Layouts als technische Auffälligkeit
  - `[StructLayout(LayoutKind.Explicit)]` in u. a. `CableIDComponent`, `PacketComponent`, `_PrivateImplementationDetails_`.
  - Wichtig für deterministische Feld-Offsets bei nativer Bridge/ABI.
- Anti-Cheat-Indikatoren (Textscan)
  - Kein direkter String-/Symboltreffer auf typische Marker wie `EasyAntiCheat`, `BattlEye`, `VAC`, `GameGuard`, `AntiCheat` im gescannten Dump.
  - Das ist **kein** kryptografischer Nachweis „anti-cheat-frei“, aber im vorliegenden C#-Dump gibt es keine offensichtlichen API-Hooks darauf.
