using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Camera Recoil")]
    public float camRecoilSpeed = 10f;    // how fast camera reacts
    public float camReturnSpeed = 8f;     // how fast it comes back
    private Vector3 camCurrentRot;        // current rotation applied
    private Vector3 camTargetRot;         // target rotation

    [Header("Weapon Kickback")]
    public Transform weaponHolder;        // the weapon object to move back
    public float kickbackSpeed = 12f;     // how fast weapon comes back
    private Vector3 weaponOriginalPos;    // starting position
    private Vector3 weaponTargetPos;      // kickback target position

    void Start()
    {
        weaponOriginalPos = weaponHolder.localPosition;
    }

    void Update()
    {
        // Smooth CAMERA recoil recovery
        camTargetRot = Vector3.Lerp(camTargetRot, Vector3.zero, camReturnSpeed * Time.deltaTime);
        camCurrentRot = Vector3.Lerp(camCurrentRot, camTargetRot, camRecoilSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(camCurrentRot);

        // Smooth WEAPON kickback recovery
        weaponTargetPos = Vector3.Lerp(weaponTargetPos, weaponOriginalPos, kickbackSpeed * Time.deltaTime);
        weaponHolder.localPosition = weaponTargetPos;
    }

    public void ApplyRecoil(float up, float sideways, float kickback)
    {
        camTargetRot += new Vector3(-up, sideways, 0);     // Camera goes up
        weaponTargetPos = weaponOriginalPos - new Vector3(0, 0, kickback); // Gun moves back
    }
}
