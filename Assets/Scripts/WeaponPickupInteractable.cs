using UnityEngine;

public class WeaponPickupInteractable : MonoBehaviour
{
    [Tooltip("Index in WeaponManager.weapons array to unlock")]
    public int weaponIndex = 0;

    [Tooltip("Human friendly name shown in UI prompt (if empty, GameObject name is used)")]
    public string displayName = "";

    [Tooltip("Auto-equip when picked up")]
    public bool autoEquip = true;

    [Tooltip("Destroy the pickup object when taken")]
    public bool destroyOnPickup = true;

    // bobbing motion settings
    [Tooltip("Vertical bob amplitude (units)")]
    public float bobAmplitude = 0.25f;

    [Tooltip("Bobbing speed (Hz)")]
    public float bobFrequency = 1.5f;

    // internal state for smooth bobbing
    Vector3 _startLocalPos;
    float _phase;

    void Awake()
    {
        _startLocalPos = transform.localPosition;
        _phase = Random.Range(0f, Mathf.PI * 2f); // stagger multiple pickups
    }

    void Update()
    {
        float yOffset = Mathf.Sin(Time.time * Mathf.PI * 2f * bobFrequency + _phase) * bobAmplitude;
        transform.localPosition = _startLocalPos + Vector3.up * yOffset;
    }

    // Called by player interactor when player presses the interact key
    public void OnPickedUp(GameObject player)
    {
        var wm = player.GetComponentInChildren<WeaponManager>() ?? player.GetComponent<WeaponManager>() ?? player.GetComponentInParent<WeaponManager>();
        if (wm == null)
        {
            Debug.LogWarning("WeaponPickup: no WeaponManager found on player.");
            return;
        }

        wm.UnlockWeapon(weaponIndex, autoEquip);

        // optional: play sound/FX here

        if (destroyOnPickup) Destroy(gameObject);
        else gameObject.SetActive(false);
    }
}