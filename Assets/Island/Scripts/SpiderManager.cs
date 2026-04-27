using UnityEngine;

public class SpiderManager : MonoBehaviour
{
    public GameObject spiderPrefab;
    public GameObject dropedItemPrefab;
    public int maxSpiders = 5;
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
        GameObject newSpider = Instantiate(spiderPrefab, transform.position, Quaternion.identity); //spawn spider at manager position

        UnityEngine.AI.NavMeshAgent agent = newSpider.GetComponent<UnityEngine.AI.NavMeshAgent>(); //get nav mesh agent component from spider

        if (agent != null) {
            agent.Warp(transform.position); //Warp spider to navmesh at manager location
        }
        activeSpiders++;
    }
}
