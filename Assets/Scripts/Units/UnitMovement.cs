using Combat;
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
        [SerializeField] private float chaseRange = 10f;
        [SerializeField] private float stopRange = 0.1f;

        public float AiAgentRadius => aiAgent.radius;


#region Server

        [Command]
        public void CmdMove(Vector3 position)
        {
            targeter.ClearTarget();
            
            unitController.SetTarget(position, aiAgent.maxSpeed - 1f ,aiAgent.maxSpeed);
            
            var delta = unitController.CalculateMovementDelta(transform.position, Time.deltaTime);
            transform.position += delta;

            //aiAgent.endReachedDistance = stopRange;
            aiAgent.destination = position;
            aiAgent.canMove = true;
        }
        
        private void Update()
        {
            var target = targeter.Target;
            if (target != null)
            {
                if (CanChaseTarget(target)) 
                {
                    //chase
                    aiAgent.endReachedDistance = chaseRange; 
                    aiAgent.destination = target.transform.position;
                    aiAgent.canMove = true;
                }
                else if (aiAgent.hasPath)
                {
                    //stop chasing
                    aiAgent.canMove = false;
                    aiAgent.endReachedDistance = stopRange; 
                }
            }
        }

        private bool CanChaseTarget(Targetable target) =>
            //checking if we out of chase range
            (target.transform.position - transform.position).sqrMagnitude > chaseRange * chaseRange;

#endregion
    }

}
