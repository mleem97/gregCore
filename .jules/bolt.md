## 2026-05-21 - [Deprecated Unimplemented API Methods]
**Learning:** Empty, unimplemented public API methods with logging but no functionality mislead developers into thinking the API is functional.
**Action:** Always mark known unimplemented, public-facing API methods with `[Obsolete]` (setting the `error` flag to `true` if appropriate) and throw a `NotImplementedException` instead of providing an empty implementation or a stub that silently fails.
