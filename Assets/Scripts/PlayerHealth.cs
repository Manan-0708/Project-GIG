using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHealth = 100;
    int currentHealth;

    // new: assign this in the Inspector to your UI Slider
    [SerializeField] Slider healthSlider;

    public UnityEvent onDeath;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0, currentHealth);
        Debug.Log($"Player took {amount} damage. HP: {currentHealth}/{maxHealth}");
        UpdateHealthUI();
        if (currentHealth <= 0) Die();
    }

    void Die()
    {
        Debug.Log("Player died.");
        onDeath?.Invoke();
        UpdateHealthUI();
        // handle respawn / disable player etc.
    }

    // helper to keep UI in sync; safe if healthSlider is null
    void UpdateHealthUI()
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = Mathf.Max(1, maxHealth);
        healthSlider.value = currentHealth;
    }
}