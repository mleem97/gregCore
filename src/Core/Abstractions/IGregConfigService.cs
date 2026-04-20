/// <file-summary>
/// Schicht:      Core
/// Zweck:        Interface für Konfigurations-Management.
/// Maintainer:   Wird für mod.json und globale Configs (Newtonsoft.Json) genutzt.
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregConfigService
{
    T? LoadConfig<T>(string filePath) where T : class;
    void SaveConfig<T>(string filePath, T config) where T : class;
}
