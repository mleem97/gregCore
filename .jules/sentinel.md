## 2024-05-18 - Path Traversal bypass via StartsWith
**Vulnerability:** The sandbox I/O validation used `fullPath.StartsWith(dataDirFull)` without ensuring `dataDirFull` ended with a directory separator. This allowed access to sibling directories with the same prefix (e.g., `data` vs `data-secret`).
**Learning:** `StartsWith` is insufficient for path boundary validation unless a directory separator is explicitly enforced, as prefix matching can bypass intended restrictions.
**Prevention:** Always append `Path.DirectorySeparatorChar` to base directories before validating via prefix, and explicitly handle exact matches to the base directory if necessary.
