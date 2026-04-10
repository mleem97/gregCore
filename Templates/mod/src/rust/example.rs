// gregCore Rust bridge example (native path)
// Compile to a shared library and place in Mods/RustMods/.

#[allow(dead_code)]
pub struct GregModInfo {
    pub id: &'static str,
    pub name: &'static str,
    pub version: &'static str,
}

#[allow(dead_code)]
pub fn mod_info() -> GregModInfo {
    GregModInfo {
        id: "example.rust.mod",
        name: "Rust Example Mod",
        version: "0.1.0",
    }
}
