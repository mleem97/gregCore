using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Il2CppInterop.Runtime.Injection;
using gregCore.UI;

namespace greg.Furniture
{
    /// <summary>
    /// Advanced Furniture Placement System with Grid Snapping (0.5m) and Surface Alignment.
    /// Injected into IL2CPP domain for high-performance world interaction.
    /// </summary>
    public class FurniturePlacer : MonoBehaviour
    {
        public FurniturePlacer(IntPtr ptr) : base(ptr) { }

        private GameObject? _ghost;
        private float _gridSize = 0.5f; // 1sq = 4sq
        private bool _isActive;

        public void StartPlacement(GameObject prefab)
        {
            _isActive = true;
            _ghost = UnityEngine.Object.Instantiate(prefab);
            
            // Strip colliders from ghost
            foreach (var col in _ghost.GetComponentsInChildren<Collider>()) col.enabled = false;
            
            // Set layer to ignore raycast
            _ghost.layer = 2; // Ignore Raycast
        }

        private void Update()
        {
            if (!_isActive || _ghost == null) return;

            var mouse = Mouse.current;
            var keyboard = Keyboard.current;

            Ray ray = Camera.main.ScreenPointToRay(mouse != null ? mouse.position.ReadValue() : Vector2.zero);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Snapping Logic
                float x = Mathf.Round(hit.point.x / _gridSize) * _gridSize;
                float z = Mathf.Round(hit.point.z / _gridSize) * _gridSize;
                Vector3 pos = new Vector3(x, hit.point.y, z);

                _ghost.transform.position = Vector3.Lerp(_ghost.transform.position, pos, Time.deltaTime * 20f);

                // Normal Alignment (Wall vs Floor)
                if (Mathf.Abs(hit.normal.y) < 0.1f) // Wall
                {
                    _ghost.transform.rotation = Quaternion.LookRotation(hit.normal);
                }
                else // Floor
                {
                    _ghost.transform.rotation = Quaternion.identity;
                }

                if (mouse != null && mouse.leftButton.wasPressedThisFrame) FinalizePlacement(pos, _ghost.transform.rotation);
            }

            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame) StopPlacement();
        }

        private void FinalizePlacement(Vector3 pos, Quaternion rot)
        {
            // Persistence logic here
            MelonLoader.MelonLogger.Msg($"[gC-Build] Placed object at {pos}");
            StopPlacement();
        }

        public void StopPlacement()
        {
            _isActive = false;
            if (_ghost != null) UnityEngine.Object.Destroy(_ghost);
            _ghost = null;
        }
    }
}
