// gregCore TypeScript bridge example (discovery scaffold)
// Place transpiled JS output in Mods/ScriptMods/js/

export interface GregScriptMod {
  id: string;
  name: string;
  onSceneLoaded?: (scene: string) => void;
  onUpdate?: (deltaTime: number) => void;
}

const mod: GregScriptMod = {
  id: "example.ts.mod",
  name: "TypeScript Example Mod",
  onSceneLoaded(scene) {
    console.log(`[ts] scene loaded: ${scene}`);
  },
  onUpdate(_deltaTime) {}
};

export default mod;
