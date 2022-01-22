using System;
using Combat;
using Mirror;
using UnityEngine;
using UnityEngine.Events;


namespace Units
{
    [RequireComponent(typeof(UnitMovement))]
    public class Unit : NetworkBehaviour
    {
        [SerializeField] private UnitMovement unitMovement;
        [SerializeField] private Targeter targeter;
        [SerializeField] private UnityEvent onSelected;
        [SerializeField] private UnityEvent onDeselected;

        public static event Action<Unit> ServerOnUnitSpawned;
        public static event Action<Unit> ServerOnUnitDespawned;
        public static event Action<Unit> AuthorityOnUnitSpawned;
        public static event Action<Unit> AuthorityOnUnitDespawned; 
        public UnitMovement UnitMovement => unitMovement;
        public Targeter Targeter => targeter;
        
        
#region Server

        public override void OnStartServer() => ServerOnUnitSpawned?.Invoke(this);

        public override void OnStopServer() => ServerOnUnitDespawned?.Invoke(this);

#endregion

#region Client

        [Client]
        public void Select()
        {
            if(!hasAuthority) return;
        
            onSelected?.Invoke();
        }

        [Client]
        public void Deselect()
        {
            if(!hasAuthority) return;
        
            onDeselected?.Invoke();
        }

        public override void OnStartAuthority() => AuthorityOnUnitSpawned?.Invoke(this);

        public override void OnStopAuthority() => AuthorityOnUnitDespawned?.Invoke(this);
    
#endregion
   
    }
}
