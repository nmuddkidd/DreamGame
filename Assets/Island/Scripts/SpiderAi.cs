using UnityEngine;
using UnityEngine.AI;

public class SpiderAi : MonoBehaviour
{
    public float sihtRange = 15f;
    public float roamRadius = 10f;
    public float damage = 10f;
    public float attackRate = 1f;
    public float roamWaitTime = 3f;

    private float nextAttackTime;
    private float pathUpdateTimer = 0.5f;
    private float pathUpdate;
    private float waitTimer;
    private Vector3 startPosition;
    private NavMeshAgent agent;
    private Transform player;
    private Animator anim;

    void Start()
    {

        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        startPosition = transform.position;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");

        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        Roam();
    }

    
    void Update()
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", agent.velocity.magnitude); //animation speed based on navmesh velocity/make legs move
        }

        //spiders roam if player not in scene
        if (player == null) 
        {
            RoamingLogic();
            return;
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position); 


        //spider chase/roam based on player distance 
        if (distanceToPlayer <= sihtRange)
        {
            waitTimer = 0; //prevents spiders pausing while chasing player
            if (Time.time >= pathUpdate)
            {
                agent.SetDestination(player.position); //chase player while in range
                pathUpdate = Time.time + pathUpdateTimer;
            }

            if (distanceToPlayer <= agent.stoppingDistance + 0.2f && Time.time >= nextAttackTime)
            {
                anim.SetTrigger("Attack");
            }
        }
        else
        {
            RoamingLogic(); //roam when player out of range
        }
    }

    void RoamingLogic()
    {
        //if destination reached
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            waitTimer += Time.deltaTime; //start timer

            if (Time.frameCount % 60 == 0)
            {
                Debug.Log(gameObject.name + "wait timer: " + waitTimer);
            }

            if (waitTimer >= roamWaitTime) // roam again after timer
            {
                Debug.Log(gameObject.name + "new roam position");
                Roam();
                waitTimer = 0; //reset timer

            }
        }
    }

    void Roam()
    {
        //generate random point in radius/set as destination
        Vector3 randomDirection = Random.insideUnitSphere * roamRadius;
        randomDirection += startPosition;
        NavMeshHit hit;

        //check if point is on navmesh/set destination if it is
        if (NavMesh.SamplePosition(randomDirection, out hit, 2f, NavMesh.AllAreas))
        {
            Debug.Log(gameObject.name + "Navmesh point found " + hit.position);
            agent.SetDestination(hit.position);
        } else
        {
            Debug.Log(gameObject.name + "No navmesh point found");
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