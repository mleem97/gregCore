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

## 2024-07-01 - Prefix-Matching Sandbox Escape
**Vulnerability:** In `GregIoLuaModule.cs`, sandbox path validation used `fullPath.StartsWith(dataDirFull)` without ensuring `dataDirFull` had a trailing directory separator. This allowed escaping the intended `Mods/MyMod/data` directory into paths like `Mods/MyMod/data_secret`.
**Learning:** Prefix matching for paths is vulnerable if boundaries are not strictly defined by directory separators.
**Prevention:** Always append a trailing directory separator to the base directory before using `String.StartsWith` for path validation, and explicitly allow exact matches to the base directory itself.

## 2024-07-01 - Path Traversal in Mod Entity Registration
**Vulnerability:** `CustomEmployeeManager.Register` accepted arbitrary employee IDs without validation, which were later used directly in `Path.Combine` to construct image loading paths, enabling path traversal (CWE-22).
**Learning:** Identifiers provided by mods or external sources must be treated as untrusted input and validated before being used in file system operations.
**Prevention:** Validate input strings that form part of a file path before concatenating them. Reject them if they contain directory traversal characters like `..`, `Path.DirectorySeparatorChar`, `Path.AltDirectorySeparatorChar`, or any invalid filename characters (using `Path.GetInvalidFileNameChars()`).
## 2024-10-24 - Path Traversal via Search Pattern in Lua `list_files`
**Vulnerability:** In `GregIoLuaModule.cs`, the `list_files` function accepted a `pattern` string that was passed directly to `Directory.GetFiles(..., pattern, ...)`. Although standard file operations were sandbox-validated via `ResolveSafe`, the `pattern` argument wasn't validated, allowing paths like `../../` to be interpreted by `Directory.GetFiles`, causing directory traversal and reading files outside the Lua sandbox.
**Learning:** `Directory.GetFiles` is vulnerable to path traversal if the `searchPattern` argument contains traversal characters like `..`, `/`, or `\`.
**Prevention:** Always validate or sanitize user-provided search patterns to prevent unauthorized file system enumeration. Specifically for search patterns, deny any pattern that contains directory separators (`/` or `\`) or traversal characters (`..`).
## 2024-05-14 - Path Traversal bypass via Backslash
**Vulnerability:** Path traversal checks in Lua sandbox APIs only normalized forward slashes (`/`), allowing `\` to bypass directory containment checks on platforms where `\` is interpreted as a directory separator by the underlying IO APIs.
**Learning:** Normalizing paths for directory traversal checks requires replacing both `/` and `\` with `Path.DirectorySeparatorChar` before path combination or validation.
**Prevention:** Always normalize both `/` and `\` to `Path.DirectorySeparatorChar` before using `Path.GetFullPath` to validate sandbox boundaries.
## 2024-10-24 - Path Traversal via Un-normalized Backslashes
**Vulnerability:** In `GregIoLuaModule.cs`, the `ResolveSafe` function normalized forward slashes (`/`) to the OS directory separator to prevent traversal, but failed to normalize backslashes (`\`). This allowed path traversal on systems where `Path.Combine` and `Path.GetFullPath` interpret alternate separators (like `\`) as valid directory boundaries even if `Path.DirectorySeparatorChar` is `/`.
**Learning:** When implementing path traversal or sandbox containment checks in C#, ensure both forward slashes (`/`) and backslashes (`\`) are normalized to `Path.DirectorySeparatorChar` before combining or validating paths.
**Prevention:** Explicitly normalize both slash types to `Path.DirectorySeparatorChar` when sanitizing untrusted paths (replace `/` and `\` before validation).
## 2024-10-24 - Path Traversal via Search Pattern: explicit slash checks
**Vulnerability:** `SanitizeSearchPattern` in `GregIoLuaModule.cs` only rejected `Path.DirectorySeparatorChar` and `Path.AltDirectorySeparatorChar`. On platforms where neither matches a given slash (e.g. `\` on Linux), a crafted pattern could bypass the check while the underlying OS APIs still resolve the slash as a directory boundary, enabling traversal via `Directory.GetFiles`.
**Learning:** Platform separator chars are insufficient for sandbox validation; explicitly reject both `/` and `\` in user-supplied search patterns.
**Prevention:** In addition to the platform defaults, explicitly deny `/` and `\` in search patterns before passing them to file system enumeration APIs.
