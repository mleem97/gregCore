# gregCore FrameworkAPI Reference

> **Version:** 1.1.0 | **Generated:** 2026-06-28
>
> Auto-generated from `game_hooks.json` and `framework/greg_hooks.json`.
> Do not edit manually — re-run the generator or push changes to the source JSON files.

---

## Table of Contents

1. [Game Hooks (game_hooks.json)](#1-game-hooks)
2. [Greg Hooks (greg_hooks.json)](#2-greg-hooks)

---

## 1. Game Hooks

These hooks describe patchable methods extracted from the game's IL2CPP assembly.
They serve as the raw targets for Harmony Prefix/Postfix patches.

### Audio

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AudioManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.AudioManager` | `FadeIn` | `IEnumerator` | `(AudioSource audioSource, Single FadeTime, Single finalVolume)` |
| `Il2Cpp.AudioManager` | `FadeOut` | `IEnumerator` | `(AudioSource audioSource, Single FadeTime)` |
| `Il2Cpp.AudioManager` | `FadeOut_FadeIn` | `IEnumerator` | `(AudioSource audioSource, Single FadeTime, Single finalVolume, AudioClip newAudioClip)` |
| `Il2Cpp.AudioManager` | `SetEffectsVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetMasterVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetMusic` | `Void` | `(Int32 _clipUID)` |
| `Il2Cpp.AudioManager` | `SetMusicVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetRacksVolume` | `Void` | `(Single _volume)` |
| `Il2CppPolyStang.CarController` | `HandleAudio` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `PlayLandingSound` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `PlayRequestedStepSound` | `Void` | `(Int32 _clipArray)` |
| `Il2CppviperOSK.OSK_KeySounds` | `PlaySound` | `Void` | `(Int32 k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `ClickSound` | `Void` | `(Int32 keytypecode)` |
| `Il2Cpp.Rack` | `UpdateAudioVolume` | `Void` | `()` |
| `Il2Cpp.SettingsVolume` | `EffectVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `MasterVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `MusicVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `RacksVolume` | `Void` | `(Single volume)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `PlayStepSound` | `Void` | `()` |

### Character

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AICharacterControl` | `AgentReachTarget` | `Boolean` | `()` |
| `Il2Cpp.AICharacterControl` | `AnimSit` | `Void` | `(Boolean active)` |
| `Il2Cpp.AICharacterControl` | `GotoNextPoint` | `Void` | `(Il2CppReferenceArray`1 _waypoints)` |
| `Il2Cpp.AICharacterControl` | `OnCreated` | `Void` | `(UMAData umadata)` |
| `Il2Cpp.AICharacterControl` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.AICharacterControl` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AICharacterControl` | `SetStopLoopingDestinationPoints` | `Void` | `()` |
| `Il2Cpp.AICharacterControl` | `SetTarget` | `Void` | `(Vector3 target)` |
| `Il2Cpp.AICharacterControl` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.AICharacterControl` | `StartingAnimation` | `Void` | `()` |
| `Il2Cpp.AICharacterControl` | `Update` | `Void` | `()` |
| `Il2Cpp.AICharacterControl` | `moveBack` | `Void` | `(Vector3 direction)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_A` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_BPM` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_CDG` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_FV` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_O` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_U` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `MouthShape_none` | `Void` | `(Single t)` |
| `Il2Cpp.AICharacterExpressions` | `OnCreated` | `Void` | `(UMAData umadata)` |
| `Il2Cpp.AICharacterExpressions` | `Start` | `Void` | `()` |
| `Il2Cpp.AICharacterExpressions` | `Talk` | `Void` | `(String sentence)` |
| `Il2Cpp.AICharacterExpressions` | `Talking` | `IEnumerator` | `(List`1 _syllables)` |
| `Il2Cpp.CablePositions` | `RemoveLastPosition` | `Transform` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `RemovePosition` | `Void` | `(Int32 cableId)` |
| `Il2CppTMPro.Examples.CameraController` | `GetPlayerInput` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `Move` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `PerformOverlapCheck` | `Void` | `()` |
| `Il2Cpp.Firewall` | `RemoveRule` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `Crouch` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `GetInput` | `Void` | `(Single& speed)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `GetMouseLook` | `MouseLook` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `HandleZoom` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `OnControllerColliderHit` | `Void` | `(ControllerColliderHit hit)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `ProgressStepCycle` | `Void` | `(Single speed)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `ResetCameraPosition` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `RotateView` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `Start` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `StopCrouching` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `Update` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `UpdateCameraPosition` | `Void` | `(Single speed)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `UpdateNormalFov` | `Void` | `(Single fov)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_0` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_1` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_2` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_3` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_4` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_5` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_6` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `_Start_b__57_7` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.InputManager` | `LockedCursorForPlayerMovement` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `CloseAnyCanvas` | `Void` | `(Boolean isCustomerChoice)` |
| `Il2Cpp.MainGameManager` | `GetFreeSubnet` | `String` | `(Single appRequirements)` |
| `Il2Cpp.MainGameManager` | `GetFreeVlanId` | `Int32` | `()` |
| `Il2Cpp.MainGameManager` | `InitializeVlanPool` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `IsSubnetValid` | `Boolean` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `OpenAnyCanvas` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `RemoveUsedSubnet` | `Void` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `RemoveUsedVlanId` | `Void` | `(Int32 vlanId)` |
| `Il2Cpp.MainGameManager` | `ReturnSubnet` | `Void` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `ReturnVlanId` | `Void` | `(Int32 vlanId)` |
| `Il2Cpp.MainGameManager` | `ShuffleAvailableSubnets` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `Start` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `_Awake_b__67_0` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `LookRotation` | `Void` | `(Transform character, Transform camera, Quaternion externalRotation, Transform ladderTrigger)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `MouseLookOnDisable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `RemoveConsole` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `JoystickInput` | `Vector2` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `DpadMove` | `Void` | `(Vector2 dir)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Cancel` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Horizontal` | `Void` | `(Single f)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Vertical` | `Void` | `(Single f)` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `TMPInputFieldReActivate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `DpadMove` | `Void` | `(Vector2 dir)` |
| `Il2Cpp.Player` | `TurnOnCharacterControllerDelayed` | `IEnumerator` | `()` |
| `Il2Cpp.PlayerManager` | `LockedCursorForPlayerMovement` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `PlayerStopMovement` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `HandleLookAtRay` | `Void` | `(Transform character)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `IsPointsMoved` | `Boolean` | `()` |
| `Il2Cpp.Router` | `RemoveRoute` | `Void` | `(Int32 sourceVlanId)` |
| `Il2Cpp.SFPModule` | `RemoveFromPort` | `Void` | `()` |
| `Il2Cpp.SettingsControls` | `LookSensitivity` | `Void` | `(Single fl)` |
| `Il2Cpp.SettingsGraphics` | `MoveToMonitorCoroutine` | `IEnumerator` | `(DisplayInfo targetDisplay)` |
| `Il2Cpp.ShopCartItem` | `OnRemoveClicked` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `CopyAnimationCurve` | `AnimationCurve` | `(AnimationCurve curve)` |
| `Il2Cpp.StaticUIElements` | `RemoveCustomKeyHint` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ShowInputNumpadtOverlay` | `Void` | `(String title, Action`1 onConfirmed, GameObject selectOnClose)` |
| `Il2Cpp.TerrainDetector` | `ConvertToSplatMapCoordinate` | `Vector3` | `(Vector3 worldPosition)` |
| `Il2Cpp.TerrainDetector` | `SetCurrentTerrain` | `Void` | `(Terrain _terrain)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `Awake` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `HandleGroundedMovement` | `Void` | `(Boolean crouch, Boolean jump)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `Move` | `Void` | `(Vector3 move, Boolean crouch, Boolean jump, Boolean onlyturn, Boolean backward)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `OnAnimationEventFootStep` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `OnAnimatorMove` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `MoveBetweenPositions` | `Void` | `(Vector3 _position, Vector3 _rotation)` |
| `Il2Cpp.UsableObject` | `MoveToHand` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `MoveToStorage` | `Void` | `(Transform _pos, Int32 _positionIndex, Int32 _storageUid)` |
| `Il2Cpp.UsableObject` | `RemoveRigidbody` | `Void` | `()` |
| `Il2CppTMPro.Examples.WarpTextExample` | `CopyAnimationCurve` | `AnimationCurve` | `(AnimationCurve curve)` |
| `Il2Cpp.WaypointInitializationSystem` | `CleanUpSystem` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `CollectPortPosition` | `Void` | `(String switchID, Vector3 position, Dictionary`2 switchToPos, Dictionary`2 prefixToPos)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetEvaluationCooldown` | `Single` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCreate` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCreateForCompiler` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnUpdate` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `SetEvaluationCooldown` | `Void` | `(Single seconds)` |
| `Il2Cpp.WaypointInitializationSystem` | `__AssignQueries` | `Void` | `(SystemState& state)` |
| `Il2Cpp._PrivateImplementationDetails_` | `ComputeStringHash` | `UInt32` | `(String s)` |
| `Il2CppviperTools.viperInput` | `GetControllerNames` | `Il2CppStringArray` | `()` |
| `Il2CppviperTools.viperInput` | `GetPlayerJoystickInput` | `Vector2` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `NumControllers` | `Int32` | `()` |

### CustomImport

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ModLoader` | `Awake` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `CreateMaterial` | `Material` | `(String folderPath, String textureFile)` |
| `Il2Cpp.ModLoader` | `GetModPrefab` | `GameObject` | `(Int32 modID)` |
| `Il2Cpp.ModLoader` | `GetModPrefabByFolder` | `GameObject` | `(String folderName)` |
| `Il2Cpp.ModLoader` | `Start` | `Void` | `()` |
| `Il2Cpp.ObjImporter` | `ImportOBJ` | `Mesh` | `(String filePath)` |
| `Il2Cpp.ObjImporter` | `ParseFloat` | `Single` | `(String s)` |
| `Il2Cpp.ObjImporter` | `ProcessFaceVertex` | `Int32` | `(String faceVertex, List`1 positions, List`1 uvs, List`1 normals, List`1 outVerts, List`1 outUVs, List`1 outNorms, Dictionary`2 cache)` |

### DevTools

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.GODMOD` | `Awake` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `GODMOD_delayed` | `IEnumerator` | `()` |
| `Il2Cpp.GODMOD` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `StartGodMod` | `Void` | `()` |
| `Il2Cpp.RackMount` | `CheatInsertRack` | `Void` | `(GameObject go, Int32 type)` |

### Economy

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.BalanceSheet` | `AddExpenseRow` | `Void` | `(String label, Single amount)` |
| `Il2Cpp.BalanceSheet` | `AddHeaderRow` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `AddRow` | `Void` | `(String name, Single revenue, Single penalties, Single total, Sprite logo)` |
| `Il2Cpp.BalanceSheet` | `AddSectionTitle` | `Void` | `(String title)` |
| `Il2Cpp.BalanceSheet` | `AddTotalRow` | `Void` | `(Single revenue, Single penalties, Single total)` |
| `Il2Cpp.BalanceSheet` | `Awake` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `ClearRows` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `CountFailingApps` | `Int32` | `(CustomerBase cb)` |
| `Il2Cpp.BalanceSheet` | `FillInBalanceSheet` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `GetLatestSnapshot` | `MonthlySnapshot` | `()` |
| `Il2Cpp.BalanceSheet` | `GetOrCreateRecord` | `CustomerRecord` | `(CustomerItem item)` |
| `Il2Cpp.BalanceSheet` | `InstantiateRow` | `BalanceSheetRow` | `()` |
| `Il2Cpp.BalanceSheet` | `RegisterRepairExpense` | `Void` | `(Single amount)` |
| `Il2Cpp.BalanceSheet` | `RegisterSalary` | `Void` | `(Int32 monthlySalary)` |
| `Il2Cpp.BalanceSheet` | `RegisterShopExpense` | `Void` | `(Single amount)` |
| `Il2Cpp.BalanceSheet` | `RestoreRecord` | `CustomerRecord` | `(CustomerRecordSaveData recData)` |
| `Il2Cpp.BalanceSheet` | `Start` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `TrackFinances` | `IEnumerator` | `()` |
| `Il2Cpp.BalanceSheetRow` | `SetAsExpenseRow` | `Void` | `(String label, Single amount)` |
| `Il2Cpp.BalanceSheetRow` | `SetAsHeader` | `Void` | `()` |
| `Il2Cpp.BalanceSheetRow` | `SetAsSectionTitle` | `Void` | `(String title)` |
| `Il2Cpp.BalanceSheetRow` | `SetAsTotalRow` | `Void` | `(Single revenue, Single penalties, Single total)` |
| `Il2Cpp.BalanceSheetRow` | `SetData` | `Void` | `(String customerName, String revenue, String penalties, String total, Sprite customerLogo)` |
| `Il2Cpp.CableSpinner` | `ApplyColor` | `Void` | `(Color color, String rgbString)` |
| `Il2Cpp.ComputerShop` | `ApplyColorToSpawnedItem` | `Void` | `(Int32 uid, Color color, ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `Awake` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonBalanceSheetScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonBuyShopItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Boolean isCustomColor)` |
| `Il2Cpp.ComputerShop` | `ButtonCheckOut` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonShopScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `BuyAnotherItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, ShopCartItem cartItem)` |
| `Il2Cpp.ComputerShop` | `BuyNewItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName)` |
| `Il2Cpp.ComputerShop` | `CleanUpShop` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `CloseShop` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `GetPrefabForItem` | `GameObject` | `(Int32 itemID, ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `UpdateCartTotal` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `_Awake_b__36_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.CustomerBase` | `AddAppPerformance` | `Void` | `(Int32 appID, Single speed)` |
| `Il2Cpp.CustomerBase` | `AppText` | `String` | `(Int32 lastUsedApp)` |
| `Il2Cpp.CustomerBase` | `AppText` | `String` | `(Int32 appID, String subnet)` |
| `Il2Cpp.CustomerBase` | `AreAllAppRequirementsMet` | `Boolean` | `()` |
| `Il2Cpp.CustomerBase` | `Awake` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `CheckIfAppRequirementsAreMet` | `IEnumerator` | `()` |
| `Il2Cpp.CustomerBase` | `DelayedAppDoorOpening` | `IEnumerator` | `(Int32 appID)` |
| `Il2Cpp.CustomerBase` | `GetAppIDForIP` | `Int32` | `(String ip)` |
| `Il2Cpp.CustomerBase` | `GetAppsSpeedRequirements` | `Il2CppStructArray`1` | `()` |
| `Il2Cpp.CustomerBase` | `GetEffectiveMoneySpeed` | `Single` | `()` |
| `Il2Cpp.CustomerBase` | `GetSubnetsPerApp` | `Dictionary`2` | `()` |
| `Il2Cpp.CustomerBase` | `GetTotalAppSpeed` | `Single` | `()` |
| `Il2Cpp.CustomerBase` | `GetVlanIdsPerApp` | `Dictionary`2` | `()` |
| `Il2Cpp.CustomerBase` | `ResetAllAppSpeeds` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `SetUpApp` | `Void` | `(Int32 appID, Int32 difficulty, CustomerBaseSaveData saveData)` |
| `Il2Cpp.CustomerBase` | `SetUpBase` | `Void` | `(CustomerItem customerItem, CustomerBaseSaveData saveData)` |
| `Il2Cpp.CustomerBase` | `Start` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `UpdateCustomerServerCountAndSpeed` | `Void` | `(Int32 count, Single speed)` |
| `Il2Cpp.CustomerBase` | `UpdateMoney` | `IEnumerator` | `()` |
| `Il2Cpp.CustomerBase` | `UpdateSpeedOnCustomerBaseApp` | `Void` | `(Int32 appID, Single speed)` |
| `Il2Cpp.CustomerBaseDoor` | `Awake` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OpenDoor` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OpenDoorAndSetupBase` | `Void` | `(CustomerItem customerItem)` |
| `Il2Cpp.CustomerCard` | `SetCustomer` | `Void` | `(CustomerItem _customerItem)` |
| `Il2Cpp.CustomerColor` | `GetColorForCustomerId` | `Color` | `(Int32 customerId)` |
| `Il2Cpp.CustomerColor` | `GetColorForCustomerIdFloat4` | `float4` | `(Int32 customerId)` |
| `Il2Cpp.GetValueFromPlayerPrefs` | `Start` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonCancelBuying` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonBuyWall` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCancelBuyWall` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCancelCustomerChoice` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCustomerChosen` | `Void` | `(Int32 _cardID)` |
| `Il2Cpp.MainGameManager` | `CreateFallbackCustomer` | `CustomerItem` | `(CustomerItem original, Int32 customerBaseID)` |
| `Il2Cpp.MainGameManager` | `GetAppLogo` | `Sprite` | `(Int32 customerID, Int32 appID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerItemByID` | `CustomerItem` | `(Int32 customerID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerLogo` | `Sprite` | `(Int32 customerID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerTotalRequirement` | `Single` | `(CustomerItem customer)` |
| `Il2Cpp.MainGameManager` | `GetSfpBoxPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `IsCustomerSuitableForBase` | `Boolean` | `(CustomerItem customer, Int32 customerBaseID)` |
| `Il2Cpp.MainGameManager` | `OnApplicationQuit` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ShowBuyWallCanvas` | `Void` | `(Wall wall)` |
| `Il2Cpp.MainGameManager` | `ShowCustomerCardsCanvas` | `Void` | `(CustomerBaseDoor _door)` |
| `Il2Cpp.MainGameManager` | `ShuffleAvailableCustomers` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `_ShuffleAvailableCustomers_b__79_0` | `Boolean` | `(Int32 index)` |
| `Il2Cpp.MainGameManager` | `_ShuffleAvailableCustomers_b__79_1` | `Int32` | `(Int32 index)` |
| `Il2Cpp.ModLoader` | `CreateShopButton` | `Void` | `(Int32 modID, ShopItemConfig config, Sprite icon)` |
| `Il2Cpp.ModLoader` | `CreateShopTemplate` | `GameObject` | `(ShopItemConfig config, Mesh mesh, Material material, String folderName)` |
| `Il2Cpp.ModShopItem` | `ButtonBuyItem` | `Void` | `()` |
| `Il2Cpp.ModShopItem` | `Initialize` | `Void` | `(Int32 modID, ShopItemConfig config, Sprite icon)` |
| `Il2Cpp.MusicPlayer` | `Awake` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `PlayRandomSong` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `Update` | `Void` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllCustomerBases` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetCustomerBase` | `CustomerBase` | `(Int32 customerId)` |
| `Il2Cpp.NetworkMap` | `RegisterCustomerBase` | `Void` | `(CustomerBase customerBase)` |
| `Il2Cpp.NetworkMap` | `UpdateCustomerServerCountAndSpeed` | `Void` | `(Int32 customerId, Int32 serverCount, Single speed)` |
| `Il2Cpp.NetworkMap` | `UpdateDeviceCustomerID` | `Void` | `(String deviceName, Int32 customerID)` |
| `Il2Cpp.NetworkSwitch` | `AppendEolTime` | `Void` | `(StringBuilder builder, Int32 eolSeconds)` |
| `Il2CppviperOSK.OSK_Keyboard` | `RemapPhysicalKeyboard` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `RemapPhysicalKeyboard` | `Void` | `()` |
| `Il2Cpp.Objectives` | `CreateAppObjective` | `Int32` | `(Int32 customerID, Int32 appID, Int32 time, Int32 requiredIOPS)` |
| `Il2Cpp.Player` | `CheckFallsThroughMap` | `Void` | `()` |
| `Il2Cpp.Player` | `DropAllItems` | `Void` | `()` |
| `Il2Cpp.Player` | `Start` | `Void` | `()` |
| `Il2Cpp.Player` | `UpdateCoin` | `Boolean` | `(Single _coinChhangeAmount, Boolean withoutSound)` |
| `Il2Cpp.Player` | `UpdateReputation` | `Void` | `(Single amount)` |
| `Il2Cpp.Player` | `UpdateXP` | `Boolean` | `(Single amount)` |
| `Il2Cpp.Player` | `WarpPlayer` | `Void` | `(Vector3 _position, Quaternion _rotation)` |
| `Il2Cpp.PlayerHit` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `DefaultActionEffect` | `Void` | `(Vector3 _position, Single _time)` |
| `Il2Cpp.PlayerManager` | `GainIOPSEffect` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `Start` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `WaitForActionToFinish` | `IEnumerator` | `(Vector3 _position, Single _time)` |
| `Il2Cpp.RackMount` | `ApplyMaterialToLODs` | `Void` | `(GameObject rackGO, Material mat)` |
| `Il2Cpp.Router` | `ApplyRouteToCustomerBases` | `Void` | `(SubnetRoute route)` |
| `Il2Cpp.Router` | `ReapplyAllRoutes` | `Void` | `()` |
| `Il2Cpp.Router` | `WithdrawRouteFromCustomerBases` | `Void` | `(SubnetRoute route)` |
| `Il2Cpp.Server` | `AppendEolTime` | `Void` | `(StringBuilder builder, Int32 eolSeconds)` |
| `Il2Cpp.Server` | `ButtonClickChangeCustomer` | `Void` | `(Boolean forward)` |
| `Il2Cpp.Server` | `GetCustomerID` | `Int32` | `()` |
| `Il2Cpp.Server` | `GetNextCustomerID` | `Int32` | `(Int32 currentCustomerID, Boolean forward)` |
| `Il2Cpp.Server` | `UpdateAppID` | `Void` | `(Int32 _appID)` |
| `Il2Cpp.Server` | `UpdateCustomer` | `Void` | `(Int32 newCustomerID)` |
| `Il2Cpp.SettingsGraphics` | `SetExposure` | `Void` | `(Single exposure)` |
| `Il2Cpp.ShopCartItem` | `Initialize` | `Void` | `(ComputerShop shop, String itemName, Int32 itemID, Int32 price, ObjectInHand itemType, Int32 firstSpawnUID, Nullable`1 customColor)` |
| `Il2Cpp.ShopCartItem` | `OnAddClicked` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `UpdateDisplay` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `Awake` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `ButtonBuyItem` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `BuyItem` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `Start` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `TryUnlock` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `UpdateVisualState` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `UpdateCoinsAndPrestige_TopLeft` | `IEnumerator` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `ApplyExtraTurnRotation` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `DoesCableServeMultipleCustomers` | `Boolean` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCustomerRoutes` | `Dictionary`2` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCustomersUsingCable` | `HashSet`1` | `(CableInfo cable)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateServerCustomerID` | `Void` | `(String serverID, Int32 customerID)` |

### Facility

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CheckIfTouchingWall` | `Awake` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `DelayedOverlapCheck` | `IEnumerator` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `SetRenderersEnabled` | `Void` | `(Boolean isEnabled)` |
| `Il2Cpp.CheckIfTouchingWall` | `Start` | `Void` | `()` |
| `Il2Cpp.Firewall` | `AddRule` | `Void` | `(Int32 portIndex, Int32 vlanId, String ipCidr, Boolean allow)` |
| `Il2Cpp.Firewall` | `InsertedInRack` | `Void` | `(SwitchSaveData saveData)` |
| `Il2Cpp.Firewall` | `IsTrafficAllowed` | `Boolean` | `(Int32 portIndex, Int32 vlanId, String ip)` |
| `Il2Cpp.MainGameManager` | `GetFirewallPrefab` | `GameObject` | `(Int32 firewallType)` |
| `Il2Cpp.NetworkMap` | `GetAllFirewalls` | `IEnumerable`1` | `()` |
| `Il2Cpp.Technician` | `SendToContainer` | `IEnumerator` | `()` |
| `Il2Cpp.Wall` | `Awake` | `Void` | `()` |
| `Il2Cpp.Wall` | `OpenWall` | `Void` | `()` |

### Hardware

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AssetManagement` | `ButtonCancelSendingTechnician` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonConfirmSendingTechnician` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterBroken` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterServers` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterSwitches` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `GetItemCount` | `Int32` | `()` |
| `Il2Cpp.AssetManagement` | `OnAutoRepairDropdownChanged` | `Void` | `(Int32 index)` |
| `Il2Cpp.AssetManagement` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `PopulateAutoRepairDropdown` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `SendTechnician` | `Void` | `(NetworkSwitch networkSwitch, Server server)` |
| `Il2Cpp.AssetManagement` | `SetCell` | `Void` | `(ICell cell, Int32 index)` |
| `Il2Cpp.AssetManagement` | `UpdateTechnicianInformation` | `Void` | `()` |
| `Il2Cpp.AssetManagementDeviceLine` | `ButtonSendTechnician` | `Void` | `()` |
| `Il2Cpp.AssetManagementDeviceLine` | `SetupLine` | `Void` | `(AssetManagementDeviceLineData data, Int32 index)` |
| `Il2Cpp.CommandCenter` | `AutoRepairRoutine` | `IEnumerator` | `()` |
| `Il2Cpp.CommandCenter` | `SetAutoRepairMode` | `Void` | `(Int32 mode)` |
| `Il2Cpp.MainGameManager` | `GetServerPrefab` | `GameObject` | `(Int32 serverType)` |
| `Il2Cpp.MainGameManager` | `GetSwitchPrefab` | `GameObject` | `(Int32 switchType)` |
| `Il2Cpp.MainGameManager` | `ReturnServerNameFromType` | `String` | `(Int32 type)` |
| `Il2Cpp.MainGameManager` | `ReturnSwitchNameFromType` | `String` | `(Int32 type)` |
| `Il2Cpp.NetworkMap` | `AddBrokenServer` | `Void` | `(Server server)` |
| `Il2Cpp.NetworkMap` | `AddBrokenSwitch` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkMap` | `GetAllBrokenServers` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllBrokenSwitches` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllServers` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetServer` | `Server` | `(String serverId)` |
| `Il2Cpp.NetworkMap` | `GetSwitchById` | `NetworkSwitch` | `(String switchId)` |
| `Il2Cpp.NetworkMap` | `RegisterServer` | `Void` | `(Server server)` |
| `Il2Cpp.NetworkMap` | `RegisterSwitch` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkMap` | `RemoveBrokenServer` | `Void` | `(String serverId)` |
| `Il2Cpp.NetworkMap` | `RemoveBrokenSwitch` | `Void` | `(String switchId)` |
| `Il2Cpp.NetworkSwitch` | `GenerateUniqueSwitchId` | `String` | `()` |
| `Il2Cpp.NetworkSwitch` | `ItIsBroken` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `PatchStaleSwitchId` | `Void` | `(CableEndpoint& endpoint)` |
| `Il2Cpp.NetworkSwitch` | `PowerButton` | `Void` | `(Boolean forceState)` |
| `Il2Cpp.NetworkSwitch` | `SetPowerLightMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.NetworkSwitch` | `SwitchInsertedInRack` | `Void` | `(SwitchSaveData switchSaveData)` |
| `Il2Cpp.NetworkSwitch` | `_SwitchInsertedInRack_b__26_0` | `Boolean` | `(NetworkSwitch s)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ButtonPower` | `Void` | `()` |
| `Il2Cpp.Rack` | `Awake` | `Void` | `()` |
| `Il2Cpp.Rack` | `IsPositionAvailable` | `Boolean` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `MarkPositionAsUnused` | `Void` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `MarkPositionAsUsed` | `Void` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `Start` | `Void` | `()` |
| `Il2Cpp.Rack` | `UnmountRack` | `IEnumerator` | `()` |
| `Il2Cpp.RackAudioCuller` | `Awake` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `CullLoop` | `IEnumerator` | `()` |
| `Il2Cpp.RackAudioCuller` | `Register` | `Void` | `(Rack rack)` |
| `Il2Cpp.RackAudioCuller` | `Start` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `Unregister` | `Void` | `(Rack rack)` |
| `Il2Cpp.RackDoor` | `Awake` | `Void` | `()` |
| `Il2Cpp.RackMount` | `Awake` | `Void` | `()` |
| `Il2Cpp.RackMount` | `InstallRack` | `IEnumerator` | `(Boolean cheat, Int32 type)` |
| `Il2Cpp.RackMount` | `InstantiateRack` | `GameObject` | `(InteractObjectData saveData)` |
| `Il2Cpp.RackPosition` | `Awake` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `InsertItemInRack` | `IEnumerator` | `()` |
| `Il2Cpp.RackPosition` | `IsAllowedItem` | `Boolean` | `(Boolean checkAvailability)` |
| `Il2Cpp.RackPosition` | `SetUsed` | `Void` | `(Boolean used)` |
| `Il2Cpp.Router` | `ButtonShowNRouterSwitchConfig` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `Awake` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `GetFreeSpaceInTheBox` | `Int32` | `()` |
| `Il2Cpp.SFPBox` | `ParentTheObjectWithDelay` | `IEnumerator` | `(Transform uo, Int32 index)` |
| `Il2Cpp.SFPModule` | `Awake` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `SlideIntoPort` | `IEnumerator` | `(Transform port)` |
| `Il2Cpp.Server` | `GenerateUniqueServerId` | `String` | `()` |
| `Il2Cpp.Server` | `ItIsBroken` | `Void` | `()` |
| `Il2Cpp.Server` | `PowerButton` | `Void` | `(Boolean forceState)` |
| `Il2Cpp.Server` | `ServerInsertedInRack` | `Void` | `(ServerSaveData serverSaveData)` |
| `Il2Cpp.Server` | `SetPowerLightMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.Server` | `UpdateServerScreenUI` | `Void` | `()` |
| `Il2Cpp.Server` | `_ServerInsertedInRack_b__39_0` | `Boolean` | `(Server s)` |
| `Il2Cpp.SetIP` | `Awake` | `Void` | `()` |
| `Il2Cpp.SetIP` | `CidrToSubnetMask` | `Void` | `(Int32 cidr, Int32& m1, Int32& m2, Int32& m3, Int32& m4)` |
| `Il2Cpp.SetIP` | `ClickNumber` | `Void` | `(String number)` |
| `Il2Cpp.SetIP` | `CloseCanvas` | `Void` | `()` |
| `Il2Cpp.SetIP` | `GetMaskFromCidr` | `String` | `(Int32 cidr)` |
| `Il2Cpp.SetIP` | `IncrementOctets` | `Void` | `(Int32& o1, Int32& o2, Int32& o3, Int32& o4)` |
| `Il2Cpp.SetIP` | `PowerButton` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ShowCanvas` | `Void` | `(Server _server)` |
| `Il2Cpp.SetIP` | `Update` | `Void` | `()` |
| `Il2Cpp.SetIP` | `_Awake_b__11_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.Technician` | `Awake` | `Void` | `()` |
| `Il2Cpp.Technician` | `GettingNewServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `ReplacingServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `RotateTowardsGoal` | `Void` | `(Vector3 goal)` |
| `Il2Cpp.Technician` | `SetHandIKWeight` | `IEnumerator` | `(Single targetWeight, Single duration)` |
| `Il2Cpp.Technician` | `Start` | `Void` | `()` |
| `Il2Cpp.Technician` | `ThrowingOutServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `Update` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `AddTechnician` | `Void` | `(Technician technician)` |
| `Il2Cpp.TechnicianManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `FireTechnician` | `Void` | `(Int32 technicianID)` |
| `Il2Cpp.TechnicianManager` | `SendTechnician` | `Void` | `(NetworkSwitch networkSwitch, Server server)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetReferencedSwitchIds` | `HashSet`1` | `(NetworkSaveData networkData)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetServerProcessingSpeed` | `Single` | `(String serverName)` |

### Ignored

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ObjectPrivateAbstractSealedInVo0` | `Initialize` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.WarpTextExample` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.WarpTextExample` | `Start` | `Void` | `()` |

