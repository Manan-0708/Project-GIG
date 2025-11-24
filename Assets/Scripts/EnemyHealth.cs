using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;

    // show health to player settings
    public bool showHealthToPlayer = true;
    public float showDistance = 30f;
    public Vector3 labelOffset = new Vector3(0f, 2f, 0f);

    // new: choose which layers can block the view (set in inspector)
    public LayerMask obstructionMask = ~0;

    private GUIStyle _guiStyle;

    private void Start()
    {
        currentHealth = maxHealth;

        // initialize GUI style for the floating label
        _guiStyle = new GUIStyle();
        _guiStyle.alignment = TextAnchor.MiddleCenter;
        _guiStyle.normal.textColor = Color.white;
        _guiStyle.fontSize = 14;
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Max(0f, currentHealth);
        Debug.Log(gameObject.name + " took " + amount + " damage.");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log(gameObject.name + " died!");
        Destroy(gameObject);  // temporary: destroys enemy
    }

    // Draw a simple floating HP label above the enemy
    private void OnGUI()
    {
        if (!showHealthToPlayer || Camera.main == null) return;

        // only show when the camera is within range
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > showDistance) return;

        Vector3 worldTarget = transform.position + labelOffset;
        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldTarget);
        if (screenPos.z < 0f) return; // behind camera

        // Line-of-sight check: raycast from camera to target, ensure nothing blocks the view
        Vector3 dir = worldTarget - Camera.main.transform.position;
        float dist = dir.magnitude;
        if (dist > 0f)
        {
            if (Physics.Raycast(Camera.main.transform.position, dir.normalized, out RaycastHit hit, dist, obstructionMask, QueryTriggerInteraction.Ignore))
            {
                // If the first hit is not part of this enemy, the HP is occluded by something
                if (!hit.collider.transform.IsChildOf(transform))
                {
                    return; // occluded, don't draw
                }
            }
        }

        float percent = (maxHealth > 0f) ? (currentHealth / maxHealth * 100f) : 0f;
        string text = string.Format("HP: {0}/{1} ({2:0}%)", Mathf.CeilToInt(currentHealth), Mathf.CeilToInt(maxHealth), percent);

        Vector2 size = _guiStyle.CalcSize(new GUIContent(text));
        Rect rect = new Rect(screenPos.x - size.x * 0.5f, Screen.height - screenPos.y - size.y * 0.5f, size.x, size.y);
        GUI.Label(rect, text, _guiStyle);
    }
}
