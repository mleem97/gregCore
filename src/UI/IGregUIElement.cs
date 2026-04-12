using UnityEngine;

namespace greg.Core.UI;

/// <summary>
/// Interface for all gregUI elements.
/// </summary>
public interface IGregUIElement
{
    string PanelId { get; }
    bool   IsVisible { get; }
    void   Attach(Canvas rootCanvas);
    void   Show();
    void   Hide();
    void   Toggle();
    void   Destroy();
}
