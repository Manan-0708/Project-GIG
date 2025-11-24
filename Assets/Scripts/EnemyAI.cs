using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] Transform[] patrolPoints;
    [SerializeField] float patrolSpeed = 3.5f;
    [SerializeField] float chaseSpeed = 4.5f;

    [Header("Detection & Combat")]
    [SerializeField] float chaseRange = 10f;
    [SerializeField] float shootRange = 7f;
    [SerializeField] float fireRate = 1f;
    [SerializeField] Transform muzzle;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int projectileDamage = 10;

    [Header("Sight")]
    [SerializeField] LayerMask obstacleMask;     // set this in inspector to environment layers (NOT Player)
    [SerializeField] float eyeHeight = 1.2f;     // origin height for raycast

    NavMeshAgent agent;
    int currentPatrolIndex = 0;
    Transform player;
    float lastFireTime;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        // find player by tag; you can also assign reference in inspector
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p) player = p.transform;

        if (patrolPoints != null && patrolPoints.Length > 0)
        {
            agent.speed = patrolSpeed;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void Update()
    {
        if (player == null)
        {
            PatrolUpdate();
            return;
        }

        float distToPlayer = Vector3.Distance(transform.position, player.position);

        // compute line-of-sight to player
        Vector3 origin = transform.position + Vector3.up * eyeHeight;
        Vector3 targetPos = player.position + Vector3.up * 0.5f;
        bool hasLOS = !Physics.Linecast(origin, targetPos, obstacleMask);

        if (distToPlayer <= chaseRange && hasLOS)
        {
            // chase
            agent.isStopped = false;
            agent.speed = chaseSpeed;
            agent.SetDestination(player.position);

            if (distToPlayer <= shootRange)
            {
                // stop moving to aim (optional)
                agent.isStopped = true;
                Vector3 lookPos = player.position;
                lookPos.y = transform.position.y;
                transform.LookAt(lookPos);

                // only shoot if we have direct line of sight
                if (hasLOS) TryShoot();
            }
        }
        else
        {
            // resume patrol
            if (patrolPoints != null && patrolPoints.Length > 0)
            {
                agent.isStopped = false;
                agent.speed = patrolSpeed;
                PatrolUpdate();
            }
        }
    }

    void PatrolUpdate()
    {
        if (patrolPoints == null || patrolPoints.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            agent.SetDestination(patrolPoints[currentPatrolIndex].position);
        }
    }

    void TryShoot()
    {
        if (Time.time - lastFireTime < 1f / fireRate) return;
        lastFireTime = Time.time;

        if (projectilePrefab == null || muzzle == null) return;

        // instantiate projectile and initialize
        GameObject proj = Instantiate(projectilePrefab, muzzle.position, Quaternion.identity);
        Vector3 dir = (player.position + Vector3.up * 0.5f - muzzle.position).normalized;
        proj.transform.forward = dir;

        if (proj.TryGetComponent<Projectile>(out var projectile))
        {
            projectile.Initialize(dir, projectileDamage, gameObject); // pass owner so it doesn't hit self
        }
        else
        {
            // fallback: try to set velocity if no Projectile script
            var rb = proj.GetComponent<Rigidbody>();
            if (rb) rb.velocity = dir * 20f;
        }
    }

    // Optional: draw ranges in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, shootRange);
    }
}