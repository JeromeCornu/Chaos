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
            RpcHandleDeath();
        }
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

    [ClientRpc]
    void RpcHandleDeath()
    {
        if (combatController != null)
        {
            combatController.HandleDeath();
        }
    }
}
