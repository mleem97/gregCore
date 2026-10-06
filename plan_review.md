1. **Optimize `TryFindInServers`, `TryFindInSwitches`, and `TryFindInPatchPanels` in `src/gregCore.Compatibility/Patch_Rack_MarkPositionAsUsed.cs`**
   - Use `python3` string replacement to optimize the O(N) `FindObjectsOfType<T>` calls in `Patch_Rack_MarkPositionAsUsed.cs` with an O(1) loop iteration by directly checking the dictionaries inside `global::Il2Cpp.NetworkMap.instance` (`.servers`, `.switches`, `.patchPanels`) first, and falling back to `FindObjectsOfType<T>` only when `NetworkMap` is unavailable or empty. Note that values must be cast safely using `?.TryCast<T>()`.
   - Remove temporary script files matching `patch_*.py`.
2. **Verify changes**
   - Use `git diff` to ensure the modifications were successfully applied to the correct file.
3. **Update Journal `bolt.md`**
   - Append the learning regarding the performance optimization in `Patch_Rack_MarkPositionAsUsed.cs` to `.jules/bolt.md` without modifying other contents.
4. **Pre-commit Rule**
   - Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.
5. **Submit the changes**
   - Commit and submit the code changes with an appropriate title and PR description according to the guidelines.
