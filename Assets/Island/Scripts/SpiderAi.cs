using UnityEngine;
using UnityEngine.AI;

public class SpiderAi : MonoBehaviour
{
    public float sihtRange = 15f;
    public float roamRadius = 10f;
    public float damage = 10f;
    public float attackRate = 1f;

    private float nextAttackTime;
    private float pathUpdateTimer = 0.5f;
    private float pathUpdate;
    private Vector3 startPosition;
    private NavMeshAgent agent;
    private Transform player;

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        startPosition = transform.position;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        Roam();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= sihtRange)
        {
            if (Time.time >= pathUpdate)
            {
                agent.SetDestination(player.position); //chase player while in range
                pathUpdate = Time.time + pathUpdateTimer;
            }
        }
        else if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            Roam(); //roam when player out of range
        }
    }

    void Roam()
    {
        //generate random point in radius/set as destination
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startPosition;
        NavMeshHit hit;

        //check if point is on navmesh/set destination if it is
        if (NavMesh.SamplePosition(randomDirection, out hit, roamRadius, 1))
        {
            agent.SetDestination(hit.position);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && Time.time >= nextAttackTime)
        {
            Health playerHealth = collision.gameObject.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                nextAttackTime = Time.time + 1f / attackRate; //next attack time based on attack rate
            }
        }
    }
}