### Input

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ActionKeyHint` | `CustomKey` | `Void` | `(InputAction action, String _customText)` |
| `Il2Cpp.ActionKeyHint` | `GetBindingInfo` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnValidate` | `Void` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyCode` | `OSK_KeyCode` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyName` | `String` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyTransform` | `Transform` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `KeyType` | `OSK_KEY_TYPES` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `OnKeyDepress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.I_OSK_Key` | `OnKeyPress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2Cpp.InputManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.InputManager` | `DoRebind` | `Void` | `(InputAction actionToRebind, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean allCompositeParts, Boolean excludeMouse)` |
| `Il2Cpp.InputManager` | `ForceMousePositionToCenterOfGameWindow` | `Void` | `()` |
| `Il2Cpp.InputManager` | `GetBindingName` | `String` | `(String actionName, Int32 bindingIndex)` |
| `Il2Cpp.InputManager` | `ResetBinding` | `Void` | `(String actionName, Int32 bindingIndex)` |
| `Il2Cpp.InputManager` | `StartRebind` | `Void` | `(String actionName, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean excludeMouse)` |
| `Il2Cpp.InputManager` | `_Awake_b__14_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.KeyHint` | `SetInactiveAll` | `Void` | `()` |
| `Il2Cpp.KeyHint` | `ShowKeyboadMelee` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnWaitForPressKey` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetKeyCode` | `OSK_KeyCode` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetKeyName` | `String` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetKeyTransform` | `Transform` | `()` |
| `Il2CppviperOSK.OSK_Key` | `KeyFont` | `Void` | `(TMP_FontAsset keyfont)` |
| `Il2CppviperOSK.OSK_Key` | `KeyType` | `OSK_KEY_TYPES` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnKeyDepress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `OnKeyPress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `OnMouseDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnMouseUp` | `Void` | `()` |
| `Il2CppviperOSK.OSK_KeyTypeMeta` | `KeyType` | `OSK_KEY_TYPES` | `(OSK_KeyCode key)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetKeyCode` | `KeyCode` | `(String c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `HasKey` | `Boolean` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyBackspace` | `Void` | `(OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyCall` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyCallBase` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyDelete` | `Void` | `(OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyShift` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `OnPhysicalKeyStroke` | `Void` | `(Char c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `ReHighlightKey` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ResizeKeyToFit` | `Void` | `(Vector2 scrSize)` |
| `Il2CppviperOSK.OSK_Keymap` | `GenKeyMapDict` | `Dictionary`2` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `GenKeyMapStr` | `String` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `GetCorrectedKey` | `String` | `(String key)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SetBaseKey` | `Void` | `(GameObject base_key)` |
| `Il2CppviperOSK.OSK_Receiver` | `OnMouseDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `OnMouseUp` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetKeyCode` | `OSK_KeyCode` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetKeyName` | `String` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetKeyTransform` | `Transform` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `KeyFont` | `Void` | `(TMP_FontAsset keyfont)` |
| `Il2CppviperOSK.OSK_UI_Key` | `KeyType` | `OSK_KEY_TYPES` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnKeyDepress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnKeyPress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `HasKey` | `Boolean` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `KeyCall` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `KeyCallBase` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `ResizeKeyToFit` | `Void` | `(Vector2 scrSize)` |
| `Il2Cpp.SettingsControls` | `InvertY` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `CreateCustomKeyHint` | `GameObject` | `(InputAction action, Int32 textUID, Transform parent, Boolean isPermanent)` |
| `Il2CppviperTools.viperInput` | `AnyPhysicalKey` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `ConvertKeyCodeToKey` | `Key` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `ConvertToLegacyAxis` | `String` | `(AXIS_INPUT axis)` |
| `Il2CppviperTools.viperInput` | `Fire1` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `GetAllAxis` | `Single` | `()` |
| `Il2CppviperTools.viperInput` | `GetAxis` | `Single` | `(AXIS_INPUT axis)` |
| `Il2CppviperTools.viperInput` | `GetPhysicalKey` | `String` | `()` |
| `Il2CppviperTools.viperInput` | `IsLetterAZ` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyDown` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyPress` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyUp` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `RegisterKeyStrokeCallback` | `Void` | `(Action`1 action, Boolean enable)` |
| `Il2CppviperTools.viperInput` | `ResetAllAxis` | `Void` | `()` |
| `Il2CppviperTools.viperInput` | `Start` | `Void` | `()` |

### Interaction

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CableLink` | `IsAllowedToDoSecondAction` | `Boolean` | `()` |
| `Il2Cpp.CableLink` | `LabelActionOnClick` | `Void` | `()` |
| `Il2Cpp.CableLink` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.CableLink` | `SecondActionOnClick` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.GateLever` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Interact` | `Awake` | `Void` | `()` |
| `Il2Cpp.Interact` | `IsAllowedToDoSecondAction` | `Boolean` | `()` |
| `Il2Cpp.Interact` | `LabelActionOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Interact` | `SecondActionOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `_LabelActionOnClick_b__18_0` | `Void` | `(String text)` |
| `Il2Cpp.MusicPlayer` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackDoor` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackMount` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `SecondActionOnClick` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Wall` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.WorldObjectButton` | `OnHoverOver` | `Void` | `()` |

