using UnityEngine;

namespace gregCoreSDK.Core.UI.Components;

/// <summary>
/// Interface for a component within a gregUI panel.
/// </summary>
public interface IGregUIComponent
{
    GameObject Build(Transform parent);
}
