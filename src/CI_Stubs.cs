using System;
using MoonSharp.Interpreter;
using gregCore.Core.Events;
using gregCore.Core.Abstractions;

namespace gregCore.Infrastructure.Scripting.Lua.Dev {
    public class LuaPluginReloadInfo {
        public string ModId = "";
        public string MainFilePath = "";
    }
    public class LuaHotReload {
        public LuaHotReload(string dir, Action<LuaPluginReloadInfo> callback) { }
        public void Start() { }
        public void Stop() { }
        public void RegisterPlugin(string id, Script script, string path) { }
    }
    public class LuaHookBindingGenerator {
        public int TotalHookCount = 0;
        public LuaHookBindingGenerator(GregEventBus bus, string file) { }
        public void LoadHooks() { }
        public void RegisterInScript(Script script, Table table, string id) { }
    }
    public class LuaRepl {
        public void Initialize() { }
        public void Update() { }
    }
    public class LuaProfiler {
        public LuaProfiler(float budget) { }
        public IDisposable BeginScope(string name) { return null; }
        public void EndFrame() { }
    }
    public class LuaErrorOverlay {
        public void ReportError(string id, string msg) { }
    }
}

namespace gregCore.Bridge.LuaFFI {
    public class LuaCoroutineScheduler {
        public LuaCoroutineScheduler(Script script) { }
        public void Register(Table table) { }
        public void OnUpdate(float dt) { }
    }
}

namespace gregCore.Compatibility.FishNet { }

namespace gregCore.Infrastructure.Networking {
    public class GregNetworkRack : IDisposable {
        public GregNetworkRack(GregEventBus bus, IGregLogger logger) { }
        public void Dispose() { }
    }
    public class GregNetworkServer : IDisposable {
        public GregNetworkServer(GregEventBus bus, IGregLogger logger) { }
        public void Dispose() { }
    }
    public class GregNetworkCables : IDisposable {
        public int CableCount = 0;
        public GregNetworkCables(GregEventBus bus, IGregLogger logger) { }
        public void Dispose() { }
    }
}

namespace gregCore.Infrastructure.Scripting.Lua.Modules {
    public class LuaModuleLoader {
        public LuaModuleLoader(Script script, string dir, string shared) { }
        public void Register() { }
    }
    public class LuaPlayerModule { public static void Register(Table t, Script s, string id) { } }
    public class LuaWorldModule { public static void Register(Table t, Script s, string id) { } }
    public class LuaRackModule { public static void Register(Table t, Script s, string id) { } }
    public class LuaServerModule { public static void Register(Table t, Script s, string id) { } }
    public class LuaCableModule { public static void Register(Table t, Script s, string id) { } }
    public class LuaUiModule { public static void Register(Table t, Script s, string id) { } }
}
