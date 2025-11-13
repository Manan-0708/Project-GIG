using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons;
    private int currentWeapon = 0;

    private void Start()
    {
        SelectWeapon(currentWeapon);
    }

    // SHOOT
    public void FireCurrentWeapon()
    {
        weapons[currentWeapon].GetComponent<Weapon>().Attack();
    }

    // MELEE
    public void UseMelee()
    {
        if (weapons[currentWeapon].TryGetComponent<MeleeWeapon>(out MeleeWeapon melee))
        {
            melee.Attack();
        }
    }

    // SCROLL WHEEL
    public void ScrollSwitch(float scrollValue)
    {
        if (scrollValue > 0) NextWeapon();
        else if (scrollValue < 0) PreviousWeapon();
    }

    // GAMEPAD NEXT
    public void NextWeapon()
    {
        currentWeapon++;
        if (currentWeapon >= weapons.Length)
            currentWeapon = 0;

        SelectWeapon(currentWeapon);
    }

    // GAMEPAD PREVIOUS
    public void PreviousWeapon()
    {
        currentWeapon--;
        if (currentWeapon < 0)
            currentWeapon = weapons.Length - 1;

        SelectWeapon(currentWeapon);
    }

    // Enable only selected weapon
    private void SelectWeapon(int index)
    {
        for (int i = 0; i < weapons.Length; i++)
            weapons[i].SetActive(i == index);
    }
}
