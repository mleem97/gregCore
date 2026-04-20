/// <file-summary>
/// Schicht:      Infrastructure
/// Zweck:        Config-Service basierend auf Newtonsoft.Json.
/// Maintainer:   Unterstützt JsonComments für mod.json und globale Configs.
/// </file-summary>

using System.IO;
using Newtonsoft.Json;

namespace gregCore.Infrastructure.Config;

public sealed class GregConfigService : IGregConfigService
{
    private readonly IGregLogger _logger;

    public GregConfigService(IGregLogger logger)
    {
        _logger = logger.ForContext("ConfigService");
    }

    public T? LoadConfig<T>(string filePath) where T : class
    {
        try
        {
            if (!File.Exists(filePath)) return null;
            var json = File.ReadAllText(filePath);
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
            {
                MissingMemberHandling = MissingMemberHandling.Ignore
            });
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler beim Laden der Config {filePath}", ex);
            return null;
        }
    }

    public void SaveConfig<T>(string filePath, T config) where T : class
    {
        try
        {
            var dir = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);

            var json = JsonConvert.SerializeObject(config, Formatting.Indented);
            File.WriteAllText(filePath, json);
        }
        catch (Exception ex)
        {
            _logger.Error($"Fehler beim Speichern der Config {filePath}", ex);
        }
    }
}
