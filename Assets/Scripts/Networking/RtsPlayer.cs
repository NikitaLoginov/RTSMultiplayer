using System.Collections.Generic;
using Mirror;
using Units;
using UnityEngine;


namespace Networking
{
    public class RtsPlayer : NetworkBehaviour
    {
        private List<Unit> playerUnits = new List<Unit>();

        public List<Unit> GetPlayerUnits() => playerUnits;


#region Server
        public override void OnStartServer()
        {
            Unit.ServerOnUnitSpawned += ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned += ServerHandleUnitDespawned;
        }

        public override void OnStopServer()
        {
            Unit.ServerOnUnitSpawned -= ServerHandleUnitSpawned;
            Unit.ServerOnUnitDespawned -= ServerHandleUnitDespawned;
        }

        private void ServerHandleUnitSpawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            playerUnits.Add(unit);
        }
        
        private void ServerHandleUnitDespawned(Unit unit)
        {
            if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
            
            playerUnits.Remove(unit);
        }
#endregion

#region Client

        public override void OnStartAuthority()
        {
            if (NetworkServer.active) return; //if this machine is running as a server
            
            Unit.AuthorityOnUnitSpawned += AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned += AuthorityHandleUnitDespawned;
        }

        public override void OnStopClient()
        {
            if (!isClientOnly || !hasAuthority) return;
            
            Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
            Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;
        }
        
        private void AuthorityHandleUnitSpawned(Unit unit) => playerUnits.Add(unit);

        private void AuthorityHandleUnitDespawned(Unit unit) => playerUnits.Remove(unit);

        #endregion
    }
}
