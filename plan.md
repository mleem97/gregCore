1. **Optimize `GetRackCount()` in `src/PublicApi/Modules/GregFacilityModule.cs`**
   - Read the file `src/PublicApi/Modules/GregFacilityModule.cs`.
   - Update `GetRackCount()` to use `Il2Cpp.NetworkMap.instance.GetNumberOfDevices()` for O(1) performance.
   - Define a constant `DEVICE_INDEX_RACKS = 2` as instructed by the knowledge base.
   - Fallback to `FindObjectsOfType` if `instance` or the array is null or shorter than the index.
   - Save the file using `run_in_bash_session` with heredoc.
   - Verify the modification using `cat src/PublicApi/Modules/GregFacilityModule.cs`.
2. **Optimize `GetRackCount()` in `src/API/GregAPI.cs`**
   - Read the file `src/API/GregAPI.cs` to confirm the code.
   - Update `GetRackCount()` similarly to use `NetworkMap.instance.GetNumberOfDevices()`.
   - Define `DEVICE_INDEX_RACKS = 2`.
   - Keep the fallback.
   - Save the file using `run_in_bash_session` and a python script.
   - Verify the modification.
3. **Run tests**
   - Run the tests with `dotnet test tests/gregCore.Tests.csproj --filter "FullyQualifiedName!~CablePatchTests"`.
4. **Complete pre-commit steps to ensure proper testing, verification, review, and reflection are done.**
5. **Submit the PR**
   - Use the `submit` tool to present the performance improvement as "Bolt".
