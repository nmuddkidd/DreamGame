using UnityEngine;

public class GasNPC : MonoBehaviour
{
    //private List<GameObject> NPCs;
    [Header("Prefabs")]
    [SerializeField] private GameObject NPC1;
    private bool waiting = true;
    private GameObject current;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // NPCs = [NPC1, NPC2, NPC3]

    }

    void Wait()
    {
        waiting = true;
    }

    void Creation()
    {
        //spawn in a random NPC
        current = NPC1;
        Instantiate(current, this.transform.position, this.transform.rotation);
        //different spawn times and whatnot
        Invoke("Wait", Random.Range(5, 30));
    }

    // Update is called once per frame
    void Update()
    {
        if (waiting)
        {
            waiting = false;
            Creation();
        }
    }

}
