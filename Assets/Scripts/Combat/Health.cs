using System;
using Mirror;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;

    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;

    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthUpdated;


    #region Server

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void DealDamage(int damageAmount)
    {
        if(currentHealth == 0) return;

        currentHealth = Mathf.Max(currentHealth - damageAmount, 0);

        if (currentHealth == 0)
        {
            ServerOnDie?.Invoke();
            Debug.Log($"We die!");
        }

    }

  #endregion

    #region Client

    private void HandleHealthUpdated(int oldHealth, int newHealth) 
        => ClientOnHealthUpdated?.Invoke(newHealth, maxHealth);

  #endregion
}
