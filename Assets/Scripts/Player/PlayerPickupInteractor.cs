using UnityEngine;

public class PlayerPickupInteractor : MonoBehaviour
{
    [Tooltip("Prompt object (optional) to enable when near a pickup")]
    public GameObject uiPrompt;

    // current nearby pickup interactable
    WeaponPickupInteractable nearbyPickup;

    // reference to InputManager on the player
    InputManager inputManager;

    void Awake()
    {
        inputManager = GetComponent<InputManager>() ?? GetComponentInChildren<InputManager>() ?? GetComponentInParent<InputManager>();
        if (inputManager != null)
            inputManager.OnInteractPerformed += HandleInteract;
    }

    void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnInteractPerformed -= HandleInteract;
    }

    void HandleInteract()
    {
        if (nearbyPickup != null)
        {
            nearbyPickup.OnPickedUp(gameObject);
            nearbyPickup = null;
            if (uiPrompt) uiPrompt.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<WeaponPickupInteractable>();
        if (pickup != null)
        {
            nearbyPickup = pickup;
            if (uiPrompt) uiPrompt.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var pickup = other.GetComponent<WeaponPickupInteractable>();
        if (pickup != null && pickup == nearbyPickup)
        {
            nearbyPickup = null;
            if (uiPrompt) uiPrompt.SetActive(false);
        }
    }
}