using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject[] weapons; // assign weapon GameObjects (child models/roots) in inspector
    private bool[] unlocked;
    private int currentWeapon = 0;

    void Awake()
    {
        unlocked = new bool[weapons.Length];
        // ensure all weapons are initially disabled in gameplay
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null) weapons[i].SetActive(false);
            unlocked[i] = false;
        }
    }

    private void Start()
    {
        // pick first unlocked if any
        int first = -1;
        for (int i = 0; i < unlocked.Length; i++) if (unlocked[i]) { first = i; break; }
        if (first >= 0) SelectWeapon(first);
    }

    // called by pickup to unlock a weapon slot
    public void UnlockWeapon(int index, bool equip = true)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (weapons[index] == null) return;
        unlocked[index] = true;
        weapons[index].SetActive(true);

        if (equip) SelectWeapon(index);
    }

    public bool IsUnlocked(int index)
    {
        if (index < 0 || index >= unlocked.Length) return false;
        return unlocked[index];
    }

    // SHOOT
    public void FireCurrentWeapon()
    {
        if (weapons.Length == 0) return;
        var w = weapons[currentWeapon];
        if (w == null) return;
        var weaponComp = w.GetComponent<Weapon>();
        if (weaponComp != null) weaponComp.Attack();
    }

    // MELEE
    public void UseMelee()
    {
        if (weapons.Length == 0) return;
        var w = weapons[currentWeapon];
        if (w == null) return;
        if (w.TryGetComponent<MeleeWeapon>(out MeleeWeapon melee))
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

    public void NextWeapon()
    {
        if (weapons.Length == 0) return;
        int start = currentWeapon;
        do
        {
            currentWeapon = (currentWeapon + 1) % weapons.Length;
            if (unlocked[currentWeapon]) { SelectWeapon(currentWeapon); return; }
        } while (currentWeapon != start);
    }

    public void PreviousWeapon()
    {
        if (weapons.Length == 0) return;
        int start = currentWeapon;
        do
        {
            currentWeapon = (currentWeapon - 1 + weapons.Length) % weapons.Length;
            if (unlocked[currentWeapon]) { SelectWeapon(currentWeapon); return; }
        } while (currentWeapon != start);
    }

    // Enable only selected (and unlocked) weapon
    private void SelectWeapon(int index)
    {
        if (index < 0 || index >= weapons.Length) return;
        if (!unlocked[index]) return;
        currentWeapon = index;
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i] != null)
                weapons[i].SetActive(i == index && unlocked[i]);
        }
    }
}
