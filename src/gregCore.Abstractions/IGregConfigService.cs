/// <file-summary>
/// Layer:       Core
/// Purpose:      Interface for configuration management.
/// Maintainer:   Used for mod.json and global configs (Newtonsoft.Json).
/// </file-summary>

namespace gregCore.Core.Abstractions;

public interface IGregConfigService
{
    T? LoadConfig<T>(string filePath) where T : class;
    void SaveConfig<T>(string filePath, T config) where T : class;
}
