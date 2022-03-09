using System;
using System.Collections.Generic;
using System.Linq;
using Buildings;
using Combat;
using Pathfinding;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Units
{
    [RequireComponent(typeof(UnitSelectionHandler))]
    public class UnitCommandManager : MonoBehaviour
    {
        [SerializeField] private UnitSelectionHandler unitSelectionHandler;
        [SerializeField] private LayerMask layerMask = new LayerMask();
        private Camera mainCamera;


        private void Start()
        {
            mainCamera = Camera.main;
            GameOverHandler.ClientOnGameOver += ClientHandleGameOver;
        }

        private void OnDestroy()
        {
            GameOverHandler.ClientOnGameOver -= ClientHandleGameOver;
        }

        private void Update()
        {
            if(!Mouse.current.rightButton.wasPressedThisFrame || unitSelectionHandler.SelectedUnits.Count < 1) return;

            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;

            var pointsToMove = GetPointsToMove(hit.point);

            if (hit.collider.TryGetComponent<Targetable>(out var target))
            {
                if (target.hasAuthority)
                {
                    TryMove(pointsToMove);
                    return;
                }

                TryTarget(target);
                return;
            }

            TryMove(pointsToMove);
        }
        
        private void TryTarget(Targetable target)
        {
            foreach (var unit in unitSelectionHandler.SelectedUnits)
            {
                unit.Targeter.CmdSetTarget(target.transform.gameObject);
            }
        }

        private void TryMove(List<Vector3> pointsToMove)
        {
            for (int i = 0; i < unitSelectionHandler.SelectedUnits.Count; i++)
            {
                unitSelectionHandler.SelectedUnits[i].UnitMovement.CmdMove(pointsToMove[i]);
            }
        }

        private List<Vector3> GetPointsToMove(Vector3 hitPosition)
        {
            var radius = unitSelectionHandler.SelectedUnits[0].UnitMovement.AiAgentRadius * 2;
            var clearingRadius = radius;
            
            List<Vector3> previousPoints = unitSelectionHandler.SelectedUnits.Select(unit => unit.transform.position).ToList();

            PathUtilities.GetPointsAroundPoint(hitPosition, AstarPath.active.graphs[0] as IRaycastableGraph, previousPoints, radius, clearingRadius);

            return previousPoints;
        }
        
        private void ClientHandleGameOver(string winnerName)
        {
            enabled = false;
        }
    }
}