### Lifecycle

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AICharacterControl` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.AICharacterExpressions` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `OnDestroy` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CarryModelPool` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `Awake` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `DestroyOperatorsForLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `SpawnOperatorsForLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `SpawnOperatorsForSingleLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `ToggleClearWarningAuto` | `Void` | `(Boolean isOn)` |
| `Il2Cpp.CommandCenterOperator` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CommandCenterOperator` | `Start` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ClearTrackingWithoutDestroying` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `DestroyAllSpawnedItems` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `FreeUpSpawnPoint` | `Void` | `(Int32 spawnIndex)` |
| `Il2Cpp.ComputerShop` | `GetNextAvailableSpawnPoint` | `Dictionary`2` | `()` |
| `Il2Cpp.ComputerShop` | `HandleObjectives` | `Void` | `(ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `RemoveSpawnedItem` | `Void` | `(Int32 uid)` |
| `Il2Cpp.ComputerShop` | `SpawnNewCartItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Nullable`1 chosenColor)` |
| `Il2Cpp.ComputerShop` | `SpawnPhysicalItem` | `Nullable`1` | `(GameObject prefab, Int32 price, ObjectInHand itemType)` |
| `Il2Cpp.CustomerBaseDoor` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Register` | `Void` | `(ITimedDevice device)` |
| `Il2Cpp.DeviceTimerManager` | `TimerLoop` | `IEnumerator` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Unregister` | `Void` | `(ITimedDevice device)` |
| `Il2Cpp.FCP_Persistence` | `OnDestroy` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ITimedDevice` | `TickTimer` | `Void` | `()` |
| `Il2Cpp.InputManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.LoadingScreen` | `Awake` | `Void` | `()` |
| `Il2Cpp.LoadingScreen` | `SetDifficualty` | `Void` | `(Int32 i)` |
| `Il2Cpp.LoadingScreen` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.LocalisedText` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `QuitGame` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `OnDestroy` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `GetReward` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `Start` | `Void` | `()` |
| `Il2Cpp.ObjectiveTimed` | `SetupObjectiveTimed` | `Void` | `(Int32 _maxTime, String _objectiveText, Int32 customerID, Int32 appID, Int32 _requiredIOPS)` |
| `Il2Cpp.ObjectiveTimed` | `UpdateDisplay` | `Void` | `(Int32 currentIOPS, Int32 remainingTime)` |
| `Il2Cpp.Objectives` | `Awake` | `Void` | `()` |
| `Il2Cpp.Objectives` | `ClearObjectives` | `Void` | `()` |
| `Il2Cpp.Objectives` | `CreateNewObjective` | `Void` | `(Int32 localisationUID, Int32 _objectiveUID, Vector3 objectivePosition, Int32 xpReward, Int32 reputationReward, Boolean isSub)` |
| `Il2Cpp.Objectives` | `DestroyObjective` | `Void` | `(Int32 _objectiveUID)` |
| `Il2Cpp.Objectives` | `EffectOnDestroy` | `IEnumerator` | `(Int32 _objectiveUID)` |
| `Il2Cpp.Objectives` | `GetTimedObjective` | `ObjectiveTimed` | `(Int32 objectiveUID)` |
| `Il2Cpp.Objectives` | `InstantiateObjectiveSign` | `Void` | `(Int32 objectiveUID, Vector3 objectPos)` |
| `Il2Cpp.Objectives` | `ObjectiveTimedText` | `String` | `()` |
| `Il2Cpp.Objectives` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Objectives` | `RemoveObjectiveSign` | `Void` | `(Int32 objectiveUID)` |
| `Il2Cpp.Objectives` | `Start` | `Void` | `()` |
| `Il2Cpp.Objectives` | `StartObjective` | `Void` | `(Int32 _objectiveUID, Vector3 objectivePosition, Boolean _loadSave)` |
| `Il2Cpp.Objectives` | `StartObjective` | `Void` | `(Int32 _objectiveUID, Boolean _loadSave)` |
| `Il2Cpp.PacketSpawnerSystem` | `SpawnPacket` | `Void` | `(EntityCommandBuffer ecb, PacketSpawnerComponent spawner, Int32 spawnerIndex, Entity spawnerEntity, BlobArray`1& waypoints, Int32 packetType)` |
| `Il2Cpp.PatchPanel` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `Awake` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `ExitGame` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `HandleAddCommand` | `Void` | `(Il2CppStringArray parts)` |
| `Il2Cpp.PauseMenu` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnPause` | `Void` | `(Int32 openMenu)` |
| `Il2Cpp.PauseMenu` | `Pause` | `Void` | `(Int32 openMenu)` |
| `Il2Cpp.PauseMenu` | `ProcessConsoleCommand` | `Void` | `(String input)` |
| `Il2Cpp.PauseMenu` | `Resume` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenuVideoTutorial` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabButton` | `Start` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabEnter` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabExit` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PauseMenu_TabGroup` | `ResetTabs` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabGroup` | `Subscribe` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.Rack` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.RackMount` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ReusableFunctions` | `DestroyChildren` | `Void` | `(Transform root)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Server` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.SetIP` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `AddSpawnedItem` | `Void` | `(Int32 uid)` |
| `Il2Cpp.ShopCartItem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `RemoveLastSpawnedItem` | `Int32` | `()` |
| `Il2Cpp.ShopItem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `DestroyErrorWarningSign` | `Void` | `(Int32 errorWarningUID)` |
| `Il2Cpp.SteamManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Technician` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.TimeController` | `Awake` | `Void` | `()` |
| `Il2Cpp.TimeController` | `CurrentTimeInHours` | `Single` | `()` |
| `Il2Cpp.TimeController` | `HoursFromDate` | `Int32` | `(Single _time, Int32 _day)` |
| `Il2Cpp.TimeController` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.TimeController` | `Start` | `Void` | `()` |
| `Il2Cpp.TimeController` | `TimeIsBetween` | `Boolean` | `(Single startHour, Single endHour)` |
| `Il2Cpp.TimeController` | `Update` | `Void` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `Awake` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `ParentTheObjectWithDelay` | `IEnumerator` | `(UsableObject uo)` |
| `Il2Cpp.TrolleyLoadingBay` | `ResetAllSlots` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `Start` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `ButtonShowTutorialInPauseMenu` | `Void` | `(Int32 i)` |
| `Il2Cpp.Tutorials` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `StopVideoInPauseMenu` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Wall` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawner` | `Entity` | `(List`1 waypoints, Vector3 spawnerPos, Int32 cableId, Int32 customerID, PacketSpawnerComponent prefabComponent, Boolean isForward)` |
| `Il2Cpp.WaypointInitializationSystem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `ResetAllSpawners` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `SafelyDisposeSpawner` | `Void` | `(Entity spawnerEntity, Int32 cableId, String direction)` |
| `Il2Cpp.WaypointInitializationSystem` | `SetPacketSpawnerEnabled` | `Void` | `(Boolean enabled)` |

### Maintenance

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.PacketSpawnerSystem` | `__ScheduleViaJobChunkExtension_0` | `JobHandle` | `(UpdatePacketsJob job, EntityQuery query, JobHandle dependency, SystemState& state, Boolean hasUserDefinedQuery)` |
| `Il2Cpp.Technician` | `AssignJob` | `Void` | `(RepairJob job)` |
| `Il2Cpp.Technician` | `RequestJobDelayed` | `IEnumerator` | `()` |
| `Il2Cpp.TechnicianManager` | `EnqueueDispatch` | `Void` | `(RepairJob job)` |
| `Il2Cpp.TechnicianManager` | `GetActiveJobs` | `List`1` | `()` |
| `Il2Cpp.TechnicianManager` | `GetQueuedJobs` | `List`1` | `()` |
| `Il2Cpp.TechnicianManager` | `ProcessDispatchQueue` | `IEnumerator` | `()` |
| `Il2Cpp.TechnicianManager` | `RequestNextJob` | `Void` | `(Technician technician)` |
| `Il2Cpp.TechnicianManager` | `RestoreJobQueue` | `Void` | `(List`1 savedJobs)` |

### Networking

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AssetManagement` | `ButtonAddAllBrokenDevicesToQueue` | `Void` | `()` |
| `Il2Cpp.AudioManager` | `PlayEffectAudioClip` | `Void` | `(AudioClip audioClip, Single volume, Single delayed)` |
| `Il2Cpp.AudioManager` | `PlayRandomImpactClip` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `PlayRandomRJ45Clip` | `Void` | `()` |
| `Il2Cpp.CableIDComponent` | `BoxIl2CppObject` | `Object` | `()` |
| `Il2Cpp.CableLink` | `CollectPatchPanelChainCables` | `List`1` | `(Int32 startCableId)` |
| `Il2Cpp.CableLink` | `InsertSFP` | `Void` | `(Single speed, Int32 type, SFPModule module)` |
| `Il2Cpp.CableLink` | `RemoveSFP` | `Void` | `()` |
| `Il2Cpp.CableLink` | `SetConnectionSpeed` | `Void` | `(Single speed)` |
| `Il2Cpp.CableLink` | `Start` | `Void` | `()` |
| `Il2Cpp.CablePositions` | `AssignEntity` | `Void` | `(Int32 cableId, Entity entity)` |
| `Il2Cpp.CablePositions` | `AssignNewPosition` | `Void` | `(Int32 cableId, Transform linkTransform, Boolean isStartPoint, Boolean isEndPoint, TypeOfLink typeOfLink, String serverID)` |
| `Il2Cpp.CablePositions` | `Awake` | `Void` | `()` |
| `Il2Cpp.CablePositions` | `ClearAllCables` | `Void` | `()` |
| `Il2Cpp.CablePositions` | `CreateNewCable` | `Int32` | `()` |
| `Il2Cpp.CablePositions` | `CreateNewReverseCable` | `Int32` | `()` |
| `Il2Cpp.CablePositions` | `CreateTubeMesh` | `Mesh` | `(List`1 path)` |
| `Il2Cpp.CablePositions` | `GenerateBentSegment` | `IEnumerable`1` | `(Vector3 connectionPoint, Vector3 nextPoint, Transform linkTransform, Boolean isStart)` |
| `Il2Cpp.CablePositions` | `GenerateCornerBend` | `IEnumerable`1` | `(Vector3 p_prev, Vector3 p_curr, Vector3 p_next, Transform t_curr)` |
| `Il2Cpp.CablePositions` | `GenerateFinalPath` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `GetCableMaterial` | `Material` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `GetCablePositions` | `List`1` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `GetRawCablePositions` | `List`1` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `GetRawLinkTransforms` | `List`1` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `IsCableComplete` | `Boolean` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `RedrawCable` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `Start` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `DropObject` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `IsCableLenghtEnough` | `Boolean` | `()` |
| `Il2Cpp.CableSpinner` | `LowerAmountOfCable` | `Void` | `(Single length)` |
| `Il2Cpp.CableSpinner` | `Start` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `UpdateCurrentLength` | `Void` | `(Single length)` |
| `Il2Cpp.CarryModelPool` | `StripGameplayComponents` | `Void` | `(GameObject obj)` |
| `Il2Cpp.ComputerShop` | `ButtonNetworkMap` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `GetServerTypeForIP` | `Int32` | `(String ip)` |
| `Il2Cpp.CustomerBase` | `IsIPPresent` | `Boolean` | `(String ip)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `PlayRandomAudioClip` | `AudioClip` | `(Il2CppReferenceArray`1 audioClips, Single volume)` |
| `Il2Cpp.FootSteps` | `GetRandomClip` | `AudioClip` | `()` |
| `Il2Cpp.MainGameManager` | `CloseNetworkConfigCanvas` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `GetCableSpinnerPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `GetSfpPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `ShowNetworkConfigCanvas` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkMap` | `AddDevice` | `Void` | `(String name, TypeOfLink type, Int32 customerID)` |
| `Il2Cpp.NetworkMap` | `AddSwitchConnection` | `Void` | `(String switchName, String deviceName)` |
| `Il2Cpp.NetworkMap` | `Awake` | `Void` | `()` |
| `Il2Cpp.NetworkMap` | `ClearMap` | `Void` | `()` |
| `Il2Cpp.NetworkMap` | `Connect` | `Void` | `(String from, String to)` |
| `Il2Cpp.NetworkMap` | `CreateLACPGroup` | `Int32` | `(String deviceA, String deviceB, List`1 cableIds)` |
| `Il2Cpp.NetworkMap` | `Disconnect` | `Void` | `(String from, String to)` |
| `Il2Cpp.NetworkMap` | `FindAllReachablePathsFrom` | `Dictionary`2` | `(String startDevice)` |
| `Il2Cpp.NetworkMap` | `FindPhysicalPath` | `List`1` | `(String start, String target)` |
| `Il2Cpp.NetworkMap` | `GenerateDeviceName` | `String` | `(TypeOfLink type, Vector3 position)` |
| `Il2Cpp.NetworkMap` | `GetAllDevices` | `List`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllLACPGroups` | `Dictionary`2` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllNetworkSwitches` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetDevice` | `Device` | `(String name)` |
| `Il2Cpp.NetworkMap` | `GetLACPGroupBetween` | `LACPGroup` | `(String deviceA, String deviceB)` |
| `Il2Cpp.NetworkMap` | `GetLACPGroupForCable` | `LACPGroup` | `(Int32 cableId)` |
| `Il2Cpp.NetworkMap` | `GetNumberOfDevices` | `Il2CppStructArray`1` | `()` |
| `Il2Cpp.NetworkMap` | `IsIpAddressDuplicate` | `Boolean` | `(String ip, Server serverToExclude)` |
| `Il2Cpp.NetworkMap` | `PrintNetworkMap` | `String` | `()` |
| `Il2Cpp.NetworkMap` | `RegisterCableConnection` | `Void` | `(Int32 cableId, Vector3 startPos, Vector3 endPos, TypeOfLink startType, TypeOfLink endType, String startSwitchID, String endSwitchID, Int32 startCustomerID, Int32 endCustomerID, String startServerID, String endServerID)` |
| `Il2Cpp.NetworkMap` | `RemapDeviceId` | `Void` | `(String oldId, String newId)` |
| `Il2Cpp.NetworkMap` | `RemoveCableConnection` | `Void` | `(Int32 cableId, Boolean preserveLACP)` |
| `Il2Cpp.NetworkMap` | `RemoveCableFromLACPGroups` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.NetworkMap` | `RemoveDevice` | `Void` | `(String name)` |
| `Il2Cpp.NetworkMap` | `RemoveIsolatedDevice` | `Void` | `(String deviceName)` |
| `Il2Cpp.NetworkMap` | `RemoveLACPGroup` | `Void` | `(Int32 groupId)` |
| `Il2Cpp.NetworkMap` | `SetLACPGroups` | `Void` | `(Dictionary`2 groups)` |
| `Il2Cpp.NetworkMap` | `_PrintNetworkMap_b__47_0` | `Boolean` | `(Device d)` |
| `Il2Cpp.NetworkMap` | `_PrintNetworkMap_b__47_1` | `Boolean` | `(Device d)` |
| `Il2Cpp.NetworkSwitch` | `Awake` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `ButtonShowNetworkSwitchConfig` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `ClearErrorSign` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `ClearWarningSign` | `Void` | `(Boolean isPreserved)` |
| `Il2Cpp.NetworkSwitch` | `DisconnectCables` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `DisconnectCablesWhenSwitchIsOff` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `GetAllDisallowedVlans` | `Dictionary`2` | `()` |
| `Il2Cpp.NetworkSwitch` | `GetConnectedDevices` | `List`1` | `()` |
| `Il2Cpp.NetworkSwitch` | `GetDeviceId` | `String` | `()` |
| `Il2Cpp.NetworkSwitch` | `GetDisallowedVlans` | `HashSet`1` | `(Int32 portIndex)` |
| `Il2Cpp.NetworkSwitch` | `HandleNewCableWhileOff` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.NetworkSwitch` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.NetworkSwitch` | `IsVlanAllowedOnCable` | `Boolean` | `(Int32 cableId, Int32 vlanId)` |
| `Il2Cpp.NetworkSwitch` | `IsVlanAllowedOnPort` | `Boolean` | `(Int32 portIndex, Int32 vlanId)` |
| `Il2Cpp.NetworkSwitch` | `ReconnectCables` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `SetDisallowedVlansPerPort` | `Void` | `(Dictionary`2 data)` |
| `Il2Cpp.NetworkSwitch` | `SetVlanAllowed` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
| `Il2Cpp.NetworkSwitch` | `SetVlanDisallowed` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
| `Il2Cpp.NetworkSwitch` | `Start` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `TickTimer` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `TurnOffCommonFunctions` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `TurnOnCommonFunction` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `ValidateRackPosition` | `Boolean` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `Awake` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ClearVLANDisplay` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ClickPort` | `Void` | `(Int32 i)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `CloseConfig` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `CreateLACP` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `GetDevicePrefix` | `String` | `(String deviceId)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `GetVisibleVLANs` | `HashSet`1` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `NormalizeDeviceKey` | `String` | `(String deviceName)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `OpenConfig` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `RefreshPortDisplay` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `RemoveLACP` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ResolveOtherEndpoint` | `String` | `(ValueTuple`2 conn, String primaryId, String fallbackId)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ResolveRemoteDevice` | `String` | `(CableLink port)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ToggleVLANMulti` | `Void` | `(List`1 portIndices, Int32 vlanId)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `GetRangesForScript` | `IReadOnlyList`1` | `(Script script)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ResolvePrimaryScript` | `Script` | `(CultureInfo culture)` |
| `Il2CppviperOSK.OSK_Keyboard` | `InputFromPointerDevice` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `InputFromPointerDevice` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `UnityEngine_EventSystems_IPointerClickHandler_OnPointerClick` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.PacketComponent` | `BoxIl2CppObject` | `Object` | `()` |
| `Il2Cpp.PacketSettings` | `BoxIl2CppObject` | `Object` | `()` |
| `Il2Cpp.PacketSpawnerSystem` | `OnCreate` | `Void` | `(SystemState& state)` |
| `Il2Cpp.PacketSpawnerSystem` | `OnCreateForCompiler` | `Void` | `(SystemState& state)` |
| `Il2Cpp.PacketSpawnerSystem` | `OnUpdate` | `Void` | `(SystemState& state)` |
| `Il2Cpp.PacketSpawnerSystem` | `__AssignQueries` | `Void` | `(SystemState& state)` |
| `Il2Cpp.PacketSpawnerSystem` | `__codegen__OnCreate` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.PacketSpawnerSystem` | `__codegen__OnCreateForCompiler` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.PacketSpawnerSystem` | `__codegen__OnUpdate` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.PatchPanel` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.PauseMenu_TabButton` | `UnityEngine_EventSystems_IPointerClickHandler_OnPointerClick` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.PauseMenu_TabButton` | `UnityEngine_EventSystems_IPointerEnterHandler_OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.PauseMenu_TabButton` | `UnityEngine_EventSystems_IPointerExitHandler_OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.SFPBox` | `CanAcceptSFP` | `Boolean` | `(Int32 sfpType)` |
| `Il2Cpp.SFPBox` | `InsertSFPBackIntoBox` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `RemoveSFPFromBox` | `Void` | `(Int32 position)` |
| `Il2Cpp.SFPBox` | `ReturnSFPDirectly` | `Boolean` | `(SFPModule sfpmodule)` |
| `Il2Cpp.SFPBox` | `TakeSFPFromBox` | `SFPModule` | `()` |
| `Il2Cpp.SFPModule` | `InsertedInSFPPort` | `Void` | `(CableLink _link, Boolean immediate)` |
| `Il2Cpp.SFPModule` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.Server` | `Awake` | `Void` | `()` |
| `Il2Cpp.Server` | `ButtonClickChangeIP` | `Void` | `()` |
| `Il2Cpp.Server` | `ClearErrorSign` | `Void` | `()` |
| `Il2Cpp.Server` | `ClearWarningSign` | `Void` | `(Boolean isPreserved)` |
| `Il2Cpp.Server` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.Server` | `RegisterLink` | `Void` | `(CableLink link)` |
| `Il2Cpp.Server` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.Server` | `SetIP` | `Void` | `(String _ip)` |
| `Il2Cpp.Server` | `Start` | `Void` | `()` |
| `Il2Cpp.Server` | `TickTimer` | `Void` | `()` |
| `Il2Cpp.Server` | `TurnOffCommonFunctions` | `Void` | `()` |
| `Il2Cpp.Server` | `TurnOnCommonFunction` | `Void` | `()` |
| `Il2Cpp.Server` | `UnregisterLink` | `Void` | `(CableLink link)` |
| `Il2Cpp.Server` | `ValidateRackPosition` | `Boolean` | `()` |
| `Il2Cpp.Server` | `_ValidateRackPosition_b__53_0` | `Boolean` | `(RackPosition r)` |
| `Il2Cpp.SetIP` | `ClickButtonNextIP` | `Void` | `()` |
| `Il2Cpp.SetIP` | `GetUsableIPsFromSubnet` | `Il2CppStringArray` | `(String subnet)` |
| `Il2Cpp.SetIP` | `TryParseIpToOctets` | `Boolean` | `(String ipString, Int32& o1, Int32& o2, Int32& o3, Int32& o4)` |
| `Il2Cpp.Technician` | `CacheDeviceBounds` | `Void` | `(GameObject device)` |
| `Il2Cpp.Technician` | `GetCorrectDevicePrefab` | `GameObject` | `()` |
| `Il2Cpp.Technician` | `GetCurrentDevicePrefabID` | `Int32` | `()` |
| `Il2Cpp.Technician` | `PositionHandTargetsOnDevice` | `Void` | `(GameObject device)` |
| `Il2Cpp.Technician` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `IsDeviceAlreadyAssigned` | `Boolean` | `(NetworkSwitch networkSwitch, Server server)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `PlayRandomAudioClip` | `AudioClip` | `(Il2CppReferenceArray`1 audioClips, Single volume)` |
| `Il2Cpp.ToolTipInteract` | `HideTooltipForInteract` | `Void` | `()` |
| `Il2Cpp.ToolTipInteract` | `ShowTooltipForInteract` | `Void` | `(String _text, Sprite _sprite)` |
| `Il2Cpp.ToolTipOnUIText` | `ToolTip` | `Void` | `()` |
| `Il2Cpp.Tooltip` | `HideTooltip` | `Void` | `()` |
| `Il2Cpp.Tooltip` | `ShowTooltipOverlayCanvas` | `Void` | `(String tooltipText, Vector3 _position, Int32 differentXOffset)` |
| `Il2Cpp.Tooltip` | `ShowTooltipWorldCanvas` | `Void` | `(String _text, RectTransform _transform, Camera cam)` |
| `Il2Cpp.Tutorials` | `SkipTutorials` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `ActivateSpawnerOnCable` | `Void` | `(Entity spawnerEntity, Single speed, Int32 customerId)` |
| `Il2Cpp.WaypointInitializationSystem` | `ActivateSpawnersForCable` | `Void` | `(CableInfo cable, Single finalSpeed, HashSet`1 customersOnCable, HashSet`1 directions)` |
| `Il2Cpp.WaypointInitializationSystem` | `ClearNetworkState` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateCableWithSpawners` | `Void` | `(Int32 cableId, List`1 positions)` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawnersForCable` | `Void` | `(CableInfo& cableInfo)` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawnersForCable` | `Void` | `(CableInfo& cableInfo, PacketSpawnerComponent prefabComponent)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetAllCables` | `List`1` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCableCurrentSpeed` | `Single` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCableInfo` | `Nullable`1` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetDeviceName` | `String` | `(CableEndpoint endpoint)` |
| `Il2Cpp.WaypointInitializationSystem` | `IsCableInRoute` | `Boolean` | `(CableInfo cable, List`1 route)` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCableRemoved` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `RegisterCableInNetworkMap` | `Void` | `(CableInfo cableInfo)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateCableInfo` | `Void` | `(Int32 cableId, CableInfo info)` |

### Persistence

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.BalanceSheet` | `GetSaveData` | `BalanceSheetSaveData` | `()` |
| `Il2Cpp.BalanceSheet` | `LoadFromSave` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `SaveSnapshot` | `Void` | `(Int32 month, DateTime snapshotTime)` |
| `Il2Cpp.CablePositions` | `LoadCable` | `Void` | `(CableSaveData cableData)` |
| `Il2Cpp.CableSpinner` | `LoadSavedColor` | `Void` | `()` |
| `Il2Cpp.ColorSerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.ColorSerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |
| `Il2Cpp.CommandCenter` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.CommandCenterOperator` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `UnlockFromSave` | `Void` | `(Dictionary`2 savedStates)` |
| `Il2Cpp.CustomerBase` | `LoadData` | `Void` | `(CustomerBaseSaveData data)` |
| `Il2Cpp.CustomerBaseDoor` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `Awake` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `GenerateID` | `String` | `()` |
| `Il2Cpp.FCP_Persistence` | `LoadColor` | `Boolean` | `(Color& c)` |
| `Il2Cpp.FCP_Persistence` | `LoadDataFile` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `SaveColor` | `Void` | `(Color c)` |
| `Il2Cpp.FCP_Persistence` | `SaveDataFile` | `Void` | `()` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `GetSettingHash` | `Int32` | `()` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `MakeMesh` | `Void` | `(Sprite sprite, Int32 x, Int32 y, MeshType meshtype)` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `Update` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `DelayedLoad` | `IEnumerator` | `()` |
| `Il2Cpp.IModPlugin` | `OnModLoad` | `Void` | `(String modFolderPath)` |
| `Il2Cpp.IModPlugin` | `OnModUnload` | `Void` | `()` |
| `Il2Cpp.InputManager` | `LoadAllBindingOverrides` | `Void` | `()` |
| `Il2Cpp.InputManager` | `LoadBindingOverride` | `Void` | `(String actionName)` |
| `Il2Cpp.InputManager` | `SaveBindingOverride` | `Void` | `(InputAction action)` |
| `Il2Cpp.Interact` | `OnLoad` | `Void` | `(InteractObjectData data)` |
| `Il2Cpp.LoadingScreen` | `AsynchronousLoad` | `IEnumerator` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `AsynchronousUnLoad` | `IEnumerator` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `IsSceneLoaded` | `Boolean` | `(String name)` |
| `Il2Cpp.LoadingScreen` | `LoadGameLoadScene` | `IEnumerator` | `(Il2CppStructArray`1 loadedScenes)` |
| `Il2Cpp.LoadingScreen` | `LoadGameScenesVoid` | `Void` | `(PlayerData playerData, List`1 technicianData, Il2CppStructArray`1 loadedScenes, Il2CppStructArray`1 hiredTechnicians, List`1 repairJobQueue)` |
| `Il2Cpp.LoadingScreen` | `LoadLevel` | `Void` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `LoadPlayerAndNPCDataWithDelay` | `IEnumerator` | `(PlayerData playerData, List`1 technicianData, Il2CppStructArray`1 hiredTechnicians, List`1 repairJobQueue)` |
| `Il2Cpp.LoadingScreen` | `UnLoadLevel` | `Void` | `(Int32 sceneIndex)` |
| `Il2Cpp.Localisation` | `LoadLocalisation` | `Dictionary`2` | `(Int32 _uid)` |
| `Il2Cpp.MainGameManager` | `AutoSaveCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.MainGameManager` | `LoadTrolleyPosition` | `Void` | `(Vector3 _position, Quaternion _rotation)` |
| `Il2Cpp.MainGameManager` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `RestartAutoSave` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `SetAutoSaveEnabled` | `Void` | `(Boolean enabled)` |
| `Il2Cpp.MainGameManager` | `SetAutoSaveInterval` | `Void` | `(Single minutes)` |
| `Il2Cpp.MainMenu` | `Load` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.MainMenu` | `LoadGame` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `LoadAllMods` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `LoadDll` | `Void` | `(String folderPath, DllEntry dll)` |
| `Il2Cpp.ModLoader` | `LoadIcon` | `Sprite` | `(String folderPath, String iconFile)` |
| `Il2Cpp.ModLoader` | `LoadMesh` | `Mesh` | `(String folderPath, String modelFile)` |
| `Il2Cpp.ModLoader` | `LoadModPack` | `Void` | `(String folderPath)` |
| `Il2Cpp.ModLoader` | `LoadShopItem` | `Void` | `(String folderPath, String folderName, ShopItemConfig config)` |
| `Il2Cpp.ModLoader` | `LoadStaticItem` | `Void` | `(String folderPath, String folderName, StaticItemConfig config)` |
| `Il2Cpp.ModLoader` | `LoadTexture` | `Texture2D` | `(String path)` |
| `Il2Cpp.ModLoader` | `SyncWorkshopThenLoadAll` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `LoadAccentMap` | `Void` | `(OSK_AccentAssetObj accents)` |
| `Il2CppviperOSK.OSK_Keyboard` | `LoadLayout` | `Void` | `(String lay)` |
| `Il2Cpp.Objectives` | `LoadObjectives` | `Void` | `(HashSet`1 _activeObjectives)` |
| `Il2Cpp.Objectives` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `CloseLoadSaveOverlay` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `DeleteSaveButtonClick` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.PauseMenu` | `DeleteSaveConfirm` | `Void` | `(Boolean yes)` |
| `Il2Cpp.PauseMenu` | `Load` | `Void` | `(String savename)` |
| `Il2Cpp.PauseMenu` | `LoadSaveOnButtonClick` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.PauseMenu` | `LoadWithOverlay` | `IEnumerator` | `(String savename)` |
| `Il2Cpp.PauseMenu` | `NotAllowedToSaveOverlayOff` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `PopulateLoadSaveMenu` | `Void` | `(Boolean _savingGame)` |
| `Il2Cpp.PauseMenu` | `Save` | `Void` | `(String saveName, String _stringNameOfSave)` |
| `Il2Cpp.PauseMenu` | `SaveConfirm` | `Void` | `(Boolean yes)` |
| `Il2Cpp.PauseMenu` | `_LoadSaveOnButtonClick_b__35_0` | `Void` | `(String saveName)` |
| `Il2Cpp.PauseMenu` | `_SaveConfirm_b__37_0` | `Void` | `(String saveName)` |
| `Il2Cpp.Player` | `LoadPlayer` | `Void` | `(PlayerData data)` |
| `Il2Cpp.QuaternionSerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.QuaternionSerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |
| `Il2Cpp.Rack` | `InitializeLoadedRack` | `Void` | `(Il2CppStructArray`1 loadedPositions)` |
| `Il2Cpp.Rack` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.RackMount` | `OnLoad` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `ReloadData` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `ReloadData` | `Void` | `(IRecyclableScrollRectDataSource dataSource)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `_ReloadData_b__17_0` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `LoadSFPsFromSave` | `Void` | `()` |
| `Il2Cpp.SaveData` | `Validate` | `String` | `()` |
| `Il2Cpp.SaveSystem` | `AutoSave` | `Void` | `()` |
| `Il2Cpp.SaveSystem` | `DeleteSaveFile` | `Void` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `FormatDisplayName` | `String` | `(String rawEntry)` |
| `Il2Cpp.SaveSystem` | `GetBinaryFormatter` | `BinaryFormatter` | `()` |
| `Il2Cpp.SaveSystem` | `GetRawSaveEntry` | `String` | `(String displayName)` |
| `Il2Cpp.SaveSystem` | `Listofsaves` | `List`1` | `()` |
| `Il2Cpp.SaveSystem` | `Load` | `Void` | `(String savename, Boolean isFromPauseMenu)` |
| `Il2Cpp.SaveSystem` | `LoadGame` | `SaveData` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `LoadGameData` | `Void` | `()` |
| `Il2Cpp.SaveSystem` | `NewestSave` | `String` | `()` |
| `Il2Cpp.SaveSystem` | `ReadMeta` | `SaveMeta` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `SaveGame` | `Void` | `(String savename, String stringNameOfSave)` |
| `Il2Cpp.SaveSystem` | `SaveGameData` | `Void` | `()` |
| `Il2Cpp.SaveSystem` | `WriteMeta` | `Void` | `(String savename, Int32 version, String nameOfSave)` |
| `Il2Cpp.Server` | `OnLoadingComplete` | `Void` | `()` |
| `Il2Cpp.Server` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.SettingsControls` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `SetAutoSaveInterval` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetAutoSaveOnOff` | `Void` | `(Boolean isActive)` |
| `Il2Cpp.SettingsVolume` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonSaveInputNumpadOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonSaveInputTextOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `SetLoadingInfo` | `Void` | `(String s)` |
| `Il2Cpp.SteamLeaderboards` | `OnScoreUploaded` | `Void` | `(LeaderboardScoreUploaded_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `OnUserScoreDownloaded` | `Void` | `(LeaderboardScoresDownloaded_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `UploadScore` | `Void` | `(Single moneyPerSecond)` |
| `Il2Cpp.Technician` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `OnLoadDestroy` | `Void` | `()` |
| `Il2Cpp.Vector3SerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.Vector3SerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |
| `Il2Cpp.Wall` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `LoadNetworkState` | `Void` | `(NetworkSaveData networkData, List`1 allRackPositions, Int32 saveVersion)` |
| `Il2Cpp.WaypointInitializationSystem` | `LoadNetworkStateCoroutine` | `IEnumerator` | `(NetworkSaveData networkData, List`1 allRackPositions, Int32 saveVersion)` |

### Serialization

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `BoxedGetHashCode` | `Int32` | `(Object val, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `Equals` | `Boolean` | `(Object lhs, Object rhs, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `Equals` | `Boolean` | `(Object lhs, Void* rhs, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `SetSharedStaticTypeIndices` | `Void` | `(Int32* pTypeInfos, Int32 count)` |
| `Il2Cpp.FCP_Persistence` | `InitStatic` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateStatic` | `Void` | `(PickerType type)` |
| `Il2Cpp.ModLoader` | `CreateStaticInstance` | `GameObject` | `(StaticItemConfig config, Mesh mesh, Material material, String folderName)` |
| `Il2Cpp.PacketSpawnerSystem` | `Method_Internal_Static_Void_IntPtr_IntPtr_PDM_0` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.PacketSpawnerSystem` | `Method_Internal_Static_Void_IntPtr_IntPtr_PDM_1` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.StaticUIElements` | `ShowStaticCanvas` | `Void` | `(Boolean active)` |
| `Il2CppTMPro.Examples.TeleType` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TeleType` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.UnitySourceGeneratedAssemblyMonoScriptTypes_v1` | `Get` | `MonoScriptData` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_Boolean_byref___c__DisplayClass15_0_PDM_0` | `Int32` | `(Vector3 searchPos, Boolean mustBeStartOrEnd, __c__DisplayClass15_0& A_2)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_Boolean_byref___c__DisplayClass53_0_PDM_0` | `Int32` | `(Vector3 searchPos, Boolean mustBeStartOrEnd, __c__DisplayClass53_0& A_2)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_byref___c__DisplayClass15_0_PDM_0` | `Int32` | `(Vector3 searchPos, __c__DisplayClass15_0& A_1)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_byref___c__DisplayClass53_0_PDM_0` | `Int32` | `(Vector3 searchPos, __c__DisplayClass53_0& A_1)` |

### Settings

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CustomerBase` | `TryRegisterRoutedSubnet` | `Boolean` | `(Int32 targetVlanId, Int32 sourceVlanId, Il2CppStringArray ips)` |
| `Il2Cpp.CustomerBase` | `TryUnregisterRoutedSubnet` | `Void` | `(Int32 sourceVlanId)` |
| `Il2Cpp.InputManager` | `CheckCurrentControls` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.MainGameManager` | `GetRouterPrefab` | `GameObject` | `(Int32 routerType)` |
| `Il2Cpp.MainGameManager` | `ShowRouterConfigCanvas` | `Void` | `(Router router)` |
| `Il2Cpp.MainMenu` | `Settings` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `CopyDirectory` | `Void` | `(String sourceDir, String destDir)` |
| `Il2Cpp.NetworkMap` | `FindAllRoutes` | `List`1` | `(String baseName, String serverName)` |
| `Il2Cpp.NetworkMap` | `GetAllRouters` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `IsVlanAllowedOnRoute` | `Boolean` | `(List`1 path, Int32 vlanId, Dictionary`2 cablePairLookup)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AutoCorrectLayout` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectLayout` | `String` | `(String layout)` |
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectRecursive` | `Void` | `(String input, List`1 result)` |
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectRow` | `String` | `(String row)` |
| `Il2CppviperOSK.OSK_Keymap` | `CapitalizeCorrectly` | `String` | `(String input, String correctForm)` |
| `Il2Cpp.RectExtensions` | `InverseTransform` | `Rect` | `(Rect r, Transform transform)` |
| `Il2Cpp.RectExtensions` | `Transform` | `Rect` | `(Rect r, Transform transform)` |
| `Il2Cpp.Router` | `AddRoute` | `Boolean` | `(Int32 sourceVlanId, String subnetCidr, Int32 targetVlanId)` |
| `Il2Cpp.Router` | `ExchangeRoutes` | `Void` | `(Router neighbor)` |
| `Il2Cpp.Router` | `WithdrawBgpRoutes` | `Void` | `()` |
| `Il2Cpp.Router` | `_WithdrawBgpRoutes_b__16_1` | `Boolean` | `(SubnetRoute r)` |
| `Il2Cpp.RouterConfiguration` | `StartEditRoute` | `Void` | `(Int32 sourceVlanId, GameObject rowObj)` |
| `Il2Cpp.SFPModule` | `InsertDirectlyIntoPort` | `Void` | `(CableLink _link)` |
| `Il2Cpp.SettingsControls` | `Start` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `OnLanguageDropDownChange` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetPacketType` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetRouteEvalInterval` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `Start` | `Void` | `()` |
| `Il2Cpp.SettingsSingleton` | `Awake` | `Void` | `()` |
| `Il2Cpp.SettingsSingleton` | `DisableOnAfterFirstSettingUp` | `IEnumerator` | `()` |
| `Il2Cpp.SettingsVolume` | `Start` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `EvaluateAllRoutes` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `MapDirectionToSibling` | `String` | `(CableInfo primary, CableInfo sibling, String direction)` |
| `Il2Cpp.WaypointInitializationSystem` | `RequestRouteEvaluation` | `Void` | `()` |

### Steam

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.SteamLeaderboards` | `Init` | `Void` | `()` |
| `Il2Cpp.SteamLeaderboards` | `OnLeaderboardFound` | `Void` | `(LeaderboardFindResult_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `RequestUserEntry` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `InitOnPlayMode` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `Update` | `Void` | `()` |

### Tutorials

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.Objectives` | `IsTutorialInProgress` | `Boolean` | `()` |
| `Il2Cpp.Tutorials` | `Awake` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `OnVideoPrepared` | `Void` | `(VideoPlayer vp)` |
| `Il2Cpp.Tutorials` | `PlayVideo` | `Void` | `(Int32 _tutorialIndex, Boolean isInPauseMenu)` |
| `Il2Cpp.Tutorials` | `ShowTutorial` | `Void` | `(Int32 i)` |
| `Il2Cpp.Tutorials` | `Start` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `StopTutorial` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `_Start_b__11_0` | `Void` | `(CallbackContext context)` |

### Uncategorized

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AutoDisable` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AutoDisable` | `TurnOffAfterXseconds` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.Benchmark01` | `Start` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.Benchmark02` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.Benchmark03` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.Benchmark03` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.Benchmark04` | `Start` | `Void` | `()` |
| `Il2Cpp.GetCurrentVersion` | `Start` | `Void` | `()` |
| `Il2Cpp.LocalisedText` | `Start` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `ClampRotationAroundXAxis` | `Quaternion` | `(Quaternion q)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `Init` | `Void` | `(Transform character, Transform camera)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `InternalLockUpdate` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `ResetRotation` | `Void` | `(Transform character)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `SetCursorLock` | `Void` | `(Boolean value)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `SittingClampRotation` | `Vector2` | `(Vector2 q)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `UpdateCursorLock` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `_Init_b__22_0` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `_Init_b__22_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.Numpad` | `ClickNumber` | `Void` | `(String number)` |
| `Il2Cpp.Numpad` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.Numpad` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.Numpad` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.ObjectSpin` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.ObjectSpin` | `Update` | `Void` | `()` |
| `Il2Cpp.PositionIndicator` | `Awake` | `Void` | `()` |
| `Il2Cpp.PositionIndicator` | `Update` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `Cleanup` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `HideItemNameOrSiluete` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `Init` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `ResetHold` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_0` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_1` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_2` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_3` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_4` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_5` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `_Init_b__20_6` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.Router` | `AddBgpPeer` | `Boolean` | `(String neighborRouterId, Int32 localAsn, Int32 remoteAsn)` |
| `Il2Cpp.Router` | `EstablishBgpSessions` | `Void` | `()` |
| `Il2Cpp.Router` | `ResolveTargetVlan` | `Int32` | `(String serverIp)` |
| `Il2Cpp.RouterConfiguration` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `OpenConfig` | `Void` | `(Router _router)` |
| `Il2Cpp.RouterRouteRow` | `Initialize` | `Void` | `(RouterConfiguration _config, Router _router, Int32 _sourceVlan, Int32 _targetVlan, String subnetCidr, String _gateway)` |
| `Il2Cpp.RouterRouteRow` | `OnDeleteClicked` | `Void` | `()` |
| `Il2Cpp.RouterRouteRow` | `OnEditClicked` | `Void` | `()` |
| `Il2CppTMPro.Examples.SimpleScript` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.SimpleScript` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `RevealCharacters` | `IEnumerator` | `(TMP_Text textComponent)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `RevealWords` | `IEnumerator` | `(TMP_Text textComponent)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshSpawner` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshSpawner` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `Start` | `Void` | `()` |

### UnityEngine

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.FootSteps` | `Awake` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `GetRandomFromRequest` | `AudioClip` | `(Int32 _clipArray)` |
| `Il2Cpp.FootSteps` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `Step` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `checkGroundMaterial` | `IEnumerator` | `()` |
| `Il2Cpp.GamepadIcons` | `GetSprite` | `Sprite` | `(String controlPath)` |
| `Il2Cpp.HRSystem` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.Localisation` | `Awake` | `Void` | `()` |
| `Il2Cpp.Localisation` | `ChangeLocalisation` | `Void` | `(Int32 _uid)` |
| `Il2Cpp.OpenURL` | `OpenURLInBrowser` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `Start` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `Update` | `Void` | `()` |
| `Il2Cpp.StaminaOverlayOnEnable` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.UserReport` | `ClearForm` | `Void` | `()` |
| `Il2Cpp.UserReport` | `ClearReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `CreateUserReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `SetThumbnail` | `Void` | `(Texture2D thumbnail)` |
| `Il2Cpp.UserReport` | `ShowError` | `IEnumerator` | `()` |
| `Il2Cpp.UserReport` | `Start` | `Void` | `()` |
| `Il2Cpp.UserReport` | `Update` | `Void` | `()` |

### VisualUI

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ActionKeyHint` | `DelayedUpdateUI` | `IEnumerator` | `()` |
| `Il2Cpp.ActionKeyHint` | `UpdateUI` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonClearAllWarnings` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterAll` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterEOL` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterOff` | `Void` | `()` |
| `Il2Cpp.AssetManagementDeviceLine` | `ButtonClearWarningSign` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `ScrollAuto` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `Update` | `Void` | `()` |
| `Il2Cpp.BalanceSheetRow` | `SetBackgroundColor` | `Void` | `(Color color)` |
| `Il2CppTMPro.Examples.Benchmark01_UGUI` | `Start` | `IEnumerator` | `()` |
| `UnityEngine.UI.ButtonExtended` | `OnDeselect` | `Void` | `(BaseEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `OnEnable` | `Void` | `()` |
| `UnityEngine.UI.ButtonExtended` | `OnFinishSubmit` | `IEnumerator` | `()` |
| `UnityEngine.UI.ButtonExtended` | `OnPointerClick` | `Void` | `(PointerEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `OnSelect` | `Void` | `(BaseEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `OnSubmit` | `Void` | `(BaseEventData eventData)` |
| `UnityEngine.UI.ButtonExtended` | `Press` | `Void` | `()` |
| `UnityEngine.UI.ButtonExtended` | `Start` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `UpdateText` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `ButtonDowngradeCommandCenter` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `ButtonUpgradeCommandCenter` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonAssetManagementScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonCancel` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonCancelColorPicker` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonChosenColor` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonClear` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonHireScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonReturnMainScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OpenColorPicker` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `RemoveCartUIItem` | `Void` | `(ShopCartItem cartItem)` |
| `Il2Cpp.ComputerShop` | `SelectNextAvailable` | `Void` | `(Int32 removedIndex)` |
| `Il2Cpp.DropdownSample` | `OnButtonClick` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `Awake` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `ChangeMode` | `Void` | `(Int32 newMode)` |
| `Il2Cpp.FlexibleColorPicker` | `ChangeMode` | `Void` | `(MainPickingMode mode)` |
| `Il2Cpp.FlexibleColorPicker` | `FinishTypeHex` | `Void` | `(String input)` |
| `Il2Cpp.FlexibleColorPicker` | `GetColor` | `Color` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `GetColorFullAlpha` | `Color` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `GetGradientMode` | `Int32` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `GetMarker` | `RectTransform` | `(Image picker, String search)` |
| `Il2Cpp.FlexibleColorPicker` | `GetNormScreenSpace` | `Vector2` | `(RectTransform rect, BaseEventData e)` |
| `Il2Cpp.FlexibleColorPicker` | `GetNormWorldSpace` | `Vector2` | `(Canvas canvas, RectTransform rect, BaseEventData e)` |
| `Il2Cpp.FlexibleColorPicker` | `GetNormalizedPointerPosition` | `Vector2` | `(Canvas canvas, RectTransform rect, BaseEventData e)` |
| `Il2Cpp.FlexibleColorPicker` | `GetSanitizedHex` | `String` | `(String input, Boolean full)` |
| `Il2Cpp.FlexibleColorPicker` | `GetValue` | `Vector2` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `GetValue` | `Vector2` | `(MainPickingMode mode)` |
| `Il2Cpp.FlexibleColorPicker` | `GetValue1D` | `Single` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `HSVToRGB` | `Color` | `(Vector3 hsv)` |
| `Il2Cpp.FlexibleColorPicker` | `HSVToRGB` | `Color` | `(Single h, Single s, Single v)` |
| `Il2Cpp.FlexibleColorPicker` | `IsAlphaType` | `Boolean` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `IsHorizontal` | `Boolean` | `(Picker p)` |
| `Il2Cpp.FlexibleColorPicker` | `IsPickerAvailable` | `Boolean` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `IsPickerAvailable` | `Boolean` | `(Int32 index)` |
| `Il2Cpp.FlexibleColorPicker` | `IsPreviewType` | `Boolean` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `IsValidHexChar` | `Boolean` | `(Char c)` |
| `Il2Cpp.FlexibleColorPicker` | `MakeModeOptions` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `ParseHex` | `Color` | `(String input)` |
| `Il2Cpp.FlexibleColorPicker` | `ParseHex` | `Color` | `(String input, Color defaultColor)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColor` | `BufferedColor` | `(BufferedColor color, PickerType type, Vector2 v)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColor1D` | `BufferedColor` | `(BufferedColor color, PickerType type, Vector2 v)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColor1D` | `BufferedColor` | `(BufferedColor color, PickerType type, Single value)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColor2D` | `BufferedColor` | `(BufferedColor color, PickerType type1, Single value1, PickerType type2, Single value2)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColorMain` | `BufferedColor` | `(BufferedColor color, Vector2 v)` |
| `Il2Cpp.FlexibleColorPicker` | `PickColorMain` | `BufferedColor` | `(BufferedColor color, MainPickingMode mode, Vector2 v)` |
| `Il2Cpp.FlexibleColorPicker` | `PointerUpdate` | `Void` | `(BaseEventData e)` |
| `Il2Cpp.FlexibleColorPicker` | `RGBToHSV` | `Vector3` | `(Color color)` |
| `Il2Cpp.FlexibleColorPicker` | `RGBToHSV` | `Vector3` | `(Single r, Single g, Single b)` |
| `Il2Cpp.FlexibleColorPicker` | `SeperateMaterials` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `SetColor` | `Void` | `(Color color)` |
| `Il2Cpp.FlexibleColorPicker` | `SetColorNoAlpha` | `Void` | `(Color color)` |
| `Il2Cpp.FlexibleColorPicker` | `SetMarker` | `Void` | `(Image picker, Vector2 v, Boolean setX, Boolean setY)` |
| `Il2Cpp.FlexibleColorPicker` | `SetPointerFocus` | `Void` | `(Int32 i)` |
| `Il2Cpp.FlexibleColorPicker` | `ShiftColor` | `Void` | `(Int32 type, Single delta)` |
| `Il2Cpp.FlexibleColorPicker` | `ShiftHue` | `Void` | `(Single delta)` |
| `Il2Cpp.FlexibleColorPicker` | `SliderUpdate` | `Void` | `(PickerType type, Single value)` |
| `Il2Cpp.FlexibleColorPicker` | `TypeHex` | `Void` | `(String input)` |
| `Il2Cpp.FlexibleColorPicker` | `TypeHex` | `Void` | `(String input, Boolean finish)` |
| `Il2Cpp.FlexibleColorPicker` | `Update` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateDynamic` | `Void` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateHex` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateMarker` | `Void` | `(Picker picker, PickerType type, Vector2 v)` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateMarkers` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateMode` | `Void` | `(MainPickingMode mode)` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateTextures` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_0` | `Void` | `(Single v)` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_1` | `Void` | `(Single v)` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_2` | `Void` | `(Single v)` |
| `Il2Cpp.HRSystem` | `ButtonConfirmFireEmployee` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonConfirmHire` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonFireEmployee` | `Void` | `(Int32 i)` |
| `Il2Cpp.HRSystem` | `ButtonHireEmployee` | `Void` | `(Int32 i)` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `CreateCellPool` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `InitCoroutine` | `IEnumerator` | `(Action onInitialized)` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `OnDrawGizmos` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `OnValueChangedListener` | `Vector2` | `(Vector2 direction)` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `RecycleLeftToRight` | `Vector2` | `()` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `RecycleRightToleft` | `Vector2` | `()` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `SetLeftAnchor` | `Void` | `(RectTransform rectTransform)` |
| `Il2CppPolyAndCode.UI.HorizontalRecyclingSystem` | `SetRecyclingBounds` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.IRecyclableScrollRectDataSource` | `GetItemCount` | `Int32` | `()` |
| `Il2CppPolyAndCode.UI.IRecyclableScrollRectDataSource` | `SetCell` | `Void` | `(ICell cell, Int32 index)` |
| `Il2CppviperOSK.I_OSK_Cursor` | `Cursor` | `Void` | `()` |
| `Il2CppviperOSK.I_OSK_Cursor` | `Show` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.I_OSK_Key` | `Click` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.I_OSK_Key` | `GetGameObject` | `GameObject` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetLayoutLocation` | `Vector2Int` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetObject` | `Object` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `Highlight` | `Void` | `(Boolean hi, Color c)` |
| `Il2CppviperOSK.I_OSK_Key` | `getXSize` | `Single` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `getYSize` | `Single` | `()` |
| `Il2Cpp.InputManager` | `ConfinedCursorforUI` | `Void` | `()` |
| `Il2Cpp.Interact` | `CloseInteractionMenu` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `Awake` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `Disabling` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `KeepRotating` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `TweenHorizontal` | `Void` | `(Boolean leanout)` |
| `Il2Cpp.LeanTweenUIElement` | `TweenScaleInOut` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `TweenVertical` | `Void` | `(Boolean leanout)` |
| `Il2Cpp.LeanTweenUIElement` | `Update` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.Localisation` | `ReturnTextByID` | `String` | `(Int32 _uid)` |
| `Il2Cpp.LocalisedText` | `ChangeText` | `Void` | `()` |
| `Il2Cpp.LocalisedText` | `SetText` | `Void` | `(Int32 _localisation_uid)` |
| `Il2Cpp.MainGameManager` | `GetPatchPanelPrefab` | `GameObject` | `(Int32 switchType)` |
| `Il2Cpp.MainMenu` | `Continue` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `HideMiddleMenu` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `NewGame` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `Start` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `Start` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `Update` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `setmount` | `Void` | `(Transform newmount)` |
| `Il2Cpp.NetworkMap` | `IsPatchPanelPort` | `Boolean` | `(String deviceName)` |
| `Il2Cpp.NetworkMap` | `ResolveThroughPatchPanel` | `String` | `(String patchPanelPort, String fromDevice)` |
| `Il2Cpp.NetworkSwitch` | `UpdateScreenUI` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `BuildPatchPanelCache` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ButtonEditLabel` | `Void` | `()` |
| `Il2Cpp.NetworkSwitchConfiguration` | `CreateVLANButtonMulti` | `ButtonExtended` | `(Int32 vlanId, List`1 portIndices, Transform parent)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `OnEndEditingInputText` | `Void` | `(String s)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `RefreshVLANDisplayForSelection` | `Void` | `(Int32 selectVlanId)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `TraversePatchPanels` | `ValueTuple`2` | `(CableLink port)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `_ButtonEditLabel_b__31_0` | `Void` | `(String label)` |
| `Il2Cpp.Numpad` | `ClickButtonClear` | `Void` | `()` |
| `Il2Cpp.Numpad` | `ClickButtonCopy` | `Void` | `()` |
| `Il2Cpp.Numpad` | `ClickButtonDel` | `Void` | `()` |
| `Il2Cpp.Numpad` | `ClickButtonOK` | `Void` | `()` |
| `Il2Cpp.Numpad` | `ClickButtonPaste` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `AccentCharClick` | `Void` | `(String accentedChar, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Generate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `GenerateCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `IsVisible` | `Boolean` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Set` | `Boolean` | `(OSK_LongPressPacket packet)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `SetConsole` | `Void` | `(OSK_LongPressPacket packet)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `ShowBackground` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `AutoFindKeyboard` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `ResizeToFit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `BlinkCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Cursor` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `FindComponentInParentOrSiblings` | `T` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `OnDisable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `OnEnable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Show` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.OSK_Cursor` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `Activate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `DeActivate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `FixedUpdate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `GamepadPrep` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `GetSelectedKey` | `OSK_Key` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `JoystickButtonA` | `Boolean` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `JoystickButtonB` | `Boolean` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `SetSelectedKey` | `Void` | `(OSK_Key k)` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `SetSelectedKey` | `Void` | `(String k)` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `BuildAssignments` | `List`1` | `(OSK_LanguagePackage profile)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `BuildLookup` | `Dictionary`2` | `(OSK_LanguagePackage profile)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `Canonicalize` | `String` | `(String s, Nullable`1 script)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `EnumerateLetterGlyphs` | `List`1` | `(List`1 ranges, Boolean includeUppercase, Boolean includeLowercase, Boolean collapseCase, Boolean preferLowercase, Nullable`1 scriptForSpecials)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ExcludeRanges` | `List`1` | `(List`1 source, List`1 excludes)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `GetGlyphEnumSlots` | `List`1` | `()` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsLetter` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsLowercase` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsUppercase` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsValidCodePoint` | `Boolean` | `(Int32 cp)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `MergeRanges` | `List`1` | `(IReadOnlyList`1 a, List`1 b)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ToCodePoint` | `Int32` | `(String s)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ToIntRanges` | `List`1` | `(List`1 hexRanges)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `TryParseGlyphSuffix` | `Boolean` | `(String name, Int32& n)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `TryParseHex` | `Boolean` | `(String hex, Int32& value)` |
| `Il2CppviperOSK.OSK_Key` | `Assign` | `Void` | `(OSK_KeyCode newKey, OSK_KEY_TYPES ktype, String key_name)` |
| `Il2CppviperOSK.OSK_Key` | `AssignSpecialAction` | `Void` | `(Action`2 action)` |
| `Il2CppviperOSK.OSK_Key` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `BackScale` | `Void` | `(Vector3 scale)` |
| `Il2CppviperOSK.OSK_Key` | `Click` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `ClickCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetGameObject` | `GameObject` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetLayoutLocation` | `Vector2Int` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetObject` | `Object` | `()` |
| `Il2CppviperOSK.OSK_Key` | `Highlight` | `Void` | `(Boolean hi, Color c)` |
| `Il2CppviperOSK.OSK_Key` | `JoystickPressDown` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `JoystickPressUp` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `LastPressed` | `Single` | `()` |
| `Il2CppviperOSK.OSK_Key` | `LongPressCheck` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnDepressed` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnPressed` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `SetColors` | `Void` | `(Color bk_color, Color label_color)` |
| `Il2CppviperOSK.OSK_Key` | `SetLayoutLocation` | `Void` | `(Int32 x, Int32 y)` |
| `Il2CppviperOSK.OSK_Key` | `ShiftDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `ShiftUp` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `getXSize` | `Single` | `()` |
| `Il2CppviperOSK.OSK_Key` | `getYSize` | `Single` | `()` |
| `Il2CppviperOSK.OSK_KeySounds` | `PlaySelectKeySound` | `Void` | `()` |
| `Il2CppviperOSK.OSK_KeySounds` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_KeySounds` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `AcceptPhysicalKeyboard` | `Void` | `(Boolean accept)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddNewLine` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddString` | `Void` | `(String multichar)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddText` | `Void` | `(String newText)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddText_ShftEnabled` | `Void` | `(String newText)` |
| `Il2CppviperOSK.OSK_Keyboard` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ButtonA` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Submit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Generate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetOSKKey` | `OSK_Key` | `(String k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetOSKKeyCode` | `OSK_KeyCode` | `(String c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetSelectedKey` | `OSK_Key` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `HasFocus` | `Void` | `(Boolean isFocus)` |
| `Il2CppviperOSK.OSK_Keyboard` | `InsertText` | `Void` | `(String newText, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyScreenSize` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyboardSizeEstimator` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `OSK_to_KeyCode` | `KeyCode` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `OnGUI` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `OutputTextUpdate` | `Void` | `(String newchar, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `Prep` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `SelectSound` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `SelectedKeyMove` | `OSK_Key` | `(Vector2 dir, Vector2Int currentLoc, Boolean makeSoundIfMove)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SetOutput` | `Void` | `(OSK_Receiver newOutput)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SetSelectedKey` | `Void` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SetSelectedKey` | `Void` | `(String c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SpanBottomRight` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `SpanTopLeft` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Submit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Text` | `String` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Traverse` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `AddDiacritic` | `String` | `(Char baseChar, Il2CppStructArray`1 diacritics)` |
| `Il2CppviperOSK.OSK_Keymap` | `AddDiacritic` | `String` | `(Char baseChar, Char[] diacritics)` |
| `Il2CppviperOSK.OSK_Keymap` | `BaseCharacter` | `String` | `(String accentedChar)` |
| `Il2CppviperOSK.OSK_Keymap` | `IsAccentedCharacter` | `Boolean` | `(Char c)` |
| `Il2CppviperOSK.OSK_Keymap` | `SupportGlyphs` | `Void` | `(OSK_LanguagePackage glyphProfile)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `CreateBackground` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Generate` | `Void` | `(List`1 chars, Boolean shiftup, Action`2 callbackAction, Boolean bottomLeftOrder)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `GetSize` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `ResizeBackground` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SelectedFirstKey` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SelectedKeyMove` | `Void` | `(Vector2 dir)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `AddText` | `Void` | `(String newchar)` |
| `Il2CppviperOSK.OSK_Receiver` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Backspace` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `ClearText` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Del` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Deselect` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `LateUpdate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `ModifyLastChar` | `Void` | `(String newLastChar)` |
| `Il2CppviperOSK.OSK_Receiver` | `NewLine` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `NewLineFix` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `OnFocus` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `OnFocusLost` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `ParsedText` | `String` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Selection` | `Int32` | `(Vector3 hitpoint, Boolean charhit)` |
| `Il2CppviperOSK.OSK_Receiver` | `SelectionHighlight` | `Void` | `(Color32 c, Boolean all)` |
| `Il2CppviperOSK.OSK_Receiver` | `SetText` | `Void` | `(String newText)` |
| `Il2CppviperOSK.OSK_Receiver` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Submit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `Text` | `String` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `ToggleCharMask` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `ToggleCharMask` | `Void` | `(Boolean on_off_charmask)` |
| `Il2CppviperOSK.OSK_Receiver` | `ValueChanged` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `isFocused` | `Boolean` | `()` |
| `Il2CppviperOSK.OSK_Settings` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Settings` | `SetLongPressAction` | `Void` | `(UnityAction`1 action)` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `BlinkCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Cursor` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `FindComponentInParentOrSiblings` | `T` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `OnDisable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `OnEnable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Show` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `Deselect` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `OnPointerDown` | `Void` | `(PointerEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `OnPointerUp` | `Void` | `(PointerEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `OnSelect` | `Void` | `(BaseEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `Selection` | `Int32` | `(Vector3 hitpoint, Boolean charhit)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `SelectionHighlight` | `Void` | `(Color32 c, Boolean all)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `UnityEngine_EventSystems_ISubmitHandler_OnSubmit` | `Void` | `(BaseEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_CustomReceiver` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `AddText` | `Void` | `(String newchar)` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Backspace` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `ClearText` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Del` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `NewLine` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `OnDrag` | `Void` | `(PointerEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `OnFocus` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `OnFocusLost` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `ParsedText` | `String` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `SelectionEnd` | `Int32` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Submit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Text` | `String` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `ToggleCharMask` | `Void` | `(Boolean on_off_charmask)` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `UnityEngine_EventSystems_ISubmitHandler_OnSubmit` | `Void` | `(BaseEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_Key` | `Assign` | `Void` | `(OSK_KeyCode newKey, OSK_KEY_TYPES ktype, String name)` |
| `Il2CppviperOSK.OSK_UI_Key` | `AssignSpecialAction` | `Void` | `(Action`2 action)` |
| `Il2CppviperOSK.OSK_UI_Key` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `BackScale` | `Void` | `(Vector3 scale)` |
| `Il2CppviperOSK.OSK_UI_Key` | `Click` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_UI_Key` | `ClickCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `Dir` | `OSK_UI_Key` | `(Int32 x, Int32 y)` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetGameObject` | `GameObject` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetLayoutLocation` | `Vector2Int` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `GetObject` | `Object` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `Highlight` | `Void` | `(Boolean hi, Color c)` |
| `Il2CppviperOSK.OSK_UI_Key` | `JoystickPressDown` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_UI_Key` | `JoystickPressUp` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_UI_Key` | `LastPressed` | `Single` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `LongPressCheck` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnDepressed` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnPointerDown` | `Void` | `(PointerEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnPointerUp` | `Void` | `(PointerEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnPressed` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `OnSubmit` | `Void` | `(BaseEventData eventData)` |
| `Il2CppviperOSK.OSK_UI_Key` | `SetBkColor` | `Void` | `(Color bk_color, Boolean reset_base_color)` |
| `Il2CppviperOSK.OSK_UI_Key` | `SetColors` | `Void` | `(Color bk_color, Color label_color)` |
| `Il2CppviperOSK.OSK_UI_Key` | `SetLayoutLocation` | `Void` | `(Int32 x, Int32 y)` |
| `Il2CppviperOSK.OSK_UI_Key` | `ShiftDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `ShiftUp` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `getXSize` | `Single` | `()` |
| `Il2CppviperOSK.OSK_UI_Key` | `getYSize` | `Single` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `ButtonA` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `FixedUpdate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `GamepadWrapNavigation` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Generate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `KeyScreenSize` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `OnGUI` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `PrepAssetGroup` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SelectKey` | `IEnumerator` | `(OSK_UI_Key selKey)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SelectedKey` | `OSK_UI_Key` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SetSelectedKey` | `Void` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SetSelectedKey` | `Void` | `(String c)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SetSelectedKey` | `Void` | `(OSK_UI_Key k)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `ShowHideKeyboard` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SpanBottomRight` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SpanTopLeft` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Traverse` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Update` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `PlayUIEffectDisolve` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `Awake` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `GenerateUniquePatchPanelId` | `String` | `()` |
| `Il2Cpp.PatchPanel` | `GetPairedLink` | `CableLink` | `(CableLink link)` |
| `Il2Cpp.PatchPanel` | `InsertedInRack` | `Void` | `(PatchPanelSaveData saveData)` |
| `Il2Cpp.PatchPanel` | `ValidateRackPosition` | `Boolean` | `()` |
| `Il2Cpp.PauseMenu` | `MainMenu` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabButton` | `UnityEngine_EventSystems_ISelectHandler_OnSelect` | `Void` | `(BaseEventData eventData)` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabSelected` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PlayerManager` | `ConfinedCursorforUI` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `Awake` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `TweenTheColors` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `Update` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `setColorCallback` | `Void` | `(Color c)` |
| `Il2Cpp.PulsatingText` | `TweenTheColors` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `setColorCallback` | `Void` | `(Color c)` |
| `Il2Cpp.Rack` | `ButtonDisablePositionsInRack` | `Void` | `()` |
| `Il2Cpp.Rack` | `ButtonUnmountRack` | `Void` | `()` |
| `Il2Cpp.Rack` | `SetDisablePositionsButtonMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.RackPosition` | `GetByUID` | `RackPosition` | `(Int32 uid)` |
| `Il2Cpp.RackPosition` | `SetUID` | `Void` | `(Int32 uid)` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `CloseInteractionMenu` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `Awake` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `DoRebind` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `GetBindingInfo` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `OnValidate` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `ResetBinding` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `UpdateUI` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `_OnEnable_b__16_0` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `_OnEnable_b__16_1` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `OnActionChange` | `Void` | `(Object obj, InputActionChange change)` |
| `Il2Cpp.RebindUIv2` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `ResetToDefault` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `ResolveActionAndBinding` | `Boolean` | `(InputAction& action, Int32& bindingIndex)` |
| `Il2Cpp.RebindUIv2` | `UpdateActionLabel` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `UpdateBindingDisplay` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `_UpdateBindingDisplay_b__30_0` | `Boolean` | `(InputBinding x)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Initialize` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Initialize` | `Void` | `(IRecyclableScrollRectDataSource dataSource)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `OnValueChangedListener` | `Void` | `(Vector2 normalizedPos)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Start` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `_Initialize_b__13_0` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclingSystem` | `InitCoroutine` | `IEnumerator` | `(Action onInitialized)` |
| `Il2CppPolyAndCode.UI.RecyclingSystem` | `OnValueChangedListener` | `Vector2` | `(Vector2 direction)` |
| `Il2Cpp.ReusableFunctions` | `ChangeButtonNormalColor` | `Void` | `(Button button, Color color)` |
| `Il2Cpp.ReusableFunctions` | `HexToColor` | `Color` | `(String hex)` |
| `Il2Cpp.ReusableFunctions` | `ImageScrollingUI` | `IEnumerator` | `(Il2CppReferenceArray`1 _sprites, Image _image)` |
| `Il2Cpp.ReusableFunctions` | `NumberScrollingUI` | `IEnumerator` | `(TextMeshProUGUI _text, Int32 _endNumber)` |
| `Il2Cpp.RouterConfiguration` | `ButtonAddBgpPeer` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonAddRoute` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetMask` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetNeighborId` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetSubnet` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetTargetVLAN` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetVLAN` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `CreateRouteRowUI` | `Void` | `(Int32 sVlan, Int32 tVlan, String subnetCidr)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetMask_b__17_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetNeighborId_b__25_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetSubnet_b__16_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetTargetVLAN_b__19_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetVLAN_b__18_0` | `Void` | `(String text)` |
| `Il2Cpp.SetIP` | `ButtonEditLabel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ButtonHideShowHint` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonCancel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonClear` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonCopy` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonDel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonOK` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonPaste` | `Void` | `()` |
| `Il2Cpp.SetIP` | `_ButtonEditLabel_b__30_0` | `Void` | `(String label)` |
| `Il2Cpp.SettingsGameplay` | `ButtonUnstuckPlayer` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `ChangeDepthOfField` | `Void` | `(Single startFarFocus, Single endFarFocus)` |
| `Il2Cpp.SettingsGraphics` | `IsDLSSSupported` | `Boolean` | `()` |
| `Il2Cpp.SettingsGraphics` | `LimitFrameRate` | `Void` | `(Int32 _framerate)` |
| `Il2Cpp.SettingsGraphics` | `PopulateMonitors` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `RepopulateResolutions` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `ResetDepthOfField` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `SetAAQuality` | `Void` | `(Int32 index)` |
| `Il2Cpp.SettingsGraphics` | `SetAntiAliasing` | `Void` | `(Int32 index)` |
| `Il2Cpp.SettingsGraphics` | `SetFieldOfView` | `Void` | `(Single fov)` |
| `Il2Cpp.SettingsGraphics` | `SetFullScreen` | `Void` | `(Int32 index)` |
| `Il2Cpp.SettingsGraphics` | `SetMonitor` | `Void` | `(Int32 monitorIndex)` |
| `Il2Cpp.SettingsGraphics` | `SetMotionBlur` | `Void` | `(Single motion)` |
| `Il2Cpp.SettingsGraphics` | `SetQuality` | `Void` | `(Int32 qualityIndex)` |
| `Il2Cpp.SettingsGraphics` | `SetResDropDown` | `Void` | `(Int32 resolutionIndex)` |
| `Il2Cpp.SettingsGraphics` | `SetResolution` | `Void` | `(Int32 width, Int32 height)` |
| `Il2Cpp.SettingsGraphics` | `SetShadowDistance` | `Void` | `(Single distance)` |
| `Il2Cpp.SettingsGraphics` | `SetupAA` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `Start` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `ClearAllUIDs` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `UnlockButton` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `WarpText` | `IEnumerator` | `()` |
| `Il2Cpp.StaticUIElements` | `AddMeesageInField` | `Void` | `(String message)` |
| `Il2Cpp.StaticUIElements` | `Awake` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonCancelInputNumpadOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonCancelInputTextOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `CalculateRates` | `Void` | `(Single& moneyPerSec, Single& xpPerSec, Single& expensesPerSec)` |
| `Il2Cpp.StaticUIElements` | `ClearSpriteNextToPointer` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `HideTextUnderCursor` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `InstantiateErrorWarningSign` | `Int32` | `(Boolean isError, Vector3 objectPos)` |
| `Il2Cpp.StaticUIElements` | `InstantiateParticleUpgrade` | `Void` | `(Transform _transform)` |
| `Il2Cpp.StaticUIElements` | `RestorePreviousSelection` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `SetNotification` | `Void` | `(Int32 _localisationUID, Sprite _sprite, String _text)` |
| `Il2Cpp.StaticUIElements` | `ShowInputTextOverlay` | `Void` | `(String title, Action`1 onConfirmed, String defaultText, Boolean isOpenedFromWorld, GameObject selectOnClose)` |
| `Il2Cpp.StaticUIElements` | `ShowSpriteNextToPointer` | `Void` | `(Sprite _sprite)` |
| `Il2Cpp.StaticUIElements` | `ShowTextUnderCursor` | `Void` | `(String text)` |
| `Il2Cpp.StaticUIElements` | `Start` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `UpdateHoldProgress` | `Void` | `(Single value)` |
| `Il2Cpp.StaticUIElements` | `UpdateMessageDisplay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `UpdateMessagesCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.SteamManager` | `SteamAPIDebugTextHook` | `Void` | `(Int32 nSeverity, StringBuilder pchDebugText)` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `FormatDistance` | `String` | `(Double meters)` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `OnGlobalStatsReceived` | `Void` | `(GlobalStatsReceived_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `Start` | `Void` | `()` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `WaitAndDisplay` | `IEnumerator` | `()` |
| `Il2CppTMPro.TMP_DigitValidator` | `Validate` | `Char` | `(String& text, Int32& pos, Char ch)` |
| `Il2CppTMPro.Examples.TMP_ExampleScript_01` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_ExampleScript_01` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Set_FrameCounter_Position` | `Void` | `(FpsCounterAnchorPositions anchor_position)` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Update` | `Void` | `()` |
| `Il2CppTMPro.TMP_PhoneNumberValidator` | `Validate` | `Char` | `(String& text, Int32& pos, Char ch)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnCharacterSelection` | `Void` | `(Char c, Int32 index)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnLineSelection` | `Void` | `(String lineText, Int32 firstCharacterIndex, Int32 length)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnLinkSelection` | `Void` | `(String linkID, String linkText, Int32 linkIndex)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnSpriteSelection` | `Void` | `(Char c, Int32 index)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnWordSelection` | `Void` | `(String word, Int32 firstCharacterIndex, Int32 length)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.TMP_TextEventHandler` | `LateUpdate` | `Void` | `()` |
| `Il2CppTMPro.TMP_TextEventHandler` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `SendOnCharacterSelection` | `Void` | `(Char character, Int32 characterIndex)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `SendOnLineSelection` | `Void` | `(String line, Int32 charIndex, Int32 length)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `SendOnLinkSelection` | `Void` | `(String linkID, String linkText, Int32 linkIndex)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `SendOnSpriteSelection` | `Void` | `(Char character, Int32 characterIndex)` |
| `Il2CppTMPro.TMP_TextEventHandler` | `SendOnWordSelection` | `Void` | `(String word, Int32 charIndex, Int32 length)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_A` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_A` | `LateUpdate` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_A` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_A` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `LateUpdate` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnPointerClick` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `OnPointerUp` | `Void` | `(PointerEventData eventData)` |
| `Il2CppTMPro.Examples.TMP_TextSelector_B` | `RestoreCachedVertexAttributes` | `Void` | `(Int32 index)` |
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Set_FrameCounter_Position` | `Void` | `(FpsCounterAnchorPositions anchor_position)` |
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMPro_InstructionOverlay` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMPro_InstructionOverlay` | `Set_FrameCounter_Position` | `Void` | `(FpsCounterAnchorPositions anchor_position)` |
| `Il2Cpp.Technician` | `StartTextingAnimation` | `IEnumerator` | `()` |
| `Il2Cpp.TerrainDetector` | `GetActiveTerrainTextureIdx` | `Int32` | `(Vector3 position)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `DisplayTextMeshFloatingText` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `DisplayTextMeshProFloatingText` | `IEnumerator` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnDeselect` | `Void` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.ToolTipOnUIText` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.ToolTipOnUIText` | `OnSelect` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `ButtonOK` | `Void` | `()` |
| `Il2Cpp.UIExtension` | `GetCorners` | `Il2CppStructArray`1` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MaxX` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MaxY` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MinX` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MinY` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UI_Section` | `OpenCloseSection` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `Awake` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `Update` | `Void` | `()` |
| `Il2Cpp.UserReport` | `SubmitUserReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `_SubmitUserReport_b__19_0` | `Void` | `(Single uploadProgress)` |
| `Il2Cpp.UserReport` | `_SubmitUserReport_b__19_1` | `Void` | `(Boolean success)` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.VertexShakeA` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.VertexShakeB` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.VertexZoom` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `CreateCellPool` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `InitCoroutine` | `IEnumerator` | `(Action onInitialized)` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `OnDrawGizmos` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `OnValueChangedListener` | `Vector2` | `(Vector2 direction)` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `RecycleBottomToTop` | `Vector2` | `()` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `RecycleTopToBottom` | `Vector2` | `()` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `SetRecyclingBounds` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `SetTopAnchor` | `Void` | `(RectTransform rectTransform)` |
| `Il2CppPolyAndCode.UI.VerticalRecyclingSystem` | `SetTopLeftAnchor` | `Void` | `(RectTransform rectTransform)` |
| `Il2CppTMPro.Examples.WarpTextExample` | `WarpText` | `IEnumerator` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `RecoverMissingPatchPanels` | `IEnumerator` | `(NetworkSaveData networkData, Int32 saveVersion)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateAllUI` | `Void` | `(Dictionary`2 customerInfo, List`1 allRoutes, Dictionary`2 cableLoad, Dictionary`2 cablePairLookup)` |
| `Il2Cpp.WorldObjectButton` | `Awake` | `Void` | `()` |
| `Il2CppviperTools.viperInput` | `AButtonDown` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `AButtonUp` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `BButtonDown` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `BButtonUp` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `GetPlayerAButton` | `Boolean` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `GetPlayerBButton` | `Boolean` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `GetPointerPos` | `Vector2` | `()` |
| `Il2CppviperTools.viperInput` | `PointerDown` | `Boolean` | `(Int32 mouseBtn)` |
| `Il2CppviperTools.viperInput` | `PointerUp` | `Boolean` | `(Int32 mouseBtn)` |

### World

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AudioManager` | `PlayRackDoorOpen` | `Void` | `()` |
| `Il2Cpp.CableLink` | `CreateRopeAttachPoint` | `Void` | `()` |
| `Il2Cpp.CableLink` | `GetRopeAttachPoint` | `Transform` | `()` |
| `Il2Cpp.CableLink` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.CableLink` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.CableSpinner` | `InteractOnClick` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `LateUpdate` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `Start` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `BrakeAndDeacceleration` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `DisableTheTriggerColliderAfterDealy` | `IEnumerator` | `()` |
| `Il2CppPolyStang.CarController` | `FixedUpdate` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `LeaveTheTrolley` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `OnCollisionEnter` | `Void` | `(Collision collision)` |
| `Il2CppPolyStang.CarController` | `ResetTrolleyPosition` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `ResetingTrollerPosition` | `IEnumerator` | `()` |
| `Il2CppPolyStang.CarController` | `Start` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `Steer` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `StopCar` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `TakeTheWheel` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `TurnBackOnCollidersInTRolley` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `TurnOffCollidersInTrolley` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `_Start_b__32_0` | `Void` | `(CallbackContext ctx)` |
| `Il2CppPolyStang.CarController` | `_Start_b__32_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.CarryModelPool` | `Awake` | `Void` | `()` |
| `Il2Cpp.CarryModelPool` | `ClearPool` | `Void` | `()` |
| `Il2Cpp.CarryModelPool` | `Get` | `GameObject` | `(GameObject prefab, Int32 prefabID, Vector3 position, Quaternion rotation, Transform parent)` |
| `Il2Cpp.CarryModelPool` | `Return` | `Void` | `(GameObject obj, Int32 prefabID)` |
| `Il2Cpp.ChatController` | `AddToChatOutput` | `Void` | `(String newText)` |
| `Il2Cpp.ChatController` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ChatController` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.CustomerBaseDoor` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Dumpster` | `Awake` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.EnvMapAnimator` | `Awake` | `Void` | `()` |
| `Il2Cpp.EnvMapAnimator` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.GateLever` | `Awake` | `Void` | `()` |
| `Il2Cpp.GateLever` | `CloseGate` | `Void` | `()` |
| `Il2Cpp.GateLever` | `GateCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.GateLever` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.GateLever` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.GateLever` | `OpenGate` | `Void` | `()` |
| `Il2Cpp.GateLever` | `TruckComing` | `Void` | `()` |
| `Il2Cpp.InputController` | `Contains` | `Boolean` | `(InputAction action)` |
| `Il2Cpp.InputController` | `Disable` | `Void` | `()` |
| `Il2Cpp.InputController` | `Dispose` | `Void` | `()` |
| `Il2Cpp.InputController` | `Enable` | `Void` | `()` |
| `Il2Cpp.InputController` | `Finalize` | `Void` | `()` |
| `Il2Cpp.InputController` | `FindAction` | `InputAction` | `(String actionNameOrId, Boolean throwIfNotFound)` |
| `Il2Cpp.InputController` | `FindBinding` | `Int32` | `(InputBinding bindingMask, InputAction& action)` |
| `Il2Cpp.InputController` | `GetEnumerator` | `IEnumerator`1` | `()` |
| `Il2Cpp.InputController` | `System_Collections_IEnumerable_GetEnumerator` | `IEnumerator` | `()` |
| `Il2Cpp.Interact` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.MainGameManager` | `ResetTrolleyPosition` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.NetworkSwitch` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsSurrogate` | `Boolean` | `(Int32 cp)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SetInteractable` | `Void` | `(Boolean isInteractable)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SetInteractable` | `Void` | `(Boolean isInteractable)` |
| `Il2Cpp.PatchPanel` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.PushTrolleyHandle` | `Awake` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RackDoor` | `DelayedTrigger` | `IEnumerator` | `()` |
| `Il2Cpp.RackDoor` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackDoor` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RackMount` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackMount` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RackPosition` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RebindUIv2` | `PerformInteractiveRebind` | `Void` | `(InputAction action, Int32 bindingIndex, Boolean allCompositeParts)` |
| `Il2Cpp.RebindUIv2` | `StartInteractiveRebind` | `Void` | `()` |
| `Il2Cpp.ReusableFunctions` | `CalculateHowManyTimesIsNumberInIntArray` | `Int32` | `(Int32 numberToFind, Il2CppStructArray`1 inArray)` |
| `Il2Cpp.ReusableFunctions` | `CalculatePercentage` | `Double` | `(Int32 total, Int32 number)` |
| `Il2Cpp.ReusableFunctions` | `DisableGameObjectWithDelay` | `IEnumerator` | `(GameObject go, Single time)` |
| `Il2Cpp.ReusableFunctions` | `IsBetweenRange` | `Boolean` | `(Single thisValue, Single value1, Single value2)` |
| `Il2Cpp.ReusableFunctions` | `ShuffledArrayOfInts` | `Il2CppStructArray`1` | `(Int32 arrayLenght)` |
| `Il2Cpp.ReusableFunctions` | `SplitCsvLine` | `Il2CppStringArray` | `(String line)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `AreEndPointsValid` | `Boolean` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `CalculateYFactorAdjustment` | `Single` | `(Single weight)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `FixedUpdate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `GetMidPoint` | `Vector3` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `GetPointAt` | `Vector3` | `(Single t)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `GetRationalBezierPoint` | `Vector3` | `(Vector3 p0, Vector3 p1, Vector3 p2, Single t, Single w0, Single w1, Single w2)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `InitializeLineRenderer` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `IsRopeSettingsChanged` | `Boolean` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `NotifyPointsChanged` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `OnDrawGizmos` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `OnValidate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `RecalculateRope` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `SetEndPoint` | `Void` | `(Transform newEndPoint, Boolean instantAssign)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `SetMidPoint` | `Void` | `(Transform newMidPoint, Boolean instantAssign)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `SetSplinePoint` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `SetStartPoint` | `Void` | `(Transform newStartPoint, Boolean instantAssign)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `SimulatePhysics` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `Start` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `Update` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `Awake` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `CheckEndPoints` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `CreateRopeMesh` | `Void` | `(Il2CppStructArray`1 points, Single radius, Int32 segmentsPerWire)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `DelayedGenerateMesh` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `GenerateMesh` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `InitializeComponents` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnDisable` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnEnable` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnValidate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `SubscribeToRopeEvents` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `UnsubscribeFromRopeEvents` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `Update` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Awake` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `FixedUpdate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `GenerateWind` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `SimulatePhysics` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Start` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Update` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.SFPModule` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Server` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Server` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `AnimateProperties` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `Start` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `GetClosestOpenedDumpsterIndex` | `Int32` | `(Vector3 position)` |
| `Il2Cpp.TechnicianManager` | `OpenDumpsterArea` | `Void` | `(Int32 areaID)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `UpdateAnimator` | `Void` | `(Vector3 move)` |
| `Il2Cpp.TrolleyLoadingBay` | `FreeTrolleySlot` | `Void` | `(Int32 startIdx, Int32 sizeInU)` |
| `Il2Cpp.TrolleyLoadingBay` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.TrolleyTrigger` | `ObjectAdded` | `IEnumerator` | `(Collider other, UsableObject uo)` |
| `Il2Cpp.TrolleyTrigger` | `OnTriggerEnter` | `Void` | `(Collider other)` |
| `Il2Cpp.UsableObject` | `ActionInHand` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `Awake` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `CheckIfLost` | `IEnumerator` | `()` |
| `Il2Cpp.UsableObject` | `DisalowDrop` | `IEnumerator` | `()` |
| `Il2Cpp.UsableObject` | `DistanceKinematicCheck` | `IEnumerator` | `()` |
| `Il2Cpp.UsableObject` | `DropObject` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `FixedUpdate` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.UsableObject` | `MakeInteractableAgain` | `IEnumerator` | `()` |
| `Il2Cpp.UsableObject` | `OnCollisionEnter` | `Void` | `(Collision collision)` |
| `Il2Cpp.UsableObject` | `RestoreRigidbody` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.Wall` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Wall` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.WorldCanvasCuller` | `Awake` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `Init` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.WorldObjectButton` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.WorldObjectButton` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |


---

## 2. Greg Hooks

These are the canonical greg-domain hooks registered in the framework hook bus.
Use them in your mod's event subscriptions.

_No greg hooks defined._


---

*Generated by `scripts/generate_api_docs.py` — see [README.md](../README.md) for details.*
