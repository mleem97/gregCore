## 2024-04-28 - Path Traversal Vulnerability in Persistence Service

**Vulnerability:** The `GregPersistenceService` used un-sanitized user input (`key`) to construct file paths for reading, writing, and deleting configuration files (`Path.Combine(_saveDirectory, $"{key}.json")`), leading to a critical Path Traversal (CWE-22) vulnerability.
**Learning:** The public API surface allowed callers to pass arbitrary keys (e.g. `../../Windows/System32/config/SAM`). This vulnerability was caused by blindly trusting user-provided file names.
**Prevention:** Always validate user input against path traversal attacks when constructing file paths dynamically. Ensure that the input does not contain directory separators (`Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or `..`) or invalid filename characters (`Path.GetInvalidFileNameChars()`). Use a validation wrapper or helper method before applying `Path.Combine`.## 2024-04-26 - [Path Traversal in Persistence Layer]
**Vulnerability:** Path Traversal vulnerability in `src/Infrastructure/Config/GregPersistenceService.cs`.
**Learning:** Keys provided to the persistence service were interpolated directly into file paths without validation. If a user provided a key like `../../windows/system32/cmd`, it could allow reading or writing to arbitrary locations on the system.
**Prevention:** Validate file paths constructed from dynamic input to ensure they don't contain path traversal characters like `../`, invalid characters from `Path.GetInvalidFileNameChars()`, or directory separators (`Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`).

## 2024-05-01 - Path Traversal in ModConfigSystem
**Vulnerability:** Path traversal vulnerability due to unsanitized `modId` in `GetConfigPath` in `src/Compatibility/DataCenterModLoader/ModConfigSystem.cs`.
**Learning:** Concatenating user input (like a `modId`) directly into `Path.Combine` allows for directory traversal attacks (`../`, etc.) leading to arbitrary file read/write issues.
**Prevention:** Validate input strings that form part of a file path before concatenating them. Reject them if they contain directory traversal characters like `..`, `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or any invalid filename characters (using `Path.GetInvalidFileNameChars()`).

## 2024-05-15 - Prefix-Matching Path Traversal Bypass in Lua Sandbox
**Vulnerability:** The Lua sandbox used `String.StartsWith` to check if resolved paths were within the allowed sandbox directories (`GregIoLuaModule.cs`, `LuaModuleLoader.cs`, `LuaHotReload.cs`). This allowed a prefix-matching path traversal bypass. For example, if the sandbox directory is `/mods/modA`, a path like `/mods/modA_secret/secret.txt` would pass the `StartsWith` check, allowing unauthorized access outside the sandbox.
**Learning:** Using `String.StartsWith` on file paths without ensuring the base directory ends with a directory separator character is a common security pitfall that allows bypassing directory boundary checks.
**Prevention:** When validating sandbox paths using string prefixes, always ensure the base directory string ends with `Path.DirectorySeparatorChar` before performing the `String.StartsWith` check, and handle exact directory matches if necessary.

## 2024-05-16 - CI Build Failure due to Missing Artifact
**Vulnerability:** The CI workflow `.github/workflows/build.yml` failed because it unconditionally attempted to copy `framework/greg_hooks.json`, which was not always present in the build environment. While not a security vulnerability in the code, fragile CI pipelines can impede the rapid deployment of security fixes.
**Learning:** Hardcoding file copies in CI pipelines without checking for file existence can lead to unexpected build failures when artifact generation is conditional or environments differ.
**Prevention:** Always wrap file operations (like `cp`) for potentially missing artifacts in existence checks (e.g., `if [ -f "file" ]; then cp ...; fi`) within CI scripts.
