using Buildings;
using Mirror;
using UnityEngine;

namespace Combat
{
    public class Targeter : NetworkBehaviour
    {
        private Targetable target;

        public Targetable Target => target;


        public override void OnStartServer()
        {
            GameOverHandler.ServerOnGameOver += ServerHandlePlayerDie;
        }

        public override void OnStopServer()
        {
            GameOverHandler.ServerOnGameOver -= ServerHandlePlayerDie;
        }

        [Server]
        private void ServerHandlePlayerDie()
        {
            ClearTarget();
        }

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
