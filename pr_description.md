🎯 **What:**
Replaced an unimplemented `TODO: implement event queue for lobby callbacks` in `GameAPI.cs` with a fully functional, thread-safe `ConcurrentQueue` implementation for `SteamLobbyEvent` structs.

💡 **Why:**
This prevents lobby callbacks from being silently dropped and avoids returning `0` continuously for `SteamPollEventImpl`. Using a thread-safe queue ensures reliable cross-thread communication between Steam networking events and the game's polling thread.

✅ **Verification:**
- The codebase was successfully built locally (`dotnet build gregCore.csproj -c Release`).
- Tests passed (with known environment limitations handled gracefully).
- Reviewed against interop memory safety patterns (proper use of `Marshal.WriteInt64` and `TryDequeue`).

✨ **Result:**
`SteamPollEventImpl` now correctly populates `outType` and `outData` when events are queued, restoring correct functionality to the multiplayer/steam lobby modding API.
