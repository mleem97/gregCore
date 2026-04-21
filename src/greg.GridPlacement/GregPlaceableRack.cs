using UnityEngine;

namespace greg.GridPlacement
{
    public class GregPlaceableRack
    {
        public string RackId { get; set; } = string.Empty;
        public Vector2Int GridCoord { get; set; }
        public GameObject? UnityGameObject { get; set; }
        // public Il2CppObjectBase? VanillaRackRef { get; set; } // Omitted because Il2Cpp types cannot be imported safely without specific Il2CppInterop assemblies

        public void PlaceAt(Vector2Int coord, Vector3 worldPos)
        {
            GridCoord = coord;
            if (UnityGameObject != null)
            {
                UnityGameObject.transform.position = worldPos;
            }
        }

        public void Remove()
        {
            if (UnityGameObject != null)
            {
                UnityEngine.Object.Destroy(UnityGameObject);
            }
        }

        public void Highlight(bool active)
        {
            // Dummy highlight code
            if (UnityGameObject != null)
            {
                var renderer = UnityGameObject.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = active ? Color.cyan : Color.white;
                }
            }
        }
    }
}
