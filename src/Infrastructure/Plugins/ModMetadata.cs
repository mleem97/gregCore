using System.Collections.Generic;

namespace gregCore.Infrastructure.Plugins;

public class ModMetadata
{
    public string ModId { get; set; }
    public string Name { get; set; }
    public string Version { get; set; }
    public object ApiObject { get; set; }
    public bool HasSettings { get; set; }
    public bool HasKeybinds { get; set; }
    public List<string> CustomTabs { get; set; } = new();
}
