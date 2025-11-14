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

    private void Start()
    {
        playerCam = Camera.main;

        recoil = Camera.main.GetComponentInParent<Recoil>();

    }

    public override void Attack()
    {
        if (Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }
    }
        void Shoot()
        {
        Vector3 origin = playerCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0f));
        Vector3 direction = playerCam.transform.forward;

            
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, range))
        {
            Debug.Log("Ray hit: " + hit.transform.name);

            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        recoil.ApplyRecoil(
    recoilUp,
    Random.Range(-recoilSideways, recoilSideways),
    recoilKickback
);

    }


}
