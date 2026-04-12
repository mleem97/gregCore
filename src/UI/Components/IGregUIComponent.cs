using UnityEngine;

namespace greg.Core.UI.Components;

/// <summary>
/// Interface for a component within a gregUI panel.
/// </summary>
public interface IGregUIComponent
{
    GameObject Build(Transform parent);
}
