using System.IO;
using System.Text.Json;
using MelonLoader;

namespace greg.Multiplayer
{
    public class MultiplayerConfig
    {
        public int Port { get; set; } = 7777;
        public int MaxPlayers { get; set; } = 8;
        public string RelayHost { get; set; } = "relay.gregcore.local";
        public int RelayPort { get; set; } = 7778;
        public bool UseRelay { get; set; } = false;
        public bool RequireApproval { get; set; } = true;
        public bool EnableVoiceChat { get; set; } = false;
        public string LogLevel { get; set; } = "Info";

        private static readonly string ConfigPath =
            Path.Combine(MelonEnvironment.ModsDirectory, "greg.Multiplayer.config.json");

        public static MultiplayerConfig Load()
        {
            if (!File.Exists(ConfigPath))
            {
                var defaults = new MultiplayerConfig();
                Save(defaults);
                return defaults;
            }
            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<MultiplayerConfig>(json) ?? new MultiplayerConfig();
        }

        public static void Save(MultiplayerConfig cfg)
        {
            var opts = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(cfg, opts));
        }
    }
}
