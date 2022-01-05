using System.Collections.Generic;
using Mirror;
using Networking;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Units
{
    public class UnitSelectionHandler : MonoBehaviour
    {
        [SerializeField] private RectTransform unitSelectionArea;
        [SerializeField] private LayerMask layerMask;

        private Camera mainCamera;
        private Vector2 startPosition;
        
        private RtsPlayer player;
        private List<Unit> selectedUnits = new List<Unit>();

        public List<Unit> SelectedUnits => selectedUnits;

        private void Start() => mainCamera = Camera.main;

        private void Update()
        {
            if (player == null) 
                player = NetworkClient.connection.identity.GetComponent<RtsPlayer>();
            
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                StartSelectionArea();
            }
            else if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                ClearSelectionArea();
            }
            else if (Mouse.current.leftButton.isPressed)
            {
                UpdateSelectionArea();
            }
        }

        private void StartSelectionArea()
        {
            if (!Keyboard.current.leftShiftKey.isPressed)
            {
                foreach (var selectedUnit in selectedUnits)
                {
                    selectedUnit.Deselect();
                    Debug.Log($"Deselected units");
                }
                selectedUnits.Clear();
            }

            unitSelectionArea.gameObject.SetActive(true);
            startPosition = Mouse.current.position.ReadValue();
            
            UpdateSelectionArea();
        }
        
        private void UpdateSelectionArea()
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();

            float areaWidth = mousePos.x - startPosition.x;
            float areaHeight = mousePos.y - startPosition.y;

            unitSelectionArea.sizeDelta = new Vector2(Mathf.Abs(areaWidth), 
                Mathf.Abs(areaHeight)); //size of selection field
            unitSelectionArea.anchoredPosition = startPosition + 
                                                 new Vector2(areaWidth / 2, areaHeight / 2); // position of selection field
        }

        private void ClearSelectionArea()
        {
            Debug.Log($"Clear selection area");
            unitSelectionArea.gameObject.SetActive(false);

            if (unitSelectionArea.sizeDelta.magnitude == 0)
            {
                Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

                if (!hit.collider.TryGetComponent(out Unit unit)) return;

                if (!unit.hasAuthority) return;
        
                selectedUnits.Add(unit);

                foreach (var selectedUnit in selectedUnits)
                {
                    selectedUnit.Select();
                    Debug.Log($"Selected single unit");
                }

                return;
            }

            Vector2 min = unitSelectionArea.anchoredPosition - (unitSelectionArea.sizeDelta / 2);
            Vector2 max = unitSelectionArea.anchoredPosition + (unitSelectionArea.sizeDelta / 2);

            foreach (var unit in player.GetPlayerUnits())
            {
                if (selectedUnits.Contains(unit)) continue;
                
                Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

                if (!InsideSelectionArea(screenPos, min, max)) continue;
                
                selectedUnits.Add(unit);
                unit.Select();
                
                Debug.Log($"Unit selected with mouse drag!");
            }
        }

        private bool InsideSelectionArea(Vector3 screenPos, Vector2 min, Vector2 max) => 
            screenPos.x > min.x && 
            screenPos.x < max.x && 
            screenPos.y > min.y && 
            screenPos.y < max.y;
    }
}
