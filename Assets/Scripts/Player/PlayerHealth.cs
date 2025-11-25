using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Tooltip("Player max HP")]
    public float maxHealth = 100f;

    [Tooltip("Scene name to load when player dies (must be in Build Settings). If empty, loads build index 0.")]
    public string mainMenuSceneName = "main_menu";

    [Tooltip("Delay (seconds) before returning to main menu")]
    public float deathDelay = 2f;

    float currentHealth;
    bool isDead = false;

    // new: assign this in the Inspector to your UI Slider
    [SerializeField] Slider healthSlider;

    void Awake()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    // Called by projectiles / damage sources
    public void TakeDamage(float amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        Debug.Log($"{gameObject.name} took {amount} damage. HP: {currentHealth}/{maxHealth}");
        UpdateHealthUI();

        if (currentHealth <= 0f) Die();
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log($"{gameObject.name} died. Returning to main menu in {deathDelay} seconds.");

        // disable common player scripts to stop input/aim/fire (best-effort)
        var motor = GetComponent<PlayerMotor>();
        if (motor) motor.enabled = false;

        var look = GetComponent<PlayerLook>();
        if (look) look.enabled = false;

        var gun = GetComponentInChildren<Gun>();
        if (gun) gun.enabled = false;

        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(deathDelay);

        if (!string.IsNullOrEmpty(mainMenuSceneName))
            SceneManager.LoadScene(mainMenuSceneName);
        else
            SceneManager.LoadScene(0); // fallback to first build scene
    }

    // helper to keep UI in sync; safe if healthSlider is null
    void UpdateHealthUI()
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = Mathf.Max(1, maxHealth);
        healthSlider.value = currentHealth;
    }
}