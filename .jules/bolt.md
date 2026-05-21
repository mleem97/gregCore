## 2026-05-21 - Optimize FindObjectsOfTypeAll

**Learning:** `UnityEngine.Resources.FindObjectsOfTypeAll<T>` is a very expensive O(N) operation to use for live lookups during gameplay. GameAPI methods like `ObjFindByIdImpl` and `FindHandleByStableId` were scanning all objects to find a handle matching an ID.
**Action:** Use global singletons managed by the game, such as `Il2Cpp.NetworkMap.instance.servers` and `Il2Cpp.NetworkMap.instance.switches` which are dictionaries allowing O(1) or faster enumeration than scanning all objects in memory. The fallback `FindObjectsOfTypeAll` is still maintained for `PatchPanel` or cases where objects are not in the dictionary.
