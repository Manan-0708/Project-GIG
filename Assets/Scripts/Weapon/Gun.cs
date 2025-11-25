using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Weapon
{
    private Recoil recoil;
    public float range;
    public float fireRate = .1f;
    public Camera playerCam;

    private float nextTimeToFire = 0f;

    // --- ammo fields ---
    [Tooltip("Bullets per magazine")]
    public int magazineSize = 30;
    [Tooltip("Current bullets in magazine")]
    public int ammoInMagazine = 30;
    [Tooltip("Reserve ammo (carried)")]
    public int reserveAmmo = 90;
    [Tooltip("Seconds to reload")]
    public float reloadTime = 2f;
    bool isReloading = false;
    // --------------------

    // added: reference to input manager so we can subscribe to Reload
    InputManager inputManager;

    // initialize data even if the object starts disabled
    void Awake()
    {
        playerCam = Camera.main;
        recoil = Camera.main.GetComponentInParent<Recoil>();

        // ensure ammo starts valid
        ammoInMagazine = Mathf.Clamp(ammoInMagazine, 0, magazineSize);
        if (ammoInMagazine == 0 && reserveAmmo > 0)
        {
            int take = Mathf.Min(magazineSize, reserveAmmo);
            ammoInMagazine = take;
            reserveAmmo -= take;
        }

        // start hidden UI is handled by AmmoUI.Awake; do NOT force UI here
    }

    // subscribe when the weapon becomes active (equipped)
    void OnEnable()
    {
        inputManager = FindObjectOfType<InputManager>();
        if (inputManager != null)
            inputManager.OnReloadPerformed += OnReloadInput;

        // update UI only when this gun is enabled (i.e. equipped)
        AmmoUI.Instance?.SetAmmo(ammoInMagazine, reserveAmmo);
        AmmoUI.Instance?.SetVisible(true);
    }

    void OnDisable()
    {
        if (inputManager != null)
            inputManager.OnReloadPerformed -= OnReloadInput;
    }

    void OnDestroy()
    {
        if (inputManager != null)
            inputManager.OnReloadPerformed -= OnReloadInput;
    }

    void OnReloadInput()
    {
        // hide hint while reloading attempt starts
        AmmoUI.Instance?.ShowReloadHint(false);
        // start reload via coroutine (safe to call even if already reloading)
        StartCoroutine(ReloadCoroutine());
    }

    public override void Attack()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    IEnumerator ReloadCoroutine()
    {
        if (isReloading) yield break;
        if (ammoInMagazine >= magazineSize) yield break;
        if (reserveAmmo <= 0) yield break;

        isReloading = true;
        // hide hint while reloading
        AmmoUI.Instance?.ShowReloadHint(false);

        // TODO: play reload animation/sfx
        yield return new WaitForSeconds(reloadTime);

        int needed = magazineSize - ammoInMagazine;
        int taken = Mathf.Min(needed, reserveAmmo);
        ammoInMagazine += taken;
        reserveAmmo -= taken;

        isReloading = false;
        AmmoUI.Instance?.SetAmmo(ammoInMagazine, reserveAmmo);
    }

    void Shoot()
    {
        if (isReloading) return;

        if (ammoInMagazine <= 0)
        {
            // no bullets -> start reload (or play dry fire)
            StartCoroutine(ReloadCoroutine());
            return;
        }

        // consume ammo
        ammoInMagazine--;
        AmmoUI.Instance?.SetAmmo(ammoInMagazine, reserveAmmo);

        // Use camera position as origin (or nearClipPlane) to avoid origin-inside-collider issues
        Vector3 origin = playerCam.transform.position;
        Vector3 direction = playerCam.transform.forward;

        // Visual debug
        Debug.DrawRay(origin, direction * range, Color.red, 1f);

        // RaycastAll so we can inspect hits in order and ignore player/camera colliders
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, range);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

        // root transform of the camera (likely the player) so we can ignore hits on it
        Transform cameraRoot = playerCam.transform.root;

        bool didHit = false;
        foreach (var h in hits)
        {
            // ignore hits on the player/camera root
            if (h.collider.transform.IsChildOf(cameraRoot)) continue;

            // if we hit an enemy, apply damage and stop processing
            EnemyHealth enemy = h.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
                didHit = true;
                break;
            }

            // hit something else (wall, props) -> bullet is blocked
            if (!h.collider.isTrigger)
            {
                didHit = true;
                break;
            }

            // otherwise (trigger) continue checking next hit
        }

        if (!didHit)
        {
            // optional: handle miss (impact VFX at origin + range)
        }

        recoil.ApplyRecoil(
            recoilUp,
            Random.Range(-recoilSideways, recoilSideways),
            recoilKickback
        );
    }
}
