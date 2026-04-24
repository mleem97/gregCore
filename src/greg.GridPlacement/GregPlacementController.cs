using UnityEngine;
using gregCore.API;
using System;

namespace greg.GridPlacement
{
    public class GregPlacementController : MonoBehaviour
    {
        public bool BuildModeActive { get; private set; }
        private GregPlaceableRack? _previewRack;
        private GregGridManager _grid = null!;
        private Material? _gridMaterial;

        private void Start()
        {
            _grid = GregGridManager.Instance;
            if (_grid == null)
            {
                _grid = new GregGridManager();
                _grid.Initialize(Vector3.zero, 50, 50);
            }
        }

        public void ActivateBuildMode()
        {
            if (frameworkSdk.GregFeatureGuard.IsVanillaSave)
            {
                GregAPI.LogWarning("Vanilla Save detected — Grid Placement disabled");
                return;
            }
            if (!frameworkSdk.GregFeatureGuard.IsEnabled("GridPlacement")) return;

            BuildModeActive = true;
            _previewRack = new GregPlaceableRack { RackId = "preview", UnityGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube) };
            if (_previewRack.UnityGameObject != null)
            {
                var col = _previewRack.UnityGameObject.GetComponent<UnityEngine.Collider>();
                if (col != null) UnityEngine.Object.Destroy(col);
                
                var renderer = _previewRack.UnityGameObject.GetComponent<UnityEngine.Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = new Color(0.38f, 0.96f, 0.85f, 0.4f); // 61F4D8 40% alpha
                }
            }

            var payload = new gregCore.Sdk.Models.GregPayload("greg.WORLD.BuildModeToggled", "gregCore");
            payload.Data["active"] = true;
            GregAPI.Hooks.Fire("greg.WORLD.BuildModeToggled", payload);
        }

        public void DeactivateBuildMode()
        {
            BuildModeActive = false;
            if (_previewRack != null)
            {
                _previewRack.Remove();
                _previewRack = null;
            }

            var payload = new gregCore.Sdk.Models.GregPayload("greg.WORLD.BuildModeToggled", "gregCore");
            payload.Data["active"] = false;
            GregAPI.Hooks.Fire("greg.WORLD.BuildModeToggled", payload);
        }

        public void OnUpdate()
        {
            if (!BuildModeActive || _previewRack?.UnityGameObject == null) return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (UnityEngine.Physics.Raycast(ray, out UnityEngine.RaycastHit hit))
            {
                Vector3 snapPos = _grid.SnapToGrid(hit.point);
                _previewRack.UnityGameObject.transform.position = snapPos;

                if (Input.GetMouseButtonDown(0))
                {
                    TryPlace(snapPos);
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    TryRemove(hit.point);
                }
            }
        }

        public void TryPlace(Vector3 worldPos)
        {
            var cell = _grid.GetCellAtWorldPos(worldPos);
            if (cell != null && !cell.IsOccupied)
            {
                var newRackGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                newRackGo.transform.position = _grid.SnapToGrid(worldPos);
                var newRack = new GregPlaceableRack { RackId = Guid.NewGuid().ToString(), UnityGameObject = newRackGo };
                _grid.PlaceRack(cell.Coord, newRack);
            }
        }

        public void TryRemove(Vector3 worldPos)
        {
            var cell = _grid.GetCellAtWorldPos(worldPos);
            if (cell != null && cell.IsOccupied)
            {
                _grid.RemoveRack(cell.Coord);
                cell.Occupant?.Remove();
            }
        }

        private void CreateLineMaterial()
        {
            if (!_gridMaterial)
            {
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                _gridMaterial = new Material(shader) { hideFlags = HideFlags.HideAndDontSave };
                _gridMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                _gridMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                _gridMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                _gridMaterial.SetInt("_ZWrite", 0);
            }
        }

        public void OnGUI()
        {
            // IMGUI disabled
        }
    }
}
