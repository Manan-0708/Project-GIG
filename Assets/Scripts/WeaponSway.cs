using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [Header("Mouse Sway")]
    public float swayAmount = 0.05f;
    public float maxSwayAmount = 0.1f;
    public float swaySmooth = 8f;

    [Header("Idle Sway (Breathing)")]
    public float idleAmount = 0.02f;
    public float idleSpeed = 1f;

    private Vector3 initialPos;

    private void Start()
    {
        initialPos = transform.localPosition;
    }

     void LateUpdate()
    {
        ApplySway();
    }

    void ApplySway()
    {
        // Mouse movement
        float mouseX = Input.GetAxis("Mouse X") * swayAmount;
        float mouseY = Input.GetAxis("Mouse Y") * swayAmount;

        float swayX = Mathf.Clamp(mouseX, -maxSwayAmount, maxSwayAmount);
        float swayY = Mathf.Clamp(mouseY, -maxSwayAmount, maxSwayAmount);

        Vector3 mouseSway = new Vector3(swayX, swayY, 0f);

        // Idle motion
        float idleX = Mathf.Sin(Time.time * idleSpeed) * idleAmount;
        float idleY = Mathf.Cos(Time.time * idleSpeed) * idleAmount;

        Vector3 idleSway = new Vector3(idleX, idleY, 0f);

        // Final sway (mouse + breathing)
        Vector3 targetPos = initialPos + mouseSway + idleSway;

        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * swaySmooth);
    }
}
