## 2025-05-21 - Path Traversal bypass via StartsWith
**Vulnerability:** Path Traversal bypass via `StartsWith`. `Path.GetFullPath` combined with `StartsWith` could allow access to a sibling directory if it shares the same prefix. E.g., validating that `/app/data-secret` starts with `/app/data` returns true.
**Learning:** Checking prefixes with `StartsWith` for paths does not guarantee that the path actually resides within a specific directory, unless the base directory path is guaranteed to end with a directory separator.
**Prevention:** Always ensure the directory validation base path string ends with a directory separator (`/` or `\`) before checking with `StartsWith`. In addition, allow an exact equality check to ensure the base directory itself can still be referenced.
