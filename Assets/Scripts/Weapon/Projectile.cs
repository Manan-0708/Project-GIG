using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 25f;
    [SerializeField] float lifeTime = 6f;
    [SerializeField] float hitRadius = 0.1f; // used by SphereCast fallback
    Rigidbody rb;
    int damage = 10;
    GameObject owner;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Initialize(Vector3 direction, int damageAmount, GameObject ownerObj = null)
    {
        damage = damageAmount;
        owner = ownerObj;

        // ensure we have a Rigidbody and it's set up for physics motion
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>() ?? gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = false;
        rb.useGravity = false;
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        transform.forward = direction;
        if (rb) rb.velocity = direction * speed;

        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner) return; // ignore owner

        // 1) Enemy hit: support child trigger hitboxes by searching parents
        var enemyHealth = other.GetComponentInParent<EnemyHealth>();
        if (enemyHealth != null)
        {
            int finalDamage = damage;
            // optional: tag specific hitboxes like "Chest" for multipliers
            if (other.CompareTag("Chest")) finalDamage = Mathf.RoundToInt(damage * 1.5f);
            enemyHealth.TakeDamage(finalDamage);
            Destroy(gameObject);
            return;
        }

        // 2) Player hit
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 3) Destroy on other environment hits (still ignore player tag if needed)
        if (!other.CompareTag("Player")) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}