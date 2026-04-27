using UnityEngine;
using UnityEngine.AI;

public class SpiderAi : MonoBehaviour
{
    public float sightRange = 15f;
    public float roamRadius = 20f;
    public float damage = 10f;
    public float attackRate = 1f;
    public float roamWaitTime = 0.5f;

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

        //ignore spider/player height for distance 
        Vector3 flatSpiderPos = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 flatPlayerPos = new Vector3(player.position.x, 0, player.position.z);
        float flatDistance = Vector3.Distance(flatSpiderPos, flatPlayerPos);

        //spider chase/roam based on player distance 
        if (flatDistance <= sightRange)
        {
            waitTimer = 0; //prevents spiders pausing while chasing player
            if (Time.time >= pathUpdate)
            {
                agent.SetDestination(player.position); //chase player while in range
                pathUpdate = Time.time + pathUpdateTimer;
            }

            if (flatDistance <= agent.stoppingDistance + 5.0f && Time.time >= nextAttackTime)
            {
                Health playerHealth = player.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    anim.SetTrigger("Attack"); // Play the animation
                    nextAttackTime = Time.time + 1f / attackRate;
                    Debug.Log("Spider bit the player!");
                }
            }
        }
        else
        {
            RoamingLogic(); //roam when player out of range
        }
        //keep spider on ground when chasing player
        transform.position = new Vector3(transform.position.x, agent.nextPosition.y, transform.position.z);
        //stop spider from flying into air when metting player
        if (agent.isOnNavMesh)
        {
            Vector3 currentPos = transform.position;
            currentPos.y = agent.nextPosition.y;
            transform.position = currentPos;
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
                //Debug.Log(gameObject.name + "wait timer: " + waitTimer);
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
        
            agent.SetDestination(randomDirection);
       
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