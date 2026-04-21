using MelonLoader;

namespace greg.Logging
{
    public static class GregBanner
    {
        public static void Print(string version, string mlVersion, bool debugMode)
        {
            string modeString = debugMode ? "DEBUG" : "RELEASE";

            MelonLogger.Msg("");
            MelonLogger.Msg("  =============================================");
            MelonLogger.Msg("   ____  ____  _____ ____  ____  ___  ____  _____ ");
            MelonLogger.Msg(@"  / ___|  _ \| ____/ ___|/ ___|/ _ \|  _ \| ____|");
            MelonLogger.Msg(@" | |  _| |_) |  _|| |  | |  _| | | | |_) |  _|  ");
            MelonLogger.Msg(@" | |_| |  _ <| |__| |__| |_| | |_| |  _ <| |___  ");
            MelonLogger.Msg(@"  \____|_| \_\_____\____|\____|\___/|_| \_\_____|");
            MelonLogger.Msg("  =============================================");
            MelonLogger.Msg("  gregCore Modding Framework");
            MelonLogger.Msg($"  Version : {version}");
            MelonLogger.Msg("  Game    : Data Center (Waseku)");
            MelonLogger.Msg($"  Loader  : MelonLoader {mlVersion}");
            MelonLogger.Msg($"  Mode    : {modeString}");
            MelonLogger.Msg("  =============================================");
            MelonLogger.Msg("");
        }
    }
}
