using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed = 25f;
    [SerializeField] float lifeTime = 6f;
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
        // damage player
        var health = other.GetComponent<PlayerHealth>();
        if (health != null)
        {
            health.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // optional: destroy on environment hit
        if (!other.CompareTag("Player")) Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }
}