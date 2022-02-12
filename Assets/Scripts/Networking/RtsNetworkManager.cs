using Buildings;
using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Networking
{
    public class RtsNetworkManager : NetworkManager
    {
        [SerializeField] private GameObject unitSpawnerPrefab;
        [SerializeField] private GameOverHandler gameOverHandlePrefab;
        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            base.OnServerAddPlayer(conn);

            var identityTransform = conn.identity.transform;
            var unitSpawnerInstance = Instantiate(unitSpawnerPrefab,
                                                identityTransform.position,
                                                identityTransform.rotation);
            
            NetworkServer.Spawn(unitSpawnerInstance, conn);
        }

        public override void OnServerSceneChanged(string sceneName) //calls right after scene is changed
        {
            if (SceneManager.GetActiveScene().name.StartsWith("Scene_map"))
            {
                GameOverHandler gameOverHandlerInstance = Instantiate(gameOverHandlePrefab);
                NetworkServer.Spawn(gameOverHandlerInstance.gameObject);
            }
        }
    }
}
