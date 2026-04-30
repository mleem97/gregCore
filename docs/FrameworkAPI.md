# gregCore FrameworkAPI Reference

> **Version:** 1.0.0.38 | **Generated:** 2026-04-30
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
| `Il2Cpp.AudioManager` | `PlayEffectAudioClip` | `Void` | `(AudioClip audioClip, Single volume, Single delayed)` |
| `Il2Cpp.AudioManager` | `PlayRackDoorOpen` | `Void` | `()` |
| `Il2Cpp.AudioManager` | `PlayRandomImpactClip` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `PlayRandomRJ45Clip` | `Void` | `()` |
| `Il2Cpp.AudioManager` | `SetEffectsVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetMasterVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetMusic` | `Void` | `(Int32 _clipUID)` |
| `Il2Cpp.AudioManager` | `SetMusicVolume` | `Void` | `(Single _volume)` |
| `Il2Cpp.AudioManager` | `SetRacksVolume` | `Void` | `(Single _volume)` |
| `Il2CppviperOSK.OSK_KeySounds` | `PlaySound` | `Void` | `(Int32 k)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `PlayStepSound` | `Void` | `()` |

### Automation

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.MainGameManager` | `AutoSaveCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.MainGameManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonBuyWall` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCancelBuyWall` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCancelCustomerChoice` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ButtonCustomerChosen` | `Void` | `(Int32 _cardID)` |
| `Il2Cpp.MainGameManager` | `CloseAnyCanvas` | `Void` | `(Boolean isCustomerChoice)` |
| `Il2Cpp.MainGameManager` | `CloseNetworkConfigCanvas` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `CreateFallbackCustomer` | `CustomerItem` | `(CustomerItem original, Int32 customerBaseID)` |
| `Il2Cpp.MainGameManager` | `GetAppLogo` | `Sprite` | `(Int32 customerID, Int32 appID)` |
| `Il2Cpp.MainGameManager` | `GetCableSpinnerPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerItemByID` | `CustomerItem` | `(Int32 customerID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerLogo` | `Sprite` | `(Int32 customerID)` |
| `Il2Cpp.MainGameManager` | `GetCustomerTotalRequirement` | `Single` | `(CustomerItem customer)` |
| `Il2Cpp.MainGameManager` | `GetFirewallPrefab` | `GameObject` | `(Int32 firewallType)` |
| `Il2Cpp.MainGameManager` | `GetFreeSubnet` | `String` | `(Single appRequirements)` |
| `Il2Cpp.MainGameManager` | `GetPatchPanelPrefab` | `GameObject` | `(Int32 switchType)` |
| `Il2Cpp.MainGameManager` | `GetRouterPrefab` | `GameObject` | `(Int32 routerType)` |
| `Il2Cpp.MainGameManager` | `GetServerPrefab` | `GameObject` | `(Int32 serverType)` |
| `Il2Cpp.MainGameManager` | `GetSfpBoxPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `GetSfpPrefab` | `GameObject` | `(Int32 prefabID)` |
| `Il2Cpp.MainGameManager` | `GetSwitchPrefab` | `GameObject` | `(Int32 switchType)` |
| `Il2Cpp.MainGameManager` | `InitializeVlanPool` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `IsCustomerSuitableForBase` | `Boolean` | `(CustomerItem customer, Int32 customerBaseID)` |
| `Il2Cpp.MainGameManager` | `IsSubnetValid` | `Boolean` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `LoadTrolleyPosition` | `Void` | `(Vector3 _position, Quaternion _rotation)` |
| `Il2Cpp.MainGameManager` | `OnApplicationQuit` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `OpenAnyCanvas` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `RemoveUsedSubnet` | `Void` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `RemoveUsedVlanId` | `Void` | `(Int32 vlanId)` |
| `Il2Cpp.MainGameManager` | `ResetTrolleyPosition` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `RestartAutoSave` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ReturnServerNameFromType` | `String` | `(Int32 type)` |
| `Il2Cpp.MainGameManager` | `ReturnSubnet` | `Void` | `(String subnet)` |
| `Il2Cpp.MainGameManager` | `ReturnSwitchNameFromType` | `String` | `(Int32 type)` |
| `Il2Cpp.MainGameManager` | `ReturnVlanId` | `Void` | `(Int32 vlanId)` |
| `Il2Cpp.MainGameManager` | `SetAutoSaveEnabled` | `Void` | `(Boolean enabled)` |
| `Il2Cpp.MainGameManager` | `SetAutoSaveInterval` | `Void` | `(Single minutes)` |
| `Il2Cpp.MainGameManager` | `ShowBuyWallCanvas` | `Void` | `(Wall wall)` |
| `Il2Cpp.MainGameManager` | `ShowCustomerCardsCanvas` | `Void` | `(CustomerBaseDoor _door)` |
| `Il2Cpp.MainGameManager` | `ShowNetworkConfigCanvas` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.MainGameManager` | `ShowRouterConfigCanvas` | `Void` | `(Router router)` |
| `Il2Cpp.MainGameManager` | `ShuffleAvailableCustomers` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `ShuffleAvailableSubnets` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `Start` | `Void` | `()` |
| `Il2Cpp.MainGameManager` | `_Awake_b__67_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.MainGameManager` | `_ShuffleAvailableCustomers_b__79_0` | `Boolean` | `(Int32 index)` |
| `Il2Cpp.MainGameManager` | `_ShuffleAvailableCustomers_b__79_1` | `Int32` | `(Int32 index)` |
| `Il2Cpp.SaveSystem` | `AutoSave` | `Void` | `()` |
| `Il2Cpp.SaveSystem` | `DeleteSaveFile` | `Void` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `FormatDisplayName` | `String` | `(String rawEntry)` |
| `Il2Cpp.SaveSystem` | `GetBinaryFormatter` | `BinaryFormatter` | `()` |
| `Il2Cpp.SaveSystem` | `GetRawSaveEntry` | `String` | `(String displayName)` |
| `Il2Cpp.SaveSystem` | `Listofsaves` | `List`1` | `()` |
| `Il2Cpp.SaveSystem` | `NewestSave` | `String` | `()` |
| `Il2Cpp.SaveSystem` | `ReadMeta` | `SaveMeta` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `WriteMeta` | `Void` | `(String savename, Int32 version, String nameOfSave)` |
| `Il2Cpp.TimeController` | `Awake` | `Void` | `()` |
| `Il2Cpp.TimeController` | `CurrentTimeInHours` | `Single` | `()` |
| `Il2Cpp.TimeController` | `HoursFromDate` | `Int32` | `(Single _time, Int32 _day)` |
| `Il2Cpp.TimeController` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.TimeController` | `Start` | `Void` | `()` |
| `Il2Cpp.TimeController` | `TimeIsBetween` | `Boolean` | `(Single startHour, Single endHour)` |
| `Il2Cpp.TimeController` | `Update` | `Void` | `()` |

### Character

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AICharacterControl` | `AgentReachTarget` | `Boolean` | `()` |
| `Il2Cpp.AICharacterControl` | `AnimSit` | `Void` | `(Boolean active)` |
| `Il2Cpp.AICharacterControl` | `GotoNextPoint` | `Void` | `(Il2CppReferenceArray`1 _waypoints)` |
| `Il2Cpp.AICharacterControl` | `OnCreated` | `Void` | `(UMAData umadata)` |
| `Il2Cpp.AICharacterControl` | `OnDestroy` | `Void` | `()` |
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
| `Il2Cpp.AICharacterExpressions` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.AICharacterExpressions` | `Start` | `Void` | `()` |
| `Il2Cpp.AICharacterExpressions` | `Talk` | `Void` | `(String sentence)` |
| `Il2Cpp.AICharacterExpressions` | `Talking` | `IEnumerator` | `(List`1 _syllables)` |
| `Il2CppTMPro.Examples.Benchmark01` | `Start` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.Benchmark01_UGUI` | `Start` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.Benchmark02` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.Benchmark03` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.Benchmark04` | `Start` | `Void` | `()` |
| `Il2Cpp.CableLink` | `Start` | `Void` | `()` |
| `Il2Cpp.CablePositions` | `RemoveLastPosition` | `Transform` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `RemovePosition` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.CablePositions` | `Start` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `GetPlayerInput` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `Start` | `Void` | `()` |
| `Il2Cpp.CommandCenterOperator` | `Start` | `Void` | `()` |
| `Il2Cpp.Firewall` | `RemoveRule` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `Crouch` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `GetInput` | `Void` | `(Single& speed)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `GetMouseLook` | `MouseLook` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `HandleZoom` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `OnControllerColliderHit` | `Void` | `(ControllerColliderHit hit)` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `OnDestroy` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `PlayLandingSound` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.FirstPersonController` | `PlayRandomAudioClip` | `AudioClip` | `(Il2CppReferenceArray`1 audioClips, Single volume)` |
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
| `Il2Cpp.GetCurrentVersion` | `Start` | `Void` | `()` |
| `Il2Cpp.GetValueFromPlayerPrefs` | `Start` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `Start` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `ClampRotationAroundXAxis` | `Quaternion` | `(Quaternion q)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `Init` | `Void` | `(Transform character, Transform camera)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `InternalLockUpdate` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `LookRotation` | `Void` | `(Transform character, Transform camera, Quaternion externalRotation, Transform ladderTrigger)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `MouseLookOnDisable` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `ResetRotation` | `Void` | `(Transform character)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `SetCursorLock` | `Void` | `(Boolean value)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `SittingClampRotation` | `Vector2` | `(Vector2 q)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `UpdateCursorLock` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `_Init_b__22_0` | `Void` | `(CallbackContext ctx)` |
| `UnityStandardAssets.Characters.FirstPerson.MouseLook` | `_Init_b__22_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.NetworkSwitch` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `JoystickInput` | `Vector2` | `()` |
| `Il2CppviperOSK.OSK_GamepadHelper` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_KeySounds` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_InputReceiver` | `TMPInputFieldReActivate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `DpadMove` | `Void` | `(Vector2 dir)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Start` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `Start` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabButton` | `Start` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `Start` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `Cleanup` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `CloseInteractionMenu` | `Void` | `()` |
| `UnityStandardAssets.Characters.FirstPerson.RayLookAt` | `HandleLookAtRay` | `Void` | `(Transform character)` |
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
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Start` | `Void` | `()` |
| `Il2Cpp.Router` | `RemoveRoute` | `Void` | `(Int32 sourceVlanId)` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `Start` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `OnRemoveClicked` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.SimpleScript` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `CopyAnimationCurve` | `AnimationCurve` | `(AnimationCurve curve)` |
| `Il2CppTMPro.Examples.SkewTextExample` | `Start` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `RemoveCustomKeyHint` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ShowInputNumpadtOverlay` | `Void` | `(String title, Action`1 onConfirmed, GameObject selectOnClose)` |
| `Il2Cpp.StaticUIElements` | `Start` | `Void` | `()` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TeleType` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.TerrainDetector` | `ConvertToSplatMapCoordinate` | `Vector3` | `(Vector3 worldPosition)` |
| `Il2Cpp.TerrainDetector` | `SetCurrentTerrain` | `Void` | `(Terrain _terrain)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshSpawner` | `Start` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `Awake` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `HandleGroundedMovement` | `Void` | `(Boolean crouch, Boolean jump)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `Move` | `Void` | `(Vector3 move, Boolean crouch, Boolean jump, Boolean onlyturn, Boolean backward)` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `OnAnimationEventFootStep` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `OnAnimatorMove` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `Start` | `Void` | `()` |
| `Il2CppTMPro.Examples.WarpTextExample` | `CopyAnimationCurve` | `AnimationCurve` | `(AnimationCurve curve)` |
| `Il2CppTMPro.Examples.WarpTextExample` | `Start` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `ActivateSpawnerOnCable` | `Void` | `(Entity spawnerEntity, Single speed, Int32 customerId)` |
| `Il2Cpp.WaypointInitializationSystem` | `ActivateSpawnersForCable` | `Void` | `(CableInfo cable, Single finalSpeed, HashSet`1 customersOnCable, HashSet`1 directions)` |
| `Il2Cpp.WaypointInitializationSystem` | `CleanUpSystem` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `ClearNetworkState` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateCableWithSpawners` | `Void` | `(Int32 cableId, List`1 positions)` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawner` | `Entity` | `(List`1 waypoints, Vector3 spawnerPos, Int32 cableId, Int32 customerID, PacketSpawnerComponent prefabComponent, Boolean isForward)` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawnersForCable` | `Void` | `(CableInfo& cableInfo)` |
| `Il2Cpp.WaypointInitializationSystem` | `CreateSpawnersForCable` | `Void` | `(CableInfo& cableInfo, PacketSpawnerComponent prefabComponent)` |
| `Il2Cpp.WaypointInitializationSystem` | `DoesCableServeMultipleCustomers` | `Boolean` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `EvaluateAllRoutes` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetAllCables` | `List`1` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCableCurrentSpeed` | `Single` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCableInfo` | `Nullable`1` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCustomerRoutes` | `Dictionary`2` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetCustomersUsingCable` | `HashSet`1` | `(CableInfo cable)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetDeviceName` | `String` | `(CableEndpoint endpoint)` |
| `Il2Cpp.WaypointInitializationSystem` | `GetEvaluationCooldown` | `Single` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `GetServerProcessingSpeed` | `Single` | `(String serverName)` |
| `Il2Cpp.WaypointInitializationSystem` | `IsCableInRoute` | `Boolean` | `(CableInfo cable, List`1 route)` |
| `Il2Cpp.WaypointInitializationSystem` | `LoadNetworkState` | `Void` | `(NetworkSaveData networkData, List`1 allRackPositions, Int32 saveVersion)` |
| `Il2Cpp.WaypointInitializationSystem` | `LoadNetworkStateCoroutine` | `IEnumerator` | `(NetworkSaveData networkData, List`1 allRackPositions, Int32 saveVersion)` |
| `Il2Cpp.WaypointInitializationSystem` | `MapDirectionToSibling` | `String` | `(CableInfo primary, CableInfo sibling, String direction)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_Boolean_byref___c__DisplayClass15_0_PDM_0` | `Int32` | `(Vector3 searchPos, Boolean mustBeStartOrEnd, __c__DisplayClass15_0& A_2)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_Boolean_byref___c__DisplayClass50_0_PDM_0` | `Int32` | `(Vector3 searchPos, Boolean mustBeStartOrEnd, __c__DisplayClass50_0& A_2)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_byref___c__DisplayClass15_0_PDM_0` | `Int32` | `(Vector3 searchPos, __c__DisplayClass15_0& A_1)` |
| `Il2Cpp.WaypointInitializationSystem` | `Method_Internal_Static_Int32_Vector3_byref___c__DisplayClass50_0_PDM_0` | `Int32` | `(Vector3 searchPos, __c__DisplayClass50_0& A_1)` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCableRemoved` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCreate` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnCreateForCompiler` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `OnUpdate` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `RegisterCableInNetworkMap` | `Void` | `(CableInfo cableInfo)` |
| `Il2Cpp.WaypointInitializationSystem` | `RequestRouteEvaluation` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `ResetAllSpawners` | `Void` | `()` |
| `Il2Cpp.WaypointInitializationSystem` | `SafelyDisposeSpawner` | `Void` | `(Entity spawnerEntity, Int32 cableId, String direction)` |
| `Il2Cpp.WaypointInitializationSystem` | `SetEvaluationCooldown` | `Void` | `(Single seconds)` |
| `Il2Cpp.WaypointInitializationSystem` | `SetPacketSpawnerEnabled` | `Void` | `(Boolean enabled)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateAllUI` | `Void` | `(Dictionary`2 customerInfo, List`1 allRoutes, Dictionary`2 cableLoad, Dictionary`2 cablePairLookup)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateCableInfo` | `Void` | `(Int32 cableId, CableInfo info)` |
| `Il2Cpp.WaypointInitializationSystem` | `UpdateServerCustomerID` | `Void` | `(String serverID, Int32 customerID)` |
| `Il2Cpp.WaypointInitializationSystem` | `__AssignQueries` | `Void` | `(SystemState& state)` |
| `Il2Cpp._PrivateImplementationDetails_` | `ComputeStringHash` | `UInt32` | `(String s)` |

### ClientsContracts

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.MainGameManager` | `GetFreeVlanId` | `Int32` | `()` |

### CustomImport

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ModLoader` | `Awake` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `CopyDirectory` | `Void` | `(String sourceDir, String destDir)` |
| `Il2Cpp.ModLoader` | `CreateMaterial` | `Material` | `(String folderPath, String textureFile)` |
| `Il2Cpp.ModLoader` | `CreateShopButton` | `Void` | `(Int32 modID, ShopItemConfig config, Sprite icon)` |
| `Il2Cpp.ModLoader` | `CreateShopTemplate` | `GameObject` | `(ShopItemConfig config, Mesh mesh, Material material, String folderName)` |
| `Il2Cpp.ModLoader` | `CreateStaticInstance` | `GameObject` | `(StaticItemConfig config, Mesh mesh, Material material, String folderName)` |
| `Il2Cpp.ModLoader` | `GetModPrefab` | `GameObject` | `(Int32 modID)` |
| `Il2Cpp.ModLoader` | `GetModPrefabByFolder` | `GameObject` | `(String folderName)` |
| `Il2Cpp.ModLoader` | `LoadAllMods` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `LoadDll` | `Void` | `(String folderPath, DllEntry dll)` |
| `Il2Cpp.ModLoader` | `LoadIcon` | `Sprite` | `(String folderPath, String iconFile)` |
| `Il2Cpp.ModLoader` | `LoadMesh` | `Mesh` | `(String folderPath, String modelFile)` |
| `Il2Cpp.ModLoader` | `LoadModPack` | `Void` | `(String folderPath)` |
| `Il2Cpp.ModLoader` | `LoadShopItem` | `Void` | `(String folderPath, String folderName, ShopItemConfig config)` |
| `Il2Cpp.ModLoader` | `LoadStaticItem` | `Void` | `(String folderPath, String folderName, StaticItemConfig config)` |
| `Il2Cpp.ModLoader` | `LoadTexture` | `Texture2D` | `(String path)` |
| `Il2Cpp.ModLoader` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `Start` | `Void` | `()` |
| `Il2Cpp.ModLoader` | `SyncWorkshopThenLoadAll` | `IEnumerator` | `()` |
| `Il2Cpp.ObjImporter` | `ImportOBJ` | `Mesh` | `(String filePath)` |
| `Il2Cpp.ObjImporter` | `ParseFloat` | `Single` | `(String s)` |
| `Il2Cpp.ObjImporter` | `ProcessFaceVertex` | `Int32` | `(String faceVertex, List`1 positions, List`1 uvs, List`1 normals, List`1 outVerts, List`1 outUVs, List`1 outNorms, Dictionary`2 cache)` |

### DevTools

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ChatController` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `Awake` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `DelayedLoad` | `IEnumerator` | `()` |
| `Il2Cpp.GODMOD` | `GODMOD_delayed` | `IEnumerator` | `()` |
| `Il2Cpp.GODMOD` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.GODMOD` | `StartGodMod` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.Numpad` | `OnDisable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `OnDisable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PauseMenuVideoTutorial` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.RackMount` | `CheatInsertRack` | `Void` | `(GameObject go, Int32 type)` |
| `Il2Cpp.ReBindUI` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `OnDisable` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `OnDisable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `OnDisable` | `Void` | `()` |

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
| `Il2Cpp.BalanceSheet` | `GetSaveData` | `BalanceSheetSaveData` | `()` |
| `Il2Cpp.BalanceSheet` | `InstantiateRow` | `BalanceSheetRow` | `()` |
| `Il2Cpp.BalanceSheet` | `LoadFromSave` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `RegisterRepairExpense` | `Void` | `(Single amount)` |
| `Il2Cpp.BalanceSheet` | `RegisterSalary` | `Void` | `(Int32 monthlySalary)` |
| `Il2Cpp.BalanceSheet` | `RegisterShopExpense` | `Void` | `(Single amount)` |
| `Il2Cpp.BalanceSheet` | `RestoreRecord` | `CustomerRecord` | `(CustomerRecordSaveData recData)` |
| `Il2Cpp.BalanceSheet` | `SaveSnapshot` | `Void` | `(Int32 month, DateTime snapshotTime)` |
| `Il2Cpp.BalanceSheet` | `Start` | `Void` | `()` |
| `Il2Cpp.BalanceSheet` | `TrackFinances` | `IEnumerator` | `()` |
| `Il2Cpp.BalanceSheetRow` | `SetAsExpenseRow` | `Void` | `(String label, Single amount)` |
| `Il2Cpp.BalanceSheetRow` | `SetAsHeader` | `Void` | `()` |
| `Il2Cpp.BalanceSheetRow` | `SetAsSectionTitle` | `Void` | `(String title)` |
| `Il2Cpp.BalanceSheetRow` | `SetAsTotalRow` | `Void` | `(Single revenue, Single penalties, Single total)` |
| `Il2Cpp.BalanceSheetRow` | `SetBackgroundColor` | `Void` | `(Color color)` |
| `Il2Cpp.BalanceSheetRow` | `SetData` | `Void` | `(String customerName, String revenue, String penalties, String total, Sprite customerLogo)` |
| `Il2Cpp.CableLink` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.CableSpinner` | `ApplyColor` | `Void` | `(Color color, String rgbString)` |
| `Il2Cpp.ComputerShop` | `ApplyColorToSpawnedItem` | `Void` | `(Int32 uid, Color color, ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `Awake` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonAssetManagementScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonBalanceSheetScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonBuyShopItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Boolean isCustomColor)` |
| `Il2Cpp.ComputerShop` | `ButtonCancel` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonCancelColorPicker` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonCheckOut` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonChosenColor` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonClear` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonHireScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonNetworkMap` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonReturnMainScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ButtonShopScreen` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `BuyAnotherItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, ShopCartItem cartItem)` |
| `Il2Cpp.ComputerShop` | `BuyNewItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName)` |
| `Il2Cpp.ComputerShop` | `CleanUpShop` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `ClearTrackingWithoutDestroying` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `CloseShop` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `DestroyAllSpawnedItems` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `FreeUpSpawnPoint` | `Void` | `(Int32 spawnIndex)` |
| `Il2Cpp.ComputerShop` | `GetNextAvailableSpawnPoint` | `Dictionary`2` | `()` |
| `Il2Cpp.ComputerShop` | `GetPrefabForItem` | `GameObject` | `(Int32 itemID, ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `HandleObjectives` | `Void` | `(ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.ComputerShop` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `OpenColorPicker` | `Void` | `()` |
| `Il2Cpp.ComputerShop` | `RemoveCartUIItem` | `Void` | `(ShopCartItem cartItem)` |
| `Il2Cpp.ComputerShop` | `RemoveSpawnedItem` | `Void` | `(Int32 uid)` |
| `Il2Cpp.ComputerShop` | `SelectNextAvailable` | `Void` | `(Int32 removedIndex)` |
| `Il2Cpp.ComputerShop` | `SpawnNewCartItem` | `Void` | `(Int32 itemID, Int32 price, ObjectInHand itemType, String displayName, Nullable`1 chosenColor)` |
| `Il2Cpp.ComputerShop` | `SpawnPhysicalItem` | `Nullable`1` | `(GameObject prefab, Int32 price, ObjectInHand itemType)` |
| `Il2Cpp.ComputerShop` | `UnlockFromSave` | `Void` | `(Dictionary`2 savedStates)` |
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
| `Il2Cpp.CustomerBase` | `GetServerTypeForIP` | `Int32` | `(String ip)` |
| `Il2Cpp.CustomerBase` | `GetSubnetsPerApp` | `Dictionary`2` | `()` |
| `Il2Cpp.CustomerBase` | `GetTotalAppSpeed` | `Single` | `()` |
| `Il2Cpp.CustomerBase` | `GetVlanIdsPerApp` | `Dictionary`2` | `()` |
| `Il2Cpp.CustomerBase` | `IsIPPresent` | `Boolean` | `(String ip)` |
| `Il2Cpp.CustomerBase` | `LoadData` | `Void` | `(CustomerBaseSaveData data)` |
| `Il2Cpp.CustomerBase` | `ResetAllAppSpeeds` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `SetUpApp` | `Void` | `(Int32 appID, Int32 difficulty, CustomerBaseSaveData saveData)` |
| `Il2Cpp.CustomerBase` | `SetUpBase` | `Void` | `(CustomerItem customerItem, CustomerBaseSaveData saveData)` |
| `Il2Cpp.CustomerBase` | `Start` | `Void` | `()` |
| `Il2Cpp.CustomerBase` | `TryRegisterRoutedSubnet` | `Boolean` | `(Int32 targetVlanId, Int32 sourceVlanId, Il2CppStringArray ips)` |
| `Il2Cpp.CustomerBase` | `TryUnregisterRoutedSubnet` | `Void` | `(Int32 sourceVlanId)` |
| `Il2Cpp.CustomerBase` | `UpdateCustomerServerCountAndSpeed` | `Void` | `(Int32 count, Single speed)` |
| `Il2Cpp.CustomerBase` | `UpdateMoney` | `IEnumerator` | `()` |
| `Il2Cpp.CustomerBase` | `UpdateSpeedOnCustomerBaseApp` | `Void` | `(Int32 appID, Single speed)` |
| `Il2Cpp.CustomerBaseDoor` | `Awake` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.CustomerBaseDoor` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OpenDoor` | `Void` | `()` |
| `Il2Cpp.CustomerBaseDoor` | `OpenDoorAndSetupBase` | `Void` | `(CustomerItem customerItem)` |
| `Il2Cpp.CustomerCard` | `SetCustomer` | `Void` | `(CustomerItem _customerItem)` |
| `Il2Cpp.CustomerColor` | `GetColorForCustomerId` | `Color` | `(Int32 customerId)` |
| `Il2Cpp.CustomerColor` | `GetColorForCustomerIdFloat4` | `float4` | `(Int32 customerId)` |
| `Il2Cpp.ModShopItem` | `ButtonBuyItem` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `Awake` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.MusicPlayer` | `PlayRandomSong` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `Update` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `AppendEolTime` | `Void` | `(StringBuilder builder, Int32 eolSeconds)` |
| `Il2Cpp.NetworkSwitch` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `RemapPhysicalKeyboard` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Player` | `CheckFallsThroughMap` | `Void` | `()` |
| `Il2Cpp.Player` | `DropAllItems` | `Void` | `()` |
| `Il2Cpp.Player` | `LoadPlayer` | `Void` | `(PlayerData data)` |
| `Il2Cpp.Player` | `Start` | `Void` | `()` |
| `Il2Cpp.Player` | `TurnOnCharacterControllerDelayed` | `IEnumerator` | `()` |
| `Il2Cpp.Player` | `UpdateCoin` | `Boolean` | `(Single _coinChhangeAmount, Boolean withoutSound)` |
| `Il2Cpp.Player` | `UpdateReputation` | `Void` | `(Single amount)` |
| `Il2Cpp.Player` | `UpdateXP` | `Boolean` | `(Single amount)` |
| `Il2Cpp.Player` | `WarpPlayer` | `Void` | `(Vector3 _position, Quaternion _rotation)` |
| `Il2Cpp.RackDoor` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RackMount` | `ApplyMaterialToLODs` | `Void` | `(GameObject rackGO, Material mat)` |
| `Il2Cpp.RackMount` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.RackPosition` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Router` | `ApplyRouteToCustomerBases` | `Void` | `(SubnetRoute route)` |
| `Il2Cpp.Router` | `ReapplyAllRoutes` | `Void` | `()` |
| `Il2Cpp.Router` | `WithdrawRouteFromCustomerBases` | `Void` | `(SubnetRoute route)` |
| `Il2Cpp.ShopCartItem` | `OnAddClicked` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `UpdateDisplay` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `Awake` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `ButtonBuyItem` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `BuyItem` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `TryUnlock` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `UpdateVisualState` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `UpdateCoinsAndPrestige_TopLeft` | `IEnumerator` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `ApplyExtraTurnRotation` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.WorldObjectButton` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |

### Facility

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CableLink` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Firewall` | `AddRule` | `Void` | `(Int32 portIndex, Int32 vlanId, String ipCidr, Boolean allow)` |
| `Il2Cpp.Firewall` | `InsertedInRack` | `Void` | `(SwitchSaveData saveData)` |
| `Il2Cpp.Firewall` | `IsTrafficAllowed` | `Boolean` | `(Int32 portIndex, Int32 vlanId, String ip)` |
| `Il2Cpp.GateLever` | `Awake` | `Void` | `()` |
| `Il2Cpp.GateLever` | `CloseGate` | `Void` | `()` |
| `Il2Cpp.GateLever` | `GateCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.GateLever` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.GateLever` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.GateLever` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.GateLever` | `OpenGate` | `Void` | `()` |
| `Il2Cpp.GateLever` | `TruckComing` | `Void` | `()` |
| `Il2Cpp.MusicPlayer` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `Awake` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.PushTrolleyHandle` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.PushTrolleyHandle` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackDoor` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackMount` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.TrolleyTrigger` | `ObjectAdded` | `IEnumerator` | `(Collider other, UsableObject uo)` |
| `Il2Cpp.TrolleyTrigger` | `OnTriggerEnter` | `Void` | `(Collider other)` |
| `Il2Cpp.Wall` | `Awake` | `Void` | `()` |
| `Il2Cpp.Wall` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Wall` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Wall` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Wall` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Wall` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.Wall` | `OpenWall` | `Void` | `()` |
| `Il2Cpp.WorldObjectButton` | `OnHoverOver` | `Void` | `()` |

### GameState

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.FCP_Persistence` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `OnEnable` | `Void` | `()` |

### Hardware

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AssetManagement` | `ButtonAddAllBrokenDevicesToQueue` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonCancelSendingTechnician` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonClearAllWarnings` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonConfirmSendingTechnician` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterAll` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterBroken` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterEOL` | `Void` | `()` |
| `Il2Cpp.AssetManagement` | `ButtonFilterOff` | `Void` | `()` |
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
| `Il2Cpp.InputManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.InputManager` | `CheckCurrentControls` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.InputManager` | `ConfinedCursorforUI` | `Void` | `()` |
| `Il2Cpp.InputManager` | `DoRebind` | `Void` | `(InputAction actionToRebind, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean allCompositeParts, Boolean excludeMouse)` |
| `Il2Cpp.InputManager` | `ForceMousePositionToCenterOfGameWindow` | `Void` | `()` |
| `Il2Cpp.InputManager` | `GetBindingName` | `String` | `(String actionName, Int32 bindingIndex)` |
| `Il2Cpp.InputManager` | `LoadAllBindingOverrides` | `Void` | `()` |
| `Il2Cpp.InputManager` | `LoadBindingOverride` | `Void` | `(String actionName)` |
| `Il2Cpp.InputManager` | `LockedCursorForPlayerMovement` | `Void` | `()` |
| `Il2Cpp.InputManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.InputManager` | `ResetBinding` | `Void` | `(String actionName, Int32 bindingIndex)` |
| `Il2Cpp.InputManager` | `SaveBindingOverride` | `Void` | `(InputAction action)` |
| `Il2Cpp.InputManager` | `StartRebind` | `Void` | `(String actionName, Int32 bindingIndex, TextMeshProUGUI statusText, Boolean excludeMouse)` |
| `Il2Cpp.InputManager` | `_Awake_b__14_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.NetworkSwitch` | `GenerateUniqueSwitchId` | `String` | `()` |
| `Il2Cpp.NetworkSwitch` | `ItIsBroken` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `PatchStaleSwitchId` | `Void` | `(CableEndpoint& endpoint)` |
| `Il2Cpp.NetworkSwitch` | `PowerButton` | `Void` | `(Boolean forceState)` |
| `Il2Cpp.NetworkSwitch` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `SetPowerLightMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.NetworkSwitch` | `SwitchInsertedInRack` | `Void` | `(SwitchSaveData switchSaveData)` |
| `Il2Cpp.NetworkSwitch` | `_SwitchInsertedInRack_b__26_0` | `Boolean` | `(NetworkSwitch s)` |
| `Il2Cpp.NetworkSwitchConfiguration` | `ButtonPower` | `Void` | `()` |
| `Il2Cpp.Rack` | `Awake` | `Void` | `()` |
| `Il2Cpp.Rack` | `ButtonDisablePositionsInRack` | `Void` | `()` |
| `Il2Cpp.Rack` | `ButtonUnmountRack` | `Void` | `()` |
| `Il2Cpp.Rack` | `InitializeLoadedRack` | `Void` | `(Il2CppStructArray`1 loadedPositions)` |
| `Il2Cpp.Rack` | `IsPositionAvailable` | `Boolean` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `MarkPositionAsUnused` | `Void` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `MarkPositionAsUsed` | `Void` | `(Int32 index, Int32 sizeInU)` |
| `Il2Cpp.Rack` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Rack` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.Rack` | `SetDisablePositionsButtonMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.Rack` | `Start` | `Void` | `()` |
| `Il2Cpp.Rack` | `UnmountRack` | `IEnumerator` | `()` |
| `Il2Cpp.Rack` | `UpdateAudioVolume` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `Awake` | `Void` | `()` |
| `Il2Cpp.RackAudioCuller` | `CullLoop` | `IEnumerator` | `()` |
| `Il2Cpp.RackAudioCuller` | `Register` | `Void` | `(Rack rack)` |
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
| `Il2Cpp.SFPBox` | `CanAcceptSFP` | `Boolean` | `(Int32 sfpType)` |
| `Il2Cpp.SFPBox` | `GetFreeSpaceInTheBox` | `Int32` | `()` |
| `Il2Cpp.SFPBox` | `InsertSFPBackIntoBox` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.SFPBox` | `LoadSFPsFromSave` | `Void` | `()` |
| `Il2Cpp.SFPBox` | `ParentTheObjectWithDelay` | `IEnumerator` | `(Transform uo, Int32 index)` |
| `Il2Cpp.SFPBox` | `RemoveSFPFromBox` | `Void` | `(Int32 position)` |
| `Il2Cpp.SFPBox` | `ReturnSFPDirectly` | `Boolean` | `(SFPModule sfpmodule)` |
| `Il2Cpp.SFPBox` | `TakeSFPFromBox` | `SFPModule` | `()` |
| `Il2Cpp.SFPModule` | `Awake` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `InsertDirectlyIntoPort` | `Void` | `(CableLink _link)` |
| `Il2Cpp.SFPModule` | `InsertedInSFPPort` | `Void` | `(CableLink _link, Boolean immediate)` |
| `Il2Cpp.SFPModule` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.SFPModule` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.SFPModule` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `RemoveFromPort` | `Void` | `()` |
| `Il2Cpp.SFPModule` | `SlideIntoPort` | `IEnumerator` | `(Transform port)` |
| `Il2Cpp.Server` | `AppendEolTime` | `Void` | `(StringBuilder builder, Int32 eolSeconds)` |
| `Il2Cpp.Server` | `Awake` | `Void` | `()` |
| `Il2Cpp.Server` | `ButtonClickChangeCustomer` | `Void` | `(Boolean forward)` |
| `Il2Cpp.Server` | `ButtonClickChangeIP` | `Void` | `()` |
| `Il2Cpp.Server` | `ClearErrorSign` | `Void` | `()` |
| `Il2Cpp.Server` | `ClearWarningSign` | `Void` | `(Boolean isPreserved)` |
| `Il2Cpp.Server` | `GenerateUniqueServerId` | `String` | `()` |
| `Il2Cpp.Server` | `GetCustomerID` | `Int32` | `()` |
| `Il2Cpp.Server` | `GetNextCustomerID` | `Int32` | `(Int32 currentCustomerID, Boolean forward)` |
| `Il2Cpp.Server` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Server` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Server` | `IsAnyCableConnected` | `Boolean` | `()` |
| `Il2Cpp.Server` | `ItIsBroken` | `Void` | `()` |
| `Il2Cpp.Server` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Server` | `OnLoadingComplete` | `Void` | `()` |
| `Il2Cpp.Server` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.Server` | `PowerButton` | `Void` | `(Boolean forceState)` |
| `Il2Cpp.Server` | `RegisterLink` | `Void` | `(CableLink link)` |
| `Il2Cpp.Server` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.Server` | `ServerInsertedInRack` | `Void` | `(ServerSaveData serverSaveData)` |
| `Il2Cpp.Server` | `SetIP` | `Void` | `(String _ip)` |
| `Il2Cpp.Server` | `SetPowerLightMaterial` | `Void` | `(Material material)` |
| `Il2Cpp.Server` | `Start` | `Void` | `()` |
| `Il2Cpp.Server` | `TickTimer` | `Void` | `()` |
| `Il2Cpp.Server` | `TurnOffCommonFunctions` | `Void` | `()` |
| `Il2Cpp.Server` | `TurnOnCommonFunction` | `Void` | `()` |
| `Il2Cpp.Server` | `UnregisterLink` | `Void` | `(CableLink link)` |
| `Il2Cpp.Server` | `UpdateAppID` | `Void` | `(Int32 _appID)` |
| `Il2Cpp.Server` | `UpdateCustomer` | `Void` | `(Int32 newCustomerID)` |
| `Il2Cpp.Server` | `UpdateServerScreenUI` | `Void` | `()` |
| `Il2Cpp.Server` | `ValidateRackPosition` | `Boolean` | `()` |
| `Il2Cpp.Server` | `_ServerInsertedInRack_b__39_0` | `Boolean` | `(Server s)` |
| `Il2Cpp.Server` | `_ValidateRackPosition_b__53_0` | `Boolean` | `(RackPosition r)` |
| `Il2Cpp.SetIP` | `Awake` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ButtonEditLabel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ButtonHideShowHint` | `Void` | `()` |
| `Il2Cpp.SetIP` | `CidrToSubnetMask` | `Void` | `(Int32 cidr, Int32& m1, Int32& m2, Int32& m3, Int32& m4)` |
| `Il2Cpp.SetIP` | `ClickButtonCancel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonClear` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonCopy` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonDel` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonNextIP` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonOK` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickButtonPaste` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ClickNumber` | `Void` | `(String number)` |
| `Il2Cpp.SetIP` | `CloseCanvas` | `Void` | `()` |
| `Il2Cpp.SetIP` | `GetMaskFromCidr` | `String` | `(Int32 cidr)` |
| `Il2Cpp.SetIP` | `GetUsableIPsFromSubnet` | `Il2CppStringArray` | `(String subnet)` |
| `Il2Cpp.SetIP` | `IncrementOctets` | `Void` | `(Int32& o1, Int32& o2, Int32& o3, Int32& o4)` |
| `Il2Cpp.SetIP` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.SetIP` | `PowerButton` | `Void` | `()` |
| `Il2Cpp.SetIP` | `ShowCanvas` | `Void` | `(Server _server)` |
| `Il2Cpp.SetIP` | `TryParseIpToOctets` | `Boolean` | `(String ipString, Int32& o1, Int32& o2, Int32& o3, Int32& o4)` |
| `Il2Cpp.SetIP` | `Update` | `Void` | `()` |
| `Il2Cpp.SetIP` | `_Awake_b__11_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.SetIP` | `_ButtonEditLabel_b__30_0` | `Void` | `(String label)` |
| `Il2Cpp.Technician` | `AssignJob` | `Void` | `(RepairJob job)` |
| `Il2Cpp.Technician` | `Awake` | `Void` | `()` |
| `Il2Cpp.Technician` | `CacheDeviceBounds` | `Void` | `(GameObject device)` |
| `Il2Cpp.Technician` | `GetCorrectDevicePrefab` | `GameObject` | `()` |
| `Il2Cpp.Technician` | `GetCurrentDevicePrefabID` | `Int32` | `()` |
| `Il2Cpp.Technician` | `GettingNewServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Technician` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.Technician` | `PositionHandTargetsOnDevice` | `Void` | `(GameObject device)` |
| `Il2Cpp.Technician` | `RepairDevice` | `Void` | `()` |
| `Il2Cpp.Technician` | `ReplacingServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `RequestJobDelayed` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `RotateTowardsGoal` | `Void` | `(Vector3 goal)` |
| `Il2Cpp.Technician` | `SendToContainer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `SetHandIKWeight` | `IEnumerator` | `(Single targetWeight, Single duration)` |
| `Il2Cpp.Technician` | `Start` | `Void` | `()` |
| `Il2Cpp.Technician` | `StartTextingAnimation` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `ThrowingOutServer` | `IEnumerator` | `()` |
| `Il2Cpp.Technician` | `Update` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `AddTechnician` | `Void` | `(Technician technician)` |
| `Il2Cpp.TechnicianManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `FireTechnician` | `Void` | `(Int32 technicianID)` |
| `Il2Cpp.TechnicianManager` | `GetClosestOpenedDumpsterIndex` | `Int32` | `(Vector3 position)` |
| `Il2Cpp.TechnicianManager` | `IsDeviceAlreadyAssigned` | `Boolean` | `(NetworkSwitch networkSwitch, Server server)` |
| `Il2Cpp.TechnicianManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.TechnicianManager` | `OpenDumpsterArea` | `Void` | `(Int32 areaID)` |
| `Il2Cpp.TechnicianManager` | `SendTechnician` | `Void` | `(NetworkSwitch networkSwitch, Server server)` |

### Ignored

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ObjectPrivateAbstractSealedInVo0` | `Initialize` | `Void` | `()` |
| `Il2CppTMPro.Examples.SkewTextExample` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.WarpTextExample` | `Awake` | `Void` | `()` |

### Input

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyCode` | `OSK_KeyCode` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyName` | `String` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `GetKeyTransform` | `Transform` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `KeyType` | `OSK_KEY_TYPES` | `()` |
| `Il2CppviperOSK.I_OSK_Key` | `OnKeyDepress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.I_OSK_Key` | `OnKeyPress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2Cpp.KeyHint` | `SetInactiveAll` | `Void` | `()` |
| `Il2Cpp.KeyHint` | `ShowKeyboadMelee` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnWaitForPressKey` | `Void` | `()` |
| `Il2CppviperOSK.OSK_KeyTypeMeta` | `KeyType` | `OSK_KEY_TYPES` | `(OSK_KeyCode key)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SetBaseKey` | `Void` | `(GameObject base_key)` |
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
| `Il2Cpp.StaticUIElements` | `CreateCustomKeyHint` | `GameObject` | `(InputAction action, Int32 textUID, Transform parent, Boolean isPermanent)` |

### Interaction

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CableLink` | `IsAllowedToDoSecondAction` | `Boolean` | `()` |
| `Il2Cpp.CableLink` | `LabelActionOnClick` | `Void` | `()` |
| `Il2Cpp.CableLink` | `SecondActionOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `Awake` | `Void` | `()` |
| `Il2Cpp.Interact` | `CloseInteractionMenu` | `Void` | `()` |
| `Il2Cpp.Interact` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Interact` | `IsAllowedToDoSecondAction` | `Boolean` | `()` |
| `Il2Cpp.Interact` | `LabelActionOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.Interact` | `OnLoad` | `Void` | `(InteractObjectData data)` |
| `Il2Cpp.Interact` | `SecondActionOnClick` | `Void` | `()` |
| `Il2Cpp.Interact` | `_LabelActionOnClick_b__18_0` | `Void` | `(String text)` |
| `Il2Cpp.RackPosition` | `SecondActionOnClick` | `Void` | `()` |

### Lifecycle

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CarryModelPool` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `AutoRepairRoutine` | `IEnumerator` | `()` |
| `Il2Cpp.CommandCenter` | `Awake` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `ButtonDowngradeCommandCenter` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `ButtonUpgradeCommandCenter` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `DestroyOperatorsForLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.CommandCenter` | `SetAutoRepairMode` | `Void` | `(Int32 mode)` |
| `Il2Cpp.CommandCenter` | `SpawnOperatorsForLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `SpawnOperatorsForSingleLevel` | `Void` | `(Int32 level)` |
| `Il2Cpp.CommandCenter` | `ToggleClearWarningAuto` | `Void` | `(Boolean isOn)` |
| `Il2Cpp.CommandCenterOperator` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Register` | `Void` | `(ITimedDevice device)` |
| `Il2Cpp.DeviceTimerManager` | `TimerLoop` | `IEnumerator` | `()` |
| `Il2Cpp.DeviceTimerManager` | `Unregister` | `Void` | `(ITimedDevice device)` |
| `Il2Cpp.ITimedDevice` | `TickTimer` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.LoadingScreen` | `AsynchronousLoad` | `IEnumerator` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `AsynchronousUnLoad` | `IEnumerator` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `Awake` | `Void` | `()` |
| `Il2Cpp.LoadingScreen` | `IsSceneLoaded` | `Boolean` | `(String name)` |
| `Il2Cpp.LoadingScreen` | `LoadGameLoadScene` | `IEnumerator` | `(Il2CppStructArray`1 loadedScenes)` |
| `Il2Cpp.LoadingScreen` | `LoadGameScenesVoid` | `Void` | `(PlayerData playerData, List`1 technicianData, Il2CppStructArray`1 loadedScenes, Il2CppStructArray`1 hiredTechnicians, List`1 repairJobQueue)` |
| `Il2Cpp.LoadingScreen` | `LoadLevel` | `Void` | `(Int32 sceneIndex)` |
| `Il2Cpp.LoadingScreen` | `LoadPlayerAndNPCDataWithDelay` | `IEnumerator` | `(PlayerData playerData, List`1 technicianData, Il2CppStructArray`1 hiredTechnicians, List`1 repairJobQueue)` |
| `Il2Cpp.LoadingScreen` | `SetDifficualty` | `Void` | `(Int32 i)` |
| `Il2Cpp.LoadingScreen` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.LoadingScreen` | `UnLoadLevel` | `Void` | `(Int32 sceneIndex)` |
| `Il2Cpp.NetworkSwitch` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `GetReward` | `Void` | `()` |
| `Il2Cpp.ObjectiveTimed` | `SetupObjectiveTimed` | `Void` | `(Int32 _maxTime, String _objectiveText, Int32 customerID, Int32 appID, Int32 _requiredIOPS)` |
| `Il2Cpp.ObjectiveTimed` | `UpdateDisplay` | `Void` | `(Int32 currentIOPS, Int32 remainingTime)` |
| `Il2Cpp.Objectives` | `Awake` | `Void` | `()` |
| `Il2Cpp.Objectives` | `ClearObjectives` | `Void` | `()` |
| `Il2Cpp.Objectives` | `CreateAppObjective` | `Int32` | `(Int32 customerID, Int32 appID, Int32 time, Int32 requiredIOPS)` |
| `Il2Cpp.Objectives` | `CreateNewObjective` | `Void` | `(Int32 localisationUID, Int32 _objectiveUID, Vector3 objectivePosition, Int32 xpReward, Int32 reputationReward, Boolean isSub)` |
| `Il2Cpp.Objectives` | `DestroyObjective` | `Void` | `(Int32 _objectiveUID)` |
| `Il2Cpp.Objectives` | `EffectOnDestroy` | `IEnumerator` | `(Int32 _objectiveUID)` |
| `Il2Cpp.Objectives` | `GetTimedObjective` | `ObjectiveTimed` | `(Int32 objectiveUID)` |
| `Il2Cpp.Objectives` | `InstantiateObjectiveSign` | `Void` | `(Int32 objectiveUID, Vector3 objectPos)` |
| `Il2Cpp.Objectives` | `IsTutorialInProgress` | `Boolean` | `()` |
| `Il2Cpp.Objectives` | `LoadObjectives` | `Void` | `(HashSet`1 _activeObjectives)` |
| `Il2Cpp.Objectives` | `ObjectiveTimedText` | `String` | `()` |
| `Il2Cpp.Objectives` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Objectives` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.Objectives` | `RemoveObjectiveSign` | `Void` | `(Int32 objectiveUID)` |
| `Il2Cpp.Objectives` | `Start` | `Void` | `()` |
| `Il2Cpp.Objectives` | `StartObjective` | `Void` | `(Int32 _objectiveUID, Vector3 objectivePosition, Boolean _loadSave)` |
| `Il2Cpp.Objectives` | `StartObjective` | `Void` | `(Int32 _objectiveUID, Boolean _loadSave)` |
| `Il2Cpp.PacketSpawnerSystem` | `SpawnPacket` | `Void` | `(EntityCommandBuffer ecb, PacketSpawnerComponent spawner, Int32 spawnerIndex, Entity spawnerEntity, BlobArray`1& waypoints, Int32 packetType)` |
| `Il2Cpp.PatchPanel` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `Pause` | `Void` | `(Int32 openMenu)` |
| `Il2Cpp.PauseMenu` | `Resume` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabEnter` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabExit` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PauseMenu_TabGroup` | `ResetTabs` | `Void` | `()` |
| `Il2Cpp.PauseMenu_TabGroup` | `Subscribe` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PlayerManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `ConfinedCursorforUI` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `DefaultActionEffect` | `Void` | `(Vector3 _position, Single _time)` |
| `Il2Cpp.PlayerManager` | `GainIOPSEffect` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `LockedCursorForPlayerMovement` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `PlayerStopMovement` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `Start` | `Void` | `()` |
| `Il2Cpp.PlayerManager` | `WaitForActionToFinish` | `IEnumerator` | `(Vector3 _position, Single _time)` |
| `Il2Cpp.RackAudioCuller` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.RackMount` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ReusableFunctions` | `DestroyChildren` | `Void` | `(Transform root)` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `AddSpawnedItem` | `Void` | `(Int32 uid)` |
| `Il2Cpp.ShopCartItem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.ShopCartItem` | `RemoveLastSpawnedItem` | `Int32` | `()` |
| `Il2Cpp.ShopItem` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `DestroyErrorWarningSign` | `Void` | `(Int32 errorWarningUID)` |
| `Il2Cpp.ToolTipOnUIText` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `Awake` | `Void` | `()` |
| `Il2Cpp.TrolleyLoadingBay` | `ParentTheObjectWithDelay` | `IEnumerator` | `(UsableObject uo)` |
| `Il2Cpp.TrolleyLoadingBay` | `ResetAllSlots` | `Void` | `()` |

### Maintenance

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.PacketSpawnerSystem` | `__ScheduleViaJobChunkExtension_0` | `JobHandle` | `(UpdatePacketsJob job, EntityQuery query, JobHandle dependency, SystemState& state, Boolean hasUserDefinedQuery)` |
| `Il2Cpp.TechnicianManager` | `EnqueueDispatch` | `Void` | `(RepairJob job)` |
| `Il2Cpp.TechnicianManager` | `GetActiveJobs` | `List`1` | `()` |
| `Il2Cpp.TechnicianManager` | `GetQueuedJobs` | `List`1` | `()` |
| `Il2Cpp.TechnicianManager` | `ProcessDispatchQueue` | `IEnumerator` | `()` |
| `Il2Cpp.TechnicianManager` | `RequestNextJob` | `Void` | `(Technician technician)` |
| `Il2Cpp.TechnicianManager` | `RestoreJobQueue` | `Void` | `(List`1 savedJobs)` |

### Networking

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CableIDComponent` | `BoxIl2CppObject` | `Object` | `()` |
| `Il2Cpp.CableLink` | `CollectPatchPanelChainCables` | `List`1` | `(Int32 startCableId)` |
| `Il2Cpp.CableLink` | `InsertSFP` | `Void` | `(Single speed, Int32 type, SFPModule module)` |
| `Il2Cpp.CableLink` | `RemoveSFP` | `Void` | `()` |
| `Il2Cpp.CableLink` | `SetConnectionSpeed` | `Void` | `(Single speed)` |
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
| `Il2Cpp.CableSpinner` | `DropObject` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `IsCableLenghtEnough` | `Boolean` | `()` |
| `Il2Cpp.CableSpinner` | `LowerAmountOfCable` | `Void` | `(Single length)` |
| `Il2Cpp.CableSpinner` | `UpdateCurrentLength` | `Void` | `(Single length)` |
| `Il2Cpp.CarryModelPool` | `StripGameplayComponents` | `Void` | `(GameObject obj)` |
| `Il2Cpp.NetworkMap` | `AddBrokenServer` | `Void` | `(Server server)` |
| `Il2Cpp.NetworkMap` | `AddBrokenSwitch` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkMap` | `AddDevice` | `Void` | `(String name, TypeOfLink type, Int32 customerID)` |
| `Il2Cpp.NetworkMap` | `AddSwitchConnection` | `Void` | `(String switchName, String deviceName)` |
| `Il2Cpp.NetworkMap` | `Awake` | `Void` | `()` |
| `Il2Cpp.NetworkMap` | `ClearMap` | `Void` | `()` |
| `Il2Cpp.NetworkMap` | `Connect` | `Void` | `(String from, String to)` |
| `Il2Cpp.NetworkMap` | `CreateLACPGroup` | `Int32` | `(String deviceA, String deviceB, List`1 cableIds)` |
| `Il2Cpp.NetworkMap` | `Disconnect` | `Void` | `(String from, String to)` |
| `Il2Cpp.NetworkMap` | `FindAllReachablePathsFrom` | `Dictionary`2` | `(String startDevice)` |
| `Il2Cpp.NetworkMap` | `FindAllRoutes` | `List`1` | `(String baseName, String serverName)` |
| `Il2Cpp.NetworkMap` | `FindPhysicalPath` | `List`1` | `(String start, String target)` |
| `Il2Cpp.NetworkMap` | `GenerateDeviceName` | `String` | `(TypeOfLink type, Vector3 position)` |
| `Il2Cpp.NetworkMap` | `GetAllBrokenServers` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllBrokenSwitches` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllCustomerBases` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllDevices` | `List`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllFirewalls` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllLACPGroups` | `Dictionary`2` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllNetworkSwitches` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllRouters` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetAllServers` | `IEnumerable`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetCustomerBase` | `CustomerBase` | `(Int32 customerId)` |
| `Il2Cpp.NetworkMap` | `GetDevice` | `Device` | `(String name)` |
| `Il2Cpp.NetworkMap` | `GetLACPGroupBetween` | `LACPGroup` | `(String deviceA, String deviceB)` |
| `Il2Cpp.NetworkMap` | `GetLACPGroupForCable` | `LACPGroup` | `(Int32 cableId)` |
| `Il2Cpp.NetworkMap` | `GetNumberOfDevices` | `Il2CppStructArray`1` | `()` |
| `Il2Cpp.NetworkMap` | `GetServer` | `Server` | `(String serverId)` |
| `Il2Cpp.NetworkMap` | `GetSwitchById` | `NetworkSwitch` | `(String switchId)` |
| `Il2Cpp.NetworkMap` | `IsIpAddressDuplicate` | `Boolean` | `(String ip, Server serverToExclude)` |
| `Il2Cpp.NetworkMap` | `IsPatchPanelPort` | `Boolean` | `(String deviceName)` |
| `Il2Cpp.NetworkMap` | `IsVlanAllowedOnRoute` | `Boolean` | `(List`1 path, Int32 vlanId, Dictionary`2 cablePairLookup)` |
| `Il2Cpp.NetworkMap` | `PrintNetworkMap` | `String` | `()` |
| `Il2Cpp.NetworkMap` | `RegisterCableConnection` | `Void` | `(Int32 cableId, Vector3 startPos, Vector3 endPos, TypeOfLink startType, TypeOfLink endType, String startSwitchID, String endSwitchID, Int32 startCustomerID, Int32 endCustomerID, String startServerID, String endServerID)` |
| `Il2Cpp.NetworkMap` | `RegisterCustomerBase` | `Void` | `(CustomerBase customerBase)` |
| `Il2Cpp.NetworkMap` | `RegisterServer` | `Void` | `(Server server)` |
| `Il2Cpp.NetworkMap` | `RegisterSwitch` | `Void` | `(NetworkSwitch networkSwitch)` |
| `Il2Cpp.NetworkMap` | `RemapDeviceId` | `Void` | `(String oldId, String newId)` |
| `Il2Cpp.NetworkMap` | `RemoveBrokenServer` | `Void` | `(String serverId)` |
| `Il2Cpp.NetworkMap` | `RemoveBrokenSwitch` | `Void` | `(String switchId)` |
| `Il2Cpp.NetworkMap` | `RemoveCableConnection` | `Void` | `(Int32 cableId, Boolean preserveLACP)` |
| `Il2Cpp.NetworkMap` | `RemoveCableFromLACPGroups` | `Void` | `(Int32 cableId)` |
| `Il2Cpp.NetworkMap` | `RemoveDevice` | `Void` | `(String name)` |
| `Il2Cpp.NetworkMap` | `RemoveIsolatedDevice` | `Void` | `(String deviceName)` |
| `Il2Cpp.NetworkMap` | `RemoveLACPGroup` | `Void` | `(Int32 groupId)` |
| `Il2Cpp.NetworkMap` | `ResolveThroughPatchPanel` | `String` | `(String patchPanelPort, String fromDevice)` |
| `Il2Cpp.NetworkMap` | `SetLACPGroups` | `Void` | `(Dictionary`2 groups)` |
| `Il2Cpp.NetworkMap` | `UpdateCustomerServerCountAndSpeed` | `Void` | `(Int32 customerId, Int32 serverCount, Single speed)` |
| `Il2Cpp.NetworkMap` | `UpdateDeviceCustomerID` | `Void` | `(String deviceName, Int32 customerID)` |
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
| `Il2Cpp.NetworkSwitch` | `SetDisallowedVlansPerPort` | `Void` | `(Dictionary`2 data)` |
| `Il2Cpp.NetworkSwitch` | `SetVlanAllowed` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
| `Il2Cpp.NetworkSwitch` | `SetVlanDisallowed` | `Void` | `(Int32 portIndex, Int32 vlanId)` |
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
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `PlayRandomAudioClip` | `AudioClip` | `(Il2CppReferenceArray`1 audioClips, Single volume)` |
| `Il2Cpp.ToolTipInteract` | `HideTooltipForInteract` | `Void` | `()` |
| `Il2Cpp.ToolTipInteract` | `ShowTooltipForInteract` | `Void` | `(String _text, Sprite _sprite)` |
| `Il2Cpp.ToolTipOnUIText` | `ToolTip` | `Void` | `()` |
| `Il2Cpp.Tooltip` | `HideTooltip` | `Void` | `()` |
| `Il2Cpp.Tooltip` | `ShowTooltipOverlayCanvas` | `Void` | `(String tooltipText, Vector3 _position, Int32 differentXOffset)` |
| `Il2Cpp.Tooltip` | `ShowTooltipWorldCanvas` | `Void` | `(String _text, RectTransform _transform, Camera cam)` |

### Persistence

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.CablePositions` | `LoadCable` | `Void` | `(CableSaveData cableData)` |
| `Il2Cpp.CableSpinner` | `LoadSavedColor` | `Void` | `()` |
| `Il2Cpp.ColorSerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.ColorSerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |
| `Il2Cpp.CommandCenterOperator` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `Awake` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `GenerateID` | `String` | `()` |
| `Il2Cpp.FCP_Persistence` | `InitStatic` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `LoadColor` | `Boolean` | `(Color& c)` |
| `Il2Cpp.FCP_Persistence` | `LoadDataFile` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.FCP_Persistence` | `SaveColor` | `Void` | `(Color c)` |
| `Il2Cpp.FCP_Persistence` | `SaveDataFile` | `Void` | `()` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `GetSettingHash` | `Int32` | `()` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `MakeMesh` | `Void` | `(Sprite sprite, Int32 x, Int32 y, MeshType meshtype)` |
| `Il2Cpp.FCP_SpriteMeshEditor` | `Update` | `Void` | `()` |
| `Il2Cpp.IModPlugin` | `OnModLoad` | `Void` | `(String modFolderPath)` |
| `Il2Cpp.IModPlugin` | `OnModUnload` | `Void` | `()` |
| `Il2Cpp.QuaternionSerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.QuaternionSerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |
| `Il2Cpp.RackMount` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.SaveData` | `Validate` | `String` | `()` |
| `Il2Cpp.SaveSystem` | `Load` | `Void` | `(String savename, Boolean isFromPauseMenu)` |
| `Il2Cpp.SaveSystem` | `LoadGame` | `SaveData` | `(String savename)` |
| `Il2Cpp.SaveSystem` | `LoadGameData` | `Void` | `()` |
| `Il2Cpp.SaveSystem` | `SaveGame` | `Void` | `(String savename, String stringNameOfSave)` |
| `Il2Cpp.SaveSystem` | `SaveGameData` | `Void` | `()` |
| `Il2Cpp.ShopItem` | `OnLoad` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonSaveInputNumpadOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `ButtonSaveInputTextOverlay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `OnLoadingStarted` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `SetLoadingInfo` | `Void` | `(String s)` |
| `Il2Cpp.Vector3SerializationSurrogate` | `GetObjectData` | `Void` | `(Object obj, SerializationInfo info, StreamingContext context)` |
| `Il2Cpp.Vector3SerializationSurrogate` | `SetObjectData` | `Object` | `(Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)` |

### Serialization

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `BoxedGetHashCode` | `Int32` | `(Object val, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `Equals` | `Boolean` | `(Object lhs, Object rhs, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `Equals` | `Boolean` | `(Object lhs, Void* rhs, Int32 typeIndex)` |
| `Unity.Entities.CodeGeneratedRegistry.AssemblyTypeRegistry` | `SetSharedStaticTypeIndices` | `Void` | `(Int32* pTypeInfos, Int32 count)` |
| `Il2Cpp.PacketSpawnerSystem` | `Method_Internal_Static_Void_IntPtr_IntPtr_PDM_0` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.PacketSpawnerSystem` | `Method_Internal_Static_Void_IntPtr_IntPtr_PDM_1` | `Void` | `(IntPtr self, IntPtr state)` |
| `Il2Cpp.StaticUIElements` | `ShowStaticCanvas` | `Void` | `(Boolean active)` |
| `Il2CppTMPro.Examples.TeleType` | `Awake` | `Void` | `()` |
| `Il2Cpp.UnitySourceGeneratedAssemblyMonoScriptTypes_v1` | `Get` | `MonoScriptData` | `()` |

### Settings

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.RectExtensions` | `InverseTransform` | `Rect` | `(Rect r, Transform transform)` |
| `Il2Cpp.RectExtensions` | `Transform` | `Rect` | `(Rect r, Transform transform)` |
| `Il2Cpp.Router` | `AddRoute` | `Boolean` | `(Int32 sourceVlanId, String subnetCidr, Int32 targetVlanId)` |
| `Il2Cpp.RouterConfiguration` | `StartEditRoute` | `Void` | `(Int32 sourceVlanId, GameObject rowObj)` |
| `Il2Cpp.SettingsControls` | `InvertY` | `Void` | `()` |
| `Il2Cpp.SettingsControls` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.SettingsControls` | `LookSensitivity` | `Void` | `(Single fl)` |
| `Il2Cpp.SettingsControls` | `Start` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `ButtonUnstuckPlayer` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.SettingsGameplay` | `OnLanguageDropDownChange` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetAutoSaveInterval` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetAutoSaveOnOff` | `Void` | `(Boolean isActive)` |
| `Il2Cpp.SettingsGameplay` | `SetPacketType` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `SetRouteEvalInterval` | `Void` | `(Int32 i)` |
| `Il2Cpp.SettingsGameplay` | `Start` | `Void` | `()` |
| `Il2Cpp.SettingsSingleton` | `Awake` | `Void` | `()` |
| `Il2Cpp.SettingsSingleton` | `DisableOnAfterFirstSettingUp` | `IEnumerator` | `()` |
| `Il2Cpp.SettingsVolume` | `EffectVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `LoadSettings` | `Void` | `()` |
| `Il2Cpp.SettingsVolume` | `MasterVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `MusicVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `RacksVolume` | `Void` | `(Single volume)` |
| `Il2Cpp.SettingsVolume` | `Start` | `Void` | `()` |

### Steam

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.SteamLeaderboards` | `Init` | `Void` | `()` |
| `Il2Cpp.SteamLeaderboards` | `OnLeaderboardFound` | `Void` | `(LeaderboardFindResult_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `OnScoreUploaded` | `Void` | `(LeaderboardScoreUploaded_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `OnUserScoreDownloaded` | `Void` | `(LeaderboardScoresDownloaded_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamLeaderboards` | `RequestUserEntry` | `Void` | `()` |
| `Il2Cpp.SteamLeaderboards` | `UploadScore` | `Void` | `(Single moneyPerSecond)` |
| `Il2Cpp.SteamManager` | `Awake` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `InitOnPlayMode` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.SteamManager` | `SteamAPIDebugTextHook` | `Void` | `(Int32 nSeverity, StringBuilder pchDebugText)` |
| `Il2Cpp.SteamManager` | `Update` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `Init` | `Void` | `()` |

### Tutorials

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ChatController` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.DeviceTimerManager` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.Numpad` | `OnEnable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `OnEnable` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Cursor` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PlayerHit` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `OnEnable` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `Awake` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `ButtonOK` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `ButtonShowTutorialInPauseMenu` | `Void` | `(Int32 i)` |
| `Il2Cpp.Tutorials` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `OnVideoPrepared` | `Void` | `(VideoPlayer vp)` |
| `Il2Cpp.Tutorials` | `PlayVideo` | `Void` | `(Int32 _tutorialIndex, Boolean isInPauseMenu)` |
| `Il2Cpp.Tutorials` | `ShowTutorial` | `Void` | `(Int32 i)` |
| `Il2Cpp.Tutorials` | `SkipTutorials` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `Start` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `StopTutorial` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `StopVideoInPauseMenu` | `Void` | `()` |
| `Il2Cpp.Tutorials` | `_Start_b__11_0` | `Void` | `(CallbackContext context)` |
| `Il2CppTMPro.Examples.VertexJitter` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `OnEnable` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.WorldCanvasCuller` | `OnEnable` | `Void` | `()` |

### Uncategorized

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2CppTMPro.Examples.Benchmark03` | `Awake` | `Void` | `()` |
| `Il2Cpp.Numpad` | `ClickNumber` | `Void` | `(String number)` |
| `Il2Cpp.Numpad` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.ObjectSpin` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.ObjectSpin` | `Update` | `Void` | `()` |
| `Il2Cpp.Router` | `ResolveTargetVlan` | `Int32` | `(String serverIp)` |
| `Il2Cpp.RouterConfiguration` | `OpenConfig` | `Void` | `(Router _router)` |
| `Il2Cpp.RouterRouteRow` | `OnDeleteClicked` | `Void` | `()` |
| `Il2Cpp.RouterRouteRow` | `OnEditClicked` | `Void` | `()` |
| `Il2CppTMPro.Examples.SimpleScript` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `RevealCharacters` | `IEnumerator` | `(TMP_Text textComponent)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `RevealWords` | `IEnumerator` | `(TMP_Text textComponent)` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TextMeshSpawner` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexJitter` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeA` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexShakeB` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexZoom` | `Awake` | `Void` | `()` |

### UnityEngine

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AutoDisable` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AutoDisable` | `TurnOffAfterXseconds` | `IEnumerator` | `()` |
| `Il2Cpp.FootSteps` | `Awake` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `GetRandomClip` | `AudioClip` | `()` |
| `Il2Cpp.FootSteps` | `GetRandomFromRequest` | `AudioClip` | `(Int32 _clipArray)` |
| `Il2Cpp.FootSteps` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `PlayRequestedStepSound` | `Void` | `(Int32 _clipArray)` |
| `Il2Cpp.FootSteps` | `Step` | `Void` | `()` |
| `Il2Cpp.FootSteps` | `checkGroundMaterial` | `IEnumerator` | `()` |
| `Il2Cpp.GamepadIcons` | `GetSprite` | `Sprite` | `(String controlPath)` |
| `Il2Cpp.HRSystem` | `ButtonCancelBuying` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonConfirmFireEmployee` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonConfirmHire` | `Void` | `()` |
| `Il2Cpp.HRSystem` | `ButtonFireEmployee` | `Void` | `(Int32 i)` |
| `Il2Cpp.HRSystem` | `ButtonHireEmployee` | `Void` | `(Int32 i)` |
| `Il2Cpp.HRSystem` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.Localisation` | `Awake` | `Void` | `()` |
| `Il2Cpp.Localisation` | `ChangeLocalisation` | `Void` | `(Int32 _uid)` |
| `Il2Cpp.Localisation` | `LoadLocalisation` | `Dictionary`2` | `(Int32 _uid)` |
| `Il2Cpp.Localisation` | `ReturnTextByID` | `String` | `(Int32 _uid)` |
| `Il2Cpp.LocalisedText` | `ChangeText` | `Void` | `()` |
| `Il2Cpp.LocalisedText` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.LocalisedText` | `SetText` | `Void` | `(Int32 _localisation_uid)` |
| `Il2Cpp.LocalisedText` | `Start` | `Void` | `()` |
| `Il2Cpp.OpenURL` | `OpenURLInBrowser` | `Void` | `()` |
| `Il2Cpp.PositionIndicator` | `Awake` | `Void` | `()` |
| `Il2Cpp.PositionIndicator` | `Update` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `Start` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `TweenTheColors` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `Update` | `Void` | `()` |
| `Il2Cpp.PulsatingText` | `setColorCallback` | `Void` | `(Color c)` |
| `Il2Cpp.StaminaOverlayOnEnable` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.UserReport` | `ClearForm` | `Void` | `()` |
| `Il2Cpp.UserReport` | `ClearReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `CreateUserReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `SetThumbnail` | `Void` | `(Texture2D thumbnail)` |
| `Il2Cpp.UserReport` | `ShowError` | `IEnumerator` | `()` |
| `Il2Cpp.UserReport` | `Start` | `Void` | `()` |
| `Il2Cpp.UserReport` | `SubmitUserReport` | `Void` | `()` |
| `Il2Cpp.UserReport` | `Update` | `Void` | `()` |
| `Il2Cpp.UserReport` | `_SubmitUserReport_b__19_0` | `Void` | `(Single uploadProgress)` |
| `Il2Cpp.UserReport` | `_SubmitUserReport_b__19_1` | `Void` | `(Boolean success)` |

### VisualUI

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.AssetManagementDeviceLine` | `ButtonClearWarningSign` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `ScrollAuto` | `Void` | `()` |
| `Il2Cpp.AutoScrollRect` | `Update` | `Void` | `()` |
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
| `Il2Cpp.FlexibleColorPicker` | `UpdateStatic` | `Void` | `(PickerType type)` |
| `Il2Cpp.FlexibleColorPicker` | `UpdateTextures` | `Void` | `()` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_0` | `Void` | `(Single v)` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_1` | `Void` | `(Single v)` |
| `Il2Cpp.FlexibleColorPicker` | `_Awake_b__43_2` | `Void` | `(Single v)` |
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
| `Il2Cpp.LeanTweenUIElement` | `Awake` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `Disabling` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `KeepRotating` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `TweenHorizontal` | `Void` | `(Boolean leanout)` |
| `Il2Cpp.LeanTweenUIElement` | `TweenScaleInOut` | `IEnumerator` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `TweenVertical` | `Void` | `(Boolean leanout)` |
| `Il2Cpp.LeanTweenUIElement` | `Update` | `Void` | `()` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.LeanTweenUIElement` | `_Awake_b__16_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.MainMenu` | `Continue` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `HideMiddleMenu` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `Load` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.MainMenu` | `LoadGame` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `NewGame` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `QuitGame` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `Settings` | `Void` | `()` |
| `Il2Cpp.MainMenu` | `Start` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `Update` | `Void` | `()` |
| `Il2Cpp.MainMenuCamera` | `setmount` | `Void` | `(Transform newmount)` |
| `Il2Cpp.ModShopItem` | `Initialize` | `Void` | `(Int32 modID, ShopItemConfig config, Sprite icon)` |
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
| `Il2CppviperOSK.OSK_AccentConsole` | `LoadAccentMap` | `Void` | `(OSK_AccentAssetObj accents)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `OnDestroy` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `RemoveConsole` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Set` | `Boolean` | `(OSK_LongPressPacket packet)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `SetConsole` | `Void` | `(OSK_LongPressPacket packet)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `ShowBackground` | `Void` | `(Boolean show)` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Start` | `Void` | `()` |
| `Il2CppviperOSK.OSK_AccentConsole` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `AutoFindKeyboard` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `ResizeToFit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Background` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `BlinkCoroutine` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Cursor` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `FindComponentInParentOrSiblings` | `T` | `()` |
| `Il2CppviperOSK.OSK_Cursor` | `Show` | `Void` | `(Boolean show)` |
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
| `Il2CppviperOSK.OSK_GamepadHelper` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `BuildAssignments` | `List`1` | `(OSK_LanguagePackage profile)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `BuildLookup` | `Dictionary`2` | `(OSK_LanguagePackage profile)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `Canonicalize` | `String` | `(String s, Nullable`1 script)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `EnumerateLetterGlyphs` | `List`1` | `(List`1 ranges, Boolean includeUppercase, Boolean includeLowercase, Boolean collapseCase, Boolean preferLowercase, Nullable`1 scriptForSpecials)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ExcludeRanges` | `List`1` | `(List`1 source, List`1 excludes)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `GetGlyphEnumSlots` | `List`1` | `()` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `GetRangesForScript` | `IReadOnlyList`1` | `(Script script)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsLetter` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsLowercase` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsSurrogate` | `Boolean` | `(Int32 cp)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsUppercase` | `Boolean` | `(UnicodeCategory c)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `IsValidCodePoint` | `Boolean` | `(Int32 cp)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `MergeRanges` | `List`1` | `(IReadOnlyList`1 a, List`1 b)` |
| `Il2CppviperOSK.OSK_GlyphHandler` | `ResolvePrimaryScript` | `Script` | `(CultureInfo culture)` |
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
| `Il2CppviperOSK.OSK_Key` | `GetKeyCode` | `OSK_KeyCode` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetKeyName` | `String` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetKeyTransform` | `Transform` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetLayoutLocation` | `Vector2Int` | `()` |
| `Il2CppviperOSK.OSK_Key` | `GetObject` | `Object` | `()` |
| `Il2CppviperOSK.OSK_Key` | `Highlight` | `Void` | `(Boolean hi, Color c)` |
| `Il2CppviperOSK.OSK_Key` | `JoystickPressDown` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `JoystickPressUp` | `Void` | `(OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `KeyFont` | `Void` | `(TMP_FontAsset keyfont)` |
| `Il2CppviperOSK.OSK_Key` | `KeyType` | `OSK_KEY_TYPES` | `()` |
| `Il2CppviperOSK.OSK_Key` | `LastPressed` | `Single` | `()` |
| `Il2CppviperOSK.OSK_Key` | `LongPressCheck` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnDepressed` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnKeyDepress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `OnKeyPress` | `Void` | `(String keyDevice, OSK_Receiver inputfield)` |
| `Il2CppviperOSK.OSK_Key` | `OnMouseDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Key` | `OnMouseUp` | `Void` | `()` |
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
| `Il2CppviperOSK.OSK_KeySounds` | `Update` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `AcceptPhysicalKeyboard` | `Void` | `(Boolean accept)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddNewLine` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddString` | `Void` | `(String multichar)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddText` | `Void` | `(String newText)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AddText_ShftEnabled` | `Void` | `(String newText)` |
| `Il2CppviperOSK.OSK_Keyboard` | `AutoCorrectLayout` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Awake` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ButtonA` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ClickSound` | `Void` | `(Int32 keytypecode)` |
| `Il2CppviperOSK.OSK_Keyboard` | `DpadMove` | `Void` | `(Vector2 dir)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Cancel` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Horizontal` | `Void` | `(Single f)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Submit` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GamepadInput_Vertical` | `Void` | `(Single f)` |
| `Il2CppviperOSK.OSK_Keyboard` | `Generate` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetKeyCode` | `KeyCode` | `(String c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetOSKKey` | `OSK_Key` | `(String k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetOSKKeyCode` | `OSK_KeyCode` | `(String c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `GetSelectedKey` | `OSK_Key` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `HasFocus` | `Void` | `(Boolean isFocus)` |
| `Il2CppviperOSK.OSK_Keyboard` | `HasKey` | `Boolean` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `InputFromPointerDevice` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `InsertText` | `Void` | `(String newText, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyBackspace` | `Void` | `(OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyCall` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyCallBase` | `Void` | `(OSK_KeyCode k, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyDelete` | `Void` | `(OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyScreenSize` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyShift` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `KeyboardSizeEstimator` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `LoadLayout` | `Void` | `(String lay)` |
| `Il2CppviperOSK.OSK_Keyboard` | `OSK_to_KeyCode` | `KeyCode` | `(OSK_KeyCode k)` |
| `Il2CppviperOSK.OSK_Keyboard` | `OnGUI` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `OnPhysicalKeyStroke` | `Void` | `(Char c)` |
| `Il2CppviperOSK.OSK_Keyboard` | `OutputTextUpdate` | `Void` | `(String newchar, OSK_Receiver receiver)` |
| `Il2CppviperOSK.OSK_Keyboard` | `Prep` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ReHighlightKey` | `IEnumerator` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `RemapPhysicalKeyboard` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `ResizeKeyToFit` | `Void` | `(Vector2 scrSize)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SelectSound` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Keyboard` | `SelectedKeyMove` | `OSK_Key` | `(Vector2 dir, Vector2Int currentLoc, Boolean makeSoundIfMove)` |
| `Il2CppviperOSK.OSK_Keyboard` | `SetInteractable` | `Void` | `(Boolean isInteractable)` |
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
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectLayout` | `String` | `(String layout)` |
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectRecursive` | `Void` | `(String input, List`1 result)` |
| `Il2CppviperOSK.OSK_Keymap` | `AutoCorrectRow` | `String` | `(String row)` |
| `Il2CppviperOSK.OSK_Keymap` | `BaseCharacter` | `String` | `(String accentedChar)` |
| `Il2CppviperOSK.OSK_Keymap` | `CapitalizeCorrectly` | `String` | `(String input, String correctForm)` |
| `Il2CppviperOSK.OSK_Keymap` | `GenKeyMapDict` | `Dictionary`2` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `GenKeyMapStr` | `String` | `()` |
| `Il2CppviperOSK.OSK_Keymap` | `GetCorrectedKey` | `String` | `(String key)` |
| `Il2CppviperOSK.OSK_Keymap` | `IsAccentedCharacter` | `Boolean` | `(Char c)` |
| `Il2CppviperOSK.OSK_Keymap` | `SupportGlyphs` | `Void` | `(OSK_LanguagePackage glyphProfile)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `CreateBackground` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Generate` | `Void` | `(List`1 chars, Boolean shiftup, Action`2 callbackAction, Boolean bottomLeftOrder)` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `GetSize` | `Vector3` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `Reset` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `ResizeBackground` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SelectedFirstKey` | `Void` | `()` |
| `Il2CppviperOSK.OSK_MiniKeyboard` | `SelectedKeyMove` | `Void` | `(Vector2 dir)` |
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
| `Il2CppviperOSK.OSK_Receiver` | `OnMouseDown` | `Void` | `()` |
| `Il2CppviperOSK.OSK_Receiver` | `OnMouseUp` | `Void` | `()` |
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
| `Il2CppviperOSK.OSK_UI_Cursor` | `Show` | `Void` | `(Boolean show)` |
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
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Traverse` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `Update` | `Void` | `()` |
| `Il2Cpp.ObjectiveObject` | `PlayUIEffectDisolve` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `Awake` | `Void` | `()` |
| `Il2Cpp.PatchPanel` | `GenerateUniquePatchPanelId` | `String` | `()` |
| `Il2Cpp.PatchPanel` | `GetPairedLink` | `CableLink` | `(CableLink link)` |
| `Il2Cpp.PatchPanel` | `InsertedInRack` | `Void` | `(PatchPanelSaveData saveData)` |
| `Il2Cpp.PatchPanel` | `ValidateRackPosition` | `Boolean` | `()` |
| `Il2Cpp.PauseMenu` | `Awake` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `CloseLoadSaveOverlay` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `DeleteSaveButtonClick` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.PauseMenu` | `DeleteSaveConfirm` | `Void` | `(Boolean yes)` |
| `Il2Cpp.PauseMenu` | `ExitGame` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `HandleAddCommand` | `Void` | `(Il2CppStringArray parts)` |
| `Il2Cpp.PauseMenu` | `Load` | `Void` | `(String savename)` |
| `Il2Cpp.PauseMenu` | `LoadSaveOnButtonClick` | `Void` | `(TextMeshProUGUI _text)` |
| `Il2Cpp.PauseMenu` | `LoadWithOverlay` | `IEnumerator` | `(String savename)` |
| `Il2Cpp.PauseMenu` | `MainMenu` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `NotAllowedToSaveOverlayOff` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.PauseMenu` | `OnPause` | `Void` | `(Int32 openMenu)` |
| `Il2Cpp.PauseMenu` | `PopulateLoadSaveMenu` | `Void` | `(Boolean _savingGame)` |
| `Il2Cpp.PauseMenu` | `ProcessConsoleCommand` | `Void` | `(String input)` |
| `Il2Cpp.PauseMenu` | `Save` | `Void` | `(String saveName, String _stringNameOfSave)` |
| `Il2Cpp.PauseMenu` | `SaveConfirm` | `Void` | `(Boolean yes)` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenu` | `_Awake_b__28_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.PauseMenu` | `_LoadSaveOnButtonClick_b__35_0` | `Void` | `(String saveName)` |
| `Il2Cpp.PauseMenu` | `_SaveConfirm_b__37_0` | `Void` | `(String saveName)` |
| `Il2Cpp.PauseMenu_TabButton` | `UnityEngine_EventSystems_ISelectHandler_OnSelect` | `Void` | `(BaseEventData eventData)` |
| `Il2Cpp.PauseMenu_TabGroup` | `OnTabSelected` | `Void` | `(PauseMenu_TabButton tabbutton)` |
| `Il2Cpp.PulsatingImageColor` | `Awake` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `TweenTheColors` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `Update` | `Void` | `()` |
| `Il2Cpp.PulsatingImageColor` | `setColorCallback` | `Void` | `(Color c)` |
| `Il2Cpp.RackPosition` | `GetByUID` | `RackPosition` | `(Int32 uid)` |
| `Il2Cpp.RackPosition` | `SetUID` | `Void` | `(Int32 uid)` |
| `Il2Cpp.ReBindUI` | `Awake` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `DoRebind` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `GetBindingInfo` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `OnValidate` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `ResetBinding` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `UpdateUI` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `_OnEnable_b__16_0` | `Void` | `()` |
| `Il2Cpp.ReBindUI` | `_OnEnable_b__16_1` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `OnActionChange` | `Void` | `(Object obj, InputActionChange change)` |
| `Il2Cpp.RebindUIv2` | `ResetToDefault` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `ResolveActionAndBinding` | `Boolean` | `(InputAction& action, Int32& bindingIndex)` |
| `Il2Cpp.RebindUIv2` | `UpdateActionLabel` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `UpdateBindingDisplay` | `Void` | `()` |
| `Il2Cpp.RebindUIv2` | `_UpdateBindingDisplay_b__30_0` | `Boolean` | `(InputBinding x)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Initialize` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Initialize` | `Void` | `(IRecyclableScrollRectDataSource dataSource)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `OnValueChangedListener` | `Void` | `(Vector2 normalizedPos)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `ReloadData` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `ReloadData` | `Void` | `(IRecyclableScrollRectDataSource dataSource)` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `Start` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `_Initialize_b__13_0` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclableScrollRect` | `_ReloadData_b__17_0` | `Void` | `()` |
| `Il2CppPolyAndCode.UI.RecyclingSystem` | `InitCoroutine` | `IEnumerator` | `(Action onInitialized)` |
| `Il2CppPolyAndCode.UI.RecyclingSystem` | `OnValueChangedListener` | `Vector2` | `(Vector2 direction)` |
| `Il2Cpp.ReusableFunctions` | `ChangeButtonNormalColor` | `Void` | `(Button button, Color color)` |
| `Il2Cpp.ReusableFunctions` | `HexToColor` | `Color` | `(String hex)` |
| `Il2Cpp.ReusableFunctions` | `ImageScrollingUI` | `IEnumerator` | `(Il2CppReferenceArray`1 _sprites, Image _image)` |
| `Il2Cpp.ReusableFunctions` | `NumberScrollingUI` | `IEnumerator` | `(TextMeshProUGUI _text, Int32 _endNumber)` |
| `Il2Cpp.RouterConfiguration` | `ButtonAddRoute` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetMask` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetSubnet` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetTargetVLAN` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `ButtonSetVLAN` | `Void` | `()` |
| `Il2Cpp.RouterConfiguration` | `CreateRouteRowUI` | `Void` | `(Int32 sVlan, Int32 tVlan, String subnetCidr)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetMask_b__13_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetSubnet_b__12_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetTargetVLAN_b__15_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterConfiguration` | `_ButtonSetVLAN_b__14_0` | `Void` | `(String text)` |
| `Il2Cpp.RouterRouteRow` | `Initialize` | `Void` | `(RouterConfiguration _config, Router _router, Int32 _sourceVlan, Int32 _targetVlan, String subnetCidr, String _gateway)` |
| `Il2Cpp.SettingsGraphics` | `ChangeDepthOfField` | `Void` | `(Single startFarFocus, Single endFarFocus)` |
| `Il2Cpp.SettingsGraphics` | `IsDLSSSupported` | `Boolean` | `()` |
| `Il2Cpp.SettingsGraphics` | `LimitFrameRate` | `Void` | `(Int32 _framerate)` |
| `Il2Cpp.SettingsGraphics` | `MoveToMonitorCoroutine` | `IEnumerator` | `(DisplayInfo targetDisplay)` |
| `Il2Cpp.SettingsGraphics` | `PopulateMonitors` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `RepopulateResolutions` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `ResetDepthOfField` | `Void` | `()` |
| `Il2Cpp.SettingsGraphics` | `SetAAQuality` | `Void` | `(Int32 index)` |
| `Il2Cpp.SettingsGraphics` | `SetAntiAliasing` | `Void` | `(Int32 index)` |
| `Il2Cpp.SettingsGraphics` | `SetExposure` | `Void` | `(Single exposure)` |
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
| `Il2Cpp.ShopCartItem` | `Initialize` | `Void` | `(ComputerShop shop, String itemName, Int32 itemID, Int32 price, ObjectInHand itemType, Int32 firstSpawnUID, Nullable`1 customColor)` |
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
| `Il2Cpp.StaticUIElements` | `UpdateHoldProgress` | `Void` | `(Single value)` |
| `Il2Cpp.StaticUIElements` | `UpdateMessageDisplay` | `Void` | `()` |
| `Il2Cpp.StaticUIElements` | `UpdateMessagesCoroutine` | `IEnumerator` | `()` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `FormatDistance` | `String` | `(Double meters)` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `OnGlobalStatsReceived` | `Void` | `(GlobalStatsReceived_t result, Boolean ioFailure)` |
| `Il2Cpp.SteamStatsOnMainMenuTop` | `WaitAndDisplay` | `IEnumerator` | `()` |
| `Il2CppTMPro.TMP_DigitValidator` | `Validate` | `Char` | `(String& text, Int32& pos, Char ch)` |
| `Il2CppTMPro.Examples.TMP_ExampleScript_01` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_ExampleScript_01` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Set_FrameCounter_Position` | `Void` | `(FpsCounterAnchorPositions anchor_position)` |
| `Il2CppTMPro.Examples.TMP_FrameRateCounter` | `Update` | `Void` | `()` |
| `Il2CppTMPro.TMP_PhoneNumberValidator` | `Validate` | `Char` | `(String& text, Int32& pos, Char ch)` |
| `Il2CppTMPro.Examples.TMP_TextEventCheck` | `OnCharacterSelection` | `Void` | `(Char c, Int32 index)` |
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
| `Il2CppTMPro.Examples.TMP_UiFrameRateCounter` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMPro_InstructionOverlay` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.TMPro_InstructionOverlay` | `Set_FrameCounter_Position` | `Void` | `(FpsCounterAnchorPositions anchor_position)` |
| `Il2Cpp.TerrainDetector` | `GetActiveTerrainTextureIdx` | `Int32` | `(Vector3 position)` |
| `Il2CppTMPro.Examples.TextConsoleSimulator` | `ON_TEXT_CHANGED` | `Void` | `(Object obj)` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `DisplayTextMeshFloatingText` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.TextMeshProFloatingText` | `DisplayTextMeshProFloatingText` | `IEnumerator` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnDeselect` | `Void` | `()` |
| `Il2Cpp.ToolTipOnUIText` | `OnPointerEnter` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.ToolTipOnUIText` | `OnPointerExit` | `Void` | `(PointerEventData eventData)` |
| `Il2Cpp.ToolTipOnUIText` | `OnSelect` | `Void` | `()` |
| `Il2Cpp.UIExtension` | `GetCorners` | `Il2CppStructArray`1` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MaxX` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MaxY` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MinX` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UIExtension` | `MinY` | `Single` | `(RectTransform rectTransform)` |
| `Il2Cpp.UI_Section` | `OpenCloseSection` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `Awake` | `Void` | `()` |
| `Il2Cpp.UI_SelectedBorder` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `AnimateVertexColors` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.VertexColorCycler` | `Awake` | `Void` | `()` |
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
| `Il2Cpp.WorldObjectButton` | `Awake` | `Void` | `()` |

### World

| Class | Method | Return | Parameters |
|-------|--------|--------|------------|
| `Il2Cpp.ActionKeyHint` | `CustomKey` | `Void` | `(InputAction action, String _customText)` |
| `Il2Cpp.ActionKeyHint` | `DelayedUpdateUI` | `IEnumerator` | `()` |
| `Il2Cpp.ActionKeyHint` | `GetBindingInfo` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnDisable` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnEnable` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `OnValidate` | `Void` | `()` |
| `Il2Cpp.ActionKeyHint` | `UpdateUI` | `Void` | `()` |
| `Il2Cpp.CableLink` | `CreateRopeAttachPoint` | `Void` | `()` |
| `Il2Cpp.CableLink` | `GetRopeAttachPoint` | `Transform` | `()` |
| `Il2Cpp.CableLink` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.CableSpinner` | `InteractOnClick` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `Awake` | `Void` | `()` |
| `Il2CppTMPro.Examples.CameraController` | `LateUpdate` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `BrakeAndDeacceleration` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `DisableTheTriggerColliderAfterDealy` | `IEnumerator` | `()` |
| `Il2CppPolyStang.CarController` | `FixedUpdate` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `HandleAudio` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `LeaveTheTrolley` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `Move` | `Void` | `()` |
| `Il2CppPolyStang.CarController` | `OnCollisionEnter` | `Void` | `(Collision collision)` |
| `Il2CppPolyStang.CarController` | `OnDestroy` | `Void` | `()` |
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
| `Il2Cpp.CheckIfTouchingWall` | `Awake` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `DelayedOverlapCheck` | `IEnumerator` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `PerformOverlapCheck` | `Void` | `()` |
| `Il2Cpp.CheckIfTouchingWall` | `SetRenderersEnabled` | `Void` | `(Boolean isEnabled)` |
| `Il2Cpp.CheckIfTouchingWall` | `Start` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `Awake` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.Dumpster` | `InteractOnHover` | `Void` | `(RaycastHit hit)` |
| `Il2Cpp.Dumpster` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.EnvMapAnimator` | `Awake` | `Void` | `()` |
| `Il2Cpp.EnvMapAnimator` | `Start` | `IEnumerator` | `()` |
| `Il2Cpp.InputController` | `Contains` | `Boolean` | `(InputAction action)` |
| `Il2Cpp.InputController` | `Disable` | `Void` | `()` |
| `Il2Cpp.InputController` | `Dispose` | `Void` | `()` |
| `Il2Cpp.InputController` | `Enable` | `Void` | `()` |
| `Il2Cpp.InputController` | `Finalize` | `Void` | `()` |
| `Il2Cpp.InputController` | `FindAction` | `InputAction` | `(String actionNameOrId, Boolean throwIfNotFound)` |
| `Il2Cpp.InputController` | `FindBinding` | `Int32` | `(InputBinding bindingMask, InputAction& action)` |
| `Il2Cpp.InputController` | `GetEnumerator` | `IEnumerator`1` | `()` |
| `Il2Cpp.InputController` | `System_Collections_IEnumerable_GetEnumerator` | `IEnumerator` | `()` |
| `Il2Cpp.MusicPlayer` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.NetworkSwitch` | `InteractOnClick` | `Void` | `()` |
| `Il2CppviperOSK.OSK_UI_Keyboard` | `SetInteractable` | `Void` | `(Boolean isInteractable)` |
| `Il2Cpp.PatchPanel` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackDoor` | `DelayedTrigger` | `IEnumerator` | `()` |
| `Il2Cpp.RackDoor` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackMount` | `InteractOnClick` | `Void` | `()` |
| `Il2Cpp.RackPosition` | `InteractOnClick` | `Void` | `()` |
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
| `Il2CppGogoGaga.OptimizedRopesAndCables.Rope` | `IsPointsMoved` | `Boolean` | `()` |
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
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `OnValidate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `SubscribeToRopeEvents` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `UnsubscribeFromRopeEvents` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeMesh` | `Update` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Awake` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `FixedUpdate` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `GenerateWind` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `SimulatePhysics` | `Void` | `()` |
| `Il2CppGogoGaga.OptimizedRopesAndCables.RopeWindEffect` | `Update` | `Void` | `()` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `AnimateProperties` | `IEnumerator` | `()` |
| `Il2CppTMPro.Examples.ShaderPropAnimator` | `Awake` | `Void` | `()` |
| `UnityStandardAssets.Characters.ThirdPerson.ThirdPersonCharacter` | `UpdateAnimator` | `Void` | `(Vector3 move)` |
| `Il2Cpp.TrolleyLoadingBay` | `FreeTrolleySlot` | `Void` | `(Int32 startIdx, Int32 sizeInU)` |
| `Il2Cpp.TrolleyLoadingBay` | `InteractOnClick` | `Void` | `()` |
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
| `Il2Cpp.UsableObject` | `MoveBetweenPositions` | `Void` | `(Vector3 _position, Vector3 _rotation)` |
| `Il2Cpp.UsableObject` | `MoveToHand` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `MoveToStorage` | `Void` | `(Transform _pos, Int32 _positionIndex, Int32 _storageUid)` |
| `Il2Cpp.UsableObject` | `OnCollisionEnter` | `Void` | `(Collision collision)` |
| `Il2Cpp.UsableObject` | `OnDestroy` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `OnHoverOver` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `OnLoadDestroy` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `RemoveRigidbody` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `RestoreRigidbody` | `Void` | `()` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_0` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_1` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.UsableObject` | `_Awake_b__47_2` | `Void` | `(CallbackContext ctx)` |
| `Il2Cpp.WorldCanvasCuller` | `Awake` | `Void` | `()` |
| `Il2Cpp.WorldObjectButton` | `InteractOnClick` | `Void` | `()` |
| `Il2CppviperTools.viperInput` | `AButtonDown` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `AButtonUp` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `AnyPhysicalKey` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `BButtonDown` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `BButtonUp` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `ConvertKeyCodeToKey` | `Key` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `ConvertToLegacyAxis` | `String` | `(AXIS_INPUT axis)` |
| `Il2CppviperTools.viperInput` | `Fire1` | `Boolean` | `()` |
| `Il2CppviperTools.viperInput` | `GetAllAxis` | `Single` | `()` |
| `Il2CppviperTools.viperInput` | `GetAxis` | `Single` | `(AXIS_INPUT axis)` |
| `Il2CppviperTools.viperInput` | `GetControllerNames` | `Il2CppStringArray` | `()` |
| `Il2CppviperTools.viperInput` | `GetPhysicalKey` | `String` | `()` |
| `Il2CppviperTools.viperInput` | `GetPlayerAButton` | `Boolean` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `GetPlayerBButton` | `Boolean` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `GetPlayerJoystickInput` | `Vector2` | `(Int32 p)` |
| `Il2CppviperTools.viperInput` | `GetPointerPos` | `Vector2` | `()` |
| `Il2CppviperTools.viperInput` | `IsLetterAZ` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyDown` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyPress` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `KeyUp` | `Boolean` | `(KeyCode k)` |
| `Il2CppviperTools.viperInput` | `NumControllers` | `Int32` | `()` |
| `Il2CppviperTools.viperInput` | `PointerDown` | `Boolean` | `(Int32 mouseBtn)` |
| `Il2CppviperTools.viperInput` | `PointerUp` | `Boolean` | `(Int32 mouseBtn)` |
| `Il2CppviperTools.viperInput` | `RegisterKeyStrokeCallback` | `Void` | `(Action`1 action, Boolean enable)` |
| `Il2CppviperTools.viperInput` | `ResetAllAxis` | `Void` | `()` |
| `Il2CppviperTools.viperInput` | `Start` | `Void` | `()` |


---

## 2. Greg Hooks

These are the canonical greg-domain hooks registered in the framework hook bus.
Use them in your mod's event subscriptions.

### CUSTOMER

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.CUSTOMER.AppPerformanceAdded` | `Il2Cpp.CustomerBase::AddAppPerformance(int, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.AddAppPerformance |
| `greg.CUSTOMER.AppText` | `Il2Cpp.CustomerBase::AppText(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.AppText |
| `greg.CUSTOMER.AppText` | `Il2Cpp.CustomerBase::AppText(int, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.AppText |
| `greg.CUSTOMER.AreAllAppRequirementsMet` | `Il2Cpp.CustomerBase::AreAllAppRequirementsMet()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.AreAllAppRequirementsMet |
| `greg.CUSTOMER.BoxIl2CppObject` | `Il2Cpp.CustomerNetworkInfo::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerNetworkInfo.BoxIl2CppObject |
| `greg.CUSTOMER.CheckIfAppRequirementsAreMet` | `Il2Cpp.CustomerBase::CheckIfAppRequirementsAreMet()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.CheckIfAppRequirementsAreMet |
| `greg.CUSTOMER.ComponentInitialized` | `Il2Cpp.CustomerBase::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.Awake |
| `greg.CUSTOMER.ComponentInitialized` | `Il2Cpp.CustomerBase::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.Start |
| `greg.CUSTOMER.ComponentInitialized` | `Il2Cpp.CustomerBaseDoor::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.Awake |
| `greg.CUSTOMER.CustomerServerCountAndSpeedChanged` | `Il2Cpp.CustomerBase::UpdateCustomerServerCountAndSpeed(int, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.UpdateCustomerServerCountAndSpeed |
| `greg.CUSTOMER.CustomerSet` | `Il2Cpp.CustomerCard::SetCustomer(CustomerItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerCard.SetCustomer |
| `greg.CUSTOMER.DataLoaded` | `Il2Cpp.CustomerBase::LoadData(CustomerBaseSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.LoadData |
| `greg.CUSTOMER.DelayedAppDoorOpening` | `Il2Cpp.CustomerBase::DelayedAppDoorOpening(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.DelayedAppDoorOpening |
| `greg.CUSTOMER.GetAppIDForIP` | `Il2Cpp.CustomerBase::GetAppIDForIP(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetAppIDForIP |
| `greg.CUSTOMER.GetAppsSpeedRequirements` | `Il2Cpp.CustomerBase::GetAppsSpeedRequirements()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetAppsSpeedRequirements |
| `greg.CUSTOMER.GetEffectiveMoneySpeed` | `Il2Cpp.CustomerBase::GetEffectiveMoneySpeed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetEffectiveMoneySpeed |
| `greg.CUSTOMER.GetServerTypeForIP` | `Il2Cpp.CustomerBase::GetServerTypeForIP(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetServerTypeForIP |
| `greg.CUSTOMER.GetSubnetsPerApp` | `Il2Cpp.CustomerBase::GetSubnetsPerApp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetSubnetsPerApp |
| `greg.CUSTOMER.GetTotalAppSpeed` | `Il2Cpp.CustomerBase::GetTotalAppSpeed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetTotalAppSpeed |
| `greg.CUSTOMER.GetVlanIdsPerApp` | `Il2Cpp.CustomerBase::GetVlanIdsPerApp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.GetVlanIdsPerApp |
| `greg.CUSTOMER.InteractOnClick` | `Il2Cpp.CustomerBaseDoor::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.InteractOnClick |
| `greg.CUSTOMER.InteractOnHover` | `Il2Cpp.CustomerBaseDoor::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.InteractOnHover |
| `greg.CUSTOMER.IsIPPresent` | `Il2Cpp.CustomerBase::IsIPPresent(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.IsIPPresent |
| `greg.CUSTOMER.MoneyChanged` | `Il2Cpp.CustomerBase::UpdateMoney()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.UpdateMoney |
| `greg.CUSTOMER.OnDestroy` | `Il2Cpp.CustomerBaseDoor::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.OnDestroy |
| `greg.CUSTOMER.OnHoverOver` | `Il2Cpp.CustomerBaseDoor::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.OnHoverOver |
| `greg.CUSTOMER.OnLoad` | `Il2Cpp.CustomerBaseDoor::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.OnLoad |
| `greg.CUSTOMER.OpenDoor` | `Il2Cpp.CustomerBaseDoor::OpenDoor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.OpenDoor |
| `greg.CUSTOMER.OpenDoorAndSetupBase` | `Il2Cpp.CustomerBaseDoor::OpenDoorAndSetupBase(CustomerItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBaseDoor.OpenDoorAndSetupBase |
| `greg.CUSTOMER.ResetAllAppSpeeds` | `Il2Cpp.CustomerBase::ResetAllAppSpeeds()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.ResetAllAppSpeeds |
| `greg.CUSTOMER.SpeedOnCustomerBaseAppChanged` | `Il2Cpp.CustomerBase::UpdateSpeedOnCustomerBaseApp(int, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.UpdateSpeedOnCustomerBaseApp |
| `greg.CUSTOMER.UpAppSet` | `Il2Cpp.CustomerBase::SetUpApp(int, int, CustomerBaseSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.SetUpApp |
| `greg.CUSTOMER.UpBaseSet` | `Il2Cpp.CustomerBase::SetUpBase(CustomerItem, CustomerBaseSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerBase.SetUpBase |
| `greg.CUSTOMER.__AssignHandles` | `Il2Cpp.CustomerNetworkInfo::__AssignHandles(Unity.Entities.SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CustomerNetworkInfo.__AssignHandles |

### EMPLOYEE

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.EMPLOYEE.AssignJob` | `Il2Cpp.Technician::AssignJob(TechnicianManager.RepairJob)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.AssignJob |
| `greg.EMPLOYEE.ButtonCancelBuying` | `Il2Cpp.HRSystem::ButtonCancelBuying()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.ButtonCancelBuying |
| `greg.EMPLOYEE.ButtonConfirmFireEmployee` | `Il2Cpp.HRSystem::ButtonConfirmFireEmployee()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.ButtonConfirmFireEmployee |
| `greg.EMPLOYEE.ButtonConfirmHire` | `Il2Cpp.HRSystem::ButtonConfirmHire()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.ButtonConfirmHire |
| `greg.EMPLOYEE.ButtonFireEmployee` | `Il2Cpp.HRSystem::ButtonFireEmployee(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.ButtonFireEmployee |
| `greg.EMPLOYEE.ButtonHireEmployee` | `Il2Cpp.HRSystem::ButtonHireEmployee(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.ButtonHireEmployee |
| `greg.EMPLOYEE.CacheDeviceBounds` | `Il2Cpp.Technician::CacheDeviceBounds(GameObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.CacheDeviceBounds |
| `greg.EMPLOYEE.Changed` | `Il2Cpp.Technician::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.Update |
| `greg.EMPLOYEE.ComponentInitialized` | `Il2Cpp.HRSystem::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/HRSystem.OnEnable |
| `greg.EMPLOYEE.ComponentInitialized` | `Il2Cpp.Technician::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.Awake |
| `greg.EMPLOYEE.ComponentInitialized` | `Il2Cpp.Technician::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.Start |
| `greg.EMPLOYEE.ComponentInitialized` | `Il2Cpp.TechnicianManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.Awake |
| `greg.EMPLOYEE.DeviceRepaired` | `Il2Cpp.Technician::RepairDevice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.RepairDevice |
| `greg.EMPLOYEE.Dispatched` | `Il2Cpp.TechnicianManager::SendTechnician(NetworkSwitch, Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.SendTechnician |
| `greg.EMPLOYEE.Fired` | `Il2Cpp.TechnicianManager::FireTechnician(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.FireTechnician |
| `greg.EMPLOYEE.GetActiveJobs` | `Il2Cpp.TechnicianManager::GetActiveJobs()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.GetActiveJobs |
| `greg.EMPLOYEE.GetClosestOpenedDumpsterIndex` | `Il2Cpp.TechnicianManager::GetClosestOpenedDumpsterIndex(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.GetClosestOpenedDumpsterIndex |
| `greg.EMPLOYEE.GetCorrectDevicePrefab` | `Il2Cpp.Technician::GetCorrectDevicePrefab()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.GetCorrectDevicePrefab |
| `greg.EMPLOYEE.GetCurrentDevicePrefabID` | `Il2Cpp.Technician::GetCurrentDevicePrefabID()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.GetCurrentDevicePrefabID |
| `greg.EMPLOYEE.GetQueuedJobs` | `Il2Cpp.TechnicianManager::GetQueuedJobs()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.GetQueuedJobs |
| `greg.EMPLOYEE.GettingNewServer` | `Il2Cpp.Technician::GettingNewServer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.GettingNewServer |
| `greg.EMPLOYEE.HandIKWeightSet` | `Il2Cpp.Technician::SetHandIKWeight(float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.SetHandIKWeight |
| `greg.EMPLOYEE.Hired` | `Il2Cpp.TechnicianManager::AddTechnician(Technician)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.AddTechnician |
| `greg.EMPLOYEE.IsDeviceAlreadyAssigned` | `Il2Cpp.TechnicianManager::IsDeviceAlreadyAssigned(NetworkSwitch, Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.IsDeviceAlreadyAssigned |
| `greg.EMPLOYEE.JobQueueLoaded` | `Il2Cpp.TechnicianManager::RestoreJobQueue(List<RepairJobSaveData>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.RestoreJobQueue |
| `greg.EMPLOYEE.JobQueued` | `Il2Cpp.TechnicianManager::EnqueueDispatch(TechnicianManager.RepairJob)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.EnqueueDispatch |
| `greg.EMPLOYEE.NextJobRequested` | `Il2Cpp.TechnicianManager::RequestNextJob(Technician)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.RequestNextJob |
| `greg.EMPLOYEE.OnDestroy` | `Il2Cpp.Technician::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.OnDestroy |
| `greg.EMPLOYEE.OnDestroy` | `Il2Cpp.TechnicianManager::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.OnDestroy |
| `greg.EMPLOYEE.OnLoadingStarted` | `Il2Cpp.Technician::OnLoadingStarted()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.OnLoadingStarted |
| `greg.EMPLOYEE.OnLoadingStarted` | `Il2Cpp.TechnicianManager::OnLoadingStarted()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.OnLoadingStarted |
| `greg.EMPLOYEE.OpenDumpsterArea` | `Il2Cpp.TechnicianManager::OpenDumpsterArea(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.OpenDumpsterArea |
| `greg.EMPLOYEE.PositionHandTargetsOnDevice` | `Il2Cpp.Technician::PositionHandTargetsOnDevice(GameObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.PositionHandTargetsOnDevice |
| `greg.EMPLOYEE.ProcessDispatchQueue` | `Il2Cpp.TechnicianManager::ProcessDispatchQueue()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TechnicianManager.ProcessDispatchQueue |
| `greg.EMPLOYEE.ReplacingServer` | `Il2Cpp.Technician::ReplacingServer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.ReplacingServer |
| `greg.EMPLOYEE.RequestJobDelayed` | `Il2Cpp.Technician::RequestJobDelayed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.RequestJobDelayed |
| `greg.EMPLOYEE.RotateTowardsGoal` | `Il2Cpp.Technician::RotateTowardsGoal(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.RotateTowardsGoal |
| `greg.EMPLOYEE.StartTextingAnimation` | `Il2Cpp.Technician::StartTextingAnimation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.StartTextingAnimation |
| `greg.EMPLOYEE.ThrowingOutServer` | `Il2Cpp.Technician::ThrowingOutServer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.ThrowingOutServer |
| `greg.EMPLOYEE.ToContainerDispatched` | `Il2Cpp.Technician::SendToContainer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Technician.SendToContainer |

### GAMEPLAY

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.GAMEPLAY.ClearObjectives` | `Il2Cpp.Objectives::ClearObjectives()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.ClearObjectives |
| `greg.GAMEPLAY.ComponentInitialized` | `Il2Cpp.ObjectiveObject::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ObjectiveObject.Start |
| `greg.GAMEPLAY.ComponentInitialized` | `Il2Cpp.Objectives::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.Awake |
| `greg.GAMEPLAY.ComponentInitialized` | `Il2Cpp.Objectives::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.Start |
| `greg.GAMEPLAY.CreateAppObjective` | `Il2Cpp.Objectives::CreateAppObjective(int, int, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.CreateAppObjective |
| `greg.GAMEPLAY.DestroyObjective` | `Il2Cpp.Objectives::DestroyObjective(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.DestroyObjective |
| `greg.GAMEPLAY.DisplayChanged` | `Il2Cpp.ObjectiveTimed::UpdateDisplay(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ObjectiveTimed.UpdateDisplay |
| `greg.GAMEPLAY.EffectOnDestroy` | `Il2Cpp.Objectives::EffectOnDestroy(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.EffectOnDestroy |
| `greg.GAMEPLAY.GetReward` | `Il2Cpp.ObjectiveObject::GetReward()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ObjectiveObject.GetReward |
| `greg.GAMEPLAY.GetTimedObjective` | `Il2Cpp.Objectives::GetTimedObjective(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.GetTimedObjective |
| `greg.GAMEPLAY.InstantiateObjectiveSign` | `Il2Cpp.Objectives::InstantiateObjectiveSign(int, Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.InstantiateObjectiveSign |
| `greg.GAMEPLAY.IsTutorialInProgress` | `Il2Cpp.Objectives::IsTutorialInProgress()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.IsTutorialInProgress |
| `greg.GAMEPLAY.ObjectiveSignRemoved` | `Il2Cpp.Objectives::RemoveObjectiveSign(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.RemoveObjectiveSign |
| `greg.GAMEPLAY.ObjectiveTimedText` | `Il2Cpp.Objectives::ObjectiveTimedText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.ObjectiveTimedText |
| `greg.GAMEPLAY.ObjectivesLoaded` | `Il2Cpp.Objectives::LoadObjectives(HashSet<int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.LoadObjectives |
| `greg.GAMEPLAY.OnDestroy` | `Il2Cpp.Objectives::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.OnDestroy |
| `greg.GAMEPLAY.OnLoad` | `Il2Cpp.Objectives::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.OnLoad |
| `greg.GAMEPLAY.PlayUIEffectDisolve` | `Il2Cpp.ObjectiveObject::PlayUIEffectDisolve()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ObjectiveObject.PlayUIEffectDisolve |
| `greg.GAMEPLAY.StartObjective` | `Il2Cpp.Objectives::StartObjective(int, Vector3, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.StartObjective |
| `greg.GAMEPLAY.StartObjective` | `Il2Cpp.Objectives::StartObjective(int, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Objectives.StartObjective |

### NETWORK

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.NETWORK.AppendEolTime` | `Il2Cpp.NetworkSwitch::AppendEolTime(StringBuilder, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.AppendEolTime |
| `greg.NETWORK.ApplyColor` | `Il2Cpp.CableSpinner::ApplyColor(Color, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.ApplyColor |
| `greg.NETWORK.AssignEntity` | `Il2Cpp.CablePositions::AssignEntity(int, Entity)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.AssignEntity |
| `greg.NETWORK.BrokenServerAdded` | `Il2Cpp.NetworkMap::AddBrokenServer(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.AddBrokenServer |
| `greg.NETWORK.BrokenServerRemoved` | `Il2Cpp.NetworkMap::RemoveBrokenServer(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveBrokenServer |
| `greg.NETWORK.BrokenSwitchAdded` | `Il2Cpp.NetworkMap::AddBrokenSwitch(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.AddBrokenSwitch |
| `greg.NETWORK.BrokenSwitchRemoved` | `Il2Cpp.NetworkMap::RemoveBrokenSwitch(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveBrokenSwitch |
| `greg.NETWORK.BuildPatchPanelCache` | `Il2Cpp.NetworkSwitchConfiguration::BuildPatchPanelCache()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.BuildPatchPanelCache |
| `greg.NETWORK.ButtonEditLabel` | `Il2Cpp.NetworkSwitchConfiguration::ButtonEditLabel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ButtonEditLabel |
| `greg.NETWORK.ButtonPower` | `Il2Cpp.NetworkSwitchConfiguration::ButtonPower()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ButtonPower |
| `greg.NETWORK.ButtonShowNetworkSwitchConfig` | `Il2Cpp.NetworkSwitch::ButtonShowNetworkSwitchConfig()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ButtonShowNetworkSwitchConfig |
| `greg.NETWORK.CableConnectionRemoved` | `Il2Cpp.NetworkMap::RemoveCableConnection(int, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveCableConnection |
| `greg.NETWORK.CableFromLACPGroupsRemoved` | `Il2Cpp.NetworkMap::RemoveCableFromLACPGroups(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveCableFromLACPGroups |
| `greg.NETWORK.CableLoaded` | `Il2Cpp.CablePositions::LoadCable(CableSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.LoadCable |
| `greg.NETWORK.CanAcceptSFP` | `Il2Cpp.SFPBox::CanAcceptSFP(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.CanAcceptSFP |
| `greg.NETWORK.ClearAllCables` | `Il2Cpp.CablePositions::ClearAllCables()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.ClearAllCables |
| `greg.NETWORK.ClearErrorSign` | `Il2Cpp.NetworkSwitch::ClearErrorSign()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ClearErrorSign |
| `greg.NETWORK.ClearMap` | `Il2Cpp.NetworkMap::ClearMap()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.ClearMap |
| `greg.NETWORK.ClearVLANDisplay` | `Il2Cpp.NetworkSwitchConfiguration::ClearVLANDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ClearVLANDisplay |
| `greg.NETWORK.ClearWarningSign` | `Il2Cpp.NetworkSwitch::ClearWarningSign(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ClearWarningSign |
| `greg.NETWORK.ClickPort` | `Il2Cpp.NetworkSwitchConfiguration::ClickPort(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ClickPort |
| `greg.NETWORK.CloseConfig` | `Il2Cpp.NetworkSwitchConfiguration::CloseConfig()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.CloseConfig |
| `greg.NETWORK.CollectPatchPanelChainCables` | `Il2Cpp.CableLink::CollectPatchPanelChainCables(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.CollectPatchPanelChainCables |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.CableLink::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.Start |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.CablePositions::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.Awake |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.CablePositions::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.Start |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.CableSpinner::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.Start |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.NetworkMap::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.Awake |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.NetworkSwitch::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.Awake |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.NetworkSwitch::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.Start |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.NetworkSwitchConfiguration::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.Awake |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.SFPBox::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.Awake |
| `greg.NETWORK.ComponentInitialized` | `Il2Cpp.SFPModule::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.Awake |
| `greg.NETWORK.Connect` | `Il2Cpp.NetworkMap::Connect(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.Connect |
| `greg.NETWORK.ConnectionSpeedSet` | `Il2Cpp.CableLink::SetConnectionSpeed(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.SetConnectionSpeed |
| `greg.NETWORK.CreateLACP` | `Il2Cpp.NetworkSwitchConfiguration::CreateLACP()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.CreateLACP |
| `greg.NETWORK.CreateLACPGroup` | `Il2Cpp.NetworkMap::CreateLACPGroup(string, string, List<int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.CreateLACPGroup |
| `greg.NETWORK.CreateNewCable` | `Il2Cpp.CablePositions::CreateNewCable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.CreateNewCable |
| `greg.NETWORK.CreateNewReverseCable` | `Il2Cpp.CablePositions::CreateNewReverseCable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.CreateNewReverseCable |
| `greg.NETWORK.CreateRopeAttachPoint` | `Il2Cpp.CableLink::CreateRopeAttachPoint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.CreateRopeAttachPoint |
| `greg.NETWORK.CreateTubeMesh` | `Il2Cpp.CablePositions::CreateTubeMesh(List<Vector3>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.CreateTubeMesh |
| `greg.NETWORK.CurrentLengthChanged` | `Il2Cpp.CableSpinner::UpdateCurrentLength(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.UpdateCurrentLength |
| `greg.NETWORK.DeviceAdded` | `Il2Cpp.NetworkMap::AddDevice(string, CableLink.TypeOfLink, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.AddDevice |
| `greg.NETWORK.DeviceCustomerIDChanged` | `Il2Cpp.NetworkMap::UpdateDeviceCustomerID(string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.UpdateDeviceCustomerID |
| `greg.NETWORK.DeviceRemoved` | `Il2Cpp.NetworkMap::RemoveDevice(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveDevice |
| `greg.NETWORK.DeviceRepaired` | `Il2Cpp.NetworkSwitch::RepairDevice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.RepairDevice |
| `greg.NETWORK.DisallowedVlansPerPortSet` | `Il2Cpp.NetworkSwitch::SetDisallowedVlansPerPort(Dictionary<int, HashSet<int>>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.SetDisallowedVlansPerPort |
| `greg.NETWORK.Disconnect` | `Il2Cpp.NetworkMap::Disconnect(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.Disconnect |
| `greg.NETWORK.DisconnectCables` | `Il2Cpp.NetworkSwitch::DisconnectCables()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.DisconnectCables |
| `greg.NETWORK.DisconnectCablesWhenSwitchIsOff` | `Il2Cpp.NetworkSwitch::DisconnectCablesWhenSwitchIsOff()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.DisconnectCablesWhenSwitchIsOff |
| `greg.NETWORK.FindAllReachablePathsFrom` | `Il2Cpp.NetworkMap::FindAllReachablePathsFrom(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.FindAllReachablePathsFrom |
| `greg.NETWORK.FindAllRoutes` | `Il2Cpp.NetworkMap::FindAllRoutes(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.FindAllRoutes |
| `greg.NETWORK.FindPhysicalPath` | `Il2Cpp.NetworkMap::FindPhysicalPath(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.FindPhysicalPath |
| `greg.NETWORK.FromPortRemoved` | `Il2Cpp.SFPModule::RemoveFromPort()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.RemoveFromPort |
| `greg.NETWORK.GenerateDeviceName` | `Il2Cpp.NetworkMap::GenerateDeviceName(CableLink.TypeOfLink, Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GenerateDeviceName |
| `greg.NETWORK.GenerateFinalPath` | `Il2Cpp.CablePositions::GenerateFinalPath(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.GenerateFinalPath |
| `greg.NETWORK.GenerateUniqueSwitchId` | `Il2Cpp.NetworkSwitch::GenerateUniqueSwitchId()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.GenerateUniqueSwitchId |
| `greg.NETWORK.GetAllBrokenServers` | `Il2Cpp.NetworkMap::GetAllBrokenServers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllBrokenServers |
| `greg.NETWORK.GetAllBrokenSwitches` | `Il2Cpp.NetworkMap::GetAllBrokenSwitches()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllBrokenSwitches |
| `greg.NETWORK.GetAllDevices` | `Il2Cpp.NetworkMap::GetAllDevices()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllDevices |
| `greg.NETWORK.GetAllDisallowedVlans` | `Il2Cpp.NetworkSwitch::GetAllDisallowedVlans()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.GetAllDisallowedVlans |
| `greg.NETWORK.GetAllLACPGroups` | `Il2Cpp.NetworkMap::GetAllLACPGroups()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllLACPGroups |
| `greg.NETWORK.GetAllNetworkSwitches` | `Il2Cpp.NetworkMap::GetAllNetworkSwitches()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllNetworkSwitches |
| `greg.NETWORK.GetAllServers` | `Il2Cpp.NetworkMap::GetAllServers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetAllServers |
| `greg.NETWORK.GetCableMaterial` | `Il2Cpp.CablePositions::GetCableMaterial(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.GetCableMaterial |
| `greg.NETWORK.GetCablePositions` | `Il2Cpp.CablePositions::GetCablePositions(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.GetCablePositions |
| `greg.NETWORK.GetConnectedDevices` | `Il2Cpp.NetworkSwitch::GetConnectedDevices()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.GetConnectedDevices |
| `greg.NETWORK.GetCustomerBase` | `Il2Cpp.NetworkMap::GetCustomerBase(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetCustomerBase |
| `greg.NETWORK.GetDevice` | `Il2Cpp.NetworkMap::GetDevice(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetDevice |
| `greg.NETWORK.GetDevicePrefix` | `Il2Cpp.NetworkSwitchConfiguration::GetDevicePrefix(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.GetDevicePrefix |
| `greg.NETWORK.GetDisallowedVlans` | `Il2Cpp.NetworkSwitch::GetDisallowedVlans(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.GetDisallowedVlans |
| `greg.NETWORK.GetFreeSpaceInTheBox` | `Il2Cpp.SFPBox::GetFreeSpaceInTheBox()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.GetFreeSpaceInTheBox |
| `greg.NETWORK.GetLACPGroupBetween` | `Il2Cpp.NetworkMap::GetLACPGroupBetween(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetLACPGroupBetween |
| `greg.NETWORK.GetLACPGroupForCable` | `Il2Cpp.NetworkMap::GetLACPGroupForCable(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetLACPGroupForCable |
| `greg.NETWORK.GetNumberOfDevices` | `Il2Cpp.NetworkMap::GetNumberOfDevices()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetNumberOfDevices |
| `greg.NETWORK.GetRawCablePositions` | `Il2Cpp.CablePositions::GetRawCablePositions(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.GetRawCablePositions |
| `greg.NETWORK.GetRawLinkTransforms` | `Il2Cpp.CablePositions::GetRawLinkTransforms(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.GetRawLinkTransforms |
| `greg.NETWORK.GetRopeAttachPoint` | `Il2Cpp.CableLink::GetRopeAttachPoint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.GetRopeAttachPoint |
| `greg.NETWORK.GetServer` | `Il2Cpp.NetworkMap::GetServer(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetServer |
| `greg.NETWORK.GetSwitchById` | `Il2Cpp.NetworkMap::GetSwitchById(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.GetSwitchById |
| `greg.NETWORK.GetSwitchId` | `Il2Cpp.NetworkSwitch::GetSwitchId()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.GetSwitchId |
| `greg.NETWORK.GetVisibleVLANs` | `Il2Cpp.NetworkSwitchConfiguration::GetVisibleVLANs()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.GetVisibleVLANs |
| `greg.NETWORK.HandleNewCableWhileOff` | `Il2Cpp.NetworkSwitch::HandleNewCableWhileOff(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.HandleNewCableWhileOff |
| `greg.NETWORK.InsertDirectlyIntoPort` | `Il2Cpp.SFPModule::InsertDirectlyIntoPort(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.InsertDirectlyIntoPort |
| `greg.NETWORK.InsertSFP` | `Il2Cpp.CableLink::InsertSFP(float, int, SFPModule)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.InsertSFP |
| `greg.NETWORK.InsertSFPBackIntoBox` | `Il2Cpp.SFPBox::InsertSFPBackIntoBox()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.InsertSFPBackIntoBox |
| `greg.NETWORK.InsertedInSFPPort` | `Il2Cpp.SFPModule::InsertedInSFPPort(CableLink, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.InsertedInSFPPort |
| `greg.NETWORK.InteractOnClick` | `Il2Cpp.CableLink::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.InteractOnClick |
| `greg.NETWORK.InteractOnClick` | `Il2Cpp.CableSpinner::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.InteractOnClick |
| `greg.NETWORK.InteractOnClick` | `Il2Cpp.NetworkSwitch::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.InteractOnClick |
| `greg.NETWORK.InteractOnClick` | `Il2Cpp.SFPBox::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.InteractOnClick |
| `greg.NETWORK.InteractOnClick` | `Il2Cpp.SFPModule::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.InteractOnClick |
| `greg.NETWORK.InteractOnHover` | `Il2Cpp.CableLink::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.InteractOnHover |
| `greg.NETWORK.InteractOnHover` | `Il2Cpp.NetworkSwitch::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.InteractOnHover |
| `greg.NETWORK.InteractOnHover` | `Il2Cpp.SFPBox::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.InteractOnHover |
| `greg.NETWORK.InteractOnHover` | `Il2Cpp.SFPModule::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.InteractOnHover |
| `greg.NETWORK.IsAllowedToDoSecondAction` | `Il2Cpp.CableLink::IsAllowedToDoSecondAction()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.IsAllowedToDoSecondAction |
| `greg.NETWORK.IsAnyCableConnected` | `Il2Cpp.NetworkSwitch::IsAnyCableConnected()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.IsAnyCableConnected |
| `greg.NETWORK.IsAnyCableConnected` | `Il2Cpp.SFPModule::IsAnyCableConnected()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.IsAnyCableConnected |
| `greg.NETWORK.IsCableComplete` | `Il2Cpp.CablePositions::IsCableComplete(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.IsCableComplete |
| `greg.NETWORK.IsCableLenghtEnough` | `Il2Cpp.CableSpinner::IsCableLenghtEnough()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.IsCableLenghtEnough |
| `greg.NETWORK.IsIpAddressDuplicate` | `Il2Cpp.NetworkMap::IsIpAddressDuplicate(string, Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.IsIpAddressDuplicate |
| `greg.NETWORK.IsPatchPanelPort` | `Il2Cpp.NetworkMap::IsPatchPanelPort(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.IsPatchPanelPort |
| `greg.NETWORK.IsVlanAllowedOnCable` | `Il2Cpp.NetworkSwitch::IsVlanAllowedOnCable(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.IsVlanAllowedOnCable |
| `greg.NETWORK.IsVlanAllowedOnPort` | `Il2Cpp.NetworkSwitch::IsVlanAllowedOnPort(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.IsVlanAllowedOnPort |
| `greg.NETWORK.IsolatedDeviceRemoved` | `Il2Cpp.NetworkMap::RemoveIsolatedDevice(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveIsolatedDevice |
| `greg.NETWORK.ItIsBroken` | `Il2Cpp.NetworkSwitch::ItIsBroken()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ItIsBroken |
| `greg.NETWORK.LACPGroupRemoved` | `Il2Cpp.NetworkMap::RemoveLACPGroup(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemoveLACPGroup |
| `greg.NETWORK.LACPGroupsSet` | `Il2Cpp.NetworkMap::SetLACPGroups(Dictionary<int, NetworkMap.LACPGroup>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.SetLACPGroups |
| `greg.NETWORK.LACPRemoved` | `Il2Cpp.NetworkSwitchConfiguration::RemoveLACP()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.RemoveLACP |
| `greg.NETWORK.LabelActionOnClick` | `Il2Cpp.CableLink::LabelActionOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.LabelActionOnClick |
| `greg.NETWORK.LastPositionRemoved` | `Il2Cpp.CablePositions::RemoveLastPosition(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.RemoveLastPosition |
| `greg.NETWORK.LowerAmountOfCable` | `Il2Cpp.CableSpinner::LowerAmountOfCable(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.LowerAmountOfCable |
| `greg.NETWORK.NormalizeDeviceKey` | `Il2Cpp.NetworkSwitchConfiguration::NormalizeDeviceKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.NormalizeDeviceKey |
| `greg.NETWORK.ObjectDropped` | `Il2Cpp.CableSpinner::DropObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.DropObject |
| `greg.NETWORK.OnCreate` | `Il2Cpp.PacketSpawnerSystem::OnCreate(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.OnCreate |
| `greg.NETWORK.OnCreateForCompiler` | `Il2Cpp.PacketSpawnerSystem::OnCreateForCompiler(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.OnCreateForCompiler |
| `greg.NETWORK.OnDestroy` | `Il2Cpp.NetworkSwitch::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.OnDestroy |
| `greg.NETWORK.OnDestroy` | `Il2Cpp.SFPModule::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.OnDestroy |
| `greg.NETWORK.OnEndEditingInputText` | `Il2Cpp.NetworkSwitchConfiguration::OnEndEditingInputText(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.OnEndEditingInputText |
| `greg.NETWORK.OnHoverOver` | `Il2Cpp.CableLink::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.OnHoverOver |
| `greg.NETWORK.OnUpdate` | `Il2Cpp.PacketSpawnerSystem::OnUpdate(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.OnUpdate |
| `greg.NETWORK.OpenConfig` | `Il2Cpp.NetworkSwitchConfiguration::OpenConfig(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.OpenConfig |
| `greg.NETWORK.ParentTheObjectWithDelay` | `Il2Cpp.SFPBox::ParentTheObjectWithDelay(Transform, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.ParentTheObjectWithDelay |
| `greg.NETWORK.PositionRemoved` | `Il2Cpp.CablePositions::RemovePosition(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.RemovePosition |
| `greg.NETWORK.PowerButton` | `Il2Cpp.NetworkSwitch::PowerButton(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.PowerButton |
| `greg.NETWORK.PowerLightMaterialSet` | `Il2Cpp.NetworkSwitch::SetPowerLightMaterial(Material)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.SetPowerLightMaterial |
| `greg.NETWORK.PrintNetworkMap` | `Il2Cpp.NetworkMap::PrintNetworkMap()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.PrintNetworkMap |
| `greg.NETWORK.ReconnectCables` | `Il2Cpp.NetworkSwitch::ReconnectCables()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ReconnectCables |
| `greg.NETWORK.RedrawCable` | `Il2Cpp.CablePositions::RedrawCable(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CablePositions.RedrawCable |
| `greg.NETWORK.RefreshPortDisplay` | `Il2Cpp.NetworkSwitchConfiguration::RefreshPortDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.RefreshPortDisplay |
| `greg.NETWORK.RefreshVLANDisplayForSelection` | `Il2Cpp.NetworkSwitchConfiguration::RefreshVLANDisplayForSelection(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.RefreshVLANDisplayForSelection |
| `greg.NETWORK.RegisterCustomerBase` | `Il2Cpp.NetworkMap::RegisterCustomerBase(CustomerBase)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RegisterCustomerBase |
| `greg.NETWORK.RegisterServer` | `Il2Cpp.NetworkMap::RegisterServer(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RegisterServer |
| `greg.NETWORK.RegisterSwitch` | `Il2Cpp.NetworkMap::RegisterSwitch(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RegisterSwitch |
| `greg.NETWORK.RemapDeviceId` | `Il2Cpp.NetworkMap::RemapDeviceId(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.RemapDeviceId |
| `greg.NETWORK.ResolveRemoteDevice` | `Il2Cpp.NetworkSwitchConfiguration::ResolveRemoteDevice(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ResolveRemoteDevice |
| `greg.NETWORK.ResolveThroughPatchPanel` | `Il2Cpp.NetworkMap::ResolveThroughPatchPanel(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.ResolveThroughPatchPanel |
| `greg.NETWORK.ReturnSFPDirectly` | `Il2Cpp.SFPBox::ReturnSFPDirectly(SFPModule)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.ReturnSFPDirectly |
| `greg.NETWORK.SFPFromBoxRemoved` | `Il2Cpp.SFPBox::RemoveSFPFromBox(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.RemoveSFPFromBox |
| `greg.NETWORK.SFPRemoved` | `Il2Cpp.CableLink::RemoveSFP()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.RemoveSFP |
| `greg.NETWORK.SFPsFromSaveLoaded` | `Il2Cpp.SFPBox::LoadSFPsFromSave()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.LoadSFPsFromSave |
| `greg.NETWORK.SavedColorLoaded` | `Il2Cpp.CableSpinner::LoadSavedColor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.LoadSavedColor |
| `greg.NETWORK.ScreenUIChanged` | `Il2Cpp.NetworkSwitch::UpdateScreenUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.UpdateScreenUI |
| `greg.NETWORK.SecondActionOnClick` | `Il2Cpp.CableLink::SecondActionOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableLink.SecondActionOnClick |
| `greg.NETWORK.SlideIntoPort` | `Il2Cpp.SFPModule::SlideIntoPort(Transform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPModule.SlideIntoPort |
| `greg.NETWORK.SwitchConnectionAdded` | `Il2Cpp.NetworkMap::AddSwitchConnection(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap.AddSwitchConnection |
| `greg.NETWORK.SwitchInsertedInRack` | `Il2Cpp.NetworkSwitch::SwitchInsertedInRack(SwitchSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.SwitchInsertedInRack |
| `greg.NETWORK.TakeSFPFromBox` | `Il2Cpp.SFPBox::TakeSFPFromBox()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SFPBox.TakeSFPFromBox |
| `greg.NETWORK.TextChanged` | `Il2Cpp.CableSpinner::UpdateText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CableSpinner.UpdateText |
| `greg.NETWORK.TickTimer` | `Il2Cpp.NetworkSwitch::TickTimer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.TickTimer |
| `greg.NETWORK.ToggleVLANMulti` | `Il2Cpp.NetworkSwitchConfiguration::ToggleVLANMulti(List<int>, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.ToggleVLANMulti |
| `greg.NETWORK.TraversePatchPanels` | `Il2Cpp.NetworkSwitchConfiguration::TraversePatchPanels(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration.TraversePatchPanels |
| `greg.NETWORK.TurnOffCommonFunctions` | `Il2Cpp.NetworkSwitch::TurnOffCommonFunctions()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.TurnOffCommonFunctions |
| `greg.NETWORK.TurnOnCommonFunction` | `Il2Cpp.NetworkSwitch::TurnOnCommonFunction()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.TurnOnCommonFunction |
| `greg.NETWORK.ValidateRackPosition` | `Il2Cpp.NetworkSwitch::ValidateRackPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.ValidateRackPosition |
| `greg.NETWORK.VlanAllowedSet` | `Il2Cpp.NetworkSwitch::SetVlanAllowed(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.SetVlanAllowed |
| `greg.NETWORK.VlanDisallowedSet` | `Il2Cpp.NetworkSwitch::SetVlanDisallowed(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch.SetVlanDisallowed |
| `greg.NETWORK._ButtonEditLabel_b__31_0` | `Il2Cpp.NetworkSwitchConfiguration::_ButtonEditLabel_b__31_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitchConfiguration._ButtonEditLabel_b__31_0 |
| `greg.NETWORK._PrintNetworkMap_b__42_0` | `Il2Cpp.NetworkMap::_PrintNetworkMap_b__42_0(NetworkMap.Device)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap._PrintNetworkMap_b__42_0 |
| `greg.NETWORK._PrintNetworkMap_b__42_1` | `Il2Cpp.NetworkMap::_PrintNetworkMap_b__42_1(NetworkMap.Device)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkMap._PrintNetworkMap_b__42_1 |
| `greg.NETWORK._SwitchInsertedInRack_b__26_0` | `Il2Cpp.NetworkSwitch::_SwitchInsertedInRack_b__26_0(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/NetworkSwitch._SwitchInsertedInRack_b__26_0 |
| `greg.NETWORK.__AssignQueries` | `Il2Cpp.PacketSpawnerSystem::__AssignQueries(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.__AssignQueries |
| `greg.NETWORK.__codegen__OnCreate` | `Il2Cpp.PacketSpawnerSystem::__codegen__OnCreate(System.IntPtr, System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.__codegen__OnCreate |
| `greg.NETWORK.__codegen__OnCreateForCompiler` | `Il2Cpp.PacketSpawnerSystem::__codegen__OnCreateForCompiler(System.IntPtr, System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.__codegen__OnCreateForCompiler |
| `greg.NETWORK.__codegen__OnUpdate` | `Il2Cpp.PacketSpawnerSystem::__codegen__OnUpdate(System.IntPtr, System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PacketSpawnerSystem.__codegen__OnUpdate |

### PLAYER

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.PLAYER.CallbacksAdded` | `Il2Cpp.PlayerActions::AddCallbacks(InputController.IPlayerActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.AddCallbacks |
| `greg.PLAYER.CallbacksRemoved` | `Il2Cpp.PlayerActions::RemoveCallbacks(InputController.IPlayerActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.RemoveCallbacks |
| `greg.PLAYER.CallbacksSet` | `Il2Cpp.PlayerActions::SetCallbacks(InputController.IPlayerActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.SetCallbacks |
| `greg.PLAYER.CheckFallsThroughMap` | `Il2Cpp.Player::CheckFallsThroughMap()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.CheckFallsThroughMap |
| `greg.PLAYER.ComponentInitialized` | `Il2Cpp.Player::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.Start |
| `greg.PLAYER.ComponentInitialized` | `Il2Cpp.PlayerHit::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerHit.OnEnable |
| `greg.PLAYER.ComponentInitialized` | `Il2Cpp.PlayerManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.Awake |
| `greg.PLAYER.ComponentInitialized` | `Il2Cpp.PlayerManager::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.Start |
| `greg.PLAYER.ConfinedCursorforUI` | `Il2Cpp.PlayerManager::ConfinedCursorforUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.ConfinedCursorforUI |
| `greg.PLAYER.DefaultActionEffect` | `Il2Cpp.PlayerManager::DefaultActionEffect(Vector3, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.DefaultActionEffect |
| `greg.PLAYER.Disable` | `Il2Cpp.PlayerActions::Disable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.Disable |
| `greg.PLAYER.DroppedAllItems` | `Il2Cpp.Player::DropAllItems()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.DropAllItems |
| `greg.PLAYER.Enable` | `Il2Cpp.PlayerActions::Enable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.Enable |
| `greg.PLAYER.GainIOPSEffect` | `Il2Cpp.PlayerManager::GainIOPSEffect()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.GainIOPSEffect |
| `greg.PLAYER.Get` | `Il2Cpp.PlayerActions::Get()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.Get |
| `greg.PLAYER.InputActionMap` | `Il2Cpp.PlayerActions::InputActionMap(InputController.PlayerActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.InputActionMap |
| `greg.PLAYER.Loaded` | `Il2Cpp.Player::LoadPlayer(PlayerData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.LoadPlayer |
| `greg.PLAYER.LockedCursorForPlayerMovement` | `Il2Cpp.PlayerManager::LockedCursorForPlayerMovement()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.LockedCursorForPlayerMovement |
| `greg.PLAYER.MoneyChanged` | `Il2Cpp.Player::UpdateCoin(float, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.UpdateCoin |
| `greg.PLAYER.PlayerStopMovement` | `Il2Cpp.PlayerManager::PlayerStopMovement()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.PlayerStopMovement |
| `greg.PLAYER.ReputationChanged` | `Il2Cpp.Player::UpdateReputation(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.UpdateReputation |
| `greg.PLAYER.TurnOnCharacterControllerDelayed` | `Il2Cpp.Player::TurnOnCharacterControllerDelayed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.TurnOnCharacterControllerDelayed |
| `greg.PLAYER.UnregisterCallbacks` | `Il2Cpp.PlayerActions::UnregisterCallbacks(InputController.IPlayerActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerActions.UnregisterCallbacks |
| `greg.PLAYER.WaitForActionToFinish` | `Il2Cpp.PlayerManager::WaitForActionToFinish(Vector3, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PlayerManager.WaitForActionToFinish |
| `greg.PLAYER.Warped` | `Il2Cpp.Player::WarpPlayer(Vector3, Quaternion)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.WarpPlayer |
| `greg.PLAYER.XpChanged` | `Il2Cpp.Player::UpdateXP(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Player.UpdateXP |

### RACK

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.RACK.ApplyMaterialToLODs` | `Il2Cpp.RackMount::ApplyMaterialToLODs(GameObject, Material)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.ApplyMaterialToLODs |
| `greg.RACK.AudioVolumeChanged` | `Il2Cpp.Rack::UpdateAudioVolume()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.UpdateAudioVolume |
| `greg.RACK.ButtonDisablePositionsInRack` | `Il2Cpp.Rack::ButtonDisablePositionsInRack()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.ButtonDisablePositionsInRack |
| `greg.RACK.ButtonUnmountRack` | `Il2Cpp.Rack::ButtonUnmountRack()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.ButtonUnmountRack |
| `greg.RACK.CheatInsertRack` | `Il2Cpp.RackMount::CheatInsertRack(GameObject, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.CheatInsertRack |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.Rack::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.Awake |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.Rack::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.Start |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.RackAudioCuller::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.Awake |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.RackAudioCuller::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.Start |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.RackDoor::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackDoor.Awake |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.RackMount::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.Awake |
| `greg.RACK.ComponentInitialized` | `Il2Cpp.RackPosition::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.Awake |
| `greg.RACK.CullLoop` | `Il2Cpp.RackAudioCuller::CullLoop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.CullLoop |
| `greg.RACK.DelayedTrigger` | `Il2Cpp.RackDoor::DelayedTrigger()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackDoor.DelayedTrigger |
| `greg.RACK.DisablePositionsButtonMaterialSet` | `Il2Cpp.Rack::SetDisablePositionsButtonMaterial(Material)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.SetDisablePositionsButtonMaterial |
| `greg.RACK.GetByUID` | `Il2Cpp.RackPosition::GetByUID(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.GetByUID |
| `greg.RACK.InitializeLoadedRack` | `Il2Cpp.Rack::InitializeLoadedRack(Il2CppStructArray<int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.InitializeLoadedRack |
| `greg.RACK.InsertItemInRack` | `Il2Cpp.RackPosition::InsertItemInRack()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.InsertItemInRack |
| `greg.RACK.InstantiateRack` | `Il2Cpp.RackMount::InstantiateRack(InteractObjectData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.InstantiateRack |
| `greg.RACK.InteractOnClick` | `Il2Cpp.RackDoor::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackDoor.InteractOnClick |
| `greg.RACK.InteractOnClick` | `Il2Cpp.RackMount::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.InteractOnClick |
| `greg.RACK.InteractOnClick` | `Il2Cpp.RackPosition::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.InteractOnClick |
| `greg.RACK.InteractOnHover` | `Il2Cpp.RackDoor::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackDoor.InteractOnHover |
| `greg.RACK.InteractOnHover` | `Il2Cpp.RackMount::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.InteractOnHover |
| `greg.RACK.InteractOnHover` | `Il2Cpp.RackPosition::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.InteractOnHover |
| `greg.RACK.IsAllowedItem` | `Il2Cpp.RackPosition::IsAllowedItem(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.IsAllowedItem |
| `greg.RACK.IsPositionAvailable` | `Il2Cpp.Rack::IsPositionAvailable(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.IsPositionAvailable |
| `greg.RACK.MarkPositionAsUnused` | `Il2Cpp.Rack::MarkPositionAsUnused(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.MarkPositionAsUnused |
| `greg.RACK.MarkPositionAsUsed` | `Il2Cpp.Rack::MarkPositionAsUsed(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.MarkPositionAsUsed |
| `greg.RACK.OnDestroy` | `Il2Cpp.Rack::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.OnDestroy |
| `greg.RACK.OnDestroy` | `Il2Cpp.RackAudioCuller::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.OnDestroy |
| `greg.RACK.OnDestroy` | `Il2Cpp.RackMount::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.OnDestroy |
| `greg.RACK.OnDestroy` | `Il2Cpp.RackPosition::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.OnDestroy |
| `greg.RACK.OnHoverOver` | `Il2Cpp.RackDoor::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackDoor.OnHoverOver |
| `greg.RACK.OnHoverOver` | `Il2Cpp.RackMount::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.OnHoverOver |
| `greg.RACK.OnHoverOver` | `Il2Cpp.RackPosition::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.OnHoverOver |
| `greg.RACK.OnLoad` | `Il2Cpp.Rack::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.OnLoad |
| `greg.RACK.OnLoad` | `Il2Cpp.RackMount::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.OnLoad |
| `greg.RACK.RackInstalled` | `Il2Cpp.RackMount::InstallRack(bool, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackMount.InstallRack |
| `greg.RACK.Register` | `Il2Cpp.RackAudioCuller::Register(Rack)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.Register |
| `greg.RACK.SecondActionOnClick` | `Il2Cpp.RackPosition::SecondActionOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.SecondActionOnClick |
| `greg.RACK.UIDSet` | `Il2Cpp.RackPosition::SetUID(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.SetUID |
| `greg.RACK.UnmountRack` | `Il2Cpp.Rack::UnmountRack()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Rack.UnmountRack |
| `greg.RACK.Unregister` | `Il2Cpp.RackAudioCuller::Unregister(Rack)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackAudioCuller.Unregister |
| `greg.RACK.UsedSet` | `Il2Cpp.RackPosition::SetUsed(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RackPosition.SetUsed |

### SERVER

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.SERVER.AppIDChanged` | `Il2Cpp.Server::UpdateAppID(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.UpdateAppID |
| `greg.SERVER.AppendEolTime` | `Il2Cpp.Server::AppendEolTime(StringBuilder, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.AppendEolTime |
| `greg.SERVER.ButtonClickChangeCustomer` | `Il2Cpp.Server::ButtonClickChangeCustomer(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ButtonClickChangeCustomer |
| `greg.SERVER.ButtonClickChangeIP` | `Il2Cpp.Server::ButtonClickChangeIP()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ButtonClickChangeIP |
| `greg.SERVER.ClearErrorSign` | `Il2Cpp.Server::ClearErrorSign()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ClearErrorSign |
| `greg.SERVER.ClearWarningSign` | `Il2Cpp.Server::ClearWarningSign(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ClearWarningSign |
| `greg.SERVER.ComponentInitialized` | `Il2Cpp.Server::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.Awake |
| `greg.SERVER.ComponentInitialized` | `Il2Cpp.Server::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.Start |
| `greg.SERVER.CustomerChanged` | `Il2Cpp.Server::UpdateCustomer(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.UpdateCustomer |
| `greg.SERVER.DeviceRepaired` | `Il2Cpp.Server::RepairDevice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.RepairDevice |
| `greg.SERVER.GenerateUniqueServerId` | `Il2Cpp.Server::GenerateUniqueServerId()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.GenerateUniqueServerId |
| `greg.SERVER.GetCustomerID` | `Il2Cpp.Server::GetCustomerID()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.GetCustomerID |
| `greg.SERVER.GetNextCustomerID` | `Il2Cpp.Server::GetNextCustomerID(int, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.GetNextCustomerID |
| `greg.SERVER.IPSet` | `Il2Cpp.Server::SetIP(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.SetIP |
| `greg.SERVER.InteractOnClick` | `Il2Cpp.Server::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.InteractOnClick |
| `greg.SERVER.InteractOnHover` | `Il2Cpp.Server::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.InteractOnHover |
| `greg.SERVER.IsAnyCableConnected` | `Il2Cpp.Server::IsAnyCableConnected()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.IsAnyCableConnected |
| `greg.SERVER.ItIsBroken` | `Il2Cpp.Server::ItIsBroken()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ItIsBroken |
| `greg.SERVER.OnDestroy` | `Il2Cpp.Server::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.OnDestroy |
| `greg.SERVER.OnLoadingComplete` | `Il2Cpp.Server::OnLoadingComplete()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.OnLoadingComplete |
| `greg.SERVER.OnLoadingStarted` | `Il2Cpp.Server::OnLoadingStarted()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.OnLoadingStarted |
| `greg.SERVER.PowerButton` | `Il2Cpp.Server::PowerButton(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.PowerButton |
| `greg.SERVER.PowerLightMaterialSet` | `Il2Cpp.Server::SetPowerLightMaterial(Material)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.SetPowerLightMaterial |
| `greg.SERVER.RegisterLink` | `Il2Cpp.Server::RegisterLink(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.RegisterLink |
| `greg.SERVER.ServerInsertedInRack` | `Il2Cpp.Server::ServerInsertedInRack(ServerSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ServerInsertedInRack |
| `greg.SERVER.ServerScreenUIChanged` | `Il2Cpp.Server::UpdateServerScreenUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.UpdateServerScreenUI |
| `greg.SERVER.TickTimer` | `Il2Cpp.Server::TickTimer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.TickTimer |
| `greg.SERVER.TurnOffCommonFunctions` | `Il2Cpp.Server::TurnOffCommonFunctions()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.TurnOffCommonFunctions |
| `greg.SERVER.TurnOnCommonFunction` | `Il2Cpp.Server::TurnOnCommonFunction()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.TurnOnCommonFunction |
| `greg.SERVER.UnregisterLink` | `Il2Cpp.Server::UnregisterLink(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.UnregisterLink |
| `greg.SERVER.ValidateRackPosition` | `Il2Cpp.Server::ValidateRackPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server.ValidateRackPosition |
| `greg.SERVER._ServerInsertedInRack_b__39_0` | `Il2Cpp.Server::_ServerInsertedInRack_b__39_0(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server._ServerInsertedInRack_b__39_0 |
| `greg.SERVER._ValidateRackPosition_b__53_0` | `Il2Cpp.Server::_ValidateRackPosition_b__53_0(RackPosition)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Server._ValidateRackPosition_b__53_0 |

### SYSTEM

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.SYSTEM.1Fired` | `Il2Cpp.viperInput::Fire1()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.Fire1 |
| `greg.SYSTEM.AButtonDown` | `Il2Cpp.viperInput::AButtonDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.AButtonDown |
| `greg.SYSTEM.AButtonUp` | `Il2Cpp.viperInput::AButtonUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.AButtonUp |
| `greg.SYSTEM.AccentCharClick` | `Il2Cpp.OSK_AccentConsole::AccentCharClick(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.AccentCharClick |
| `greg.SYSTEM.AccentMapLoaded` | `Il2Cpp.OSK_AccentConsole::LoadAccentMap(OSK_AccentAssetObj)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.LoadAccentMap |
| `greg.SYSTEM.AcceptPhysicalKeyboard` | `Il2Cpp.OSK_Keyboard::AcceptPhysicalKeyboard(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AcceptPhysicalKeyboard |
| `greg.SYSTEM.ActionInHand` | `Il2Cpp.UsableObject::ActionInHand()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.ActionInHand |
| `greg.SYSTEM.Activate` | `Il2Cpp.OSK_GamepadHelper::Activate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.Activate |
| `greg.SYSTEM.ActivateSpawnerOnCable` | `Il2Cpp.WaypointInitializationSystem::ActivateSpawnerOnCable(Entity, float, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.ActivateSpawnerOnCable |
| `greg.SYSTEM.AgentReachTarget` | `Il2Cpp.AICharacterControl::AgentReachTarget()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.AgentReachTarget |
| `greg.SYSTEM.AllBindingOverridesLoaded` | `Il2Cpp.InputManager::LoadAllBindingOverrides()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.LoadAllBindingOverrides |
| `greg.SYSTEM.AllModsLoaded` | `Il2Cpp.ModLoader::LoadAllMods()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadAllMods |
| `greg.SYSTEM.AnimSit` | `Il2Cpp.AICharacterControl::AnimSit(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.AnimSit |
| `greg.SYSTEM.AnimateProperties` | `Il2Cpp.ShaderPropAnimator::AnimateProperties()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/ShaderPropAnimator.AnimateProperties |
| `greg.SYSTEM.AnimateVertexColors` | `Il2Cpp.VertexColorCycler::AnimateVertexColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexColorCycler.AnimateVertexColors |
| `greg.SYSTEM.AnimateVertexColors` | `Il2Cpp.VertexJitter::AnimateVertexColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.AnimateVertexColors |
| `greg.SYSTEM.AnimateVertexColors` | `Il2Cpp.VertexShakeA::AnimateVertexColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.AnimateVertexColors |
| `greg.SYSTEM.AnimateVertexColors` | `Il2Cpp.VertexShakeB::AnimateVertexColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.AnimateVertexColors |
| `greg.SYSTEM.AnimateVertexColors` | `Il2Cpp.VertexZoom::AnimateVertexColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.AnimateVertexColors |
| `greg.SYSTEM.AnimatorChanged` | `Il2Cpp.ThirdPersonCharacter::UpdateAnimator(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.UpdateAnimator |
| `greg.SYSTEM.AnyPhysicalKey` | `Il2Cpp.viperInput::AnyPhysicalKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.AnyPhysicalKey |
| `greg.SYSTEM.ApplyExtraTurnRotation` | `Il2Cpp.ThirdPersonCharacter::ApplyExtraTurnRotation()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.ApplyExtraTurnRotation |
| `greg.SYSTEM.AreEndPointsValid` | `Il2Cpp.Rope::AreEndPointsValid()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.AreEndPointsValid |
| `greg.SYSTEM.Assign` | `Il2Cpp.OSK_Key::Assign(OSK_KeyCode, OSK_KEY_TYPES, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Assign |
| `greg.SYSTEM.Assign` | `Il2Cpp.OSK_UI_Key::Assign(OSK_KeyCode, OSK_KEY_TYPES, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Assign |
| `greg.SYSTEM.AssignSpecialAction` | `Il2Cpp.OSK_Key::AssignSpecialAction(Il2CppSystem.Action<string, OSK_Receiver>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.AssignSpecialAction |
| `greg.SYSTEM.AssignSpecialAction` | `Il2Cpp.OSK_UI_Key::AssignSpecialAction(Il2CppSystem.Action<string, OSK_Receiver>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.AssignSpecialAction |
| `greg.SYSTEM.AutoCorrectLayout` | `Il2Cpp.OSK_Keyboard::AutoCorrectLayout()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AutoCorrectLayout |
| `greg.SYSTEM.AutoCorrectLayout` | `Il2Cpp.OSK_Keymap::AutoCorrectLayout(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.AutoCorrectLayout |
| `greg.SYSTEM.AutoCorrectRecursive` | `Il2Cpp.OSK_Keymap::AutoCorrectRecursive(string, List<string>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.AutoCorrectRecursive |
| `greg.SYSTEM.AutoCorrectRow` | `Il2Cpp.OSK_Keymap::AutoCorrectRow(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.AutoCorrectRow |
| `greg.SYSTEM.AutoFindKeyboard` | `Il2Cpp.OSK_Background::AutoFindKeyboard()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Background.AutoFindKeyboard |
| `greg.SYSTEM.AutoRepairModeSet` | `Il2Cpp.CommandCenter::SetAutoRepairMode(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.SetAutoRepairMode |
| `greg.SYSTEM.AutoRepairRoutine` | `Il2Cpp.CommandCenter::AutoRepairRoutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.AutoRepairRoutine |
| `greg.SYSTEM.AutoSaveCoroutine` | `Il2Cpp.MainGameManager::AutoSaveCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.AutoSaveCoroutine |
| `greg.SYSTEM.AutoSaveEnabledSet` | `Il2Cpp.MainGameManager::SetAutoSaveEnabled(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.SetAutoSaveEnabled |
| `greg.SYSTEM.AutoSaveIntervalSet` | `Il2Cpp.MainGameManager::SetAutoSaveInterval(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.SetAutoSaveInterval |
| `greg.SYSTEM.BButtonDown` | `Il2Cpp.viperInput::BButtonDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.BButtonDown |
| `greg.SYSTEM.BButtonUp` | `Il2Cpp.viperInput::BButtonUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.BButtonUp |
| `greg.SYSTEM.BackScale` | `Il2Cpp.OSK_Key::BackScale(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.BackScale |
| `greg.SYSTEM.BackScale` | `Il2Cpp.OSK_UI_Key::BackScale(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.BackScale |
| `greg.SYSTEM.Backspace` | `Il2Cpp.OSK_Receiver::Backspace()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Backspace |
| `greg.SYSTEM.Backspace` | `Il2Cpp.OSK_UI_InputReceiver::Backspace()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Backspace |
| `greg.SYSTEM.Bake` | `Il2Cpp.Baker::Bake(PacketSpawnerAuthoring)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Baker.Bake |
| `greg.SYSTEM.BaseCharacter` | `Il2Cpp.OSK_Keymap::BaseCharacter(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.BaseCharacter |
| `greg.SYSTEM.BaseKeySet` | `Il2Cpp.OSK_MiniKeyboard::SetBaseKey(GameObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.SetBaseKey |
| `greg.SYSTEM.BindingOverrideLoaded` | `Il2Cpp.InputManager::LoadBindingOverride(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.LoadBindingOverride |
| `greg.SYSTEM.BindingOverrideSaved` | `Il2Cpp.InputManager::SaveBindingOverride(InputAction)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.SaveBindingOverride |
| `greg.SYSTEM.BkColorSet` | `Il2Cpp.OSK_UI_Key::SetBkColor(Color, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.SetBkColor |
| `greg.SYSTEM.BlinkCoroutine` | `Il2Cpp.OSK_Cursor::BlinkCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.BlinkCoroutine |
| `greg.SYSTEM.BlinkCoroutine` | `Il2Cpp.OSK_UI_Cursor::BlinkCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.BlinkCoroutine |
| `greg.SYSTEM.BoxIl2CppObject` | `Il2Cpp._PrivateImplementationDetails_::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_PrivateImplementationDetails_.BoxIl2CppObject |
| `greg.SYSTEM.BoxIl2CppObject` | `Il2Cpp.TypeHandle::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TypeHandle.BoxIl2CppObject |
| `greg.SYSTEM.BoxIl2CppObject` | `Il2Cpp.Enumerator::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.BoxIl2CppObject |
| `greg.SYSTEM.BoxIl2CppObject` | `Il2Cpp.VertexJitter::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.BoxIl2CppObject |
| `greg.SYSTEM.BoxIl2CppObject` | `Il2Cpp.AssemblyTypeRegistry::BoxIl2CppObject()` | `Postfix` | Auto-generated from IL2CPP sources: Unity/AssemblyTypeRegistry.BoxIl2CppObject |
| `greg.SYSTEM.BoxedGetHashCode` | `Il2Cpp.AssemblyTypeRegistry::BoxedGetHashCode(Il2CppSystem.Object, int)` | `Postfix` | Auto-generated from IL2CPP sources: Unity/AssemblyTypeRegistry.BoxedGetHashCode |
| `greg.SYSTEM.BrakeAndDeacceleration` | `Il2Cpp.CarController::BrakeAndDeacceleration()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.BrakeAndDeacceleration |
| `greg.SYSTEM.ButtonA` | `Il2Cpp.OSK_Keyboard::ButtonA()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.ButtonA |
| `greg.SYSTEM.ButtonA` | `Il2Cpp.OSK_UI_Keyboard::ButtonA()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.ButtonA |
| `greg.SYSTEM.ButtonAddAllBrokenDevicesToQueue` | `Il2Cpp.AssetManagement::ButtonAddAllBrokenDevicesToQueue()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonAddAllBrokenDevicesToQueue |
| `greg.SYSTEM.ButtonAssetManagementScreen` | `Il2Cpp.ComputerShop::ButtonAssetManagementScreen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonAssetManagementScreen |
| `greg.SYSTEM.ButtonBalanceSheetScreen` | `Il2Cpp.ComputerShop::ButtonBalanceSheetScreen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonBalanceSheetScreen |
| `greg.SYSTEM.ButtonBuyItem` | `Il2Cpp.ModShopItem::ButtonBuyItem()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModShopItem.ButtonBuyItem |
| `greg.SYSTEM.ButtonBuyItem` | `Il2Cpp.ShopItem::ButtonBuyItem()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.ButtonBuyItem |
| `greg.SYSTEM.ButtonBuyWall` | `Il2Cpp.MainGameManager::ButtonBuyWall()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ButtonBuyWall |
| `greg.SYSTEM.ButtonCancel` | `Il2Cpp.ComputerShop::ButtonCancel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonCancel |
| `greg.SYSTEM.ButtonCancelBuyWall` | `Il2Cpp.MainGameManager::ButtonCancelBuyWall()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ButtonCancelBuyWall |
| `greg.SYSTEM.ButtonCancelColorPicker` | `Il2Cpp.ComputerShop::ButtonCancelColorPicker()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonCancelColorPicker |
| `greg.SYSTEM.ButtonCancelCustomerChoice` | `Il2Cpp.MainGameManager::ButtonCancelCustomerChoice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ButtonCancelCustomerChoice |
| `greg.SYSTEM.ButtonCancelInputTextOverlay` | `Il2Cpp.StaticUIElements::ButtonCancelInputTextOverlay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ButtonCancelInputTextOverlay |
| `greg.SYSTEM.ButtonCancelSendingTechnician` | `Il2Cpp.AssetManagement::ButtonCancelSendingTechnician()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonCancelSendingTechnician |
| `greg.SYSTEM.ButtonCheckOut` | `Il2Cpp.ComputerShop::ButtonCheckOut()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonCheckOut |
| `greg.SYSTEM.ButtonChosenColor` | `Il2Cpp.ComputerShop::ButtonChosenColor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonChosenColor |
| `greg.SYSTEM.ButtonClear` | `Il2Cpp.ComputerShop::ButtonClear()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonClear |
| `greg.SYSTEM.ButtonClearAllWarnings` | `Il2Cpp.AssetManagement::ButtonClearAllWarnings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonClearAllWarnings |
| `greg.SYSTEM.ButtonClearWarningSign` | `Il2Cpp.AssetManagementDeviceLine::ButtonClearWarningSign()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagementDeviceLine.ButtonClearWarningSign |
| `greg.SYSTEM.ButtonConfirmSendingTechnician` | `Il2Cpp.AssetManagement::ButtonConfirmSendingTechnician()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonConfirmSendingTechnician |
| `greg.SYSTEM.ButtonCustomerChosen` | `Il2Cpp.MainGameManager::ButtonCustomerChosen(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ButtonCustomerChosen |
| `greg.SYSTEM.ButtonDowngradeCommandCenter` | `Il2Cpp.CommandCenter::ButtonDowngradeCommandCenter()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.ButtonDowngradeCommandCenter |
| `greg.SYSTEM.ButtonEditLabel` | `Il2Cpp.SetIP::ButtonEditLabel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ButtonEditLabel |
| `greg.SYSTEM.ButtonFilterAll` | `Il2Cpp.AssetManagement::ButtonFilterAll()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterAll |
| `greg.SYSTEM.ButtonFilterBroken` | `Il2Cpp.AssetManagement::ButtonFilterBroken()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterBroken |
| `greg.SYSTEM.ButtonFilterEOL` | `Il2Cpp.AssetManagement::ButtonFilterEOL()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterEOL |
| `greg.SYSTEM.ButtonFilterOff` | `Il2Cpp.AssetManagement::ButtonFilterOff()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterOff |
| `greg.SYSTEM.ButtonFilterServers` | `Il2Cpp.AssetManagement::ButtonFilterServers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterServers |
| `greg.SYSTEM.ButtonFilterSwitches` | `Il2Cpp.AssetManagement::ButtonFilterSwitches()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.ButtonFilterSwitches |
| `greg.SYSTEM.ButtonHideShowHint` | `Il2Cpp.SetIP::ButtonHideShowHint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ButtonHideShowHint |
| `greg.SYSTEM.ButtonHireScreen` | `Il2Cpp.ComputerShop::ButtonHireScreen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonHireScreen |
| `greg.SYSTEM.ButtonNetworkMap` | `Il2Cpp.ComputerShop::ButtonNetworkMap()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonNetworkMap |
| `greg.SYSTEM.ButtonReturnMainScreen` | `Il2Cpp.ComputerShop::ButtonReturnMainScreen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonReturnMainScreen |
| `greg.SYSTEM.ButtonSaveInputTextOverlay` | `Il2Cpp.StaticUIElements::ButtonSaveInputTextOverlay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ButtonSaveInputTextOverlay |
| `greg.SYSTEM.ButtonSendTechnician` | `Il2Cpp.AssetManagementDeviceLine::ButtonSendTechnician()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagementDeviceLine.ButtonSendTechnician |
| `greg.SYSTEM.ButtonShopScreen` | `Il2Cpp.ComputerShop::ButtonShopScreen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ButtonShopScreen |
| `greg.SYSTEM.ButtonUpgradeCommandCenter` | `Il2Cpp.CommandCenter::ButtonUpgradeCommandCenter()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.ButtonUpgradeCommandCenter |
| `greg.SYSTEM.CableInfoChanged` | `Il2Cpp.WaypointInitializationSystem::UpdateCableInfo(int, WaypointInitializationSystem.CableInfo)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.UpdateCableInfo |
| `greg.SYSTEM.CalculateYFactorAdjustment` | `Il2Cpp.Rope::CalculateYFactorAdjustment(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.CalculateYFactorAdjustment |
| `greg.SYSTEM.CameraPositionChanged` | `Il2Cpp.FirstPersonController::UpdateCameraPosition(float)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.UpdateCameraPosition |
| `greg.SYSTEM.CapitalizeCorrectly` | `Il2Cpp.OSK_Keymap::CapitalizeCorrectly(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.CapitalizeCorrectly |
| `greg.SYSTEM.CartTotalChanged` | `Il2Cpp.ComputerShop::UpdateCartTotal()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.UpdateCartTotal |
| `greg.SYSTEM.CartUIItemRemoved` | `Il2Cpp.ComputerShop::RemoveCartUIItem(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.RemoveCartUIItem |
| `greg.SYSTEM.CellSet` | `Il2Cpp.AssetManagement::SetCell(ICell, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.SetCell |
| `greg.SYSTEM.CellSet` | `Il2Cpp.IRecyclableScrollRectDataSource::SetCell(ICell, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/IRecyclableScrollRectDataSource.SetCell |
| `greg.SYSTEM.ChangeLocalisation` | `Il2Cpp.Localisation::ChangeLocalisation(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Localisation.ChangeLocalisation |
| `greg.SYSTEM.ChangeMode` | `Il2Cpp.FlexibleColorPicker::ChangeMode(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ChangeMode |
| `greg.SYSTEM.ChangeMode` | `Il2Cpp.FlexibleColorPicker::ChangeMode(FlexibleColorPicker.MainPickingMode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ChangeMode |
| `greg.SYSTEM.ChangeText` | `Il2Cpp.LocalisedText::ChangeText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LocalisedText.ChangeText |
| `greg.SYSTEM.Changed` | `Il2Cpp.AICharacterControl::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.AutoScrollRect::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AutoScrollRect.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.FCP_SpriteMeshEditor::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_SpriteMeshEditor.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.FlexibleColorPicker::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.LeanTweenUIElement::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.MusicPlayer::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.TypeHandle::Update(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TypeHandle.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.PositionIndicator::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PositionIndicator.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.PulsatingImageColor::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.PulsatingText::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingText.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.SetIP::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.SteamManager::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.TimeController::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.UserReport::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.Rope::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.RopeMesh::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.RopeWindEffect::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.ObjectSpin::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/ObjectSpin.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.SimpleScript::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SimpleScript.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.TMP_ExampleScript_01::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_ExampleScript_01.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.TMP_FrameRateCounter::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_FrameRateCounter.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.TMP_UiFrameRateCounter::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_UiFrameRateCounter.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_AccentConsole::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_Background::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Background.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_Cursor::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_GamepadHelper::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_Key::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_Keyboard::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_KeySounds::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_KeySounds.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_MiniKeyboard::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_UI_Cursor::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_UI_CustomReceiver::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_UI_Key::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.OSK_UI_Keyboard::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Update |
| `greg.SYSTEM.Changed` | `Il2Cpp.FirstPersonController::Update()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.Update |
| `greg.SYSTEM.CheckCurrentControls` | `Il2Cpp.InputManager::CheckCurrentControls(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.CheckCurrentControls |
| `greg.SYSTEM.CheckEndPoints` | `Il2Cpp.RopeMesh::CheckEndPoints()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.CheckEndPoints |
| `greg.SYSTEM.CheckForErrors` | `Il2Cpp.TypeHandle::CheckForErrors(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TypeHandle.CheckForErrors |
| `greg.SYSTEM.CheckIfLost` | `Il2Cpp.UsableObject::CheckIfLost()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.CheckIfLost |
| `greg.SYSTEM.CidrToSubnetMask` | `Il2Cpp.SetIP::CidrToSubnetMask(int, int, int, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.CidrToSubnetMask |
| `greg.SYSTEM.ClampRotationAroundXAxis` | `Il2Cpp.MouseLook::ClampRotationAroundXAxis(Quaternion)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.ClampRotationAroundXAxis |
| `greg.SYSTEM.CleanUpShop` | `Il2Cpp.ComputerShop::CleanUpShop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.CleanUpShop |
| `greg.SYSTEM.CleanUpSystem` | `Il2Cpp.WaypointInitializationSystem::CleanUpSystem()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.CleanUpSystem |
| `greg.SYSTEM.Cleanup` | `Il2Cpp.RayLookAt::Cleanup()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.Cleanup |
| `greg.SYSTEM.ClearAllUIDs` | `Il2Cpp.ShopCartItem::ClearAllUIDs()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.ClearAllUIDs |
| `greg.SYSTEM.ClearForm` | `Il2Cpp.UserReport::ClearForm()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.ClearForm |
| `greg.SYSTEM.ClearNetworkState` | `Il2Cpp.WaypointInitializationSystem::ClearNetworkState()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.ClearNetworkState |
| `greg.SYSTEM.ClearPool` | `Il2Cpp.CarryModelPool::ClearPool()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CarryModelPool.ClearPool |
| `greg.SYSTEM.ClearReport` | `Il2Cpp.UserReport::ClearReport()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.ClearReport |
| `greg.SYSTEM.ClearSpriteNextToPointer` | `Il2Cpp.StaticUIElements::ClearSpriteNextToPointer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ClearSpriteNextToPointer |
| `greg.SYSTEM.ClearText` | `Il2Cpp.OSK_Receiver::ClearText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ClearText |
| `greg.SYSTEM.ClearText` | `Il2Cpp.OSK_UI_InputReceiver::ClearText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.ClearText |
| `greg.SYSTEM.ClearTrackingWithoutDestroying` | `Il2Cpp.ComputerShop::ClearTrackingWithoutDestroying()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.ClearTrackingWithoutDestroying |
| `greg.SYSTEM.Click` | `Il2Cpp.I_OSK_Key::Click(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.Click |
| `greg.SYSTEM.Click` | `Il2Cpp.OSK_Key::Click(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Click |
| `greg.SYSTEM.Click` | `Il2Cpp.OSK_UI_Key::Click(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Click |
| `greg.SYSTEM.ClickButtonCancel` | `Il2Cpp.SetIP::ClickButtonCancel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonCancel |
| `greg.SYSTEM.ClickButtonClear` | `Il2Cpp.SetIP::ClickButtonClear()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonClear |
| `greg.SYSTEM.ClickButtonCopy` | `Il2Cpp.SetIP::ClickButtonCopy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonCopy |
| `greg.SYSTEM.ClickButtonDel` | `Il2Cpp.SetIP::ClickButtonDel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonDel |
| `greg.SYSTEM.ClickButtonNextIP` | `Il2Cpp.SetIP::ClickButtonNextIP()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonNextIP |
| `greg.SYSTEM.ClickButtonOK` | `Il2Cpp.SetIP::ClickButtonOK()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonOK |
| `greg.SYSTEM.ClickButtonPaste` | `Il2Cpp.SetIP::ClickButtonPaste()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickButtonPaste |
| `greg.SYSTEM.ClickCoroutine` | `Il2Cpp.OSK_Key::ClickCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.ClickCoroutine |
| `greg.SYSTEM.ClickCoroutine` | `Il2Cpp.OSK_UI_Key::ClickCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.ClickCoroutine |
| `greg.SYSTEM.ClickNumber` | `Il2Cpp.SetIP::ClickNumber(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ClickNumber |
| `greg.SYSTEM.ClickSound` | `Il2Cpp.OSK_Keyboard::ClickSound(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.ClickSound |
| `greg.SYSTEM.CloseAnyCanvas` | `Il2Cpp.MainGameManager::CloseAnyCanvas(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.CloseAnyCanvas |
| `greg.SYSTEM.CloseCanvas` | `Il2Cpp.SetIP::CloseCanvas()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.CloseCanvas |
| `greg.SYSTEM.CloseGate` | `Il2Cpp.GateLever::CloseGate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.CloseGate |
| `greg.SYSTEM.CloseInteractionMenu` | `Il2Cpp.Interact::CloseInteractionMenu()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.CloseInteractionMenu |
| `greg.SYSTEM.CloseInteractionMenu` | `Il2Cpp.RayLookAt::CloseInteractionMenu()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.CloseInteractionMenu |
| `greg.SYSTEM.CloseNetworkConfigCanvas` | `Il2Cpp.MainGameManager::CloseNetworkConfigCanvas()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.CloseNetworkConfigCanvas |
| `greg.SYSTEM.CloseShop` | `Il2Cpp.ComputerShop::CloseShop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.CloseShop |
| `greg.SYSTEM.CoinsAndPrestige_TopLeftChanged` | `Il2Cpp.StaticUIElements::UpdateCoinsAndPrestige_TopLeft()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.UpdateCoinsAndPrestige_TopLeft |
| `greg.SYSTEM.ColorLoaded` | `Il2Cpp.FCP_Persistence::LoadColor(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.LoadColor |
| `greg.SYSTEM.ColorNoAlphaSet` | `Il2Cpp.FlexibleColorPicker::SetColorNoAlpha(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SetColorNoAlpha |
| `greg.SYSTEM.ColorSaved` | `Il2Cpp.FCP_Persistence::SaveColor(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.SaveColor |
| `greg.SYSTEM.ColorSet` | `Il2Cpp.FlexibleColorPicker::SetColor(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SetColor |
| `greg.SYSTEM.ColorsSet` | `Il2Cpp.OSK_Key::SetColors(Color, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.SetColors |
| `greg.SYSTEM.ColorsSet` | `Il2Cpp.OSK_UI_Key::SetColors(Color, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.SetColors |
| `greg.SYSTEM.CompleteDependencies` | `Il2Cpp.TypeHandle::CompleteDependencies(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TypeHandle.CompleteDependencies |
| `greg.SYSTEM.CompleteDependencies` | `Il2Cpp.Enumerator::CompleteDependencies(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.CompleteDependencies |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.ActionKeyHint::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.AICharacterControl::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.FCP_Persistence::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.GODMOD::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.LeanTweenUIElement::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.PulsatingImageColor::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.TimeController::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.WorldCanvasCuller::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldCanvasCuller.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.RopeMesh::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.TextConsoleSimulator::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.TMP_TextEventCheck::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.TMP_TextSelector_B::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.VertexJitter::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.VertexShakeA::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.VertexShakeB::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.VertexZoom::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.OSK_Cursor::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.OnDisable |
| `greg.SYSTEM.ComponentDisabled` | `Il2Cpp.OSK_UI_Cursor::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.OnDisable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ActionKeyHint::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AICharacterControl::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AICharacterControl::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AICharacterExpressions::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AssetManagement::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AudioManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AutoDisable::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AutoDisable.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.AutoScrollRect::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AutoScrollRect.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CarryModelPool::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CarryModelPool.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CheckIfTouchingWall::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CheckIfTouchingWall::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CommandCenter::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CommandCenterOperator::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenterOperator.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ComputerShop::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.DeviceTimerManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DeviceTimerManager.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.DeviceTimerManager::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DeviceTimerManager.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Dumpster::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Dumpster.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.EnvMapAnimator::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/EnvMapAnimator.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.EnvMapAnimator::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/EnvMapAnimator.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FCP_Persistence::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FCP_Persistence::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FlexibleColorPicker::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FlexibleColorPicker::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FootSteps::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FootSteps::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.GateLever::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.GetCurrentVersion::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GetCurrentVersion.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.GetValueFromPlayerPrefs::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GetValueFromPlayerPrefs.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.GODMOD::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.GODMOD::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.InputManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Interact::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.LeanTweenUIElement::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.LeanTweenUIElement::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Localisation::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Localisation.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.LocalisedText::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LocalisedText.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.MainGameManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.MainGameManager::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ModLoader::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ModLoader::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.MusicPlayer::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PatchPanel::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PositionIndicator::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PositionIndicator.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PulsatingImageColor::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PulsatingImageColor::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PulsatingText::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingText.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.PushTrolleyHandle::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PushTrolleyHandle.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SetIP::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ShopItem::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ShopItem::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.StaminaOverlayOnEnable::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaminaOverlayOnEnable.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.StaticUIElements::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.StaticUIElements::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SteamManager::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SteamManager::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SteamStatsOnMainMenuTop::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamStatsOnMainMenuTop.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TimeController::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TimeController::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TrolleyLoadingBay::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TrolleyLoadingBay::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.UsableObject::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.UserReport::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Wall::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.WorldCanvasCuller::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldCanvasCuller.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.WorldCanvasCuller::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldCanvasCuller.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CullDriver::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CullDriver.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.WorldObjectButton::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldObjectButton.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Rope::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.RopeMesh::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.RopeMesh::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.RopeWindEffect::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.RopeWindEffect::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.RecyclableScrollRect::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CarController::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_TextEventHandler::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark01_UGUI::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark01_UGUI.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark01::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark01.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark02::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark02.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark03::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark03.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark03::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark03.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.Benchmark04::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/Benchmark04.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CameraController::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/CameraController.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.CameraController::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/CameraController.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ObjectSpin::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/ObjectSpin.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ShaderPropAnimator::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/ShaderPropAnimator.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ShaderPropAnimator::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/ShaderPropAnimator.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SimpleScript::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SimpleScript.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SkewTextExample::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SkewTextExample.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.SkewTextExample::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SkewTextExample.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TeleType::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TeleType.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TeleType::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TeleType.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextConsoleSimulator::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextConsoleSimulator::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextConsoleSimulator::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextMeshProFloatingText::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshProFloatingText.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextMeshProFloatingText::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshProFloatingText.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextMeshSpawner::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshSpawner.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TextMeshSpawner::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshSpawner.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_ExampleScript_01::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_ExampleScript_01.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_FrameRateCounter::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_FrameRateCounter.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_FrameRateCounter::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_FrameRateCounter.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_TextEventCheck::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_TextSelector_A::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_A.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_TextSelector_B::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_TextSelector_B::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_UiFrameRateCounter::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_UiFrameRateCounter.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMP_UiFrameRateCounter::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_UiFrameRateCounter.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.TMPro_InstructionOverlay::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMPro_InstructionOverlay.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexColorCycler::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexColorCycler.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexColorCycler::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexColorCycler.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexJitter::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexJitter::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexJitter::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeA::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeA::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeA::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeB::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeB::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexShakeB::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexZoom::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexZoom::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.VertexZoom::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.WarpTextExample::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/WarpTextExample.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.WarpTextExample::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/WarpTextExample.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_AccentConsole::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Background::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Background.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Cursor::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Cursor::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Cursor::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_GamepadHelper::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Key::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Key::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Keyboard::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Keyboard::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_KeySounds::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_KeySounds.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_MiniKeyboard::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Receiver::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Receiver::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_Settings::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Settings.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Cursor::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Cursor::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Cursor::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_CustomReceiver::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_InputReceiver::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_InputReceiver::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Key::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Keyboard::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Awake |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.OSK_UI_Keyboard::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.viperInput::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ButtonExtended::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnEnable |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ButtonExtended::Start()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.FirstPersonController::Start()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.Start |
| `greg.SYSTEM.ComponentInitialized` | `Il2Cpp.ThirdPersonCharacter::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.Awake |
| `greg.SYSTEM.ComputeStringHash` | `Il2Cpp._PrivateImplementationDetails_::ComputeStringHash(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_PrivateImplementationDetails_.ComputeStringHash |
| `greg.SYSTEM.ConfinedCursorforUI` | `Il2Cpp.InputManager::ConfinedCursorforUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.ConfinedCursorforUI |
| `greg.SYSTEM.ConsoleRemoved` | `Il2Cpp.OSK_AccentConsole::RemoveConsole()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.RemoveConsole |
| `greg.SYSTEM.ConsoleSet` | `Il2Cpp.OSK_AccentConsole::SetConsole(OSK_LongPressPacket)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.SetConsole |
| `greg.SYSTEM.Contains` | `Il2Cpp.InputController::Contains(InputAction)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.Contains |
| `greg.SYSTEM.ConvertKeyCodeToKey` | `Il2Cpp.viperInput::ConvertKeyCodeToKey(KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.ConvertKeyCodeToKey |
| `greg.SYSTEM.ConvertToLegacyAxis` | `Il2Cpp.viperInput::ConvertToLegacyAxis(AXIS_INPUT)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.ConvertToLegacyAxis |
| `greg.SYSTEM.ConvertToSplatMapCoordinate` | `Il2Cpp.TerrainDetector::ConvertToSplatMapCoordinate(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TerrainDetector.ConvertToSplatMapCoordinate |
| `greg.SYSTEM.CopyAnimationCurve` | `Il2Cpp.SkewTextExample::CopyAnimationCurve(AnimationCurve)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SkewTextExample.CopyAnimationCurve |
| `greg.SYSTEM.CopyAnimationCurve` | `Il2Cpp.WarpTextExample::CopyAnimationCurve(AnimationCurve)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/WarpTextExample.CopyAnimationCurve |
| `greg.SYSTEM.CopyDirectory` | `Il2Cpp.ModLoader::CopyDirectory(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.CopyDirectory |
| `greg.SYSTEM.CreateBackground` | `Il2Cpp.OSK_MiniKeyboard::CreateBackground()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.CreateBackground |
| `greg.SYSTEM.CreateCableWithSpawners` | `Il2Cpp.WaypointInitializationSystem::CreateCableWithSpawners(int, List<Vector3>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.CreateCableWithSpawners |
| `greg.SYSTEM.CreateCellPool` | `Il2Cpp.HorizontalRecyclingSystem::CreateCellPool()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.CreateCellPool |
| `greg.SYSTEM.CreateCellPool` | `Il2Cpp.VerticalRecyclingSystem::CreateCellPool()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.CreateCellPool |
| `greg.SYSTEM.CreateFallbackCustomer` | `Il2Cpp.MainGameManager::CreateFallbackCustomer(CustomerItem, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.CreateFallbackCustomer |
| `greg.SYSTEM.CreateJobReflectionData` | `Il2Cpp.__JobReflectionRegistrationOutput__1221673671587648887::CreateJobReflectionData()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__JobReflectionRegistrationOutput__1221673671587648887.CreateJobReflectionData |
| `greg.SYSTEM.CreateMaterial` | `Il2Cpp.ModLoader::CreateMaterial(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.CreateMaterial |
| `greg.SYSTEM.CreateShopButton` | `Il2Cpp.ModLoader::CreateShopButton(int, ShopItemConfig, Sprite)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.CreateShopButton |
| `greg.SYSTEM.CreateUserReport` | `Il2Cpp.UserReport::CreateUserReport()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.CreateUserReport |
| `greg.SYSTEM.Crouch` | `Il2Cpp.FirstPersonController::Crouch()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.Crouch |
| `greg.SYSTEM.CullRoutine` | `Il2Cpp.CullDriver::CullRoutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CullDriver.CullRoutine |
| `greg.SYSTEM.CurrentTerrainSet` | `Il2Cpp.TerrainDetector::SetCurrentTerrain(Terrain)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TerrainDetector.SetCurrentTerrain |
| `greg.SYSTEM.CurrentTimeInHours` | `Il2Cpp.TimeController::CurrentTimeInHours()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.CurrentTimeInHours |
| `greg.SYSTEM.Cursor` | `Il2Cpp.I_OSK_Cursor::Cursor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Cursor.Cursor |
| `greg.SYSTEM.Cursor` | `Il2Cpp.OSK_Cursor::Cursor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.Cursor |
| `greg.SYSTEM.Cursor` | `Il2Cpp.OSK_UI_Cursor::Cursor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.Cursor |
| `greg.SYSTEM.CursorLockChanged` | `Il2Cpp.MouseLook::UpdateCursorLock()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.UpdateCursorLock |
| `greg.SYSTEM.CursorLockSet` | `Il2Cpp.MouseLook::SetCursorLock(bool)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.SetCursorLock |
| `greg.SYSTEM.CustomKey` | `Il2Cpp.ActionKeyHint::CustomKey(InputAction, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.CustomKey |
| `greg.SYSTEM.CustomKeyHintRemoved` | `Il2Cpp.StaticUIElements::RemoveCustomKeyHint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.RemoveCustomKeyHint |
| `greg.SYSTEM.DataFileLoaded` | `Il2Cpp.FCP_Persistence::LoadDataFile()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.LoadDataFile |
| `greg.SYSTEM.DataFileSaved` | `Il2Cpp.FCP_Persistence::SaveDataFile()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.SaveDataFile |
| `greg.SYSTEM.DeActivate` | `Il2Cpp.OSK_GamepadHelper::DeActivate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.DeActivate |
| `greg.SYSTEM.Del` | `Il2Cpp.OSK_Receiver::Del()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Del |
| `greg.SYSTEM.Del` | `Il2Cpp.OSK_UI_InputReceiver::Del()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Del |
| `greg.SYSTEM.DelayedGenerateMesh` | `Il2Cpp.RopeMesh::DelayedGenerateMesh()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.DelayedGenerateMesh |
| `greg.SYSTEM.DelayedLoad` | `Il2Cpp.GODMOD::DelayedLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.DelayedLoad |
| `greg.SYSTEM.DelayedOverlapCheck` | `Il2Cpp.CheckIfTouchingWall::DelayedOverlapCheck()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.DelayedOverlapCheck |
| `greg.SYSTEM.DelayedUpdateUI` | `Il2Cpp.ActionKeyHint::DelayedUpdateUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.DelayedUpdateUI |
| `greg.SYSTEM.Deselect` | `Il2Cpp.OSK_Receiver::Deselect()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Deselect |
| `greg.SYSTEM.Deselect` | `Il2Cpp.OSK_UI_CustomReceiver::Deselect()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.Deselect |
| `greg.SYSTEM.DestroyAllSpawnedItems` | `Il2Cpp.ComputerShop::DestroyAllSpawnedItems()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.DestroyAllSpawnedItems |
| `greg.SYSTEM.DestroyErrorWarningSign` | `Il2Cpp.StaticUIElements::DestroyErrorWarningSign(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.DestroyErrorWarningSign |
| `greg.SYSTEM.DestroyOperatorsForLevel` | `Il2Cpp.CommandCenter::DestroyOperatorsForLevel(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.DestroyOperatorsForLevel |
| `greg.SYSTEM.DiacriticAdded` | `Il2Cpp.OSK_Keymap::AddDiacritic(char, [Optional] Il2CppStructArray<char>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.AddDiacritic |
| `greg.SYSTEM.DiacriticAdded` | `Il2Cpp.OSK_Keymap::AddDiacritic(char, params char[])` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.AddDiacritic |
| `greg.SYSTEM.Dir` | `Il2Cpp.OSK_UI_Key::Dir(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Dir |
| `greg.SYSTEM.Disable` | `Il2Cpp.InputController::Disable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.Disable |
| `greg.SYSTEM.DisableTheTriggerColliderAfterDealy` | `Il2Cpp.CarController::DisableTheTriggerColliderAfterDealy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.DisableTheTriggerColliderAfterDealy |
| `greg.SYSTEM.Disabling` | `Il2Cpp.LeanTweenUIElement::Disabling()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.Disabling |
| `greg.SYSTEM.DisalowDrop` | `Il2Cpp.UsableObject::DisalowDrop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.DisalowDrop |
| `greg.SYSTEM.DisplayChanged` | `Il2Cpp.ShopCartItem::UpdateDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.UpdateDisplay |
| `greg.SYSTEM.DisplayTextMeshFloatingText` | `Il2Cpp.TextMeshProFloatingText::DisplayTextMeshFloatingText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshProFloatingText.DisplayTextMeshFloatingText |
| `greg.SYSTEM.DisplayTextMeshProFloatingText` | `Il2Cpp.TextMeshProFloatingText::DisplayTextMeshProFloatingText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextMeshProFloatingText.DisplayTextMeshProFloatingText |
| `greg.SYSTEM.Dispose` | `Il2Cpp.InputController::Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.Dispose |
| `greg.SYSTEM.Dispose` | `Il2Cpp.Enumerator::Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.Dispose |
| `greg.SYSTEM.DistanceKinematicCheck` | `Il2Cpp.UsableObject::DistanceKinematicCheck()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.DistanceKinematicCheck |
| `greg.SYSTEM.DllLoaded` | `Il2Cpp.ModLoader::LoadDll(string, DllEntry)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadDll |
| `greg.SYSTEM.DoesCableServeMultipleCustomers` | `Il2Cpp.WaypointInitializationSystem::DoesCableServeMultipleCustomers(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.DoesCableServeMultipleCustomers |
| `greg.SYSTEM.DpadMove` | `Il2Cpp.OSK_Keyboard::DpadMove(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.DpadMove |
| `greg.SYSTEM.DpadMove` | `Il2Cpp.OSK_UI_Keyboard::DpadMove(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.DpadMove |
| `greg.SYSTEM.DynamicChanged` | `Il2Cpp.FlexibleColorPicker::UpdateDynamic(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateDynamic |
| `greg.SYSTEM.EarlyInit` | `Il2Cpp.__JobReflectionRegistrationOutput__1221673671587648887::EarlyInit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__JobReflectionRegistrationOutput__1221673671587648887.EarlyInit |
| `greg.SYSTEM.EarlyInit` | `Il2Cpp.__UnmanagedPostProcessorOutput__1221673671587648887::EarlyInit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__UnmanagedPostProcessorOutput__1221673671587648887.EarlyInit |
| `greg.SYSTEM.EffectsVolumeSet` | `Il2Cpp.AudioManager::SetEffectsVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.SetEffectsVolume |
| `greg.SYSTEM.Enable` | `Il2Cpp.InputController::Enable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.Enable |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnControlChange::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnControlChange.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.GameIsLoaded::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GameIsLoaded.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnLanguageChange::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLanguageChange.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnPauseMenuOpen::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuOpen.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnPauseMenuClose::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuClose.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnBuyingWall::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnBuyingWall.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnSavingData::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnSavingData.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnLoadingData::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingData.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnLoadingDataLater::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingDataLater.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnTurnOffPublic::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnTurnOffPublic.EndInvoke |
| `greg.SYSTEM.EndInvoke` | `Il2Cpp.OnEndOfTheDay::EndInvoke(Il2CppSystem.IAsyncResult)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnEndOfTheDay.EndInvoke |
| `greg.SYSTEM.EndPointSet` | `Il2Cpp.Rope::SetEndPoint(Transform, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.SetEndPoint |
| `greg.SYSTEM.Equals` | `Il2Cpp.AssemblyTypeRegistry::Equals(Il2CppSystem.Object, Il2CppSystem.Object, int)` | `Postfix` | Auto-generated from IL2CPP sources: Unity/AssemblyTypeRegistry.Equals |
| `greg.SYSTEM.Equals` | `Il2Cpp.AssemblyTypeRegistry::Equals(Il2CppSystem.Object, void*, int)` | `Postfix` | Auto-generated from IL2CPP sources: Unity/AssemblyTypeRegistry.Equals |
| `greg.SYSTEM.EvaluateAllRoutes` | `Il2Cpp.WaypointInitializationSystem::EvaluateAllRoutes()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.EvaluateAllRoutes |
| `greg.SYSTEM.EvaluationCooldownSet` | `Il2Cpp.WaypointInitializationSystem::SetEvaluationCooldown(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.SetEvaluationCooldown |
| `greg.SYSTEM.FadeIn` | `Il2Cpp.AudioManager::FadeIn(AudioSource, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.FadeIn |
| `greg.SYSTEM.FadeOut` | `Il2Cpp.AudioManager::FadeOut(AudioSource, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.FadeOut |
| `greg.SYSTEM.Finalize` | `Il2Cpp.InputController::Finalize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.Finalize |
| `greg.SYSTEM.FindAction` | `Il2Cpp.InputController::FindAction(string, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.FindAction |
| `greg.SYSTEM.FindBinding` | `Il2Cpp.InputController::FindBinding(InputBinding, InputAction)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.FindBinding |
| `greg.SYSTEM.FinishTypeHex` | `Il2Cpp.FlexibleColorPicker::FinishTypeHex(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.FinishTypeHex |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.UsableObject::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.FixedUpdate |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.Rope::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.FixedUpdate |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.RopeWindEffect::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.FixedUpdate |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.CarController::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.FixedUpdate |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.OSK_GamepadHelper::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.FixedUpdate |
| `greg.SYSTEM.FixedUpdate` | `Il2Cpp.OSK_UI_Keyboard::FixedUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.FixedUpdate |
| `greg.SYSTEM.ForceMousePositionToCenterOfGameWindow` | `Il2Cpp.InputManager::ForceMousePositionToCenterOfGameWindow()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.ForceMousePositionToCenterOfGameWindow |
| `greg.SYSTEM.FormatDistance` | `Il2Cpp.SteamStatsOnMainMenuTop::FormatDistance(double)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamStatsOnMainMenuTop.FormatDistance |
| `greg.SYSTEM.FreeTrolleySlot` | `Il2Cpp.TrolleyLoadingBay::FreeTrolleySlot(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.FreeTrolleySlot |
| `greg.SYSTEM.FreeUpSpawnPoint` | `Il2Cpp.ComputerShop::FreeUpSpawnPoint(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.FreeUpSpawnPoint |
| `greg.SYSTEM.GODMOD_delayed` | `Il2Cpp.GODMOD::GODMOD_delayed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.GODMOD_delayed |
| `greg.SYSTEM.GamepadInput_Cancel` | `Il2Cpp.OSK_Keyboard::GamepadInput_Cancel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GamepadInput_Cancel |
| `greg.SYSTEM.GamepadInput_Horizontal` | `Il2Cpp.OSK_Keyboard::GamepadInput_Horizontal(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GamepadInput_Horizontal |
| `greg.SYSTEM.GamepadInput_Submit` | `Il2Cpp.OSK_Keyboard::GamepadInput_Submit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GamepadInput_Submit |
| `greg.SYSTEM.GamepadInput_Vertical` | `Il2Cpp.OSK_Keyboard::GamepadInput_Vertical(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GamepadInput_Vertical |
| `greg.SYSTEM.GamepadPrep` | `Il2Cpp.OSK_GamepadHelper::GamepadPrep()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.GamepadPrep |
| `greg.SYSTEM.GamepadWrapNavigation` | `Il2Cpp.OSK_UI_Keyboard::GamepadWrapNavigation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.GamepadWrapNavigation |
| `greg.SYSTEM.GateCoroutine` | `Il2Cpp.GateLever::GateCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.GateCoroutine |
| `greg.SYSTEM.GenKeyMapDict` | `Il2Cpp.OSK_Keymap::GenKeyMapDict()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.GenKeyMapDict |
| `greg.SYSTEM.GenKeyMapStr` | `Il2Cpp.OSK_Keymap::GenKeyMapStr()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.GenKeyMapStr |
| `greg.SYSTEM.Generate` | `Il2Cpp.OSK_AccentConsole::Generate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.Generate |
| `greg.SYSTEM.Generate` | `Il2Cpp.OSK_Keyboard::Generate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Generate |
| `greg.SYSTEM.Generate` | `Il2Cpp.OSK_UI_Keyboard::Generate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Generate |
| `greg.SYSTEM.GenerateCoroutine` | `Il2Cpp.OSK_AccentConsole::GenerateCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.GenerateCoroutine |
| `greg.SYSTEM.GenerateID` | `Il2Cpp.FCP_Persistence::GenerateID()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.GenerateID |
| `greg.SYSTEM.GenerateMesh` | `Il2Cpp.RopeMesh::GenerateMesh()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.GenerateMesh |
| `greg.SYSTEM.GenerateUniquePatchPanelId` | `Il2Cpp.PatchPanel::GenerateUniquePatchPanelId()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.GenerateUniquePatchPanelId |
| `greg.SYSTEM.GenerateWind` | `Il2Cpp.RopeWindEffect::GenerateWind()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.GenerateWind |
| `greg.SYSTEM.Get` | `Il2Cpp.AdvancedSettings::Get(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AdvancedSettings.Get |
| `greg.SYSTEM.Get` | `Il2Cpp.UnitySourceGeneratedAssemblyMonoScriptTypes_v1::Get()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UnitySourceGeneratedAssemblyMonoScriptTypes_v1.Get |
| `greg.SYSTEM.GetActiveTerrainTextureIdx` | `Il2Cpp.TerrainDetector::GetActiveTerrainTextureIdx(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TerrainDetector.GetActiveTerrainTextureIdx |
| `greg.SYSTEM.GetAllAxis` | `Il2Cpp.viperInput::GetAllAxis()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetAllAxis |
| `greg.SYSTEM.GetAllCables` | `Il2Cpp.WaypointInitializationSystem::GetAllCables()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetAllCables |
| `greg.SYSTEM.GetAppLogo` | `Il2Cpp.MainGameManager::GetAppLogo(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetAppLogo |
| `greg.SYSTEM.GetAxis` | `Il2Cpp.viperInput::GetAxis(AXIS_INPUT)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetAxis |
| `greg.SYSTEM.GetBindingInfo` | `Il2Cpp.ActionKeyHint::GetBindingInfo()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.GetBindingInfo |
| `greg.SYSTEM.GetBindingName` | `Il2Cpp.InputManager::GetBindingName(string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.GetBindingName |
| `greg.SYSTEM.GetCableCurrentSpeed` | `Il2Cpp.WaypointInitializationSystem::GetCableCurrentSpeed(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetCableCurrentSpeed |
| `greg.SYSTEM.GetCableSpinnerPrefab` | `Il2Cpp.MainGameManager::GetCableSpinnerPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetCableSpinnerPrefab |
| `greg.SYSTEM.GetColor` | `Il2Cpp.FlexibleColorPicker::GetColor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetColor |
| `greg.SYSTEM.GetColorFullAlpha` | `Il2Cpp.FlexibleColorPicker::GetColorFullAlpha()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetColorFullAlpha |
| `greg.SYSTEM.GetControllerNames` | `Il2Cpp.viperInput::GetControllerNames()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetControllerNames |
| `greg.SYSTEM.GetCorrectedKey` | `Il2Cpp.OSK_Keymap::GetCorrectedKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.GetCorrectedKey |
| `greg.SYSTEM.GetCustomerItemByID` | `Il2Cpp.MainGameManager::GetCustomerItemByID(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetCustomerItemByID |
| `greg.SYSTEM.GetCustomerLogo` | `Il2Cpp.MainGameManager::GetCustomerLogo(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetCustomerLogo |
| `greg.SYSTEM.GetCustomerRoutes` | `Il2Cpp.WaypointInitializationSystem::GetCustomerRoutes()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetCustomerRoutes |
| `greg.SYSTEM.GetCustomerTotalRequirement` | `Il2Cpp.MainGameManager::GetCustomerTotalRequirement(CustomerItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetCustomerTotalRequirement |
| `greg.SYSTEM.GetCustomersUsingCable` | `Il2Cpp.WaypointInitializationSystem::GetCustomersUsingCable(WaypointInitializationSystem.CableInfo)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetCustomersUsingCable |
| `greg.SYSTEM.GetEnumerator` | `Il2Cpp.InputController::GetEnumerator()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.GetEnumerator |
| `greg.SYSTEM.GetEnumerator` | `Il2Cpp.Enumerator::GetEnumerator()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.GetEnumerator |
| `greg.SYSTEM.GetEvaluationCooldown` | `Il2Cpp.WaypointInitializationSystem::GetEvaluationCooldown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetEvaluationCooldown |
| `greg.SYSTEM.GetFreeSubnet` | `Il2Cpp.MainGameManager::GetFreeSubnet(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetFreeSubnet |
| `greg.SYSTEM.GetFreeVlanId` | `Il2Cpp.MainGameManager::GetFreeVlanId()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetFreeVlanId |
| `greg.SYSTEM.GetFunctionPointer` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0::GetFunctionPointer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0.GetFunctionPointer |
| `greg.SYSTEM.GetFunctionPointer` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1::GetFunctionPointer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1.GetFunctionPointer |
| `greg.SYSTEM.GetFunctionPointerDiscard` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0::GetFunctionPointerDiscard(System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0.GetFunctionPointerDiscard |
| `greg.SYSTEM.GetFunctionPointerDiscard` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1::GetFunctionPointerDiscard(System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1.GetFunctionPointerDiscard |
| `greg.SYSTEM.GetGameObject` | `Il2Cpp.I_OSK_Key::GetGameObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetGameObject |
| `greg.SYSTEM.GetGameObject` | `Il2Cpp.OSK_Key::GetGameObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetGameObject |
| `greg.SYSTEM.GetGameObject` | `Il2Cpp.OSK_UI_Key::GetGameObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetGameObject |
| `greg.SYSTEM.GetGradientMode` | `Il2Cpp.FlexibleColorPicker::GetGradientMode(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetGradientMode |
| `greg.SYSTEM.GetInput` | `Il2Cpp.FirstPersonController::GetInput(float)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.GetInput |
| `greg.SYSTEM.GetItemCount` | `Il2Cpp.AssetManagement::GetItemCount()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.GetItemCount |
| `greg.SYSTEM.GetItemCount` | `Il2Cpp.IRecyclableScrollRectDataSource::GetItemCount()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/IRecyclableScrollRectDataSource.GetItemCount |
| `greg.SYSTEM.GetKeyCode` | `Il2Cpp.I_OSK_Key::GetKeyCode()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetKeyCode |
| `greg.SYSTEM.GetKeyCode` | `Il2Cpp.OSK_Key::GetKeyCode()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetKeyCode |
| `greg.SYSTEM.GetKeyCode` | `Il2Cpp.OSK_Keyboard::GetKeyCode(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GetKeyCode |
| `greg.SYSTEM.GetKeyCode` | `Il2Cpp.OSK_UI_Key::GetKeyCode()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetKeyCode |
| `greg.SYSTEM.GetKeyName` | `Il2Cpp.I_OSK_Key::GetKeyName()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetKeyName |
| `greg.SYSTEM.GetKeyName` | `Il2Cpp.OSK_Key::GetKeyName()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetKeyName |
| `greg.SYSTEM.GetKeyName` | `Il2Cpp.OSK_UI_Key::GetKeyName()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetKeyName |
| `greg.SYSTEM.GetKeyTransform` | `Il2Cpp.I_OSK_Key::GetKeyTransform()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetKeyTransform |
| `greg.SYSTEM.GetKeyTransform` | `Il2Cpp.OSK_Key::GetKeyTransform()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetKeyTransform |
| `greg.SYSTEM.GetKeyTransform` | `Il2Cpp.OSK_UI_Key::GetKeyTransform()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetKeyTransform |
| `greg.SYSTEM.GetLayoutLocation` | `Il2Cpp.I_OSK_Key::GetLayoutLocation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetLayoutLocation |
| `greg.SYSTEM.GetLayoutLocation` | `Il2Cpp.OSK_Key::GetLayoutLocation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetLayoutLocation |
| `greg.SYSTEM.GetLayoutLocation` | `Il2Cpp.OSK_UI_Key::GetLayoutLocation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetLayoutLocation |
| `greg.SYSTEM.GetMarker` | `Il2Cpp.FlexibleColorPicker::GetMarker(Image, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetMarker |
| `greg.SYSTEM.GetMaskFromCidr` | `Il2Cpp.SetIP::GetMaskFromCidr(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.GetMaskFromCidr |
| `greg.SYSTEM.GetMidPoint` | `Il2Cpp.Rope::GetMidPoint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.GetMidPoint |
| `greg.SYSTEM.GetModPrefab` | `Il2Cpp.ModLoader::GetModPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.GetModPrefab |
| `greg.SYSTEM.GetModPrefabByFolder` | `Il2Cpp.ModLoader::GetModPrefabByFolder(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.GetModPrefabByFolder |
| `greg.SYSTEM.GetMouseLook` | `Il2Cpp.FirstPersonController::GetMouseLook()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.GetMouseLook |
| `greg.SYSTEM.GetNextAvailableSpawnPoint` | `Il2Cpp.ComputerShop::GetNextAvailableSpawnPoint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.GetNextAvailableSpawnPoint |
| `greg.SYSTEM.GetNormScreenSpace` | `Il2Cpp.FlexibleColorPicker::GetNormScreenSpace(RectTransform, BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetNormScreenSpace |
| `greg.SYSTEM.GetOSKKey` | `Il2Cpp.OSK_Keyboard::GetOSKKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GetOSKKey |
| `greg.SYSTEM.GetOSKKeyCode` | `Il2Cpp.OSK_Keyboard::GetOSKKeyCode(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GetOSKKeyCode |
| `greg.SYSTEM.GetObject` | `Il2Cpp.I_OSK_Key::GetObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.GetObject |
| `greg.SYSTEM.GetObject` | `Il2Cpp.OSK_Key::GetObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.GetObject |
| `greg.SYSTEM.GetObject` | `Il2Cpp.OSK_UI_Key::GetObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.GetObject |
| `greg.SYSTEM.GetPairedLink` | `Il2Cpp.PatchPanel::GetPairedLink(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.GetPairedLink |
| `greg.SYSTEM.GetPatchPanelPrefab` | `Il2Cpp.MainGameManager::GetPatchPanelPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetPatchPanelPrefab |
| `greg.SYSTEM.GetPhysicalKey` | `Il2Cpp.viperInput::GetPhysicalKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetPhysicalKey |
| `greg.SYSTEM.GetPlayerAButton` | `Il2Cpp.viperInput::GetPlayerAButton(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetPlayerAButton |
| `greg.SYSTEM.GetPlayerBButton` | `Il2Cpp.viperInput::GetPlayerBButton(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetPlayerBButton |
| `greg.SYSTEM.GetPlayerInput` | `Il2Cpp.CameraController::GetPlayerInput()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/CameraController.GetPlayerInput |
| `greg.SYSTEM.GetPlayerJoystickInput` | `Il2Cpp.viperInput::GetPlayerJoystickInput(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetPlayerJoystickInput |
| `greg.SYSTEM.GetPointAt` | `Il2Cpp.Rope::GetPointAt(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.GetPointAt |
| `greg.SYSTEM.GetPointerPos` | `Il2Cpp.viperInput::GetPointerPos()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.GetPointerPos |
| `greg.SYSTEM.GetPrefabForItem` | `Il2Cpp.ComputerShop::GetPrefabForItem(int, PlayerManager.ObjectInHand)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.GetPrefabForItem |
| `greg.SYSTEM.GetRandomClip` | `Il2Cpp.FootSteps::GetRandomClip()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.GetRandomClip |
| `greg.SYSTEM.GetRandomFromRequest` | `Il2Cpp.FootSteps::GetRandomFromRequest(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.GetRandomFromRequest |
| `greg.SYSTEM.GetSanitizedHex` | `Il2Cpp.FlexibleColorPicker::GetSanitizedHex(string, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetSanitizedHex |
| `greg.SYSTEM.GetSelectedKey` | `Il2Cpp.OSK_GamepadHelper::GetSelectedKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.GetSelectedKey |
| `greg.SYSTEM.GetSelectedKey` | `Il2Cpp.OSK_Keyboard::GetSelectedKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.GetSelectedKey |
| `greg.SYSTEM.GetServerPrefab` | `Il2Cpp.MainGameManager::GetServerPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetServerPrefab |
| `greg.SYSTEM.GetServerProcessingSpeed` | `Il2Cpp.WaypointInitializationSystem::GetServerProcessingSpeed(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.GetServerProcessingSpeed |
| `greg.SYSTEM.GetSettingHash` | `Il2Cpp.FCP_SpriteMeshEditor::GetSettingHash()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_SpriteMeshEditor.GetSettingHash |
| `greg.SYSTEM.GetSfpBoxPrefab` | `Il2Cpp.MainGameManager::GetSfpBoxPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetSfpBoxPrefab |
| `greg.SYSTEM.GetSfpPrefab` | `Il2Cpp.MainGameManager::GetSfpPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetSfpPrefab |
| `greg.SYSTEM.GetSize` | `Il2Cpp.OSK_MiniKeyboard::GetSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.GetSize |
| `greg.SYSTEM.GetSprite` | `Il2Cpp.GamepadIcons::GetSprite(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GamepadIcons.GetSprite |
| `greg.SYSTEM.GetSwitchPrefab` | `Il2Cpp.MainGameManager::GetSwitchPrefab(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.GetSwitchPrefab |
| `greg.SYSTEM.GetUsableIPsFromSubnet` | `Il2Cpp.SetIP::GetUsableIPsFromSubnet(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.GetUsableIPsFromSubnet |
| `greg.SYSTEM.GetValue` | `Il2Cpp.FlexibleColorPicker::GetValue(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetValue |
| `greg.SYSTEM.GetValue` | `Il2Cpp.FlexibleColorPicker::GetValue(FlexibleColorPicker.MainPickingMode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetValue |
| `greg.SYSTEM.GetValue1D` | `Il2Cpp.FlexibleColorPicker::GetValue1D(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.GetValue1D |
| `greg.SYSTEM.GotoNextPoint` | `Il2Cpp.AICharacterControl::GotoNextPoint(Il2CppReferenceArray<Transform>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.GotoNextPoint |
| `greg.SYSTEM.HSVToRGB` | `Il2Cpp.FlexibleColorPicker::HSVToRGB(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.HSVToRGB |
| `greg.SYSTEM.HSVToRGB` | `Il2Cpp.FlexibleColorPicker::HSVToRGB(float, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.HSVToRGB |
| `greg.SYSTEM.HandleAudio` | `Il2Cpp.CarController::HandleAudio()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.HandleAudio |
| `greg.SYSTEM.HandleGroundedMovement` | `Il2Cpp.ThirdPersonCharacter::HandleGroundedMovement(bool, bool)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.HandleGroundedMovement |
| `greg.SYSTEM.HandleLookAtRay` | `Il2Cpp.RayLookAt::HandleLookAtRay(Transform)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.HandleLookAtRay |
| `greg.SYSTEM.HandleObjectives` | `Il2Cpp.ComputerShop::HandleObjectives(PlayerManager.ObjectInHand)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.HandleObjectives |
| `greg.SYSTEM.HandleZoom` | `Il2Cpp.FirstPersonController::HandleZoom()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.HandleZoom |
| `greg.SYSTEM.HasFocus` | `Il2Cpp.OSK_Keyboard::HasFocus(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.HasFocus |
| `greg.SYSTEM.HasKey` | `Il2Cpp.OSK_Keyboard::HasKey(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.HasKey |
| `greg.SYSTEM.HasKey` | `Il2Cpp.OSK_UI_Keyboard::HasKey(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.HasKey |
| `greg.SYSTEM.HexChanged` | `Il2Cpp.FlexibleColorPicker::UpdateHex()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateHex |
| `greg.SYSTEM.HideItemNameOrSiluete` | `Il2Cpp.RayLookAt::HideItemNameOrSiluete()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.HideItemNameOrSiluete |
| `greg.SYSTEM.HideTextUnderCursor` | `Il2Cpp.StaticUIElements::HideTextUnderCursor()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.HideTextUnderCursor |
| `greg.SYSTEM.Highlight` | `Il2Cpp.I_OSK_Key::Highlight(bool, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.Highlight |
| `greg.SYSTEM.Highlight` | `Il2Cpp.OSK_Key::Highlight(bool, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.Highlight |
| `greg.SYSTEM.Highlight` | `Il2Cpp.OSK_UI_Key::Highlight(bool, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.Highlight |
| `greg.SYSTEM.HoldProgressChanged` | `Il2Cpp.StaticUIElements::UpdateHoldProgress(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.UpdateHoldProgress |
| `greg.SYSTEM.HoursFromDate` | `Il2Cpp.TimeController::HoursFromDate(float, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.HoursFromDate |
| `greg.SYSTEM.IconLoaded` | `Il2Cpp.ModLoader::LoadIcon(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadIcon |
| `greg.SYSTEM.Il2CppSystem.Object` | `Il2Cpp.OSK_UI_Keyboard::Il2CppSystem.Object()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Il2CppSystem.Object |
| `greg.SYSTEM.IncrementOctets` | `Il2Cpp.SetIP::IncrementOctets(int, int, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.IncrementOctets |
| `greg.SYSTEM.Init` | `Il2Cpp.InternalCompilerQueryAndHandleData::Init(SystemState, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InternalCompilerQueryAndHandleData.Init |
| `greg.SYSTEM.Init` | `Il2Cpp.WorldCanvasCuller::Init()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldCanvasCuller.Init |
| `greg.SYSTEM.Init` | `Il2Cpp.MouseLook::Init(Transform, Transform)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.Init |
| `greg.SYSTEM.Init` | `Il2Cpp.RayLookAt::Init()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.Init |
| `greg.SYSTEM.InitCoroutine` | `Il2Cpp.HorizontalRecyclingSystem::InitCoroutine(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.InitCoroutine |
| `greg.SYSTEM.InitCoroutine` | `Il2Cpp.RecyclingSystem::InitCoroutine(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclingSystem.InitCoroutine |
| `greg.SYSTEM.InitCoroutine` | `Il2Cpp.VerticalRecyclingSystem::InitCoroutine(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.InitCoroutine |
| `greg.SYSTEM.InitOnPlayMode` | `Il2Cpp.SteamManager::InitOnPlayMode()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.InitOnPlayMode |
| `greg.SYSTEM.InitStatic` | `Il2Cpp.FCP_Persistence::InitStatic()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.InitStatic |
| `greg.SYSTEM.Initialize` | `Il2Cpp.ModShopItem::Initialize(int, ShopItemConfig, Sprite)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModShopItem.Initialize |
| `greg.SYSTEM.Initialize` | `Il2Cpp.RecyclableScrollRect::Initialize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.Initialize |
| `greg.SYSTEM.Initialize` | `Il2Cpp.RecyclableScrollRect::Initialize(IRecyclableScrollRectDataSource)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.Initialize |
| `greg.SYSTEM.InitializeComponents` | `Il2Cpp.RopeMesh::InitializeComponents()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.InitializeComponents |
| `greg.SYSTEM.InitializeLineRenderer` | `Il2Cpp.Rope::InitializeLineRenderer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.InitializeLineRenderer |
| `greg.SYSTEM.InitializeVlanPool` | `Il2Cpp.MainGameManager::InitializeVlanPool()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.InitializeVlanPool |
| `greg.SYSTEM.InputFromPointerDevice` | `Il2Cpp.OSK_Keyboard::InputFromPointerDevice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.InputFromPointerDevice |
| `greg.SYSTEM.InputFromPointerDevice` | `Il2Cpp.OSK_MiniKeyboard::InputFromPointerDevice()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.InputFromPointerDevice |
| `greg.SYSTEM.InputManager.OnControlChange` | `Il2Cpp.OnControlChange::InputManager.OnControlChange([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnControlChange.InputManager.OnControlChange |
| `greg.SYSTEM.InsertText` | `Il2Cpp.OSK_Keyboard::InsertText(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.InsertText |
| `greg.SYSTEM.InsertedInRack` | `Il2Cpp.PatchPanel::InsertedInRack(PatchPanelSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.InsertedInRack |
| `greg.SYSTEM.InstantiateErrorWarningSign` | `Il2Cpp.StaticUIElements::InstantiateErrorWarningSign(bool, Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.InstantiateErrorWarningSign |
| `greg.SYSTEM.InstantiateParticleUpgrade` | `Il2Cpp.StaticUIElements::InstantiateParticleUpgrade(Transform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.InstantiateParticleUpgrade |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.ComputerShop::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.Dumpster::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Dumpster.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.GateLever::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.Interact::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.MusicPlayer::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.PatchPanel::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.PushTrolleyHandle::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PushTrolleyHandle.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.TrolleyLoadingBay::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.UsableObject::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.Wall::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.InteractOnClick |
| `greg.SYSTEM.InteractOnClick` | `Il2Cpp.WorldObjectButton::InteractOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldObjectButton.InteractOnClick |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.ComputerShop::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.Dumpster::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Dumpster.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.GateLever::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.Interact::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.MusicPlayer::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.PatchPanel::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.PushTrolleyHandle::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PushTrolleyHandle.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.TrolleyLoadingBay::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.UsableObject::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.Wall::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.InteractOnHover |
| `greg.SYSTEM.InteractOnHover` | `Il2Cpp.WorldObjectButton::InteractOnHover(RaycastHit)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldObjectButton.InteractOnHover |
| `greg.SYSTEM.InteractableSet` | `Il2Cpp.OSK_Keyboard::SetInteractable(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SetInteractable |
| `greg.SYSTEM.InteractableSet` | `Il2Cpp.OSK_UI_Keyboard::SetInteractable(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SetInteractable |
| `greg.SYSTEM.InternalLockUpdate` | `Il2Cpp.MouseLook::InternalLockUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.InternalLockUpdate |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnControlChange::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnControlChange.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.GameIsLoaded::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GameIsLoaded.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnLanguageChange::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLanguageChange.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0::Invoke(System.IntPtr, System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe0.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1::Invoke(System.IntPtr, System.IntPtr)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MulticastDelegateNInternalSealedVoInA_InVoObseA_stBe1.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnPauseMenuOpen::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuOpen.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnPauseMenuClose::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuClose.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnBuyingWall::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnBuyingWall.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnSavingData::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnSavingData.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnLoadingData::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingData.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnLoadingDataLater::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingDataLater.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnTurnOffPublic::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnTurnOffPublic.Invoke |
| `greg.SYSTEM.Invoke` | `Il2Cpp.OnEndOfTheDay::Invoke()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnEndOfTheDay.Invoke |
| `greg.SYSTEM.IsAccentedCharacter` | `Il2Cpp.OSK_Keymap::IsAccentedCharacter(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.IsAccentedCharacter |
| `greg.SYSTEM.IsAllowedToDoSecondAction` | `Il2Cpp.Interact::IsAllowedToDoSecondAction()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.IsAllowedToDoSecondAction |
| `greg.SYSTEM.IsAlphaType` | `Il2Cpp.FlexibleColorPicker::IsAlphaType(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsAlphaType |
| `greg.SYSTEM.IsAnyCableConnected` | `Il2Cpp.PatchPanel::IsAnyCableConnected()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.IsAnyCableConnected |
| `greg.SYSTEM.IsCustomerSuitableForBase` | `Il2Cpp.MainGameManager::IsCustomerSuitableForBase(CustomerItem, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.IsCustomerSuitableForBase |
| `greg.SYSTEM.IsHorizontal` | `Il2Cpp.FlexibleColorPicker::IsHorizontal(FlexibleColorPicker.Picker)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsHorizontal |
| `greg.SYSTEM.IsLetterAZ` | `Il2Cpp.viperInput::IsLetterAZ(KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.IsLetterAZ |
| `greg.SYSTEM.IsPickerAvailable` | `Il2Cpp.FlexibleColorPicker::IsPickerAvailable(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsPickerAvailable |
| `greg.SYSTEM.IsPickerAvailable` | `Il2Cpp.FlexibleColorPicker::IsPickerAvailable(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsPickerAvailable |
| `greg.SYSTEM.IsPointsMoved` | `Il2Cpp.Rope::IsPointsMoved()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.IsPointsMoved |
| `greg.SYSTEM.IsPreviewType` | `Il2Cpp.FlexibleColorPicker::IsPreviewType(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsPreviewType |
| `greg.SYSTEM.IsRopeSettingsChanged` | `Il2Cpp.Rope::IsRopeSettingsChanged()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.IsRopeSettingsChanged |
| `greg.SYSTEM.IsSubnetValid` | `Il2Cpp.MainGameManager::IsSubnetValid(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.IsSubnetValid |
| `greg.SYSTEM.IsValidHexChar` | `Il2Cpp.FlexibleColorPicker::IsValidHexChar(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.IsValidHexChar |
| `greg.SYSTEM.IsVisible` | `Il2Cpp.OSK_AccentConsole::IsVisible()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.IsVisible |
| `greg.SYSTEM.ItemPurchased` | `Il2Cpp.ShopItem::BuyItem()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.BuyItem |
| `greg.SYSTEM.JoystickButtonA` | `Il2Cpp.OSK_GamepadHelper::JoystickButtonA()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.JoystickButtonA |
| `greg.SYSTEM.JoystickButtonB` | `Il2Cpp.OSK_GamepadHelper::JoystickButtonB()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.JoystickButtonB |
| `greg.SYSTEM.JoystickInput` | `Il2Cpp.OSK_GamepadHelper::JoystickInput()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.JoystickInput |
| `greg.SYSTEM.JoystickPressDown` | `Il2Cpp.OSK_Key::JoystickPressDown(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.JoystickPressDown |
| `greg.SYSTEM.JoystickPressDown` | `Il2Cpp.OSK_UI_Key::JoystickPressDown(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.JoystickPressDown |
| `greg.SYSTEM.JoystickPressUp` | `Il2Cpp.OSK_Key::JoystickPressUp(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.JoystickPressUp |
| `greg.SYSTEM.JoystickPressUp` | `Il2Cpp.OSK_UI_Key::JoystickPressUp(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.JoystickPressUp |
| `greg.SYSTEM.KeepRotating` | `Il2Cpp.LeanTweenUIElement::KeepRotating()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.KeepRotating |
| `greg.SYSTEM.KeyBackspace` | `Il2Cpp.OSK_Keyboard::KeyBackspace(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyBackspace |
| `greg.SYSTEM.KeyCall` | `Il2Cpp.OSK_Keyboard::KeyCall(OSK_KeyCode, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyCall |
| `greg.SYSTEM.KeyCall` | `Il2Cpp.OSK_UI_Keyboard::KeyCall(OSK_KeyCode, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.KeyCall |
| `greg.SYSTEM.KeyCallBase` | `Il2Cpp.OSK_Keyboard::KeyCallBase(OSK_KeyCode, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyCallBase |
| `greg.SYSTEM.KeyCallBase` | `Il2Cpp.OSK_UI_Keyboard::KeyCallBase(OSK_KeyCode, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.KeyCallBase |
| `greg.SYSTEM.KeyDelete` | `Il2Cpp.OSK_Keyboard::KeyDelete(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyDelete |
| `greg.SYSTEM.KeyDown` | `Il2Cpp.viperInput::KeyDown(KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.KeyDown |
| `greg.SYSTEM.KeyFont` | `Il2Cpp.OSK_Key::KeyFont(TMP_FontAsset)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.KeyFont |
| `greg.SYSTEM.KeyFont` | `Il2Cpp.OSK_UI_Key::KeyFont(TMP_FontAsset)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.KeyFont |
| `greg.SYSTEM.KeyPress` | `Il2Cpp.viperInput::KeyPress(KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.KeyPress |
| `greg.SYSTEM.KeyScreenSize` | `Il2Cpp.OSK_Keyboard::KeyScreenSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyScreenSize |
| `greg.SYSTEM.KeyScreenSize` | `Il2Cpp.OSK_UI_Keyboard::KeyScreenSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.KeyScreenSize |
| `greg.SYSTEM.KeyShift` | `Il2Cpp.OSK_Keyboard::KeyShift()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyShift |
| `greg.SYSTEM.KeyType` | `Il2Cpp.I_OSK_Key::KeyType()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.KeyType |
| `greg.SYSTEM.KeyType` | `Il2Cpp.OSK_Key::KeyType()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.KeyType |
| `greg.SYSTEM.KeyType` | `Il2Cpp.OSK_KeyTypeMeta::KeyType(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_KeyTypeMeta.KeyType |
| `greg.SYSTEM.KeyType` | `Il2Cpp.OSK_UI_Key::KeyType()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.KeyType |
| `greg.SYSTEM.KeyUp` | `Il2Cpp.viperInput::KeyUp(KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.KeyUp |
| `greg.SYSTEM.KeyboardSizeEstimator` | `Il2Cpp.OSK_Keyboard::KeyboardSizeEstimator()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.KeyboardSizeEstimator |
| `greg.SYSTEM.LabelActionOnClick` | `Il2Cpp.Interact::LabelActionOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.LabelActionOnClick |
| `greg.SYSTEM.LastPressed` | `Il2Cpp.OSK_Key::LastPressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.LastPressed |
| `greg.SYSTEM.LastPressed` | `Il2Cpp.OSK_UI_Key::LastPressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.LastPressed |
| `greg.SYSTEM.LastSpawnedItemRemoved` | `Il2Cpp.ShopCartItem::RemoveLastSpawnedItem()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.RemoveLastSpawnedItem |
| `greg.SYSTEM.LateUpdate` | `Il2Cpp.TMP_TextEventHandler::LateUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.LateUpdate |
| `greg.SYSTEM.LateUpdate` | `Il2Cpp.CameraController::LateUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/CameraController.LateUpdate |
| `greg.SYSTEM.LateUpdate` | `Il2Cpp.TMP_TextSelector_A::LateUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_A.LateUpdate |
| `greg.SYSTEM.LateUpdate` | `Il2Cpp.TMP_TextSelector_B::LateUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.LateUpdate |
| `greg.SYSTEM.LateUpdate` | `Il2Cpp.OSK_Receiver::LateUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.LateUpdate |
| `greg.SYSTEM.LayoutLoaded` | `Il2Cpp.OSK_Keyboard::LoadLayout(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.LoadLayout |
| `greg.SYSTEM.LayoutLocationSet` | `Il2Cpp.OSK_Key::SetLayoutLocation(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.SetLayoutLocation |
| `greg.SYSTEM.LayoutLocationSet` | `Il2Cpp.OSK_UI_Key::SetLayoutLocation(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.SetLayoutLocation |
| `greg.SYSTEM.LeaveTheTrolley` | `Il2Cpp.CarController::LeaveTheTrolley()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.LeaveTheTrolley |
| `greg.SYSTEM.LeftAnchorSet` | `Il2Cpp.HorizontalRecyclingSystem::SetLeftAnchor(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.SetLeftAnchor |
| `greg.SYSTEM.LoadingInfoSet` | `Il2Cpp.StaticUIElements::SetLoadingInfo(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.SetLoadingInfo |
| `greg.SYSTEM.Localisation.OnLanguageChange` | `Il2Cpp.OnLanguageChange::Localisation.OnLanguageChange([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLanguageChange.Localisation.OnLanguageChange |
| `greg.SYSTEM.LocalisationLoaded` | `Il2Cpp.Localisation::LoadLocalisation(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Localisation.LoadLocalisation |
| `greg.SYSTEM.LockedCursorForPlayerMovement` | `Il2Cpp.InputManager::LockedCursorForPlayerMovement()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.LockedCursorForPlayerMovement |
| `greg.SYSTEM.LongPressActionSet` | `Il2Cpp.OSK_Settings::SetLongPressAction(UnityAction<OSK_LongPressPacket>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Settings.SetLongPressAction |
| `greg.SYSTEM.LongPressCheck` | `Il2Cpp.OSK_Key::LongPressCheck()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.LongPressCheck |
| `greg.SYSTEM.LongPressCheck` | `Il2Cpp.OSK_UI_Key::LongPressCheck()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.LongPressCheck |
| `greg.SYSTEM.MakeInteractableAgain` | `Il2Cpp.UsableObject::MakeInteractableAgain()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.MakeInteractableAgain |
| `greg.SYSTEM.MakeMesh` | `Il2Cpp.FCP_SpriteMeshEditor::MakeMesh(Sprite, int, int, FCP_SpriteMeshEditor.MeshType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_SpriteMeshEditor.MakeMesh |
| `greg.SYSTEM.MakeModeOptions` | `Il2Cpp.FlexibleColorPicker::MakeModeOptions()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.MakeModeOptions |
| `greg.SYSTEM.MarkerSet` | `Il2Cpp.FlexibleColorPicker::SetMarker(Image, Vector2, bool, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SetMarker |
| `greg.SYSTEM.MarkersChanged` | `Il2Cpp.FlexibleColorPicker::UpdateMarkers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateMarkers |
| `greg.SYSTEM.MasterVolumeSet` | `Il2Cpp.AudioManager::SetMasterVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.SetMasterVolume |
| `greg.SYSTEM.MeesageInFieldAdded` | `Il2Cpp.StaticUIElements::AddMeesageInField(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.AddMeesageInField |
| `greg.SYSTEM.MeshLoaded` | `Il2Cpp.ModLoader::LoadMesh(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadMesh |
| `greg.SYSTEM.MessageDisplayChanged` | `Il2Cpp.StaticUIElements::UpdateMessageDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.UpdateMessageDisplay |
| `greg.SYSTEM.MessagesCoroutineChanged` | `Il2Cpp.StaticUIElements::UpdateMessagesCoroutine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.UpdateMessagesCoroutine |
| `greg.SYSTEM.Method_Internal_Void_PDM_0` | `Il2Cpp.__c__DisplayClass33_0::Method_Internal_Void_PDM_0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass33_0.Method_Internal_Void_PDM_0 |
| `greg.SYSTEM.MidPointSet` | `Il2Cpp.Rope::SetMidPoint(Transform, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.SetMidPoint |
| `greg.SYSTEM.ModPackLoaded` | `Il2Cpp.ModLoader::LoadModPack(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadModPack |
| `greg.SYSTEM.ModeChanged` | `Il2Cpp.FlexibleColorPicker::UpdateMode(FlexibleColorPicker.MainPickingMode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateMode |
| `greg.SYSTEM.ModifyLastChar` | `Il2Cpp.OSK_Receiver::ModifyLastChar(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ModifyLastChar |
| `greg.SYSTEM.MouseLookOnDisable` | `Il2Cpp.MouseLook::MouseLookOnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.MouseLookOnDisable |
| `greg.SYSTEM.MouthShape_A` | `Il2Cpp.AICharacterExpressions::MouthShape_A(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_A |
| `greg.SYSTEM.MouthShape_BPM` | `Il2Cpp.AICharacterExpressions::MouthShape_BPM(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_BPM |
| `greg.SYSTEM.MouthShape_CDG` | `Il2Cpp.AICharacterExpressions::MouthShape_CDG(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_CDG |
| `greg.SYSTEM.MouthShape_FV` | `Il2Cpp.AICharacterExpressions::MouthShape_FV(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_FV |
| `greg.SYSTEM.MouthShape_O` | `Il2Cpp.AICharacterExpressions::MouthShape_O(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_O |
| `greg.SYSTEM.MouthShape_U` | `Il2Cpp.AICharacterExpressions::MouthShape_U(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_U |
| `greg.SYSTEM.MouthShape_none` | `Il2Cpp.AICharacterExpressions::MouthShape_none(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.MouthShape_none |
| `greg.SYSTEM.Move` | `Il2Cpp.CarController::Move()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.Move |
| `greg.SYSTEM.Move` | `Il2Cpp.ThirdPersonCharacter::Move(Vector3, bool, bool, bool, bool)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.Move |
| `greg.SYSTEM.MoveBetweenPositions` | `Il2Cpp.UsableObject::MoveBetweenPositions(Vector3, Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.MoveBetweenPositions |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DelayedUpdateUI_d__12::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedUpdateUI_d__12.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__30::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__30.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Talking_d__7::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Talking_d__7.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._FadeIn_d__33::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeIn_d__33.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._FadeOut_d__32::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_d__32.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._FadeOut_FadeIn_d__34::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_FadeIn_d__34.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._TurnOffAfterXseconds_d__4::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOffAfterXseconds_d__4.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._TrackFinances_d__18::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TrackFinances_d__18.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DelayedOverlapCheck_d__6::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedOverlapCheck_d__6.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AutoRepairRoutine_d__22::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoRepairRoutine_d__22.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._CheckIfAppRequirementsAreMet_d__37::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfAppRequirementsAreMet_d__37.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DelayedAppDoorOpening_d__48::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedAppDoorOpening_d__48.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._UpdateMoney_d__38::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMoney_d__38.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._TimerLoop_d__7::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TimerLoop_d__7.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__4::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__4.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._checkGroundMaterial_d__21::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_checkGroundMaterial_d__21.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._GateCoroutine_d__15::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GateCoroutine_d__15.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DelayedLoad_d__7::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedLoad_d__7.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._GODMOD_delayed_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GODMOD_delayed_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Disabling_d__21::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Disabling_d__21.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._KeepRotating_d__25::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_KeepRotating_d__25.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._TweenScaleInOut_d__24::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TweenScaleInOut_d__24.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AsynchronousLoad_d__17::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousLoad_d__17.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AsynchronousUnLoad_d__18::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousUnLoad_d__18.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LoadGameLoadScene_d__13::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadGameLoadScene_d__13.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LoadPlayerAndNPCDataWithDelay_d__12::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadPlayerAndNPCDataWithDelay_d__12.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AutoSaveCoroutine_d__94::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoSaveCoroutine_d__94.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._SyncWorkshopThenLoadAll_d__11::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SyncWorkshopThenLoadAll_d__11.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._EffectOnDestroy_d__27::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_EffectOnDestroy_d__27.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp.Enumerator::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LoadWithOverlay_d__42::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadWithOverlay_d__42.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._TurnOnCharacterControllerDelayed_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOnCharacterControllerDelayed_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._WaitForActionToFinish_d__30::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitForActionToFinish_d__30.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._UnmountRack_d__18::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UnmountRack_d__18.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._CullLoop_d__13::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullLoop_d__13.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DelayedTrigger_d__8::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedTrigger_d__8.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._InstallRack_d__6::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InstallRack_d__6.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._InsertItemInRack_d__13::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InsertItemInRack_d__13.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisableGameObjectWithDelay_d__6::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableGameObjectWithDelay_d__6.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ImageScrollingUI_d__3::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ImageScrollingUI_d__3.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._NumberScrollingUI_d__2::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_NumberScrollingUI_d__2.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AvailableRefreshRatesAfterFrame_d__35::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AvailableRefreshRatesAfterFrame_d__35.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._MoveToMonitorCoroutine_d__52::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MoveToMonitorCoroutine_d__52.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisableOnAfterFirstSettingUp_d__5::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableOnAfterFirstSettingUp_d__5.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ParentTheObjectWithDelay_d__6::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__6.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._SlideIntoPort_d__12::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SlideIntoPort_d__12.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._UpdateCoinsAndPrestige_TopLeft_d__66::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateCoinsAndPrestige_TopLeft_d__66.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._UpdateMessagesCoroutine_d__73::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMessagesCoroutine_d__73.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._WaitAndDisplay_d__5::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitAndDisplay_d__5.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._GettingNewServer_d__39::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GettingNewServer_d__39.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ReplacingServer_d__40::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ReplacingServer_d__40.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._RequestJobDelayed_d__34::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_RequestJobDelayed_d__34.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._SendToContainer_d__38::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SendToContainer_d__38.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._SetHandIKWeight_d__46::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SetHandIKWeight_d__46.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._StartTextingAnimation_d__37::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_StartTextingAnimation_d__37.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ThrowingOutServer_d__41::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ThrowingOutServer_d__41.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ProcessDispatchQueue_d__23::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ProcessDispatchQueue_d__23.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ParentTheObjectWithDelay_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ObjectAdded_d__3::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ObjectAdded_d__3.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._CheckIfLost_d__60::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfLost_d__60.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisalowDrop_d__56::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisalowDrop_d__56.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DistanceKinematicCheck_d__49::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DistanceKinematicCheck_d__49.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._MakeInteractableAgain_d__53::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MakeInteractableAgain_d__53.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ShowError_d__25::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ShowError_d__25.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__15::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__15.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LoadNetworkStateCoroutine_d__15::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadNetworkStateCoroutine_d__15.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp.___LoadNetworkStateCoroutine_592A83D2_d__50::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/___LoadNetworkStateCoroutine_592A83D2_d__50.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._CullRoutine_d__3::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullRoutine_d__3.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._InitCoroutine_d__15::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/_InitCoroutine_d__15.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisableTheTriggerColliderAfterDealy_d__44::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_DisableTheTriggerColliderAfterDealy_d__44.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ResetingTrollerPosition_d__41::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_ResetingTrollerPosition_d__41.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AnimateProperties_d__6::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateProperties_d__6.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._WarpText_d__7::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__7.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._Start_d__4::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__4.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._RevealCharacters_d__7::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealCharacters_d__7.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._RevealWords_d__8::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealWords_d__8.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisplayTextMeshFloatingText_d__16::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshFloatingText_d__16.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._DisplayTextMeshProFloatingText_d__15::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshProFloatingText_d__15.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AnimateVertexColors_d__3::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__3.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AnimateVertexColors_d__11::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__11.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._AnimateVertexColors_d__10::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__10.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._WarpText_d__8::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__8.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._GenerateCoroutine_d__21::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_GenerateCoroutine_d__21.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._BlinkCoroutine_d__14::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_BlinkCoroutine_d__14.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ClickCoroutine_d__41::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__41.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LongPressCheck_d__35::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__35.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ReHighlightKey_d__68::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ReHighlightKey_d__68.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._ClickCoroutine_d__43::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__43.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._LongPressCheck_d__37::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__37.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._SelectKey_d__21::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_SelectKey_d__21.MoveNext |
| `greg.SYSTEM.MoveNext` | `Il2Cpp._OnFinishSubmit_d__25::MoveNext()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/_OnFinishSubmit_d__25.MoveNext |
| `greg.SYSTEM.MoveToHand` | `Il2Cpp.UsableObject::MoveToHand()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.MoveToHand |
| `greg.SYSTEM.MoveToStorage` | `Il2Cpp.UsableObject::MoveToStorage(Transform, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.MoveToStorage |
| `greg.SYSTEM.MusicSet` | `Il2Cpp.AudioManager::SetMusic(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.SetMusic |
| `greg.SYSTEM.MusicVolumeSet` | `Il2Cpp.AudioManager::SetMusicVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.SetMusicVolume |
| `greg.SYSTEM.NewLine` | `Il2Cpp.OSK_Receiver::NewLine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.NewLine |
| `greg.SYSTEM.NewLine` | `Il2Cpp.OSK_UI_InputReceiver::NewLine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.NewLine |
| `greg.SYSTEM.NewLineAdded` | `Il2Cpp.OSK_Keyboard::AddNewLine()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AddNewLine |
| `greg.SYSTEM.NewLineFix` | `Il2Cpp.OSK_Receiver::NewLineFix()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.NewLineFix |
| `greg.SYSTEM.NormalFovChanged` | `Il2Cpp.FirstPersonController::UpdateNormalFov(float)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.UpdateNormalFov |
| `greg.SYSTEM.NotificationSet` | `Il2Cpp.StaticUIElements::SetNotification(int, Sprite, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.SetNotification |
| `greg.SYSTEM.NotifyPointsChanged` | `Il2Cpp.Rope::NotifyPointsChanged()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.NotifyPointsChanged |
| `greg.SYSTEM.NumControllers` | `Il2Cpp.viperInput::NumControllers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.NumControllers |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.TextConsoleSimulator::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.ON_TEXT_CHANGED |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.TMP_TextSelector_B::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.ON_TEXT_CHANGED |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.VertexJitter::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexJitter.ON_TEXT_CHANGED |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.VertexShakeA::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeA.ON_TEXT_CHANGED |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.VertexShakeB::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexShakeB.ON_TEXT_CHANGED |
| `greg.SYSTEM.ON_TEXT_CHANGED` | `Il2Cpp.VertexZoom::ON_TEXT_CHANGED(UnityEngine.Object)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/VertexZoom.ON_TEXT_CHANGED |
| `greg.SYSTEM.OSK_to_KeyCode` | `Il2Cpp.OSK_Keyboard::OSK_to_KeyCode(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.OSK_to_KeyCode |
| `greg.SYSTEM.ObjectAdded` | `Il2Cpp.TrolleyTrigger::ObjectAdded(Collider, UsableObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyTrigger.ObjectAdded |
| `greg.SYSTEM.ObjectDropped` | `Il2Cpp.UsableObject::DropObject()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.DropObject |
| `greg.SYSTEM.OnAddClicked` | `Il2Cpp.ShopCartItem::OnAddClicked()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.OnAddClicked |
| `greg.SYSTEM.OnAnimationEventFootStep` | `Il2Cpp.ThirdPersonCharacter::OnAnimationEventFootStep()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.OnAnimationEventFootStep |
| `greg.SYSTEM.OnAnimatorMove` | `Il2Cpp.ThirdPersonCharacter::OnAnimatorMove()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.OnAnimatorMove |
| `greg.SYSTEM.OnApplicationQuit` | `Il2Cpp.MainGameManager::OnApplicationQuit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.OnApplicationQuit |
| `greg.SYSTEM.OnAutoRepairDropdownChanged` | `Il2Cpp.AssetManagement::OnAutoRepairDropdownChanged(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.OnAutoRepairDropdownChanged |
| `greg.SYSTEM.OnButtonClick` | `Il2Cpp.DropdownSample::OnButtonClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DropdownSample.OnButtonClick |
| `greg.SYSTEM.OnCableRemoved` | `Il2Cpp.WaypointInitializationSystem::OnCableRemoved(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.OnCableRemoved |
| `greg.SYSTEM.OnCancel` | `Il2Cpp.IUIActions::OnCancel(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnCancel |
| `greg.SYSTEM.OnCharacterSelection` | `Il2Cpp.TMP_TextEventCheck::OnCharacterSelection(char, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnCharacterSelection |
| `greg.SYSTEM.OnCharacterSelectionDispatched` | `Il2Cpp.TMP_TextEventHandler::SendOnCharacterSelection(char, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.SendOnCharacterSelection |
| `greg.SYSTEM.OnClick` | `Il2Cpp.IUIActions::OnClick(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnClick |
| `greg.SYSTEM.OnCloseMenu` | `Il2Cpp.IPlayerActions::OnCloseMenu(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnCloseMenu |
| `greg.SYSTEM.OnCollisionEnter` | `Il2Cpp.UsableObject::OnCollisionEnter(Collision)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.OnCollisionEnter |
| `greg.SYSTEM.OnCollisionEnter` | `Il2Cpp.CarController::OnCollisionEnter(Collision)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.OnCollisionEnter |
| `greg.SYSTEM.OnConsole` | `Il2Cpp.IUIActions::OnConsole(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnConsole |
| `greg.SYSTEM.OnConsoleSubmit` | `Il2Cpp.IUIActions::OnConsoleSubmit(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnConsoleSubmit |
| `greg.SYSTEM.OnControllerColliderHit` | `Il2Cpp.FirstPersonController::OnControllerColliderHit(ControllerColliderHit)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.OnControllerColliderHit |
| `greg.SYSTEM.OnCreate` | `Il2Cpp.WaypointInitializationSystem::OnCreate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.OnCreate |
| `greg.SYSTEM.OnCreateForCompiler` | `Il2Cpp.WaypointInitializationSystem::OnCreateForCompiler()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.OnCreateForCompiler |
| `greg.SYSTEM.OnCreated` | `Il2Cpp.AICharacterControl::OnCreated(UMAData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.OnCreated |
| `greg.SYSTEM.OnCreated` | `Il2Cpp.AICharacterExpressions::OnCreated(UMAData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.OnCreated |
| `greg.SYSTEM.OnCrouch` | `Il2Cpp.IPlayerActions::OnCrouch(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnCrouch |
| `greg.SYSTEM.OnDepressed` | `Il2Cpp.OSK_Key::OnDepressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnDepressed |
| `greg.SYSTEM.OnDepressed` | `Il2Cpp.OSK_UI_Key::OnDepressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnDepressed |
| `greg.SYSTEM.OnDeselect` | `Il2Cpp.ButtonExtended::OnDeselect(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnDeselect |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.AICharacterControl::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.AICharacterExpressions::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.CarryModelPool::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CarryModelPool.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.CheckIfTouchingWall::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.CommandCenter::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.CommandCenterOperator::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenterOperator.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.ComputerShop::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.FCP_Persistence::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FCP_Persistence.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.InputManager::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.LeanTweenUIElement::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.LocalisedText::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LocalisedText.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.MainGameManager::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.ModLoader::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.PatchPanel::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.SetIP::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.ShopCartItem::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.ShopItem::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.SteamManager::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.UsableObject::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.Wall::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.WaypointInitializationSystem::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.RopeMesh::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.CarController::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.OSK_AccentConsole::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.OnDestroy |
| `greg.SYSTEM.OnDestroy` | `Il2Cpp.FirstPersonController::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.OnDestroy |
| `greg.SYSTEM.OnDrag` | `Il2Cpp.OSK_UI_InputReceiver::OnDrag(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.OnDrag |
| `greg.SYSTEM.OnDrawGizmos` | `Il2Cpp.Rope::OnDrawGizmos()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.OnDrawGizmos |
| `greg.SYSTEM.OnDrawGizmos` | `Il2Cpp.HorizontalRecyclingSystem::OnDrawGizmos()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.OnDrawGizmos |
| `greg.SYSTEM.OnDrawGizmos` | `Il2Cpp.VerticalRecyclingSystem::OnDrawGizmos()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.OnDrawGizmos |
| `greg.SYSTEM.OnDrop` | `Il2Cpp.IPlayerActions::OnDrop(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnDrop |
| `greg.SYSTEM.OnFinishSubmit` | `Il2Cpp.ButtonExtended::OnFinishSubmit()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnFinishSubmit |
| `greg.SYSTEM.OnFocus` | `Il2Cpp.OSK_Receiver::OnFocus()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.OnFocus |
| `greg.SYSTEM.OnFocus` | `Il2Cpp.OSK_UI_InputReceiver::OnFocus()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.OnFocus |
| `greg.SYSTEM.OnFocusLost` | `Il2Cpp.OSK_Receiver::OnFocusLost()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.OnFocusLost |
| `greg.SYSTEM.OnFocusLost` | `Il2Cpp.OSK_UI_InputReceiver::OnFocusLost()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.OnFocusLost |
| `greg.SYSTEM.OnGUI` | `Il2Cpp.OSK_Keyboard::OnGUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.OnGUI |
| `greg.SYSTEM.OnGUI` | `Il2Cpp.OSK_UI_Keyboard::OnGUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.OnGUI |
| `greg.SYSTEM.OnGlobalStatsReceived` | `Il2Cpp.SteamStatsOnMainMenuTop::OnGlobalStatsReceived(GlobalStatsReceived_t, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamStatsOnMainMenuTop.OnGlobalStatsReceived |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.ComputerShop::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.Dumpster::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Dumpster.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.GateLever::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.Interact::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.MusicPlayer::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.PushTrolleyHandle::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PushTrolleyHandle.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.TrolleyLoadingBay::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.UsableObject::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.Wall::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.OnHoverOver |
| `greg.SYSTEM.OnHoverOver` | `Il2Cpp.WorldObjectButton::OnHoverOver()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WorldObjectButton.OnHoverOver |
| `greg.SYSTEM.OnInteract` | `Il2Cpp.IPlayerActions::OnInteract(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnInteract |
| `greg.SYSTEM.OnInventory` | `Il2Cpp.IUIActions::OnInventory(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnInventory |
| `greg.SYSTEM.OnJump` | `Il2Cpp.IPlayerActions::OnJump(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnJump |
| `greg.SYSTEM.OnKeyDepress` | `Il2Cpp.I_OSK_Key::OnKeyDepress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.OnKeyDepress |
| `greg.SYSTEM.OnKeyDepress` | `Il2Cpp.OSK_Key::OnKeyDepress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnKeyDepress |
| `greg.SYSTEM.OnKeyDepress` | `Il2Cpp.OSK_UI_Key::OnKeyDepress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnKeyDepress |
| `greg.SYSTEM.OnKeyPress` | `Il2Cpp.I_OSK_Key::OnKeyPress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.OnKeyPress |
| `greg.SYSTEM.OnKeyPress` | `Il2Cpp.OSK_Key::OnKeyPress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnKeyPress |
| `greg.SYSTEM.OnKeyPress` | `Il2Cpp.OSK_UI_Key::OnKeyPress(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnKeyPress |
| `greg.SYSTEM.OnLabel` | `Il2Cpp.IPlayerActions::OnLabel(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnLabel |
| `greg.SYSTEM.OnLineSelection` | `Il2Cpp.TMP_TextEventCheck::OnLineSelection(string, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnLineSelection |
| `greg.SYSTEM.OnLineSelectionDispatched` | `Il2Cpp.TMP_TextEventHandler::SendOnLineSelection(string, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.SendOnLineSelection |
| `greg.SYSTEM.OnLinkSelection` | `Il2Cpp.TMP_TextEventCheck::OnLinkSelection(string, string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnLinkSelection |
| `greg.SYSTEM.OnLinkSelectionDispatched` | `Il2Cpp.TMP_TextEventHandler::SendOnLinkSelection(string, string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.SendOnLinkSelection |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.CommandCenter::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.OnLoad |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.ComputerShop::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.OnLoad |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.Interact::OnLoad(InteractObjectData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.OnLoad |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.MainGameManager::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.OnLoad |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.ShopItem::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.OnLoad |
| `greg.SYSTEM.OnLoad` | `Il2Cpp.Wall::OnLoad()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.OnLoad |
| `greg.SYSTEM.OnLoadDestroy` | `Il2Cpp.UsableObject::OnLoadDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.OnLoadDestroy |
| `greg.SYSTEM.OnLoadingStarted` | `Il2Cpp.CommandCenterOperator::OnLoadingStarted()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenterOperator.OnLoadingStarted |
| `greg.SYSTEM.OnLoadingStarted` | `Il2Cpp.StaticUIElements::OnLoadingStarted()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.OnLoadingStarted |
| `greg.SYSTEM.OnLook` | `Il2Cpp.IPlayerActions::OnLook(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnLook |
| `greg.SYSTEM.OnLookPosition` | `Il2Cpp.IPlayerActions::OnLookPosition(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnLookPosition |
| `greg.SYSTEM.OnMap` | `Il2Cpp.IUIActions::OnMap(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnMap |
| `greg.SYSTEM.OnMiddleClick` | `Il2Cpp.IUIActions::OnMiddleClick(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnMiddleClick |
| `greg.SYSTEM.OnModLoad` | `Il2Cpp.IModPlugin::OnModLoad(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IModPlugin.OnModLoad |
| `greg.SYSTEM.OnModUnload` | `Il2Cpp.IModPlugin::OnModUnload()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IModPlugin.OnModUnload |
| `greg.SYSTEM.OnMouseDown` | `Il2Cpp.OSK_Key::OnMouseDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnMouseDown |
| `greg.SYSTEM.OnMouseDown` | `Il2Cpp.OSK_Receiver::OnMouseDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.OnMouseDown |
| `greg.SYSTEM.OnMouseUp` | `Il2Cpp.OSK_Key::OnMouseUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnMouseUp |
| `greg.SYSTEM.OnMouseUp` | `Il2Cpp.OSK_Receiver::OnMouseUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.OnMouseUp |
| `greg.SYSTEM.OnMove` | `Il2Cpp.IPlayerActions::OnMove(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnMove |
| `greg.SYSTEM.OnNavigate` | `Il2Cpp.IUIActions::OnNavigate(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnNavigate |
| `greg.SYSTEM.OnPause` | `Il2Cpp.IUIActions::OnPause(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnPause |
| `greg.SYSTEM.OnPhysicalKeyStroke` | `Il2Cpp.OSK_Keyboard::OnPhysicalKeyStroke(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.OnPhysicalKeyStroke |
| `greg.SYSTEM.OnPoint` | `Il2Cpp.IUIActions::OnPoint(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnPoint |
| `greg.SYSTEM.OnPointerClick` | `Il2Cpp.TMP_TextSelector_B::OnPointerClick(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnPointerClick |
| `greg.SYSTEM.OnPointerClick` | `Il2Cpp.ButtonExtended::OnPointerClick(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnPointerClick |
| `greg.SYSTEM.OnPointerDown` | `Il2Cpp.OSK_UI_CustomReceiver::OnPointerDown(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.OnPointerDown |
| `greg.SYSTEM.OnPointerDown` | `Il2Cpp.OSK_UI_Key::OnPointerDown(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnPointerDown |
| `greg.SYSTEM.OnPointerEnter` | `Il2Cpp.TMP_TextEventHandler::OnPointerEnter(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.OnPointerEnter |
| `greg.SYSTEM.OnPointerEnter` | `Il2Cpp.TMP_TextSelector_A::OnPointerEnter(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_A.OnPointerEnter |
| `greg.SYSTEM.OnPointerEnter` | `Il2Cpp.TMP_TextSelector_B::OnPointerEnter(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnPointerEnter |
| `greg.SYSTEM.OnPointerEnter` | `Il2Cpp.ButtonExtended::OnPointerEnter(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnPointerEnter |
| `greg.SYSTEM.OnPointerExit` | `Il2Cpp.TMP_TextEventHandler::OnPointerExit(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.OnPointerExit |
| `greg.SYSTEM.OnPointerExit` | `Il2Cpp.TMP_TextSelector_A::OnPointerExit(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_A.OnPointerExit |
| `greg.SYSTEM.OnPointerExit` | `Il2Cpp.TMP_TextSelector_B::OnPointerExit(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnPointerExit |
| `greg.SYSTEM.OnPointerExit` | `Il2Cpp.ButtonExtended::OnPointerExit(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnPointerExit |
| `greg.SYSTEM.OnPointerUp` | `Il2Cpp.TMP_TextSelector_B::OnPointerUp(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.OnPointerUp |
| `greg.SYSTEM.OnPointerUp` | `Il2Cpp.OSK_UI_CustomReceiver::OnPointerUp(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.OnPointerUp |
| `greg.SYSTEM.OnPointerUp` | `Il2Cpp.OSK_UI_Key::OnPointerUp(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnPointerUp |
| `greg.SYSTEM.OnPressed` | `Il2Cpp.OSK_Key::OnPressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.OnPressed |
| `greg.SYSTEM.OnPressed` | `Il2Cpp.OSK_UI_Key::OnPressed()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnPressed |
| `greg.SYSTEM.OnRemoveClicked` | `Il2Cpp.ShopCartItem::OnRemoveClicked()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.OnRemoveClicked |
| `greg.SYSTEM.OnRightClick` | `Il2Cpp.IUIActions::OnRightClick(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnRightClick |
| `greg.SYSTEM.OnScroll` | `Il2Cpp.IPlayerActions::OnScroll(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnScroll |
| `greg.SYSTEM.OnScrollWheel` | `Il2Cpp.IUIActions::OnScrollWheel(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnScrollWheel |
| `greg.SYSTEM.OnSecondAction` | `Il2Cpp.IPlayerActions::OnSecondAction(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnSecondAction |
| `greg.SYSTEM.OnSelect` | `Il2Cpp.OSK_UI_CustomReceiver::OnSelect(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.OnSelect |
| `greg.SYSTEM.OnSelect` | `Il2Cpp.ButtonExtended::OnSelect(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnSelect |
| `greg.SYSTEM.OnSprint` | `Il2Cpp.IPlayerActions::OnSprint(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnSprint |
| `greg.SYSTEM.OnSpriteSelection` | `Il2Cpp.TMP_TextEventCheck::OnSpriteSelection(char, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnSpriteSelection |
| `greg.SYSTEM.OnSpriteSelectionDispatched` | `Il2Cpp.TMP_TextEventHandler::SendOnSpriteSelection(char, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.SendOnSpriteSelection |
| `greg.SYSTEM.OnSubmit` | `Il2Cpp.IUIActions::OnSubmit(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnSubmit |
| `greg.SYSTEM.OnSubmit` | `Il2Cpp.OSK_UI_Key::OnSubmit(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.OnSubmit |
| `greg.SYSTEM.OnSubmit` | `Il2Cpp.ButtonExtended::OnSubmit(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.OnSubmit |
| `greg.SYSTEM.OnTimeControl` | `Il2Cpp.IUIActions::OnTimeControl(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnTimeControl |
| `greg.SYSTEM.OnTriggerEnter` | `Il2Cpp.TrolleyTrigger::OnTriggerEnter(Collider)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyTrigger.OnTriggerEnter |
| `greg.SYSTEM.OnUpdate` | `Il2Cpp.WaypointInitializationSystem::OnUpdate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.OnUpdate |
| `greg.SYSTEM.OnValidate` | `Il2Cpp.ActionKeyHint::OnValidate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.OnValidate |
| `greg.SYSTEM.OnValidate` | `Il2Cpp.Rope::OnValidate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.OnValidate |
| `greg.SYSTEM.OnValidate` | `Il2Cpp.RopeMesh::OnValidate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.OnValidate |
| `greg.SYSTEM.OnValueChangedListener` | `Il2Cpp.HorizontalRecyclingSystem::OnValueChangedListener(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.OnValueChangedListener |
| `greg.SYSTEM.OnValueChangedListener` | `Il2Cpp.RecyclableScrollRect::OnValueChangedListener(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.OnValueChangedListener |
| `greg.SYSTEM.OnValueChangedListener` | `Il2Cpp.RecyclingSystem::OnValueChangedListener(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclingSystem.OnValueChangedListener |
| `greg.SYSTEM.OnValueChangedListener` | `Il2Cpp.VerticalRecyclingSystem::OnValueChangedListener(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.OnValueChangedListener |
| `greg.SYSTEM.OnWaitForPressKey` | `Il2Cpp.IUIActions::OnWaitForPressKey(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IUIActions.OnWaitForPressKey |
| `greg.SYSTEM.OnWaitForPressKey` | `Il2Cpp.LeanTweenUIElement::OnWaitForPressKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.OnWaitForPressKey |
| `greg.SYSTEM.OnWordSelection` | `Il2Cpp.TMP_TextEventCheck::OnWordSelection(string, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventCheck.OnWordSelection |
| `greg.SYSTEM.OnWordSelectionDispatched` | `Il2Cpp.TMP_TextEventHandler::SendOnWordSelection(string, int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextEventHandler.SendOnWordSelection |
| `greg.SYSTEM.OnZoom` | `Il2Cpp.IPlayerActions::OnZoom(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/IPlayerActions.OnZoom |
| `greg.SYSTEM.OpenAnyCanvas` | `Il2Cpp.MainGameManager::OpenAnyCanvas()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.OpenAnyCanvas |
| `greg.SYSTEM.OpenColorPicker` | `Il2Cpp.ComputerShop::OpenColorPicker()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.OpenColorPicker |
| `greg.SYSTEM.OpenGate` | `Il2Cpp.GateLever::OpenGate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.OpenGate |
| `greg.SYSTEM.OpenURLInBrowser` | `Il2Cpp.OpenURL::OpenURLInBrowser()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OpenURL.OpenURLInBrowser |
| `greg.SYSTEM.OpenWall` | `Il2Cpp.Wall::OpenWall()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Wall.OpenWall |
| `greg.SYSTEM.OperatorsForLevelSpawned` | `Il2Cpp.CommandCenter::SpawnOperatorsForLevel(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.SpawnOperatorsForLevel |
| `greg.SYSTEM.OperatorsForSingleLevelSpawned` | `Il2Cpp.CommandCenter::SpawnOperatorsForSingleLevel(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.SpawnOperatorsForSingleLevel |
| `greg.SYSTEM.OutputSet` | `Il2Cpp.OSK_Keyboard::SetOutput(OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SetOutput |
| `greg.SYSTEM.OutputTextUpdate` | `Il2Cpp.OSK_Keyboard::OutputTextUpdate(string, OSK_Receiver)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.OutputTextUpdate |
| `greg.SYSTEM.PacketSpawnerEnabledSet` | `Il2Cpp.WaypointInitializationSystem::SetPacketSpawnerEnabled(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.SetPacketSpawnerEnabled |
| `greg.SYSTEM.ParentTheObjectWithDelay` | `Il2Cpp.TrolleyLoadingBay::ParentTheObjectWithDelay(UsableObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.ParentTheObjectWithDelay |
| `greg.SYSTEM.ParseHex` | `Il2Cpp.FlexibleColorPicker::ParseHex(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ParseHex |
| `greg.SYSTEM.ParseHex` | `Il2Cpp.FlexibleColorPicker::ParseHex(string, Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ParseHex |
| `greg.SYSTEM.ParsedText` | `Il2Cpp.OSK_Receiver::ParsedText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ParsedText |
| `greg.SYSTEM.ParsedText` | `Il2Cpp.OSK_UI_InputReceiver::ParsedText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.ParsedText |
| `greg.SYSTEM.PauseMenu.OnPauseMenuClose` | `Il2Cpp.OnPauseMenuClose::PauseMenu.OnPauseMenuClose([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuClose.PauseMenu.OnPauseMenuClose |
| `greg.SYSTEM.PauseMenu.OnPauseMenuOpen` | `Il2Cpp.OnPauseMenuOpen::PauseMenu.OnPauseMenuOpen([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnPauseMenuOpen.PauseMenu.OnPauseMenuOpen |
| `greg.SYSTEM.PerformOverlapCheck` | `Il2Cpp.CheckIfTouchingWall::PerformOverlapCheck()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.PerformOverlapCheck |
| `greg.SYSTEM.PickA` | `Il2Cpp.BufferedColor::PickA(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickA |
| `greg.SYSTEM.PickB` | `Il2Cpp.BufferedColor::PickB(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickB |
| `greg.SYSTEM.PickG` | `Il2Cpp.BufferedColor::PickG(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickG |
| `greg.SYSTEM.PickH` | `Il2Cpp.BufferedColor::PickH(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickH |
| `greg.SYSTEM.PickR` | `Il2Cpp.BufferedColor::PickR(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickR |
| `greg.SYSTEM.PickS` | `Il2Cpp.BufferedColor::PickS(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickS |
| `greg.SYSTEM.PickV` | `Il2Cpp.BufferedColor::PickV(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.PickV |
| `greg.SYSTEM.PlayEffectAudioClip` | `Il2Cpp.AudioManager::PlayEffectAudioClip(AudioClip, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.PlayEffectAudioClip |
| `greg.SYSTEM.PlayLandingSound` | `Il2Cpp.FirstPersonController::PlayLandingSound()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.PlayLandingSound |
| `greg.SYSTEM.PlayRackDoorOpen` | `Il2Cpp.AudioManager::PlayRackDoorOpen()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.PlayRackDoorOpen |
| `greg.SYSTEM.PlayRandomImpactClip` | `Il2Cpp.AudioManager::PlayRandomImpactClip(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.PlayRandomImpactClip |
| `greg.SYSTEM.PlayRandomRJ45Clip` | `Il2Cpp.AudioManager::PlayRandomRJ45Clip()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.PlayRandomRJ45Clip |
| `greg.SYSTEM.PlayRandomSong` | `Il2Cpp.MusicPlayer::PlayRandomSong()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MusicPlayer.PlayRandomSong |
| `greg.SYSTEM.PlayRequestedStepSound` | `Il2Cpp.FootSteps::PlayRequestedStepSound(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.PlayRequestedStepSound |
| `greg.SYSTEM.PlaySelectKeySound` | `Il2Cpp.OSK_KeySounds::PlaySelectKeySound()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_KeySounds.PlaySelectKeySound |
| `greg.SYSTEM.PlaySound` | `Il2Cpp.OSK_KeySounds::PlaySound(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_KeySounds.PlaySound |
| `greg.SYSTEM.PlayStepSound` | `Il2Cpp.ThirdPersonCharacter::PlayStepSound()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/ThirdPersonCharacter.PlayStepSound |
| `greg.SYSTEM.PlayerManager.OnBuyingWall` | `Il2Cpp.OnBuyingWall::PlayerManager.OnBuyingWall([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnBuyingWall.PlayerManager.OnBuyingWall |
| `greg.SYSTEM.PointerDown` | `Il2Cpp.viperInput::PointerDown(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.PointerDown |
| `greg.SYSTEM.PointerFocusSet` | `Il2Cpp.FlexibleColorPicker::SetPointerFocus(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SetPointerFocus |
| `greg.SYSTEM.PointerUp` | `Il2Cpp.viperInput::PointerUp(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.PointerUp |
| `greg.SYSTEM.PointerUpdate` | `Il2Cpp.FlexibleColorPicker::PointerUpdate(BaseEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.PointerUpdate |
| `greg.SYSTEM.PopulateAutoRepairDropdown` | `Il2Cpp.AssetManagement::PopulateAutoRepairDropdown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.PopulateAutoRepairDropdown |
| `greg.SYSTEM.PowerButton` | `Il2Cpp.SetIP::PowerButton()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.PowerButton |
| `greg.SYSTEM.Prep` | `Il2Cpp.OSK_Keyboard::Prep()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Prep |
| `greg.SYSTEM.PrepAssetGroup` | `Il2Cpp.OSK_UI_Keyboard::PrepAssetGroup()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.PrepAssetGroup |
| `greg.SYSTEM.Press` | `Il2Cpp.ButtonExtended::Press()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/ButtonExtended.Press |
| `greg.SYSTEM.ProgressStepCycle` | `Il2Cpp.FirstPersonController::ProgressStepCycle(float)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.ProgressStepCycle |
| `greg.SYSTEM.RGBToHSV` | `Il2Cpp.FlexibleColorPicker::RGBToHSV(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.RGBToHSV |
| `greg.SYSTEM.RGBToHSV` | `Il2Cpp.FlexibleColorPicker::RGBToHSV(float, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.RGBToHSV |
| `greg.SYSTEM.RacksVolumeSet` | `Il2Cpp.AudioManager::SetRacksVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AudioManager.SetRacksVolume |
| `greg.SYSTEM.ReHighlightKey` | `Il2Cpp.OSK_Keyboard::ReHighlightKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.ReHighlightKey |
| `greg.SYSTEM.RecalculateRope` | `Il2Cpp.Rope::RecalculateRope()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.RecalculateRope |
| `greg.SYSTEM.RecycleBottomToTop` | `Il2Cpp.VerticalRecyclingSystem::RecycleBottomToTop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.RecycleBottomToTop |
| `greg.SYSTEM.RecycleLeftToRight` | `Il2Cpp.HorizontalRecyclingSystem::RecycleLeftToRight()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.RecycleLeftToRight |
| `greg.SYSTEM.RecycleRightToleft` | `Il2Cpp.HorizontalRecyclingSystem::RecycleRightToleft()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.RecycleRightToleft |
| `greg.SYSTEM.RecycleTopToBottom` | `Il2Cpp.VerticalRecyclingSystem::RecycleTopToBottom()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.RecycleTopToBottom |
| `greg.SYSTEM.RecyclingBoundsSet` | `Il2Cpp.HorizontalRecyclingSystem::SetRecyclingBounds()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/HorizontalRecyclingSystem.SetRecyclingBounds |
| `greg.SYSTEM.RecyclingBoundsSet` | `Il2Cpp.VerticalRecyclingSystem::SetRecyclingBounds()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.SetRecyclingBounds |
| `greg.SYSTEM.Register` | `Il2Cpp.DeviceTimerManager::Register(ITimedDevice)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DeviceTimerManager.Register |
| `greg.SYSTEM.RegisterCableInNetworkMap` | `Il2Cpp.WaypointInitializationSystem::RegisterCableInNetworkMap(WaypointInitializationSystem.CableInfo)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.RegisterCableInNetworkMap |
| `greg.SYSTEM.RegisterKeyStrokeCallback` | `Il2Cpp.viperInput::RegisterKeyStrokeCallback(Il2CppSystem.Action<char>, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.RegisterKeyStrokeCallback |
| `greg.SYSTEM.ReloadData` | `Il2Cpp.RecyclableScrollRect::ReloadData()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.ReloadData |
| `greg.SYSTEM.ReloadData` | `Il2Cpp.RecyclableScrollRect::ReloadData(IRecyclableScrollRectDataSource)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect.ReloadData |
| `greg.SYSTEM.RemapPhysicalKeyboard` | `Il2Cpp.OSK_Keyboard::RemapPhysicalKeyboard()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.RemapPhysicalKeyboard |
| `greg.SYSTEM.RemapPhysicalKeyboard` | `Il2Cpp.OSK_UI_Keyboard::RemapPhysicalKeyboard()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.RemapPhysicalKeyboard |
| `greg.SYSTEM.RenderersEnabledSet` | `Il2Cpp.CheckIfTouchingWall::SetRenderersEnabled(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CheckIfTouchingWall.SetRenderersEnabled |
| `greg.SYSTEM.RequestRouteEvaluation` | `Il2Cpp.WaypointInitializationSystem::RequestRouteEvaluation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.RequestRouteEvaluation |
| `greg.SYSTEM.Reset` | `Il2Cpp.Enumerator::Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Enumerator.Reset |
| `greg.SYSTEM.Reset` | `Il2Cpp.OSK_AccentConsole::Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.Reset |
| `greg.SYSTEM.Reset` | `Il2Cpp.OSK_Keyboard::Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Reset |
| `greg.SYSTEM.Reset` | `Il2Cpp.OSK_MiniKeyboard::Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.Reset |
| `greg.SYSTEM.Reset` | `Il2Cpp.OSK_UI_Keyboard::Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Reset |
| `greg.SYSTEM.ResetAllAxis` | `Il2Cpp.viperInput::ResetAllAxis()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperTools/viperInput.ResetAllAxis |
| `greg.SYSTEM.ResetAllSlots` | `Il2Cpp.TrolleyLoadingBay::ResetAllSlots()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TrolleyLoadingBay.ResetAllSlots |
| `greg.SYSTEM.ResetAllSpawners` | `Il2Cpp.WaypointInitializationSystem::ResetAllSpawners()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.ResetAllSpawners |
| `greg.SYSTEM.ResetBinding` | `Il2Cpp.InputManager::ResetBinding(string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.ResetBinding |
| `greg.SYSTEM.ResetCameraPosition` | `Il2Cpp.FirstPersonController::ResetCameraPosition()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.ResetCameraPosition |
| `greg.SYSTEM.ResetHold` | `Il2Cpp.RayLookAt::ResetHold()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt.ResetHold |
| `greg.SYSTEM.ResetRotation` | `Il2Cpp.MouseLook::ResetRotation(Transform)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.ResetRotation |
| `greg.SYSTEM.ResetTrolleyPosition` | `Il2Cpp.MainGameManager::ResetTrolleyPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ResetTrolleyPosition |
| `greg.SYSTEM.ResetTrolleyPosition` | `Il2Cpp.CarController::ResetTrolleyPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.ResetTrolleyPosition |
| `greg.SYSTEM.ResetingTrollerPosition` | `Il2Cpp.CarController::ResetingTrollerPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.ResetingTrollerPosition |
| `greg.SYSTEM.ResizeBackground` | `Il2Cpp.OSK_MiniKeyboard::ResizeBackground()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.ResizeBackground |
| `greg.SYSTEM.ResizeKeyToFit` | `Il2Cpp.OSK_Keyboard::ResizeKeyToFit(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.ResizeKeyToFit |
| `greg.SYSTEM.ResizeKeyToFit` | `Il2Cpp.OSK_UI_Keyboard::ResizeKeyToFit(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.ResizeKeyToFit |
| `greg.SYSTEM.ResizeToFit` | `Il2Cpp.OSK_Background::ResizeToFit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Background.ResizeToFit |
| `greg.SYSTEM.RestartAutoSave` | `Il2Cpp.MainGameManager::RestartAutoSave()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.RestartAutoSave |
| `greg.SYSTEM.RestoreCachedVertexAttributes` | `Il2Cpp.TMP_TextSelector_B::RestoreCachedVertexAttributes(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_TextSelector_B.RestoreCachedVertexAttributes |
| `greg.SYSTEM.RestorePreviousSelection` | `Il2Cpp.StaticUIElements::RestorePreviousSelection()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.RestorePreviousSelection |
| `greg.SYSTEM.RestoreRigidbody` | `Il2Cpp.UsableObject::RestoreRigidbody()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.RestoreRigidbody |
| `greg.SYSTEM.Return` | `Il2Cpp.CarryModelPool::Return(GameObject, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CarryModelPool.Return |
| `greg.SYSTEM.ReturnServerNameFromType` | `Il2Cpp.MainGameManager::ReturnServerNameFromType(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ReturnServerNameFromType |
| `greg.SYSTEM.ReturnSubnet` | `Il2Cpp.MainGameManager::ReturnSubnet(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ReturnSubnet |
| `greg.SYSTEM.ReturnSwitchNameFromType` | `Il2Cpp.MainGameManager::ReturnSwitchNameFromType(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ReturnSwitchNameFromType |
| `greg.SYSTEM.ReturnTextByID` | `Il2Cpp.Localisation::ReturnTextByID(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Localisation.ReturnTextByID |
| `greg.SYSTEM.ReturnVlanId` | `Il2Cpp.MainGameManager::ReturnVlanId(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ReturnVlanId |
| `greg.SYSTEM.RevealCharacters` | `Il2Cpp.TextConsoleSimulator::RevealCharacters(TMP_Text)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.RevealCharacters |
| `greg.SYSTEM.RevealWords` | `Il2Cpp.TextConsoleSimulator::RevealWords(TMP_Text)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TextConsoleSimulator.RevealWords |
| `greg.SYSTEM.RigidbodyRemoved` | `Il2Cpp.UsableObject::RemoveRigidbody()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject.RemoveRigidbody |
| `greg.SYSTEM.RotateView` | `Il2Cpp.FirstPersonController::RotateView()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.RotateView |
| `greg.SYSTEM.Run` | `Il2Cpp.UpdatePacketsJob::Run()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Run |
| `greg.SYSTEM.Run` | `Il2Cpp.UpdatePacketsJob::Run(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Run |
| `greg.SYSTEM.Run` | `Il2Cpp.InternalCompilerQueryAndHandleData::Run(PacketSpawnerSystem.UpdatePacketsJob, EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InternalCompilerQueryAndHandleData.Run |
| `greg.SYSTEM.RunByRef` | `Il2Cpp.UpdatePacketsJob::RunByRef()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.RunByRef |
| `greg.SYSTEM.RunByRef` | `Il2Cpp.UpdatePacketsJob::RunByRef(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.RunByRef |
| `greg.SYSTEM.SafelyDisposeSpawner` | `Il2Cpp.WaypointInitializationSystem::SafelyDisposeSpawner(Entity, int, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.SafelyDisposeSpawner |
| `greg.SYSTEM.Schedule` | `Il2Cpp.UpdatePacketsJob::Schedule(JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Schedule |
| `greg.SYSTEM.Schedule` | `Il2Cpp.UpdatePacketsJob::Schedule(EntityQuery, JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Schedule |
| `greg.SYSTEM.Schedule` | `Il2Cpp.UpdatePacketsJob::Schedule()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Schedule |
| `greg.SYSTEM.Schedule` | `Il2Cpp.UpdatePacketsJob::Schedule(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.Schedule |
| `greg.SYSTEM.ScheduleByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleByRef(JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleByRef |
| `greg.SYSTEM.ScheduleByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleByRef(EntityQuery, JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleByRef |
| `greg.SYSTEM.ScheduleByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleByRef()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleByRef |
| `greg.SYSTEM.ScheduleByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleByRef(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleByRef |
| `greg.SYSTEM.ScheduleParallel` | `Il2Cpp.UpdatePacketsJob::ScheduleParallel(JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallel |
| `greg.SYSTEM.ScheduleParallel` | `Il2Cpp.UpdatePacketsJob::ScheduleParallel(EntityQuery, JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallel |
| `greg.SYSTEM.ScheduleParallel` | `Il2Cpp.UpdatePacketsJob::ScheduleParallel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallel |
| `greg.SYSTEM.ScheduleParallel` | `Il2Cpp.UpdatePacketsJob::ScheduleParallel(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallel |
| `greg.SYSTEM.ScheduleParallelByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleParallelByRef(JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallelByRef |
| `greg.SYSTEM.ScheduleParallelByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleParallelByRef(EntityQuery, JobHandle)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallelByRef |
| `greg.SYSTEM.ScheduleParallelByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleParallelByRef()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallelByRef |
| `greg.SYSTEM.ScheduleParallelByRef` | `Il2Cpp.UpdatePacketsJob::ScheduleParallelByRef(EntityQuery)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.ScheduleParallelByRef |
| `greg.SYSTEM.ScrollAuto` | `Il2Cpp.AutoScrollRect::ScrollAuto()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AutoScrollRect.ScrollAuto |
| `greg.SYSTEM.SecondActionOnClick` | `Il2Cpp.Interact::SecondActionOnClick()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact.SecondActionOnClick |
| `greg.SYSTEM.SelectKey` | `Il2Cpp.OSK_UI_Keyboard::SelectKey(OSK_UI_Key)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SelectKey |
| `greg.SYSTEM.SelectNextAvailable` | `Il2Cpp.ComputerShop::SelectNextAvailable(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.SelectNextAvailable |
| `greg.SYSTEM.SelectSound` | `Il2Cpp.OSK_Keyboard::SelectSound()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SelectSound |
| `greg.SYSTEM.SelectedFirstKey` | `Il2Cpp.OSK_MiniKeyboard::SelectedFirstKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.SelectedFirstKey |
| `greg.SYSTEM.SelectedKey` | `Il2Cpp.OSK_UI_Keyboard::SelectedKey()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SelectedKey |
| `greg.SYSTEM.SelectedKeyMove` | `Il2Cpp.OSK_MiniKeyboard::SelectedKeyMove(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_MiniKeyboard.SelectedKeyMove |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_GamepadHelper::SetSelectedKey(OSK_Key)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_GamepadHelper::SetSelectedKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_GamepadHelper.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_Keyboard::SetSelectedKey(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_Keyboard::SetSelectedKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_UI_Keyboard::SetSelectedKey(OSK_KeyCode)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_UI_Keyboard::SetSelectedKey(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SetSelectedKey |
| `greg.SYSTEM.SelectedKeySet` | `Il2Cpp.OSK_UI_Keyboard::SetSelectedKey(OSK_UI_Key)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SetSelectedKey |
| `greg.SYSTEM.Selection` | `Il2Cpp.OSK_Receiver::Selection(Vector3, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Selection |
| `greg.SYSTEM.Selection` | `Il2Cpp.OSK_UI_CustomReceiver::Selection(Vector3, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.Selection |
| `greg.SYSTEM.SelectionEnd` | `Il2Cpp.OSK_UI_InputReceiver::SelectionEnd()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.SelectionEnd |
| `greg.SYSTEM.SelectionHighlight` | `Il2Cpp.OSK_Receiver::SelectionHighlight(Color32, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.SelectionHighlight |
| `greg.SYSTEM.SelectionHighlight` | `Il2Cpp.OSK_UI_CustomReceiver::SelectionHighlight(Color32, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_CustomReceiver.SelectionHighlight |
| `greg.SYSTEM.SeperateMaterials` | `Il2Cpp.FlexibleColorPicker::SeperateMaterials()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SeperateMaterials |
| `greg.SYSTEM.ServerCustomerIDChanged` | `Il2Cpp.WaypointInitializationSystem::UpdateServerCustomerID(string, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.UpdateServerCustomerID |
| `greg.SYSTEM.Set` | `Il2Cpp.BufferedColor::Set(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.Set |
| `greg.SYSTEM.Set` | `Il2Cpp.BufferedColor::Set(Color, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BufferedColor.Set |
| `greg.SYSTEM.Set` | `Il2Cpp.OSK_AccentConsole::Set(OSK_LongPressPacket)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.Set |
| `greg.SYSTEM.SharedStaticTypeIndicesSet` | `Il2Cpp.AssemblyTypeRegistry::SetSharedStaticTypeIndices(int*, int)` | `Postfix` | Auto-generated from IL2CPP sources: Unity/AssemblyTypeRegistry.SetSharedStaticTypeIndices |
| `greg.SYSTEM.ShiftColor` | `Il2Cpp.FlexibleColorPicker::ShiftColor(int, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ShiftColor |
| `greg.SYSTEM.ShiftDown` | `Il2Cpp.OSK_Key::ShiftDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.ShiftDown |
| `greg.SYSTEM.ShiftDown` | `Il2Cpp.OSK_UI_Key::ShiftDown()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.ShiftDown |
| `greg.SYSTEM.ShiftHue` | `Il2Cpp.FlexibleColorPicker::ShiftHue(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.ShiftHue |
| `greg.SYSTEM.ShiftUp` | `Il2Cpp.OSK_Key::ShiftUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.ShiftUp |
| `greg.SYSTEM.ShiftUp` | `Il2Cpp.OSK_UI_Key::ShiftUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.ShiftUp |
| `greg.SYSTEM.ShopItemLoaded` | `Il2Cpp.ModLoader::LoadShopItem(string, string, ShopItemConfig)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadShopItem |
| `greg.SYSTEM.Show` | `Il2Cpp.I_OSK_Cursor::Show(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Cursor.Show |
| `greg.SYSTEM.Show` | `Il2Cpp.OSK_Cursor::Show(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Cursor.Show |
| `greg.SYSTEM.Show` | `Il2Cpp.OSK_UI_Cursor::Show(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Cursor.Show |
| `greg.SYSTEM.ShowBackground` | `Il2Cpp.OSK_AccentConsole::ShowBackground(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_AccentConsole.ShowBackground |
| `greg.SYSTEM.ShowBuyWallCanvas` | `Il2Cpp.MainGameManager::ShowBuyWallCanvas(Wall)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ShowBuyWallCanvas |
| `greg.SYSTEM.ShowCanvas` | `Il2Cpp.SetIP::ShowCanvas(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP.ShowCanvas |
| `greg.SYSTEM.ShowCustomerCardsCanvas` | `Il2Cpp.MainGameManager::ShowCustomerCardsCanvas(CustomerBaseDoor)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ShowCustomerCardsCanvas |
| `greg.SYSTEM.ShowError` | `Il2Cpp.UserReport::ShowError()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.ShowError |
| `greg.SYSTEM.ShowHideKeyboard` | `Il2Cpp.OSK_UI_Keyboard::ShowHideKeyboard(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.ShowHideKeyboard |
| `greg.SYSTEM.ShowNetworkConfigCanvas` | `Il2Cpp.MainGameManager::ShowNetworkConfigCanvas(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ShowNetworkConfigCanvas |
| `greg.SYSTEM.ShowSpriteNextToPointer` | `Il2Cpp.StaticUIElements::ShowSpriteNextToPointer(Sprite)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ShowSpriteNextToPointer |
| `greg.SYSTEM.ShowStaticCanvas` | `Il2Cpp.StaticUIElements::ShowStaticCanvas(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ShowStaticCanvas |
| `greg.SYSTEM.ShowTextUnderCursor` | `Il2Cpp.StaticUIElements::ShowTextUnderCursor(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/StaticUIElements.ShowTextUnderCursor |
| `greg.SYSTEM.ShuffleAvailableCustomers` | `Il2Cpp.MainGameManager::ShuffleAvailableCustomers()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ShuffleAvailableCustomers |
| `greg.SYSTEM.ShuffleAvailableSubnets` | `Il2Cpp.MainGameManager::ShuffleAvailableSubnets()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.ShuffleAvailableSubnets |
| `greg.SYSTEM.SimulatePhysics` | `Il2Cpp.Rope::SimulatePhysics()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.SimulatePhysics |
| `greg.SYSTEM.SimulatePhysics` | `Il2Cpp.RopeWindEffect::SimulatePhysics()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeWindEffect.SimulatePhysics |
| `greg.SYSTEM.SittingClampRotation` | `Il2Cpp.MouseLook::SittingClampRotation(Vector2)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook.SittingClampRotation |
| `greg.SYSTEM.SliderUpdate` | `Il2Cpp.FlexibleColorPicker::SliderUpdate(FlexibleColorPicker.PickerType, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.SliderUpdate |
| `greg.SYSTEM.SpanBottomRight` | `Il2Cpp.OSK_Keyboard::SpanBottomRight()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SpanBottomRight |
| `greg.SYSTEM.SpanBottomRight` | `Il2Cpp.OSK_UI_Keyboard::SpanBottomRight()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SpanBottomRight |
| `greg.SYSTEM.SpanTopLeft` | `Il2Cpp.OSK_Keyboard::SpanTopLeft()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.SpanTopLeft |
| `greg.SYSTEM.SpanTopLeft` | `Il2Cpp.OSK_UI_Keyboard::SpanTopLeft()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.SpanTopLeft |
| `greg.SYSTEM.SpawnedItemAdded` | `Il2Cpp.ShopCartItem::AddSpawnedItem(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopCartItem.AddSpawnedItem |
| `greg.SYSTEM.SpawnedItemRemoved` | `Il2Cpp.ComputerShop::RemoveSpawnedItem(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.RemoveSpawnedItem |
| `greg.SYSTEM.SplinePointSet` | `Il2Cpp.Rope::SetSplinePoint()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.SetSplinePoint |
| `greg.SYSTEM.StartGodMod` | `Il2Cpp.GODMOD::StartGodMod()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GODMOD.StartGodMod |
| `greg.SYSTEM.StartPointSet` | `Il2Cpp.Rope::SetStartPoint(Transform, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.SetStartPoint |
| `greg.SYSTEM.StartingAnimation` | `Il2Cpp.AICharacterControl::StartingAnimation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.StartingAnimation |
| `greg.SYSTEM.StateMachineSet` | `Il2Cpp._Start_d__15::SetStateMachine(IAsyncStateMachine)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__15.SetStateMachine |
| `greg.SYSTEM.StaticChanged` | `Il2Cpp.FlexibleColorPicker::UpdateStatic(FlexibleColorPicker.PickerType)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateStatic |
| `greg.SYSTEM.StaticItemLoaded` | `Il2Cpp.ModLoader::LoadStaticItem(string, string, StaticItemConfig)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadStaticItem |
| `greg.SYSTEM.SteamAPIDebugTextHook` | `Il2Cpp.SteamManager::SteamAPIDebugTextHook(int, StringBuilder)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamManager.SteamAPIDebugTextHook |
| `greg.SYSTEM.Steer` | `Il2Cpp.CarController::Steer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.Steer |
| `greg.SYSTEM.Step` | `Il2Cpp.FootSteps::Step()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.Step |
| `greg.SYSTEM.StopCar` | `Il2Cpp.CarController::StopCar()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.StopCar |
| `greg.SYSTEM.StopCrouching` | `Il2Cpp.FirstPersonController::StopCrouching()` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController.StopCrouching |
| `greg.SYSTEM.StopLoopingDestinationPointsSet` | `Il2Cpp.AICharacterControl::SetStopLoopingDestinationPoints()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.SetStopLoopingDestinationPoints |
| `greg.SYSTEM.StringAdded` | `Il2Cpp.OSK_Keyboard::AddString(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AddString |
| `greg.SYSTEM.StripGameplayComponents` | `Il2Cpp.CarryModelPool::StripGameplayComponents(GameObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CarryModelPool.StripGameplayComponents |
| `greg.SYSTEM.Submit` | `Il2Cpp.OSK_Keyboard::Submit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Submit |
| `greg.SYSTEM.Submit` | `Il2Cpp.OSK_Receiver::Submit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Submit |
| `greg.SYSTEM.Submit` | `Il2Cpp.OSK_UI_InputReceiver::Submit()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Submit |
| `greg.SYSTEM.SubmitUserReport` | `Il2Cpp.UserReport::SubmitUserReport()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.SubmitUserReport |
| `greg.SYSTEM.SubscribeToRopeEvents` | `Il2Cpp.RopeMesh::SubscribeToRopeEvents()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.SubscribeToRopeEvents |
| `greg.SYSTEM.SupportGlyphs` | `Il2Cpp.OSK_Keymap::SupportGlyphs(OSK_LanguagePackage)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keymap.SupportGlyphs |
| `greg.SYSTEM.SyncWorkshopThenLoadAll` | `Il2Cpp.ModLoader::SyncWorkshopThenLoadAll()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.SyncWorkshopThenLoadAll |
| `greg.SYSTEM.System.OnLoadingDataLaterSaved` | `Il2Cpp.OnLoadingDataLater::SaveSystem.OnLoadingDataLater([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingDataLater.SaveSystem.OnLoadingDataLater |
| `greg.SYSTEM.System.OnLoadingDataSaved` | `Il2Cpp.OnLoadingData::SaveSystem.OnLoadingData([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnLoadingData.SaveSystem.OnLoadingData |
| `greg.SYSTEM.System.OnSavingDataSaved` | `Il2Cpp.OnSavingData::SaveSystem.OnSavingData([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnSavingData.SaveSystem.OnSavingData |
| `greg.SYSTEM.System_Collections_IEnumerable_GetEnumerator` | `Il2Cpp.InputController::System_Collections_IEnumerable_GetEnumerator()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputController.System_Collections_IEnumerable_GetEnumerator |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DelayedUpdateUI_d__12::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedUpdateUI_d__12.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Start_d__30::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__30.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Talking_d__7::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Talking_d__7.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._FadeIn_d__33::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeIn_d__33.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._FadeOut_d__32::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_d__32.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._FadeOut_FadeIn_d__34::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_FadeIn_d__34.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._TurnOffAfterXseconds_d__4::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOffAfterXseconds_d__4.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._TrackFinances_d__18::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TrackFinances_d__18.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DelayedOverlapCheck_d__6::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedOverlapCheck_d__6.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AutoRepairRoutine_d__22::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoRepairRoutine_d__22.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._CheckIfAppRequirementsAreMet_d__37::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfAppRequirementsAreMet_d__37.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DelayedAppDoorOpening_d__48::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedAppDoorOpening_d__48.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._UpdateMoney_d__38::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMoney_d__38.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._TimerLoop_d__7::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TimerLoop_d__7.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Start_d__4::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__4.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._checkGroundMaterial_d__21::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_checkGroundMaterial_d__21.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._GateCoroutine_d__15::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GateCoroutine_d__15.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DelayedLoad_d__7::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedLoad_d__7.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._GODMOD_delayed_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GODMOD_delayed_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Disabling_d__21::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Disabling_d__21.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._KeepRotating_d__25::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_KeepRotating_d__25.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._TweenScaleInOut_d__24::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TweenScaleInOut_d__24.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AsynchronousLoad_d__17::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousLoad_d__17.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AsynchronousUnLoad_d__18::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousUnLoad_d__18.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LoadGameLoadScene_d__13::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadGameLoadScene_d__13.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LoadPlayerAndNPCDataWithDelay_d__12::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadPlayerAndNPCDataWithDelay_d__12.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Start_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AutoSaveCoroutine_d__94::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoSaveCoroutine_d__94.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._SyncWorkshopThenLoadAll_d__11::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SyncWorkshopThenLoadAll_d__11.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._EffectOnDestroy_d__27::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_EffectOnDestroy_d__27.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LoadWithOverlay_d__42::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadWithOverlay_d__42.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._TurnOnCharacterControllerDelayed_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOnCharacterControllerDelayed_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._WaitForActionToFinish_d__30::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitForActionToFinish_d__30.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._UnmountRack_d__18::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UnmountRack_d__18.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._CullLoop_d__13::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullLoop_d__13.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DelayedTrigger_d__8::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedTrigger_d__8.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._InstallRack_d__6::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InstallRack_d__6.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._InsertItemInRack_d__13::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InsertItemInRack_d__13.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisableGameObjectWithDelay_d__6::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableGameObjectWithDelay_d__6.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ImageScrollingUI_d__3::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ImageScrollingUI_d__3.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._NumberScrollingUI_d__2::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_NumberScrollingUI_d__2.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AvailableRefreshRatesAfterFrame_d__35::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AvailableRefreshRatesAfterFrame_d__35.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._MoveToMonitorCoroutine_d__52::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MoveToMonitorCoroutine_d__52.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisableOnAfterFirstSettingUp_d__5::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableOnAfterFirstSettingUp_d__5.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ParentTheObjectWithDelay_d__6::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__6.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._SlideIntoPort_d__12::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SlideIntoPort_d__12.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._UpdateCoinsAndPrestige_TopLeft_d__66::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateCoinsAndPrestige_TopLeft_d__66.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._UpdateMessagesCoroutine_d__73::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMessagesCoroutine_d__73.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._WaitAndDisplay_d__5::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitAndDisplay_d__5.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._GettingNewServer_d__39::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GettingNewServer_d__39.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ReplacingServer_d__40::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ReplacingServer_d__40.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._RequestJobDelayed_d__34::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_RequestJobDelayed_d__34.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._SendToContainer_d__38::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SendToContainer_d__38.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._SetHandIKWeight_d__46::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SetHandIKWeight_d__46.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._StartTextingAnimation_d__37::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_StartTextingAnimation_d__37.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ThrowingOutServer_d__41::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ThrowingOutServer_d__41.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ProcessDispatchQueue_d__23::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ProcessDispatchQueue_d__23.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ParentTheObjectWithDelay_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ObjectAdded_d__3::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ObjectAdded_d__3.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._CheckIfLost_d__60::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfLost_d__60.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisalowDrop_d__56::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisalowDrop_d__56.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DistanceKinematicCheck_d__49::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DistanceKinematicCheck_d__49.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._MakeInteractableAgain_d__53::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MakeInteractableAgain_d__53.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ShowError_d__25::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ShowError_d__25.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LoadNetworkStateCoroutine_d__15::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadNetworkStateCoroutine_d__15.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp.___LoadNetworkStateCoroutine_592A83D2_d__50::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/___LoadNetworkStateCoroutine_592A83D2_d__50.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._CullRoutine_d__3::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullRoutine_d__3.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._InitCoroutine_d__15::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/_InitCoroutine_d__15.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisableTheTriggerColliderAfterDealy_d__44::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_DisableTheTriggerColliderAfterDealy_d__44.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ResetingTrollerPosition_d__41::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_ResetingTrollerPosition_d__41.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Start_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AnimateProperties_d__6::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateProperties_d__6.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._WarpText_d__7::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__7.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._Start_d__4::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__4.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._RevealCharacters_d__7::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealCharacters_d__7.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._RevealWords_d__8::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealWords_d__8.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisplayTextMeshFloatingText_d__16::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshFloatingText_d__16.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._DisplayTextMeshProFloatingText_d__15::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshProFloatingText_d__15.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AnimateVertexColors_d__3::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__3.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AnimateVertexColors_d__11::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__11.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._AnimateVertexColors_d__10::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__10.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._WarpText_d__8::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__8.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._GenerateCoroutine_d__21::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_GenerateCoroutine_d__21.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._BlinkCoroutine_d__14::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_BlinkCoroutine_d__14.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ClickCoroutine_d__41::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__41.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LongPressCheck_d__35::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__35.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ReHighlightKey_d__68::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ReHighlightKey_d__68.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._ClickCoroutine_d__43::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__43.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._LongPressCheck_d__37::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__37.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._SelectKey_d__21::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_SelectKey_d__21.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_Collections_IEnumerator_Reset` | `Il2Cpp._OnFinishSubmit_d__25::System_Collections_IEnumerator_Reset()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/_OnFinishSubmit_d__25.System_Collections_IEnumerator_Reset |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DelayedUpdateUI_d__12::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedUpdateUI_d__12.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Start_d__30::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__30.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Talking_d__7::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Talking_d__7.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._FadeIn_d__33::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeIn_d__33.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._FadeOut_d__32::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_d__32.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._FadeOut_FadeIn_d__34::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_FadeOut_FadeIn_d__34.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._TurnOffAfterXseconds_d__4::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOffAfterXseconds_d__4.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._TrackFinances_d__18::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TrackFinances_d__18.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DelayedOverlapCheck_d__6::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedOverlapCheck_d__6.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AutoRepairRoutine_d__22::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoRepairRoutine_d__22.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._CheckIfAppRequirementsAreMet_d__37::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfAppRequirementsAreMet_d__37.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DelayedAppDoorOpening_d__48::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedAppDoorOpening_d__48.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._UpdateMoney_d__38::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMoney_d__38.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._TimerLoop_d__7::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TimerLoop_d__7.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Start_d__4::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__4.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._checkGroundMaterial_d__21::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_checkGroundMaterial_d__21.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._GateCoroutine_d__15::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GateCoroutine_d__15.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DelayedLoad_d__7::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedLoad_d__7.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._GODMOD_delayed_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GODMOD_delayed_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Disabling_d__21::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Disabling_d__21.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._KeepRotating_d__25::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_KeepRotating_d__25.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._TweenScaleInOut_d__24::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TweenScaleInOut_d__24.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AsynchronousLoad_d__17::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousLoad_d__17.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AsynchronousUnLoad_d__18::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AsynchronousUnLoad_d__18.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LoadGameLoadScene_d__13::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadGameLoadScene_d__13.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LoadPlayerAndNPCDataWithDelay_d__12::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadPlayerAndNPCDataWithDelay_d__12.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Start_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Start_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AutoSaveCoroutine_d__94::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AutoSaveCoroutine_d__94.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._SyncWorkshopThenLoadAll_d__11::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SyncWorkshopThenLoadAll_d__11.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._EffectOnDestroy_d__27::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_EffectOnDestroy_d__27.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LoadWithOverlay_d__42::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadWithOverlay_d__42.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._TurnOnCharacterControllerDelayed_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_TurnOnCharacterControllerDelayed_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._WaitForActionToFinish_d__30::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitForActionToFinish_d__30.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._UnmountRack_d__18::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UnmountRack_d__18.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._CullLoop_d__13::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullLoop_d__13.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DelayedTrigger_d__8::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DelayedTrigger_d__8.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._InstallRack_d__6::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InstallRack_d__6.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._InsertItemInRack_d__13::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_InsertItemInRack_d__13.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisableGameObjectWithDelay_d__6::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableGameObjectWithDelay_d__6.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ImageScrollingUI_d__3::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ImageScrollingUI_d__3.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._NumberScrollingUI_d__2::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_NumberScrollingUI_d__2.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AvailableRefreshRatesAfterFrame_d__35::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_AvailableRefreshRatesAfterFrame_d__35.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._MoveToMonitorCoroutine_d__52::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MoveToMonitorCoroutine_d__52.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisableOnAfterFirstSettingUp_d__5::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisableOnAfterFirstSettingUp_d__5.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ParentTheObjectWithDelay_d__6::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__6.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._SlideIntoPort_d__12::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SlideIntoPort_d__12.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._UpdateCoinsAndPrestige_TopLeft_d__66::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateCoinsAndPrestige_TopLeft_d__66.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._UpdateMessagesCoroutine_d__73::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_UpdateMessagesCoroutine_d__73.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._WaitAndDisplay_d__5::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_WaitAndDisplay_d__5.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._GettingNewServer_d__39::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_GettingNewServer_d__39.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ReplacingServer_d__40::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ReplacingServer_d__40.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._RequestJobDelayed_d__34::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_RequestJobDelayed_d__34.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._SendToContainer_d__38::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SendToContainer_d__38.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._SetHandIKWeight_d__46::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_SetHandIKWeight_d__46.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._StartTextingAnimation_d__37::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_StartTextingAnimation_d__37.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ThrowingOutServer_d__41::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ThrowingOutServer_d__41.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ProcessDispatchQueue_d__23::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ProcessDispatchQueue_d__23.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ParentTheObjectWithDelay_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ParentTheObjectWithDelay_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ObjectAdded_d__3::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ObjectAdded_d__3.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._CheckIfLost_d__60::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CheckIfLost_d__60.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisalowDrop_d__56::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DisalowDrop_d__56.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DistanceKinematicCheck_d__49::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_DistanceKinematicCheck_d__49.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._MakeInteractableAgain_d__53::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_MakeInteractableAgain_d__53.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ShowError_d__25::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_ShowError_d__25.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LoadNetworkStateCoroutine_d__15::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_LoadNetworkStateCoroutine_d__15.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp.___LoadNetworkStateCoroutine_592A83D2_d__50::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/___LoadNetworkStateCoroutine_592A83D2_d__50.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._CullRoutine_d__3::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_CullRoutine_d__3.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._InitCoroutine_d__15::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/_InitCoroutine_d__15.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisableTheTriggerColliderAfterDealy_d__44::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_DisableTheTriggerColliderAfterDealy_d__44.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ResetingTrollerPosition_d__41::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/_ResetingTrollerPosition_d__41.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Start_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AnimateProperties_d__6::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateProperties_d__6.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._WarpText_d__7::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__7.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._Start_d__4::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_Start_d__4.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._RevealCharacters_d__7::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealCharacters_d__7.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._RevealWords_d__8::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_RevealWords_d__8.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisplayTextMeshFloatingText_d__16::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshFloatingText_d__16.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._DisplayTextMeshProFloatingText_d__15::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_DisplayTextMeshProFloatingText_d__15.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AnimateVertexColors_d__3::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__3.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AnimateVertexColors_d__11::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__11.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._AnimateVertexColors_d__10::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_AnimateVertexColors_d__10.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._WarpText_d__8::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/_WarpText_d__8.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._GenerateCoroutine_d__21::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_GenerateCoroutine_d__21.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._BlinkCoroutine_d__14::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_BlinkCoroutine_d__14.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ClickCoroutine_d__41::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__41.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LongPressCheck_d__35::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__35.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ReHighlightKey_d__68::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ReHighlightKey_d__68.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._ClickCoroutine_d__43::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_ClickCoroutine_d__43.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._LongPressCheck_d__37::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_LongPressCheck_d__37.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._SelectKey_d__21::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/_SelectKey_d__21.System_IDisposable_Dispose |
| `greg.SYSTEM.System_IDisposable_Dispose` | `Il2Cpp._OnFinishSubmit_d__25::System_IDisposable_Dispose()` | `Postfix` | Auto-generated from IL2CPP sources: UnityEngine/_OnFinishSubmit_d__25.System_IDisposable_Dispose |
| `greg.SYSTEM.TMPInputFieldReActivate` | `Il2Cpp.OSK_UI_InputReceiver::TMPInputFieldReActivate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.TMPInputFieldReActivate |
| `greg.SYSTEM.TakeTheWheel` | `Il2Cpp.CarController::TakeTheWheel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.TakeTheWheel |
| `greg.SYSTEM.Talk` | `Il2Cpp.AICharacterExpressions::Talk(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.Talk |
| `greg.SYSTEM.Talking` | `Il2Cpp.AICharacterExpressions::Talking(List<string>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterExpressions.Talking |
| `greg.SYSTEM.TargetSet` | `Il2Cpp.AICharacterControl::SetTarget(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.SetTarget |
| `greg.SYSTEM.TechnicianDispatched` | `Il2Cpp.AssetManagement::SendTechnician(NetworkSwitch, Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.SendTechnician |
| `greg.SYSTEM.TechnicianInformationChanged` | `Il2Cpp.AssetManagement::UpdateTechnicianInformation()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagement.UpdateTechnicianInformation |
| `greg.SYSTEM.Text` | `Il2Cpp.OSK_Keyboard::Text()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Text |
| `greg.SYSTEM.Text` | `Il2Cpp.OSK_Receiver::Text()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.Text |
| `greg.SYSTEM.Text` | `Il2Cpp.OSK_UI_InputReceiver::Text()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.Text |
| `greg.SYSTEM.TextAdded` | `Il2Cpp.OSK_Keyboard::AddText(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AddText |
| `greg.SYSTEM.TextAdded` | `Il2Cpp.OSK_Receiver::AddText(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.AddText |
| `greg.SYSTEM.TextAdded` | `Il2Cpp.OSK_UI_InputReceiver::AddText(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.AddText |
| `greg.SYSTEM.TextSet` | `Il2Cpp.LocalisedText::SetText(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LocalisedText.SetText |
| `greg.SYSTEM.TextSet` | `Il2Cpp.OSK_Receiver::SetText(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.SetText |
| `greg.SYSTEM.TextWarped` | `Il2Cpp.SkewTextExample::WarpText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/SkewTextExample.WarpText |
| `greg.SYSTEM.TextWarped` | `Il2Cpp.WarpTextExample::WarpText()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/WarpTextExample.WarpText |
| `greg.SYSTEM.Text_ShftEnabledAdded` | `Il2Cpp.OSK_Keyboard::AddText_ShftEnabled(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.AddText_ShftEnabled |
| `greg.SYSTEM.TextureLoaded` | `Il2Cpp.ModLoader::LoadTexture(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ModLoader.LoadTexture |
| `greg.SYSTEM.TexturesChanged` | `Il2Cpp.FlexibleColorPicker::UpdateTextures()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.UpdateTextures |
| `greg.SYSTEM.ThumbnailSet` | `Il2Cpp.UserReport::SetThumbnail(Texture2D)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport.SetThumbnail |
| `greg.SYSTEM.TickTimer` | `Il2Cpp.ITimedDevice::TickTimer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ITimedDevice.TickTimer |
| `greg.SYSTEM.TimeController.OnEndOfTheDay` | `Il2Cpp.OnEndOfTheDay::TimeController.OnEndOfTheDay([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnEndOfTheDay.TimeController.OnEndOfTheDay |
| `greg.SYSTEM.TimeIsBetween` | `Il2Cpp.TimeController::TimeIsBetween(float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TimeController.TimeIsBetween |
| `greg.SYSTEM.TimerLoop` | `Il2Cpp.DeviceTimerManager::TimerLoop()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DeviceTimerManager.TimerLoop |
| `greg.SYSTEM.ToggleCharMask` | `Il2Cpp.OSK_Receiver::ToggleCharMask()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ToggleCharMask |
| `greg.SYSTEM.ToggleCharMask` | `Il2Cpp.OSK_Receiver::ToggleCharMask(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ToggleCharMask |
| `greg.SYSTEM.ToggleCharMask` | `Il2Cpp.OSK_UI_InputReceiver::ToggleCharMask(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_InputReceiver.ToggleCharMask |
| `greg.SYSTEM.ToggleClearWarningAuto` | `Il2Cpp.CommandCenter::ToggleClearWarningAuto(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/CommandCenter.ToggleClearWarningAuto |
| `greg.SYSTEM.TopAnchorSet` | `Il2Cpp.VerticalRecyclingSystem::SetTopAnchor(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.SetTopAnchor |
| `greg.SYSTEM.TopLeftAnchorSet` | `Il2Cpp.VerticalRecyclingSystem::SetTopLeftAnchor(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/VerticalRecyclingSystem.SetTopLeftAnchor |
| `greg.SYSTEM.Traverse` | `Il2Cpp.OSK_Keyboard::Traverse()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Keyboard.Traverse |
| `greg.SYSTEM.Traverse` | `Il2Cpp.OSK_UI_Keyboard::Traverse()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard.Traverse |
| `greg.SYSTEM.TrolleyPositionLoaded` | `Il2Cpp.MainGameManager::LoadTrolleyPosition(Vector3, Quaternion)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.LoadTrolleyPosition |
| `greg.SYSTEM.TruckComing` | `Il2Cpp.GateLever::TruckComing()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GateLever.TruckComing |
| `greg.SYSTEM.TryUnlock` | `Il2Cpp.ShopItem::TryUnlock()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.TryUnlock |
| `greg.SYSTEM.TurnBackOnCollidersInTRolley` | `Il2Cpp.CarController::TurnBackOnCollidersInTRolley()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.TurnBackOnCollidersInTRolley |
| `greg.SYSTEM.TurnOffAfterXseconds` | `Il2Cpp.AutoDisable::TurnOffAfterXseconds()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AutoDisable.TurnOffAfterXseconds |
| `greg.SYSTEM.TurnOffCollidersInTrolley` | `Il2Cpp.CarController::TurnOffCollidersInTrolley()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController.TurnOffCollidersInTrolley |
| `greg.SYSTEM.TweenHorizontal` | `Il2Cpp.LeanTweenUIElement::TweenHorizontal(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.TweenHorizontal |
| `greg.SYSTEM.TweenScaleInOut` | `Il2Cpp.LeanTweenUIElement::TweenScaleInOut()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.TweenScaleInOut |
| `greg.SYSTEM.TweenTheColors` | `Il2Cpp.PulsatingImageColor::TweenTheColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.TweenTheColors |
| `greg.SYSTEM.TweenTheColors` | `Il2Cpp.PulsatingText::TweenTheColors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingText.TweenTheColors |
| `greg.SYSTEM.TweenVertical` | `Il2Cpp.LeanTweenUIElement::TweenVertical(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement.TweenVertical |
| `greg.SYSTEM.TypeHex` | `Il2Cpp.FlexibleColorPicker::TypeHex(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.TypeHex |
| `greg.SYSTEM.TypeHex` | `Il2Cpp.FlexibleColorPicker::TypeHex(string, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker.TypeHex |
| `greg.SYSTEM.UIChanged` | `Il2Cpp.ActionKeyHint::UpdateUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ActionKeyHint.UpdateUI |
| `greg.SYSTEM.UnlockButton` | `Il2Cpp.ShopItem::UnlockButton()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.UnlockButton |
| `greg.SYSTEM.UnlockFromSave` | `Il2Cpp.ComputerShop::UnlockFromSave(Dictionary<string, bool>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop.UnlockFromSave |
| `greg.SYSTEM.Unregister` | `Il2Cpp.DeviceTimerManager::Unregister(ITimedDevice)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/DeviceTimerManager.Unregister |
| `greg.SYSTEM.UnsubscribeFromRopeEvents` | `Il2Cpp.RopeMesh::UnsubscribeFromRopeEvents()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/RopeMesh.UnsubscribeFromRopeEvents |
| `greg.SYSTEM.UsedSubnetRemoved` | `Il2Cpp.MainGameManager::RemoveUsedSubnet(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.RemoveUsedSubnet |
| `greg.SYSTEM.UsedVlanIdRemoved` | `Il2Cpp.MainGameManager::RemoveUsedVlanId(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager.RemoveUsedVlanId |
| `greg.SYSTEM.Validate` | `Il2Cpp.SaveData::Validate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SaveData.Validate |
| `greg.SYSTEM.Validate` | `Il2Cpp.TMP_DigitValidator::Validate(string, int, char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_DigitValidator.Validate |
| `greg.SYSTEM.Validate` | `Il2Cpp.TMP_PhoneNumberValidator::Validate(string, int, char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/TMP_PhoneNumberValidator.Validate |
| `greg.SYSTEM.ValidateRackPosition` | `Il2Cpp.PatchPanel::ValidateRackPosition()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PatchPanel.ValidateRackPosition |
| `greg.SYSTEM.ValueChanged` | `Il2Cpp.OSK_Receiver::ValueChanged()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.ValueChanged |
| `greg.SYSTEM.VisualStateChanged` | `Il2Cpp.ShopItem::UpdateVisualState()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ShopItem.UpdateVisualState |
| `greg.SYSTEM.WaitAndDisplay` | `Il2Cpp.SteamStatsOnMainMenuTop::WaitAndDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SteamStatsOnMainMenuTop.WaitAndDisplay |
| `greg.SYSTEM._AddTechnician_b__0` | `Il2Cpp.__c__DisplayClass19_0::_AddTechnician_b__0(Technician)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass19_0._AddTechnician_b__0 |
| `greg.SYSTEM._AnimateVertexColors_b__0` | `Il2Cpp.__c__DisplayClass10_0::_AnimateVertexColors_b__0(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppTMPro/__c__DisplayClass10_0._AnimateVertexColors_b__0 |
| `greg.SYSTEM._AutoCorrectRow_b__9_0` | `Il2Cpp.__c::_AutoCorrectRow_b__9_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c._AutoCorrectRow_b__9_0 |
| `greg.SYSTEM._AutoSave_b__0` | `Il2Cpp.__c__DisplayClass25_0::_AutoSave_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass25_0._AutoSave_b__0 |
| `greg.SYSTEM._Awake_b__11_0` | `Il2Cpp.SetIP::_Awake_b__11_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP._Awake_b__11_0 |
| `greg.SYSTEM._Awake_b__12_0` | `Il2Cpp.__c::_Awake_b__12_0(TextMeshProUGUI)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._Awake_b__12_0 |
| `greg.SYSTEM._Awake_b__14_0` | `Il2Cpp.InputManager::_Awake_b__14_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager._Awake_b__14_0 |
| `greg.SYSTEM._Awake_b__16_0` | `Il2Cpp.LeanTweenUIElement::_Awake_b__16_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement._Awake_b__16_0 |
| `greg.SYSTEM._Awake_b__16_1` | `Il2Cpp.LeanTweenUIElement::_Awake_b__16_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement._Awake_b__16_1 |
| `greg.SYSTEM._Awake_b__16_2` | `Il2Cpp.LeanTweenUIElement::_Awake_b__16_2(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LeanTweenUIElement._Awake_b__16_2 |
| `greg.SYSTEM._Awake_b__36_0` | `Il2Cpp.ComputerShop::_Awake_b__36_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ComputerShop._Awake_b__36_0 |
| `greg.SYSTEM._Awake_b__43_0` | `Il2Cpp.FlexibleColorPicker::_Awake_b__43_0(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker._Awake_b__43_0 |
| `greg.SYSTEM._Awake_b__43_1` | `Il2Cpp.FlexibleColorPicker::_Awake_b__43_1(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker._Awake_b__43_1 |
| `greg.SYSTEM._Awake_b__43_2` | `Il2Cpp.FlexibleColorPicker::_Awake_b__43_2(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FlexibleColorPicker._Awake_b__43_2 |
| `greg.SYSTEM._Awake_b__47_0` | `Il2Cpp.UsableObject::_Awake_b__47_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject._Awake_b__47_0 |
| `greg.SYSTEM._Awake_b__47_1` | `Il2Cpp.UsableObject::_Awake_b__47_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject._Awake_b__47_1 |
| `greg.SYSTEM._Awake_b__47_2` | `Il2Cpp.UsableObject::_Awake_b__47_2(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UsableObject._Awake_b__47_2 |
| `greg.SYSTEM._Awake_b__63_0` | `Il2Cpp.MainGameManager::_Awake_b__63_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager._Awake_b__63_0 |
| `greg.SYSTEM._BuildAssignments_b__0_0` | `Il2Cpp.__c__0::_BuildAssignments_b__0_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__0._BuildAssignments_b__0_0 |
| `greg.SYSTEM._ButtonBuyShopItem_b__1` | `Il2Cpp.__c__DisplayClass48_0::_ButtonBuyShopItem_b__1(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass48_0._ButtonBuyShopItem_b__1 |
| `greg.SYSTEM._ButtonBuyShopItem_b__48_0` | `Il2Cpp.__c::_ButtonBuyShopItem_b__48_0(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._ButtonBuyShopItem_b__48_0 |
| `greg.SYSTEM._ButtonCheckOut_b__59_0` | `Il2Cpp.__c::_ButtonCheckOut_b__59_0(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._ButtonCheckOut_b__59_0 |
| `greg.SYSTEM._ButtonChosenColor_b__0` | `Il2Cpp.__c__DisplayClass62_0::_ButtonChosenColor_b__0(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass62_0._ButtonChosenColor_b__0 |
| `greg.SYSTEM._ButtonEditLabel_b__30_0` | `Il2Cpp.SetIP::_ButtonEditLabel_b__30_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SetIP._ButtonEditLabel_b__30_0 |
| `greg.SYSTEM._ButtonUnmountRack_b__17_0` | `Il2Cpp.__c::_ButtonUnmountRack_b__17_0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._ButtonUnmountRack_b__17_0 |
| `greg.SYSTEM._BuyAnotherItem_b__51_0` | `Il2Cpp.__c::_BuyAnotherItem_b__51_0(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._BuyAnotherItem_b__51_0 |
| `greg.SYSTEM._CreateCellPool_b__17_0` | `Il2Cpp.__c::_CreateCellPool_b__17_0(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/__c._CreateCellPool_b__17_0 |
| `greg.SYSTEM._CreateLACP_b__26_0` | `Il2Cpp.__c::_CreateLACP_b__26_0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._CreateLACP_b__26_0 |
| `greg.SYSTEM._CreateLACP_b__26_1` | `Il2Cpp.__c::_CreateLACP_b__26_1(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._CreateLACP_b__26_1 |
| `greg.SYSTEM._CreateVLANButtonMulti_b__0` | `Il2Cpp.__c__DisplayClass23_0::_CreateVLANButtonMulti_b__0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass23_0._CreateVLANButtonMulti_b__0 |
| `greg.SYSTEM._DeleteSaveFile_b__0` | `Il2Cpp.__c__DisplayClass18_0::_DeleteSaveFile_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass18_0._DeleteSaveFile_b__0 |
| `greg.SYSTEM._EvaluateAllRoutes_b__1` | `Il2Cpp.__c__DisplayClass31_0::_EvaluateAllRoutes_b__1(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass31_0._EvaluateAllRoutes_b__1 |
| `greg.SYSTEM._EvaluateAllRoutes_b__31_0` | `Il2Cpp.__c::_EvaluateAllRoutes_b__31_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._EvaluateAllRoutes_b__31_0 |
| `greg.SYSTEM._FireTechnician_b__0` | `Il2Cpp.__c__DisplayClass28_0::_FireTechnician_b__0(Technician)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass28_0._FireTechnician_b__0 |
| `greg.SYSTEM._GenerateUniqueServerId_b__0` | `Il2Cpp.__c__DisplayClass38_0::_GenerateUniqueServerId_b__0(Server)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass38_0._GenerateUniqueServerId_b__0 |
| `greg.SYSTEM._GenerateUniqueSwitchId_b__0` | `Il2Cpp.__c__DisplayClass27_0::_GenerateUniqueSwitchId_b__0(NetworkSwitch)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass27_0._GenerateUniqueSwitchId_b__0 |
| `greg.SYSTEM._Generate_b__1` | `Il2Cpp.__c__DisplayClass55_0::_Generate_b__1(OSK_SpecialKeys)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass55_0._Generate_b__1 |
| `greg.SYSTEM._Generate_b__1` | `Il2Cpp.__c__DisplayClass12_0::_Generate_b__1(OSK_SpecialKeys)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass12_0._Generate_b__1 |
| `greg.SYSTEM._Generate_b__12_0` | `Il2Cpp.OSK_UI_Keyboard::_Generate_b__12_0(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Keyboard._Generate_b__12_0 |
| `greg.SYSTEM._Generate_b__2` | `Il2Cpp.__c__DisplayClass55_0::_Generate_b__2(OSK_KeyTypeMeta)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass55_0._Generate_b__2 |
| `greg.SYSTEM._Generate_b__2` | `Il2Cpp.__c__DisplayClass12_0::_Generate_b__2(OSK_KeyTypeMeta)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass12_0._Generate_b__2 |
| `greg.SYSTEM._Generate_b__25_0` | `Il2Cpp.__c::_Generate_b__25_0(List<I_OSK_Key>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c._Generate_b__25_0 |
| `greg.SYSTEM._Generate_b__55_0` | `Il2Cpp.__c::_Generate_b__55_0(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c._Generate_b__55_0 |
| `greg.SYSTEM._GetCorrectedKey_b__0` | `Il2Cpp.__c__DisplayClass11_0::_GetCorrectedKey_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass11_0._GetCorrectedKey_b__0 |
| `greg.SYSTEM._GetCorrectedKey_b__1` | `Il2Cpp.__c__DisplayClass11_0::_GetCorrectedKey_b__1(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass11_0._GetCorrectedKey_b__1 |
| `greg.SYSTEM._GetCustomerItemByID_b__0` | `Il2Cpp.__c__DisplayClass72_0::_GetCustomerItemByID_b__0(CustomerItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass72_0._GetCustomerItemByID_b__0 |
| `greg.SYSTEM._GetFreeSubnet_b__0` | `Il2Cpp.__c__DisplayClass77_0::_GetFreeSubnet_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass77_0._GetFreeSubnet_b__0 |
| `greg.SYSTEM._GetFreeSubnet_b__77_1` | `Il2Cpp.__c::_GetFreeSubnet_b__77_1(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._GetFreeSubnet_b__77_1 |
| `greg.SYSTEM._GetGlyphEnumSlots_b__18_0` | `Il2Cpp.__c__18::_GetGlyphEnumSlots_b__18_0(Il2CppSystem.ValueTuple<int, TEnum>, Il2CppSystem.ValueTuple<int, TEnum>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__18._GetGlyphEnumSlots_b__18_0 |
| `greg.SYSTEM._GetGlyphEnumSlots_b__18_1` | `Il2Cpp.__c__18::_GetGlyphEnumSlots_b__18_1(Il2CppSystem.ValueTuple<int, TEnum>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__18._GetGlyphEnumSlots_b__18_1 |
| `greg.SYSTEM._Init_b__20_0` | `Il2Cpp.RayLookAt::_Init_b__20_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_0 |
| `greg.SYSTEM._Init_b__20_1` | `Il2Cpp.RayLookAt::_Init_b__20_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_1 |
| `greg.SYSTEM._Init_b__20_2` | `Il2Cpp.RayLookAt::_Init_b__20_2(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_2 |
| `greg.SYSTEM._Init_b__20_3` | `Il2Cpp.RayLookAt::_Init_b__20_3(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_3 |
| `greg.SYSTEM._Init_b__20_4` | `Il2Cpp.RayLookAt::_Init_b__20_4(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_4 |
| `greg.SYSTEM._Init_b__20_5` | `Il2Cpp.RayLookAt::_Init_b__20_5(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_5 |
| `greg.SYSTEM._Init_b__20_6` | `Il2Cpp.RayLookAt::_Init_b__20_6(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/RayLookAt._Init_b__20_6 |
| `greg.SYSTEM._Init_b__22_0` | `Il2Cpp.MouseLook::_Init_b__22_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook._Init_b__22_0 |
| `greg.SYSTEM._Init_b__22_1` | `Il2Cpp.MouseLook::_Init_b__22_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/MouseLook._Init_b__22_1 |
| `greg.SYSTEM._Initialize_b__13_0` | `Il2Cpp.RecyclableScrollRect::_Initialize_b__13_0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect._Initialize_b__13_0 |
| `greg.SYSTEM._InsertItemInRack_b__0` | `Il2Cpp.__c__DisplayClass13_0::_InsertItemInRack_b__0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass13_0._InsertItemInRack_b__0 |
| `greg.SYSTEM._InsertedInRack_b__8_0` | `Il2Cpp.__c::_InsertedInRack_b__8_0(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._InsertedInRack_b__8_0 |
| `greg.SYSTEM._InstallRack_b__0` | `Il2Cpp.__c__DisplayClass6_0::_InstallRack_b__0(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass6_0._InstallRack_b__0 |
| `greg.SYSTEM._InstallRack_b__2` | `Il2Cpp.__c__DisplayClass6_0::_InstallRack_b__2(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass6_0._InstallRack_b__2 |
| `greg.SYSTEM._InstallRack_b__3` | `Il2Cpp.__c__DisplayClass6_0::_InstallRack_b__3()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass6_0._InstallRack_b__3 |
| `greg.SYSTEM._InstallRack_b__6_1` | `Il2Cpp.__c::_InstallRack_b__6_1()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._InstallRack_b__6_1 |
| `greg.SYSTEM._KeyCallBase_b__0` | `Il2Cpp.__c__DisplayClass61_0::_KeyCallBase_b__0(OSK_KeyTypeMeta)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass61_0._KeyCallBase_b__0 |
| `greg.SYSTEM._KeyCallBase_b__0` | `Il2Cpp.__c__DisplayClass16_0::_KeyCallBase_b__0(OSK_KeyTypeMeta)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass16_0._KeyCallBase_b__0 |
| `greg.SYSTEM._KeyboardSizeEstimator_b__1` | `Il2Cpp.__c__DisplayClass57_0::_KeyboardSizeEstimator_b__1(OSK_SpecialKeys)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass57_0._KeyboardSizeEstimator_b__1 |
| `greg.SYSTEM._KeyboardSizeEstimator_b__57_0` | `Il2Cpp.__c::_KeyboardSizeEstimator_b__57_0(char)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c._KeyboardSizeEstimator_b__57_0 |
| `greg.SYSTEM._LabelActionOnClick_b__18_0` | `Il2Cpp.Interact::_LabelActionOnClick_b__18_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Interact._LabelActionOnClick_b__18_0 |
| `greg.SYSTEM._LeaveTheTrolley_b__0` | `Il2Cpp.__c__DisplayClass38_0::_LeaveTheTrolley_b__0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/__c__DisplayClass38_0._LeaveTheTrolley_b__0 |
| `greg.SYSTEM._Listofsaves_b__21_0` | `Il2Cpp.__c::_Listofsaves_b__21_0(FileInfo)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._Listofsaves_b__21_0 |
| `greg.SYSTEM._Listofsaves_b__21_1` | `Il2Cpp.__c::_Listofsaves_b__21_1(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._Listofsaves_b__21_1 |
| `greg.SYSTEM._LoadLocalisation_b__0` | `Il2Cpp.__c__DisplayClass9_0::_LoadLocalisation_b__0(LanguageObject)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass9_0._LoadLocalisation_b__0 |
| `greg.SYSTEM._LoadNetworkStateCoroutine_b__2` | `Il2Cpp.__c__DisplayClass15_1::_LoadNetworkStateCoroutine_b__2(CustomerBase)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass15_1._LoadNetworkStateCoroutine_b__2 |
| `greg.SYSTEM._LoadNetworkStateCoroutine_b__3` | `Il2Cpp.__c__DisplayClass15_2::_LoadNetworkStateCoroutine_b__3(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass15_2._LoadNetworkStateCoroutine_b__3 |
| `greg.SYSTEM._LoadSettings_b__15_0` | `Il2Cpp.__c::_LoadSettings_b__15_0(Localisation.Languages)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._LoadSettings_b__15_0 |
| `greg.SYSTEM._Load_b__0` | `Il2Cpp.__c__DisplayClass24_0::_Load_b__0(RackMount)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass24_0._Load_b__0 |
| `greg.SYSTEM._MergeRanges_b__8_0` | `Il2Cpp.__c::_MergeRanges_b__8_0(OSK_GlyphHandler.Range, OSK_GlyphHandler.Range)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c._MergeRanges_b__8_0 |
| `greg.SYSTEM._NewestSave_b__22_0` | `Il2Cpp.__c::_NewestSave_b__22_0(FileInfo)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._NewestSave_b__22_0 |
| `greg.SYSTEM._PrintNetworkMap_b__42_2` | `Il2Cpp.__c::_PrintNetworkMap_b__42_2(NetworkMap.Device)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._PrintNetworkMap_b__42_2 |
| `greg.SYSTEM._PrintNetworkMap_b__42_4` | `Il2Cpp.__c::_PrintNetworkMap_b__42_4(NetworkMap.Device)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._PrintNetworkMap_b__42_4 |
| `greg.SYSTEM._PrintNetworkMap_b__42_7` | `Il2Cpp.__c::_PrintNetworkMap_b__42_7(Il2CppSystem.ValueTuple<string, CableLink.TypeOfLink>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._PrintNetworkMap_b__42_7 |
| `greg.SYSTEM._RecycleBottomToTop_b__0` | `Il2Cpp.__c__DisplayClass20_0::_RecycleBottomToTop_b__0(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/__c__DisplayClass20_0._RecycleBottomToTop_b__0 |
| `greg.SYSTEM._RecycleLeftToRight_b__0` | `Il2Cpp.__c__DisplayClass19_0::_RecycleLeftToRight_b__0(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/__c__DisplayClass19_0._RecycleLeftToRight_b__0 |
| `greg.SYSTEM._RecycleRightToleft_b__0` | `Il2Cpp.__c__DisplayClass20_0::_RecycleRightToleft_b__0(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/__c__DisplayClass20_0._RecycleRightToleft_b__0 |
| `greg.SYSTEM._RecycleTopToBottom_b__0` | `Il2Cpp.__c__DisplayClass19_0::_RecycleTopToBottom_b__0(RectTransform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/__c__DisplayClass19_0._RecycleTopToBottom_b__0 |
| `greg.SYSTEM._RefreshVLANDisplayForSelection_b__22_0` | `Il2Cpp.__c::_RefreshVLANDisplayForSelection_b__22_0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._RefreshVLANDisplayForSelection_b__22_0 |
| `greg.SYSTEM._RefreshVLANDisplayForSelection_b__22_1` | `Il2Cpp.__c::_RefreshVLANDisplayForSelection_b__22_1(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._RefreshVLANDisplayForSelection_b__22_1 |
| `greg.SYSTEM._ReloadData_b__17_0` | `Il2Cpp.RecyclableScrollRect::_ReloadData_b__17_0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyAndCode/RecyclableScrollRect._ReloadData_b__17_0 |
| `greg.SYSTEM._RemoveCableConnection_b__0` | `Il2Cpp.__c__DisplayClass40_0::_RemoveCableConnection_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass40_0._RemoveCableConnection_b__0 |
| `greg.SYSTEM._RemoveCableConnection_b__1` | `Il2Cpp.__c__DisplayClass40_0::_RemoveCableConnection_b__1(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass40_0._RemoveCableConnection_b__1 |
| `greg.SYSTEM._RemoveObjectiveSign_b__0` | `Il2Cpp.__c__DisplayClass32_0::_RemoveObjectiveSign_b__0(PositionIndicator)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass32_0._RemoveObjectiveSign_b__0 |
| `greg.SYSTEM._RepopulateResolutions_b__53_0` | `Il2Cpp.__c::_RepopulateResolutions_b__53_0(Resolution)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._RepopulateResolutions_b__53_0 |
| `greg.SYSTEM._RepopulateResolutions_b__53_1` | `Il2Cpp.__c::_RepopulateResolutions_b__53_1(Resolution)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._RepopulateResolutions_b__53_1 |
| `greg.SYSTEM._ResolveActionAndBinding_b__0` | `Il2Cpp.__c__DisplayClass29_0::_ResolveActionAndBinding_b__0(InputBinding)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass29_0._ResolveActionAndBinding_b__0 |
| `greg.SYSTEM._SaveGame_b__0` | `Il2Cpp.__c__DisplayClass13_0::_SaveGame_b__0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass13_0._SaveGame_b__0 |
| `greg.SYSTEM._Set_b__0` | `Il2Cpp.__c__DisplayClass15_0::_Set_b__0(KeyValuePair<string, OSK_KeyCode>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass15_0._Set_b__0 |
| `greg.SYSTEM._ShowCustomerCardsCanvas_b__0` | `Il2Cpp.__c__DisplayClass79_0::_ShowCustomerCardsCanvas_b__0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass79_0._ShowCustomerCardsCanvas_b__0 |
| `greg.SYSTEM._ShowCustomerCardsCanvas_b__1` | `Il2Cpp.__c__DisplayClass79_0::_ShowCustomerCardsCanvas_b__1(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass79_0._ShowCustomerCardsCanvas_b__1 |
| `greg.SYSTEM._ShuffleAvailableCustomers_b__73_0` | `Il2Cpp.MainGameManager::_ShuffleAvailableCustomers_b__73_0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager._ShuffleAvailableCustomers_b__73_0 |
| `greg.SYSTEM._ShuffleAvailableCustomers_b__73_1` | `Il2Cpp.MainGameManager::_ShuffleAvailableCustomers_b__73_1(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainGameManager._ShuffleAvailableCustomers_b__73_1 |
| `greg.SYSTEM._ShuffledArrayOfInts_b__0` | `Il2Cpp.__c__DisplayClass5_0::_ShuffledArrayOfInts_b__0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass5_0._ShuffledArrayOfInts_b__0 |
| `greg.SYSTEM._Start_b__30_0` | `Il2Cpp.__c::_Start_b__30_0(Resolution)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._Start_b__30_0 |
| `greg.SYSTEM._Start_b__30_1` | `Il2Cpp.__c::_Start_b__30_1(Resolution)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._Start_b__30_1 |
| `greg.SYSTEM._Start_b__32_0` | `Il2Cpp.CarController::_Start_b__32_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController._Start_b__32_0 |
| `greg.SYSTEM._Start_b__32_1` | `Il2Cpp.CarController::_Start_b__32_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/CarController._Start_b__32_1 |
| `greg.SYSTEM._Start_b__57_0` | `Il2Cpp.FirstPersonController::_Start_b__57_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_0 |
| `greg.SYSTEM._Start_b__57_1` | `Il2Cpp.FirstPersonController::_Start_b__57_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_1 |
| `greg.SYSTEM._Start_b__57_2` | `Il2Cpp.FirstPersonController::_Start_b__57_2(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_2 |
| `greg.SYSTEM._Start_b__57_3` | `Il2Cpp.FirstPersonController::_Start_b__57_3(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_3 |
| `greg.SYSTEM._Start_b__57_4` | `Il2Cpp.FirstPersonController::_Start_b__57_4(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_4 |
| `greg.SYSTEM._Start_b__57_5` | `Il2Cpp.FirstPersonController::_Start_b__57_5(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_5 |
| `greg.SYSTEM._Start_b__57_6` | `Il2Cpp.FirstPersonController::_Start_b__57_6(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_6 |
| `greg.SYSTEM._Start_b__57_7` | `Il2Cpp.FirstPersonController::_Start_b__57_7(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: UnityStandardAssets/FirstPersonController._Start_b__57_7 |
| `greg.SYSTEM._SubmitUserReport_b__19_0` | `Il2Cpp.UserReport::_SubmitUserReport_b__19_0(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport._SubmitUserReport_b__19_0 |
| `greg.SYSTEM._SubmitUserReport_b__19_1` | `Il2Cpp.UserReport::_SubmitUserReport_b__19_1(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UserReport._SubmitUserReport_b__19_1 |
| `greg.SYSTEM._TakeTheWheel_b__0` | `Il2Cpp.__c__DisplayClass37_0::_TakeTheWheel_b__0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppPolyStang/__c__DisplayClass37_0._TakeTheWheel_b__0 |
| `greg.SYSTEM._Traverse_b__0` | `Il2Cpp.__c__DisplayClass56_0::_Traverse_b__0(OSK_Key)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass56_0._Traverse_b__0 |
| `greg.SYSTEM._Traverse_b__0` | `Il2Cpp.__c__DisplayClass13_0::_Traverse_b__0(OSK_UI_Key)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass13_0._Traverse_b__0 |
| `greg.SYSTEM._Traverse_b__1` | `Il2Cpp.__c__DisplayClass56_0::_Traverse_b__1(OSK_SpecialKeys)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass56_0._Traverse_b__1 |
| `greg.SYSTEM._Traverse_b__1` | `Il2Cpp.__c__DisplayClass13_0::_Traverse_b__1(OSK_SpecialKeys)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/__c__DisplayClass13_0._Traverse_b__1 |
| `greg.SYSTEM._UpdateAllUI_b__0` | `Il2Cpp.__c__DisplayClass34_0::_UpdateAllUI_b__0(Il2CppSystem.ValueTuple<int, List<string>>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass34_0._UpdateAllUI_b__0 |
| `greg.SYSTEM._UpdateAllUI_b__34_1` | `Il2Cpp.__c::_UpdateAllUI_b__34_1(Il2CppSystem.ValueTuple<int, List<string>>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._UpdateAllUI_b__34_1 |
| `greg.SYSTEM._UpdateAudioVolume_b__13_0` | `Il2Cpp.__c::_UpdateAudioVolume_b__13_0(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._UpdateAudioVolume_b__13_0 |
| `greg.SYSTEM._UpdateCartTotal_b__58_0` | `Il2Cpp.__c::_UpdateCartTotal_b__58_0(ShopCartItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._UpdateCartTotal_b__58_0 |
| `greg.SYSTEM.__AssignHandles` | `Il2Cpp.TypeHandle::__AssignHandles(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/TypeHandle.__AssignHandles |
| `greg.SYSTEM.__AssignQueries` | `Il2Cpp.InternalCompilerQueryAndHandleData::__AssignQueries(SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InternalCompilerQueryAndHandleData.__AssignQueries |
| `greg.SYSTEM.__AssignQueries` | `Il2Cpp.WaypointInitializationSystem::__AssignQueries(Unity.Entities.SystemState)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/WaypointInitializationSystem.__AssignQueries |
| `greg.SYSTEM.__ThrowCodeGenException` | `Il2Cpp.UpdatePacketsJob::__ThrowCodeGenException()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UpdatePacketsJob.__ThrowCodeGenException |
| `greg.SYSTEM.___LoadNetworkStateCoroutine_592A83D2_b__2` | `Il2Cpp.__c__DisplayClass50_1::___LoadNetworkStateCoroutine_592A83D2_b__2(CustomerBase)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass50_1.___LoadNetworkStateCoroutine_592A83D2_b__2 |
| `greg.SYSTEM.___LoadNetworkStateCoroutine_592A83D2_b__3` | `Il2Cpp.__c__DisplayClass50_2::___LoadNetworkStateCoroutine_592A83D2_b__3(CableLink)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c__DisplayClass50_2.___LoadNetworkStateCoroutine_592A83D2_b__3 |
| `greg.SYSTEM.__cctor_b__16_0` | `Il2Cpp.__c::__cctor_b__16_0(Il2CppSystem.ValueTuple<Rack, float>, Il2CppSystem.ValueTuple<Rack, float>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c.__cctor_b__16_0 |
| `greg.SYSTEM.__m__Finally1` | `Il2Cpp._Talking_d__7::__m__Finally1()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/_Talking_d__7.__m__Finally1 |
| `greg.SYSTEM._get_TotalPenalties_b__7_0` | `Il2Cpp.__c::_get_TotalPenalties_b__7_0(BalanceSheet.CustomerRecord)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._get_TotalPenalties_b__7_0 |
| `greg.SYSTEM._get_TotalRevenue_b__5_0` | `Il2Cpp.__c::_get_TotalRevenue_b__5_0(BalanceSheet.CustomerRecord)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/__c._get_TotalRevenue_b__5_0 |
| `greg.SYSTEM.add_OnPointsChanged` | `Il2Cpp.Rope::add_OnPointsChanged(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.add_OnPointsChanged |
| `greg.SYSTEM.add_rebindCanceled` | `Il2Cpp.InputManager::add_rebindCanceled(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.add_rebindCanceled |
| `greg.SYSTEM.add_rebindComplete` | `Il2Cpp.InputManager::add_rebindComplete(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.add_rebindComplete |
| `greg.SYSTEM.add_rebindStarted` | `Il2Cpp.InputManager::add_rebindStarted(Il2CppSystem.Action<InputAction, int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.add_rebindStarted |
| `greg.SYSTEM.checkGroundMaterial` | `Il2Cpp.FootSteps::checkGroundMaterial()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/FootSteps.checkGroundMaterial |
| `greg.SYSTEM.getXSize` | `Il2Cpp.I_OSK_Key::getXSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.getXSize |
| `greg.SYSTEM.getXSize` | `Il2Cpp.OSK_Key::getXSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.getXSize |
| `greg.SYSTEM.getXSize` | `Il2Cpp.OSK_UI_Key::getXSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.getXSize |
| `greg.SYSTEM.getYSize` | `Il2Cpp.I_OSK_Key::getYSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/I_OSK_Key.getYSize |
| `greg.SYSTEM.getYSize` | `Il2Cpp.OSK_Key::getYSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Key.getYSize |
| `greg.SYSTEM.getYSize` | `Il2Cpp.OSK_UI_Key::getYSize()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_UI_Key.getYSize |
| `greg.SYSTEM.ingScreen.GameIsLoadedLoaded` | `Il2Cpp.GameIsLoaded::LoadingScreen.GameIsLoaded([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/GameIsLoaded.LoadingScreen.GameIsLoaded |
| `greg.SYSTEM.isFocused` | `Il2Cpp.OSK_Receiver::isFocused()` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppviperOSK/OSK_Receiver.isFocused |
| `greg.SYSTEM.moveBack` | `Il2Cpp.AICharacterControl::moveBack(Vector3)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AICharacterControl.moveBack |
| `greg.SYSTEM.remove_OnPointsChanged` | `Il2Cpp.Rope::remove_OnPointsChanged(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2CppGogoGaga/Rope.remove_OnPointsChanged |
| `greg.SYSTEM.remove_rebindCanceled` | `Il2Cpp.InputManager::remove_rebindCanceled(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.remove_rebindCanceled |
| `greg.SYSTEM.remove_rebindComplete` | `Il2Cpp.InputManager::remove_rebindComplete(Il2CppSystem.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.remove_rebindComplete |
| `greg.SYSTEM.remove_rebindStarted` | `Il2Cpp.InputManager::remove_rebindStarted(Il2CppSystem.Action<InputAction, int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/InputManager.remove_rebindStarted |
| `greg.SYSTEM.setColorCallback` | `Il2Cpp.PulsatingImageColor::setColorCallback(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingImageColor.setColorCallback |
| `greg.SYSTEM.setColorCallback` | `Il2Cpp.PulsatingText::setColorCallback(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PulsatingText.setColorCallback |
| `greg.SYSTEM.tingsSingleton.OnTurnOffPublicSet` | `Il2Cpp.OnTurnOffPublic::SettingsSingleton.OnTurnOffPublic([In] System.Action)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/OnTurnOffPublic.SettingsSingleton.OnTurnOffPublic |
| `greg.SYSTEM.upLineSet` | `Il2Cpp.AssetManagementDeviceLine::SetupLine(AssetManagementDeviceLineData, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/AssetManagementDeviceLine.SetupLine |

### UI

| Hook Name | Patch Target | Strategy | Description |
|-----------|-------------|----------|-------------|
| `greg.UI.AAQualitySet` | `Il2Cpp.SettingsGraphics::SetAAQuality(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetAAQuality |
| `greg.UI.ActionLabelChanged` | `Il2Cpp.RebindUIv2::UpdateActionLabel()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.UpdateActionLabel |
| `greg.UI.AntiAliasingSet` | `Il2Cpp.SettingsGraphics::SetAntiAliasing(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetAntiAliasing |
| `greg.UI.AsHeaderSet` | `Il2Cpp.BalanceSheetRow::SetAsHeader()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheetRow.SetAsHeader |
| `greg.UI.AsSalaryRowSet` | `Il2Cpp.BalanceSheetRow::SetAsSalaryRow(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheetRow.SetAsSalaryRow |
| `greg.UI.AsSectionTitleSet` | `Il2Cpp.BalanceSheetRow::SetAsSectionTitle(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheetRow.SetAsSectionTitle |
| `greg.UI.AsTotalRowSet` | `Il2Cpp.BalanceSheetRow::SetAsTotalRow(float, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheetRow.SetAsTotalRow |
| `greg.UI.AsynchronousLoad` | `Il2Cpp.LoadingScreen::AsynchronousLoad(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.AsynchronousLoad |
| `greg.UI.AsynchronousUnLoad` | `Il2Cpp.LoadingScreen::AsynchronousUnLoad(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.AsynchronousUnLoad |
| `greg.UI.AutoSaveIntervalSet` | `Il2Cpp.SettingsGameplay::SetAutoSaveInterval(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.SetAutoSaveInterval |
| `greg.UI.AutoSaveOnOffSet` | `Il2Cpp.SettingsGameplay::SetAutoSaveOnOff(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.SetAutoSaveOnOff |
| `greg.UI.AvailableRefreshRate` | `Il2Cpp.SettingsGraphics::AvailableRefreshRate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.AvailableRefreshRate |
| `greg.UI.AvailableRefreshRatesAfterFrame` | `Il2Cpp.SettingsGraphics::AvailableRefreshRatesAfterFrame()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.AvailableRefreshRatesAfterFrame |
| `greg.UI.BackgroundColorSet` | `Il2Cpp.BalanceSheetRow::SetBackgroundColor(Color)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheetRow.SetBackgroundColor |
| `greg.UI.BindingDisplayChanged` | `Il2Cpp.RebindUIv2::UpdateBindingDisplay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.UpdateBindingDisplay |
| `greg.UI.ButtonOK` | `Il2Cpp.Tutorials::ButtonOK()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.ButtonOK |
| `greg.UI.ButtonShowTutorialInPauseMenu` | `Il2Cpp.Tutorials::ButtonShowTutorialInPauseMenu(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.ButtonShowTutorialInPauseMenu |
| `greg.UI.ButtonUnstuckPlayer` | `Il2Cpp.SettingsGameplay::ButtonUnstuckPlayer()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.ButtonUnstuckPlayer |
| `greg.UI.CallbacksAdded` | `Il2Cpp.UIActions::AddCallbacks(InputController.IUIActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.AddCallbacks |
| `greg.UI.CallbacksRemoved` | `Il2Cpp.UIActions::RemoveCallbacks(InputController.IUIActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.RemoveCallbacks |
| `greg.UI.CallbacksSet` | `Il2Cpp.UIActions::SetCallbacks(InputController.IUIActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.SetCallbacks |
| `greg.UI.ChangeDepthOfField` | `Il2Cpp.SettingsGraphics::ChangeDepthOfField(float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.ChangeDepthOfField |
| `greg.UI.Changed` | `Il2Cpp.MainMenuCamera::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenuCamera.Update |
| `greg.UI.Changed` | `Il2Cpp.UI_SelectedBorder::Update()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UI_SelectedBorder.Update |
| `greg.UI.ClearRows` | `Il2Cpp.BalanceSheet::ClearRows()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.ClearRows |
| `greg.UI.CloseLoadSaveOverlay` | `Il2Cpp.PauseMenu::CloseLoadSaveOverlay()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.CloseLoadSaveOverlay |
| `greg.UI.ComponentDisabled` | `Il2Cpp.ChatController::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ChatController.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.PauseMenu::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.PauseMenuVideoTutorial::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenuVideoTutorial.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.ReBindUI::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.RebindUIv2::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.ToolTipOnUIText::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnDisable |
| `greg.UI.ComponentDisabled` | `Il2Cpp.UI_SelectedBorder::OnDisable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UI_SelectedBorder.OnDisable |
| `greg.UI.ComponentInitialized` | `Il2Cpp.BalanceSheet::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.BalanceSheet::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.ChatController::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ChatController.OnEnable |
| `greg.UI.ComponentInitialized` | `Il2Cpp.LoadingScreen::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.LoadingScreen::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.MainMenu::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.MainMenuCamera::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenuCamera.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.PauseMenu_TabButton::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabButton.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.PauseMenu::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.PauseMenu::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.OnEnable |
| `greg.UI.ComponentInitialized` | `Il2Cpp.ReBindUI::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.ReBindUI::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.OnEnable |
| `greg.UI.ComponentInitialized` | `Il2Cpp.RebindUIv2::OnEnable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.OnEnable |
| `greg.UI.ComponentInitialized` | `Il2Cpp.SettingsControls::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsControls.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.SettingsGameplay::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.SettingsGraphics::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.SettingsSingleton::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsSingleton.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.SettingsVolume::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.Tutorials::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.Awake |
| `greg.UI.ComponentInitialized` | `Il2Cpp.Tutorials::Start()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.Start |
| `greg.UI.ComponentInitialized` | `Il2Cpp.UI_SelectedBorder::Awake()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UI_SelectedBorder.Awake |
| `greg.UI.ConfirmSaved` | `Il2Cpp.PauseMenu::SaveConfirm(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.SaveConfirm |
| `greg.UI.Continue` | `Il2Cpp.MainMenu::Continue()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.Continue |
| `greg.UI.CountFailingApps` | `Il2Cpp.BalanceSheet::CountFailingApps(CustomerBase)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.CountFailingApps |
| `greg.UI.DeleteSaveButtonClick` | `Il2Cpp.PauseMenu::DeleteSaveButtonClick(TextMeshProUGUI)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.DeleteSaveButtonClick |
| `greg.UI.DeleteSaveConfirm` | `Il2Cpp.PauseMenu::DeleteSaveConfirm(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.DeleteSaveConfirm |
| `greg.UI.DifficualtySet` | `Il2Cpp.LoadingScreen::SetDifficualty(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.SetDifficualty |
| `greg.UI.Disable` | `Il2Cpp.UIActions::Disable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.Disable |
| `greg.UI.DisableOnAfterFirstSettingUp` | `Il2Cpp.SettingsSingleton::DisableOnAfterFirstSettingUp()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsSingleton.DisableOnAfterFirstSettingUp |
| `greg.UI.DoRebind` | `Il2Cpp.ReBindUI::DoRebind()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.DoRebind |
| `greg.UI.EffectVolume` | `Il2Cpp.SettingsVolume::EffectVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.EffectVolume |
| `greg.UI.Enable` | `Il2Cpp.UIActions::Enable()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.Enable |
| `greg.UI.ExitGame` | `Il2Cpp.PauseMenu::ExitGame()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.ExitGame |
| `greg.UI.ExposureSet` | `Il2Cpp.SettingsGraphics::SetExposure(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetExposure |
| `greg.UI.FieldOfViewSet` | `Il2Cpp.SettingsGraphics::SetFieldOfView(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetFieldOfView |
| `greg.UI.FillInBalanceSheet` | `Il2Cpp.BalanceSheet::FillInBalanceSheet()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.FillInBalanceSheet |
| `greg.UI.FromSaveLoaded` | `Il2Cpp.BalanceSheet::LoadFromSave()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.LoadFromSave |
| `greg.UI.FullScreenSet` | `Il2Cpp.SettingsGraphics::SetFullScreen(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetFullScreen |
| `greg.UI.GameLoadSceneLoaded` | `Il2Cpp.LoadingScreen::LoadGameLoadScene(Il2CppStructArray<int>)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.LoadGameLoadScene |
| `greg.UI.GameLoaded` | `Il2Cpp.MainMenu::LoadGame()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.LoadGame |
| `greg.UI.Get` | `Il2Cpp.UIActions::Get()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.Get |
| `greg.UI.GetBindingInfo` | `Il2Cpp.ReBindUI::GetBindingInfo()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.GetBindingInfo |
| `greg.UI.GetLatestSnapshot` | `Il2Cpp.BalanceSheet::GetLatestSnapshot()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.GetLatestSnapshot |
| `greg.UI.GetOrCreateRecord` | `Il2Cpp.BalanceSheet::GetOrCreateRecord(CustomerItem)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.GetOrCreateRecord |
| `greg.UI.GetSaveData` | `Il2Cpp.BalanceSheet::GetSaveData()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.GetSaveData |
| `greg.UI.HandleAddCommand` | `Il2Cpp.PauseMenu::HandleAddCommand(Il2CppStringArray)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.HandleAddCommand |
| `greg.UI.HeaderRowAdded` | `Il2Cpp.BalanceSheet::AddHeaderRow()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.AddHeaderRow |
| `greg.UI.HideMiddleMenu` | `Il2Cpp.MainMenu::HideMiddleMenu()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.HideMiddleMenu |
| `greg.UI.HideTooltip` | `Il2Cpp.Tooltip::HideTooltip()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tooltip.HideTooltip |
| `greg.UI.HideTooltipForInteract` | `Il2Cpp.ToolTipInteract::HideTooltipForInteract()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipInteract.HideTooltipForInteract |
| `greg.UI.InactiveAllSet` | `Il2Cpp.KeyHint::SetInactiveAll()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/KeyHint.SetInactiveAll |
| `greg.UI.InputActionMap` | `Il2Cpp.UIActions::InputActionMap(InputController.UIActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.InputActionMap |
| `greg.UI.InstantiateRow` | `Il2Cpp.BalanceSheet::InstantiateRow()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.InstantiateRow |
| `greg.UI.InvertY` | `Il2Cpp.SettingsControls::InvertY()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsControls.InvertY |
| `greg.UI.IsDLSSSupported` | `Il2Cpp.SettingsGraphics::IsDLSSSupported()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.IsDLSSSupported |
| `greg.UI.IsSceneLoaded` | `Il2Cpp.LoadingScreen::IsSceneLoaded(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.IsSceneLoaded |
| `greg.UI.LevelLoaded` | `Il2Cpp.LoadingScreen::LoadLevel(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.LoadLevel |
| `greg.UI.LimitFrameRate` | `Il2Cpp.SettingsGraphics::LimitFrameRate(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.LimitFrameRate |
| `greg.UI.Loaded` | `Il2Cpp.MainMenu::Load(TextMeshProUGUI)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.Load |
| `greg.UI.Loaded` | `Il2Cpp.PauseMenu::Load(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.Load |
| `greg.UI.LookSensitivity` | `Il2Cpp.SettingsControls::LookSensitivity(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsControls.LookSensitivity |
| `greg.UI.MainMenu` | `Il2Cpp.PauseMenu::MainMenu()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.MainMenu |
| `greg.UI.MasterVolume` | `Il2Cpp.SettingsVolume::MasterVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.MasterVolume |
| `greg.UI.MonitorSet` | `Il2Cpp.SettingsGraphics::SetMonitor(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetMonitor |
| `greg.UI.MotionBlurSet` | `Il2Cpp.SettingsGraphics::SetMotionBlur(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetMotionBlur |
| `greg.UI.MusicVolume` | `Il2Cpp.SettingsVolume::MusicVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.MusicVolume |
| `greg.UI.NewGame` | `Il2Cpp.MainMenu::NewGame()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.NewGame |
| `greg.UI.NotAllowedToSaveOverlayOff` | `Il2Cpp.PauseMenu::NotAllowedToSaveOverlayOff()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.NotAllowedToSaveOverlayOff |
| `greg.UI.OnActionChange` | `Il2Cpp.RebindUIv2::OnActionChange(Il2CppSystem.Object, InputActionChange)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.OnActionChange |
| `greg.UI.OnDeselect` | `Il2Cpp.ToolTipOnUIText::OnDeselect()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnDeselect |
| `greg.UI.OnDestroy` | `Il2Cpp.BalanceSheet::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.OnDestroy |
| `greg.UI.OnDestroy` | `Il2Cpp.PauseMenu::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.OnDestroy |
| `greg.UI.OnDestroy` | `Il2Cpp.ToolTipOnUIText::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnDestroy |
| `greg.UI.OnDestroy` | `Il2Cpp.Tutorials::OnDestroy()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.OnDestroy |
| `greg.UI.OnLanguageDropDownChange` | `Il2Cpp.SettingsGameplay::OnLanguageDropDownChange(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.OnLanguageDropDownChange |
| `greg.UI.OnPause` | `Il2Cpp.PauseMenu::OnPause(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.OnPause |
| `greg.UI.OnPointerEnter` | `Il2Cpp.ToolTipOnUIText::OnPointerEnter(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnPointerEnter |
| `greg.UI.OnPointerExit` | `Il2Cpp.ToolTipOnUIText::OnPointerExit(PointerEventData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnPointerExit |
| `greg.UI.OnSelect` | `Il2Cpp.ToolTipOnUIText::OnSelect()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.OnSelect |
| `greg.UI.OnTabEnter` | `Il2Cpp.PauseMenu_TabGroup::OnTabEnter(PauseMenu_TabButton)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabGroup.OnTabEnter |
| `greg.UI.OnTabExit` | `Il2Cpp.PauseMenu_TabGroup::OnTabExit(PauseMenu_TabButton)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabGroup.OnTabExit |
| `greg.UI.OnTabSelected` | `Il2Cpp.PauseMenu_TabGroup::OnTabSelected(PauseMenu_TabButton)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabGroup.OnTabSelected |
| `greg.UI.OnValidate` | `Il2Cpp.ReBindUI::OnValidate()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.OnValidate |
| `greg.UI.OnVideoPrepared` | `Il2Cpp.Tutorials::OnVideoPrepared(VideoPlayer)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.OnVideoPrepared |
| `greg.UI.OpenCloseSection` | `Il2Cpp.UI_Section::OpenCloseSection()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UI_Section.OpenCloseSection |
| `greg.UI.PacketTypeSet` | `Il2Cpp.SettingsGameplay::SetPacketType(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.SetPacketType |
| `greg.UI.Pause` | `Il2Cpp.PauseMenu::Pause(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.Pause |
| `greg.UI.PlayVideo` | `Il2Cpp.Tutorials::PlayVideo(int, bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.PlayVideo |
| `greg.UI.PopulateLoadSaveMenu` | `Il2Cpp.PauseMenu::PopulateLoadSaveMenu(bool)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.PopulateLoadSaveMenu |
| `greg.UI.PopulateMonitors` | `Il2Cpp.SettingsGraphics::PopulateMonitors()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.PopulateMonitors |
| `greg.UI.ProcessConsoleCommand` | `Il2Cpp.PauseMenu::ProcessConsoleCommand(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.ProcessConsoleCommand |
| `greg.UI.QualitySet` | `Il2Cpp.SettingsGraphics::SetQuality(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetQuality |
| `greg.UI.QuitGame` | `Il2Cpp.MainMenu::QuitGame()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.QuitGame |
| `greg.UI.RacksVolume` | `Il2Cpp.SettingsVolume::RacksVolume(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.RacksVolume |
| `greg.UI.RefreshRateSet` | `Il2Cpp.SettingsGraphics::SetRefreshRate(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetRefreshRate |
| `greg.UI.RegisterSalary` | `Il2Cpp.BalanceSheet::RegisterSalary(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.RegisterSalary |
| `greg.UI.RepopulateResolutions` | `Il2Cpp.SettingsGraphics::RepopulateResolutions()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.RepopulateResolutions |
| `greg.UI.ResDropDownSet` | `Il2Cpp.SettingsGraphics::SetResDropDown(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetResDropDown |
| `greg.UI.ResetBinding` | `Il2Cpp.ReBindUI::ResetBinding()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.ResetBinding |
| `greg.UI.ResetDepthOfField` | `Il2Cpp.SettingsGraphics::ResetDepthOfField()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.ResetDepthOfField |
| `greg.UI.ResetTabs` | `Il2Cpp.PauseMenu_TabGroup::ResetTabs()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabGroup.ResetTabs |
| `greg.UI.ResetToDefault` | `Il2Cpp.RebindUIv2::ResetToDefault()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.ResetToDefault |
| `greg.UI.ResolutionSet` | `Il2Cpp.SettingsGraphics::SetResolution(int, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetResolution |
| `greg.UI.ResolveActionAndBinding` | `Il2Cpp.RebindUIv2::ResolveActionAndBinding(InputAction, int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.ResolveActionAndBinding |
| `greg.UI.RestoreRecord` | `Il2Cpp.BalanceSheet::RestoreRecord(CustomerRecordSaveData)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.RestoreRecord |
| `greg.UI.Resume` | `Il2Cpp.PauseMenu::Resume()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.Resume |
| `greg.UI.RouteEvalIntervalSet` | `Il2Cpp.SettingsGameplay::SetRouteEvalInterval(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.SetRouteEvalInterval |
| `greg.UI.SalaryRowAdded` | `Il2Cpp.BalanceSheet::AddSalaryRow(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.AddSalaryRow |
| `greg.UI.SaveOnButtonClickLoaded` | `Il2Cpp.PauseMenu::LoadSaveOnButtonClick(TextMeshProUGUI)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.LoadSaveOnButtonClick |
| `greg.UI.Saved` | `Il2Cpp.PauseMenu::Save(string, string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.Save |
| `greg.UI.SectionTitleAdded` | `Il2Cpp.BalanceSheet::AddSectionTitle(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.AddSectionTitle |
| `greg.UI.SettingsLoaded` | `Il2Cpp.SettingsControls::LoadSettings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsControls.LoadSettings |
| `greg.UI.SettingsLoaded` | `Il2Cpp.SettingsGameplay::LoadSettings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGameplay.LoadSettings |
| `greg.UI.SettingsLoaded` | `Il2Cpp.SettingsGraphics::LoadSettings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.LoadSettings |
| `greg.UI.SettingsLoaded` | `Il2Cpp.SettingsVolume::LoadSettings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsVolume.LoadSettings |
| `greg.UI.ShadowDistanceSet` | `Il2Cpp.SettingsGraphics::SetShadowDistance(float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetShadowDistance |
| `greg.UI.ShowKeyboadMelee` | `Il2Cpp.KeyHint::ShowKeyboadMelee()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/KeyHint.ShowKeyboadMelee |
| `greg.UI.ShowTooltipForInteract` | `Il2Cpp.ToolTipInteract::ShowTooltipForInteract(string, Sprite)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipInteract.ShowTooltipForInteract |
| `greg.UI.ShowTooltipWorldCanvas` | `Il2Cpp.Tooltip::ShowTooltipWorldCanvas(string, RectTransform, Camera)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tooltip.ShowTooltipWorldCanvas |
| `greg.UI.ShowTutorial` | `Il2Cpp.Tutorials::ShowTutorial(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.ShowTutorial |
| `greg.UI.SkipTutorials` | `Il2Cpp.Tutorials::SkipTutorials()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.SkipTutorials |
| `greg.UI.SnapshotSaved` | `Il2Cpp.BalanceSheet::SaveSnapshot(int, Il2CppSystem.DateTime)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.SaveSnapshot |
| `greg.UI.StartInteractiveRebind` | `Il2Cpp.RebindUIv2::StartInteractiveRebind()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2.StartInteractiveRebind |
| `greg.UI.StopTutorial` | `Il2Cpp.Tutorials::StopTutorial()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.StopTutorial |
| `greg.UI.StopVideoInPauseMenu` | `Il2Cpp.Tutorials::StopVideoInPauseMenu()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials.StopVideoInPauseMenu |
| `greg.UI.Subscribe` | `Il2Cpp.PauseMenu_TabGroup::Subscribe(PauseMenu_TabButton)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu_TabGroup.Subscribe |
| `greg.UI.ToChatOutputAdded` | `Il2Cpp.ChatController::AddToChatOutput(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ChatController.AddToChatOutput |
| `greg.UI.ToolTip` | `Il2Cpp.ToolTipOnUIText::ToolTip()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ToolTipOnUIText.ToolTip |
| `greg.UI.TotalRowAdded` | `Il2Cpp.BalanceSheet::AddTotalRow(float, float, float)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.AddTotalRow |
| `greg.UI.TrackFinances` | `Il2Cpp.BalanceSheet::TrackFinances()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/BalanceSheet.TrackFinances |
| `greg.UI.UIChanged` | `Il2Cpp.ReBindUI::UpdateUI()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI.UpdateUI |
| `greg.UI.UnLoadLevel` | `Il2Cpp.LoadingScreen::UnLoadLevel(int)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/LoadingScreen.UnLoadLevel |
| `greg.UI.UnregisterCallbacks` | `Il2Cpp.UIActions::UnregisterCallbacks(InputController.IUIActions)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/UIActions.UnregisterCallbacks |
| `greg.UI.WithOverlayLoaded` | `Il2Cpp.PauseMenu::LoadWithOverlay(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu.LoadWithOverlay |
| `greg.UI._Awake_b__28_0` | `Il2Cpp.PauseMenu::_Awake_b__28_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu._Awake_b__28_0 |
| `greg.UI._Awake_b__28_1` | `Il2Cpp.PauseMenu::_Awake_b__28_1(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu._Awake_b__28_1 |
| `greg.UI._Awake_b__28_2` | `Il2Cpp.PauseMenu::_Awake_b__28_2(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu._Awake_b__28_2 |
| `greg.UI._LoadSaveOnButtonClick_b__35_0` | `Il2Cpp.PauseMenu::_LoadSaveOnButtonClick_b__35_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu._LoadSaveOnButtonClick_b__35_0 |
| `greg.UI._OnEnable_b__16_0` | `Il2Cpp.ReBindUI::_OnEnable_b__16_0()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI._OnEnable_b__16_0 |
| `greg.UI._OnEnable_b__16_1` | `Il2Cpp.ReBindUI::_OnEnable_b__16_1()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/ReBindUI._OnEnable_b__16_1 |
| `greg.UI._SaveConfirm_b__37_0` | `Il2Cpp.PauseMenu::_SaveConfirm_b__37_0(string)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/PauseMenu._SaveConfirm_b__37_0 |
| `greg.UI._Start_b__11_0` | `Il2Cpp.Tutorials::_Start_b__11_0(InputAction.CallbackContext)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/Tutorials._Start_b__11_0 |
| `greg.UI._UpdateBindingDisplay_b__30_0` | `Il2Cpp.RebindUIv2::_UpdateBindingDisplay_b__30_0(InputBinding)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/RebindUIv2._UpdateBindingDisplay_b__30_0 |
| `greg.UI.setmount` | `Il2Cpp.MainMenuCamera::setmount(Transform)` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenuCamera.setmount |
| `greg.UI.tingsSet` | `Il2Cpp.MainMenu::Settings()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/MainMenu.Settings |
| `greg.UI.upAASet` | `Il2Cpp.SettingsGraphics::SetupAA()` | `Postfix` | Auto-generated from IL2CPP sources: Il2Cpp/SettingsGraphics.SetupAA |


---

*Generated by `scripts/generate_api_docs.py` — see [README.md](../README.md) for details.*
