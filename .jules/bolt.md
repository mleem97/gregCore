## 2025-05-15 - [Security Fix: Weak Hashing]
**Learning:** MD5 should be avoided for ID generation or any cryptographic purpose. SHA256 is a more secure alternative. When a 16-byte Guid is needed, the first 16 bytes of a SHA256 hash can be used.
**Action:** Always prefer SHA256 over MD5 for deterministic ID generation.

## 2025-05-15 - [Environment: Dotnet Tooling Timeouts]
**Learning:** The `dotnet` CLI can time out in certain restricted environments during restore or build.
**Action:** Use `--no-restore` if dependencies are already present, or use background execution with log files if commands take longer than the session allows. Ensure junk files are cleaned up before submission.
