using System;
using Mirror;
using Pathfinding;
using Pathfinding.RVO;
using UnityEngine;


namespace Units
{
    public class UnitMovement : NetworkBehaviour
    {
        [SerializeField] private RichAI aiAgent;
        [SerializeField] private RVOController unitController;
        [SerializeField] private Targeter targeter;

        public float AiAgentRadius => aiAgent.radius;


#region Server

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();
            
            unitController.SetTarget(position, aiAgent.maxSpeed - 1f ,aiAgent.maxSpeed);
            
            var delta = unitController.CalculateMovementDelta(transform.position, Time.deltaTime);
            transform.position += delta;

            aiAgent.destination = position;
        }

#endregion
    }

}
