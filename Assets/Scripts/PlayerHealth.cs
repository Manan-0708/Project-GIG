using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Player took {amount} damage. HP: {currentHealth}/{maxHealth}");
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Player died.");
        onDeath?.Invoke();
        // handle respawn / disable player etc.
    }
}