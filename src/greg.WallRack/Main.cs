using System;
using MelonLoader;
using greg.Logging;

namespace greg.WallRack
{
    public class Main : MelonMod
    {
        private GregModLogger _log = null!;

        public override void OnInitializeMelon()
        {
            // Framework guard first
            if (gregCore.Core.GregCoreMod.Instance == null)
            {
                LoggerInstance.Warning("[gC-OnInitializeMelon] gregCore not ready.");
                return;
            }

            _log = new GregModLogger("WallRack");

            _log.Section("Init");
            _log.Msg("Starting initialization.");

            _log.PatchApplied("CustomerDevice.Initialize");
            _log.HookSubscribed("greg.SYSTEM.ButtonBuyWall");
            _log.FeatureState("WallRack", true);

            _log.Msg("Initialization complete.");
        }

        public override void OnApplicationQuit()
        {
            _log?.Section("Shutdown");
            _log?.Msg("Unloading.");
            _log?.Msg("Hooks deregistered. Goodbye.");
        }
    }
}
