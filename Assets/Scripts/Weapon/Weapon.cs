using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public float attackRate;

    [Header("Recoil Settings")]
    public float recoilUp = 2f;
    public float recoilSideways = 1f;
    public float recoilKickback = 0.1f;


    public abstract void Attack();

}
