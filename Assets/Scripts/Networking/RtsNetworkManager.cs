using Mirror;
using UnityEngine;


namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var identityTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab,
                                                identityTransform.position,
                                                identityTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }
    }
}
