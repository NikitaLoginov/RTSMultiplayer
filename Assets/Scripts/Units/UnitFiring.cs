using Combat;
using Mirror;
using UnityEngine;

namespace Units
{
    public class UnitFiring : NetworkBehaviour
    {
        [SerializeField] private Targeter targeter;
        [SerializeField] private GameObject projectilePrefab;
        [SerializeField] private Transform projectileSpawnPoint;
        [SerializeField] private float fireRange = 5f;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private float rotationSpeed = 20f;

        private float lastFireTime;


        [ServerCallback]
        private void Update()
        {
            var target = targeter.Target;
            
            if (target == null) return;
            if(!CanFireAtTarget(target)) return;

            Quaternion targetRotation = 
                Quaternion.LookRotation(target.transform.position - transform.position);

            transform.rotation = Quaternion.RotateTowards(
                transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Time.time > (1 / fireRate) + lastFireTime)
            {
                //fire
                Quaternion projectileRotation = Quaternion.LookRotation(
                    target.GetAimAtPoint().position - projectileSpawnPoint.position);

                GameObject projectileInstance = Instantiate(
                    projectilePrefab, projectileSpawnPoint.position, projectileRotation);
                
                NetworkServer.Spawn(projectileInstance, connectionToClient);
                
                lastFireTime = Time.time;
            }
        }

        [Server]
        private bool CanFireAtTarget(Targetable target)
        {
            return (target.transform.position - transform.position).sqrMagnitude 
                   <= fireRange * fireRange;
        }
    }
}
