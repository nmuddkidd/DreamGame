using UnityEngine;
using UnityEngine.AI;

public class SpiderManager : MonoBehaviour
{
    public GameObject spiderPrefab;
    public Transform[] spawnPoints;
    public GameObject dropedItemPrefab;
    public int maxSpiders = 5;
    public float spawnRadius = 5f;
    public int requiredKills = 10;

    private int currentKills = 0;
    private int activeSpiders = 0;

    void Start()
    {
        //spawn first wave of spiders
        for (int i = 0; i < maxSpiders; i++) SpawnSpider();
    }

    
    public void NotifyDeath(Vector3 deathPos)

    {
        currentKills++;
        activeSpiders--;

        if(currentKills >= requiredKills)
        {
            if (activeSpiders == 0)
            {
                Instantiate(dropedItemPrefab, deathPos, Quaternion.identity);
                Debug.Log("All spiders killed. Item dropped.");
            }
        } else
        {
            Invoke("SpawnSpider", 2f); //keep spawning spiders until req kills reached
        }
    }

    void SpawnSpider()
    {
        // pick random point from list
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform point = spawnPoints[randomIndex];

        // check point is clear 
        if (!Physics.CheckSphere(point.position, 1f))
        {
            GameObject newSpider = Instantiate(spiderPrefab, point.position, point.rotation);
            NavMeshAgent agent = newSpider.GetComponent<NavMeshAgent>();

            if (agent != null)
            {
                agent.avoidancePriority = Random.Range(30, 60); //prevent spideers from clumping together
                
            }
            activeSpiders++;
        }
        else
        {
            Invoke("SpawnSpider", 0.5f);
        }
    }
}
