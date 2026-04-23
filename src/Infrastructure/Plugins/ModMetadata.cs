using System.Collections.Generic;

namespace gregCore.Infrastructure.Plugins;

public class ModMetadata
{
    public string ModId { get; set; } = null!;
    public string PersistentId { get; set; } = string.Empty;
    public string Name { get; set; } = null!;
    public string Version { get; set; } = null!;
    public object ApiObject { get; set; } = null!;
    public bool HasSettings { get; set; }
    public bool HasKeybinds { get; set; }
    public List<string> CustomTabs { get; set; } = new();
}
