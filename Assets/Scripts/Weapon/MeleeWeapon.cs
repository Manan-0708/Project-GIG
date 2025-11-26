using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : Weapon
{
    public float range = 2f;

    public AudioSource swingSound;
    public override void Attack()
    {
        // Play swing sound
        swingSound?.Play();

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, range))
        {
            Debug.Log("Slash Hit: " + hit.transform.name);
            // Here you can add logic to apply damage to the hit target
            EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
    }
}
