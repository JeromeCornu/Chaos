using GameState;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class Health : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    public int currentHealth;

    public int maxHealth = 100;
    public Slider healthSlider;

    public PlayerCombatController combatController;

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
    }

    [Server]
    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        // Debug.Log("Took damage, current health : " + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            PlayerDead();
            RpcHandleDeath(combatController.netIdentity.netId);
        }
    }

    [Server]
    public void HealMaxHealth()
    {
        currentHealth = maxHealth;
    }

    void OnHealthChanged(int oldHealth, int newHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = newHealth;
            healthSlider.gameObject.SetActive(newHealth < maxHealth);
        }
    }

    void PlayerDead()
    {
        // Get connection of the target player (assuming you have reference to them)
        NetworkConnectionToClient connToClient = combatController.connectionToClient as NetworkConnectionToClient;
        CardSelectionHandler.Instance.TargetSetCardChooser(connToClient, combatController.netIdentity.netId);
        GameManager.Instance.GoToNextState();
    }
    
    [ClientRpc]
    void RpcHandleDeath(uint idNet)
    {
        if (combatController != null && idNet == combatController.netIdentity.netId)
        {
            combatController.HandleDeath();
        }
    }
}
