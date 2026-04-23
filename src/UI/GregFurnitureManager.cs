using System;
using UnityEngine;
using MelonLoader;
using gregCore.API;
using gregCore.UI;

namespace greg.Furniture
{
    public class GregFurnitureManager : MonoBehaviour
    {
        private static GregFurnitureManager? _instance;
        public static GregFurnitureManager Instance => _instance ??= Create();

        private bool _isBuildModeActive = false;
        private GameObject? _ghostObject;
        private float _gridSize = 0.5f; // 1sq = 4sq (0.5m steps)
        private Material? _ghostMaterial;

        private static GregFurnitureManager Create()
        {
            var go = new GameObject("greg_FurnitureManager");
            UnityEngine.Object.DontDestroyOnLoad(go);
            return go.AddComponent<GregFurnitureManager>();
        }

        public void ToggleBuildMode(GameObject? prefab = null)
        {
            _isBuildModeActive = !_isBuildModeActive;
            
            if (_isBuildModeActive && prefab != null)
            {
                CreateGhost(prefab);
                GregUIManager.SetPanelActive("PlacementWidget", true);
            }
            else
            {
                DestroyGhost();
                GregUIManager.SetPanelActive("PlacementWidget", false);
            }
        }

        private void CreateGhost(GameObject prefab)
        {
            _ghostObject = UnityEngine.Object.Instantiate(prefab);
            // Disable colliders for ghost
            foreach (var col in _ghostObject.GetComponentsInChildren<Collider>()) col.enabled = false;
            
            // Apply semi-transparent material
            if (_ghostMaterial == null)
            {
                _ghostMaterial = new Material(Shader.Find("Standard"));
                _ghostMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _ghostMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _ghostMaterial.SetInt("_ZWrite", 0);
                _ghostMaterial.DisableKeyword("_ALPHATEST_ON");
                _ghostMaterial.EnableKeyword("_ALPHABLEND_ON");
                _ghostMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                _ghostMaterial.renderQueue = 3000;
            }
            
            foreach (var renderer in _ghostObject.GetComponentsInChildren<Renderer>())
            {
                renderer.material = _ghostMaterial;
                renderer.material.color = new Color(0, 0.75f, 0.65f, 0.4f); // Primary Teal @ 40%
            }
        }

        private void DestroyGhost()
        {
            if (_ghostObject != null) UnityEngine.Object.Destroy(_ghostObject);
            _ghostObject = null;
        }

        private void Update()
        {
            if (!_isBuildModeActive || _ghostObject == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                Vector3 snappedPos = SnapToGrid(hit.point);
                _ghostObject.transform.position = snappedPos;

                // Surface alignment (Wall vs Floor)
                if (Mathf.Abs(hit.normal.y) < 0.1f) // Wall hit
                {
                    _ghostObject.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
                else // Floor hit
                {
                    _ghostObject.transform.rotation = Quaternion.identity;
                }

                if (Input.GetMouseButtonDown(0))
                {
                    PlaceObject(snappedPos, _ghostObject.transform.rotation);
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape)) ToggleBuildMode();
        }

        private Vector3 SnapToGrid(Vector3 pos)
        {
            return new Vector3(
                Mathf.Round(pos.x / _gridSize) * _gridSize,
                pos.y, // Maintain contact height
                Mathf.Round(pos.z / _gridSize) * _gridSize
            );
        }

        private void PlaceObject(Vector3 pos, Quaternion rot)
        {
            // Implementation of final placement and persistence
            MelonLogger.Msg($"Placed object at {pos}");
            // GregSaveEngine.Instance.RecordPlacement(...)
        }
    }
}
