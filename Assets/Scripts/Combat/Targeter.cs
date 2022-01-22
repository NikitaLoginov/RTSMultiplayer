using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable Target => target;


        [Command]
        public void CmdSetTarget(GameObject targetGameObject)
        {
            if (!targetGameObject.TryGetComponent<Targetable>(out var target))
                return;

            this.target = target;
        }

        [Server]
        public void ClearTarget()
        {
            target = null;
        }
    }
}
