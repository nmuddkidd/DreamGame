using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NPC_Movement : MonoBehaviour
{
    private GameObject target;
 //   [SerializeField] private LayerMask layer;
    private int Paths = 0;
    //navigating too the pills from within the store. False does not mean out of the store, just navigating out of it
    [SerializeField] private bool inside = false;
    //if within the store and attempting to navigate to pills. a false inside and walking boolean mean were using a simpler navigation
    [SerializeField] private bool walking = true;
    //navigating out of the store
    [SerializeField] private bool leaving = false;
    //forces NPC's to remain still
    [SerializeField] private bool moving = true;
    //Pills to find
    private GameObject Pills;
    //distance between NPC and the pills               
    private Vector2 pill_loc = new Vector2(0, 0);
    private Vector2 NPC_loc = new Vector2(0, 0);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        //set the starting direction
        int direction = Random.Range(0, 2);
        GameObject[] walk_to = GameObject.FindGameObjectsWithTag("Path");
        switch (Random.Range(0, 2)){
            case 0:
                target = walk_to[0];
                break;
            case 1:
                target = walk_to[1];
                break;
        }
        transform.LookAt(target.transform);
    }

    void Searcher()
    {

        //int hits = Physics.OverlapBoxNonAlloc(box_pos, box_dim, Located, Quaternion.Euler(Vector3.zero), layer);
        GameObject shelves = GameObject.Find("Pill Shelves");
        Pill_Shelves script = shelves.GetComponent<Pill_Shelves>();
        Pills = script.Shelves[Random.Range(0, (script.Shelves.Length))].gameObject;
       // Pills = bottles[Random.Range(0, len(bottles))];
      //  Pills = Located[Random.Range(0, hits)].gameObject;       
        pill_loc.x = Pills.transform.position.x;
        pill_loc.y = Pills.transform.position.z;
        //Debug.Log(Pills.gameObject.name);
        inside = true;
    }

    // Update is called once per frame
    void Update()
    {
        //move along the path
        //transform.LookAt(target.transform);
        if (walking)
        {
            if (moving)
            {
                transform.LookAt(target.transform);
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3 * Time.deltaTime);
            }
            if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z) && !(inside))
            {
                //find if the path has came to the doors 
                if (target.gameObject.name == "Paths End")
                {
                    target = GameObject.Find("Inside");
                }
                //if the target is inside of the station
                else if (target.gameObject.name == "Inside")
                {
                    target = target.transform.GetChild(Paths).gameObject;
                    Searcher();
                }
                else
                {
                    target = target.transform.GetChild(Paths).gameObject;
                    // transform.LookAt(target.transform);
                    //Paths = Paths + 1;
                }
            }
            //once inside the store, switch up navigation if statements
            else if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z))
            {
                if (Mathf.Abs(transform.position.z - Pills.transform.position.z) > 1.75)
                {
                    Paths = Paths + 1;
                    target = target.transform.parent.gameObject.transform.GetChild(Paths).gameObject;
                }
                else
                {
                    target = target.transform.GetChild(0).gameObject;
                }
            }
            if (target.gameObject.name == "Aisle End" | target.gameObject.name == "Aisle 1")
            {
                //update distance variables
                NPC_loc.x = this.transform.position.x;
                NPC_loc.y = this.transform.position.z;
                if (Vector2.Distance(NPC_loc, pill_loc) < 1.5)
                {
                    //collect the pills
                    transform.LookAt(Pills.transform);
                    Debug.Log(Pills.gameObject.name);
                    Pills.GetComponent<Collider>().enabled = false;
                    Pills.GetComponent<MeshRenderer>().enabled = false;
                    Restock.current.OnPillsTaken(Pills);
                    walking = false;
                    moving = false;
                    Invoke("Wait",2);
                    NextTargetPostPill();

                    //course is set for the register
                }
            }
        }
        else if (!leaving)
        {
            //return to the register
            if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z))
            {
                if (target.gameObject.name == "Cashier")
                {
                    transform.LookAt(GameObject.Find("Cube.013").transform);
                    moving = false;
                    Invoke("Wait",5);
                    target = GameObject.Find("Paths End"); //target.transform.parent.gameObject;
                    leaving = true;
                }
               /* else if (target.gameObject.name == "Inside")
                {
                    target = GameObject.Find("Paths End");
                    leaving = true;
                } */
                else if ((target.gameObject.name == "Aisle 2") | (target.gameObject.name == "Aisle 3"))
                {
                    target = GameObject.Find("Cashier");
                }
                if (moving)
                {
                    transform.LookAt(target.transform);

                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3 * Time.deltaTime);
                }
            }
            else if (moving)
            {
                transform.LookAt(target.transform);

                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3 * Time.deltaTime);
            }
        }
        else
        {
            //navigating up the parents

            if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, 3 * Time.deltaTime);
                transform.LookAt(target.transform);
            }

            if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z))
            {
                if(target.gameObject.name == "Starting_1")
                {
                    Destroy(this);
                }
                target = target.transform.parent.gameObject;
            }
        }

    }
    void NextTargetPostPill()
    {
        if (target.gameObject.name == "Aisle End")
        {
            target = target.transform.parent.gameObject;
        }
        else if (target.gameObject.name == "Aisle 1")
        {
            target = GameObject.Find("Cashier");
        }
    }
    //pause at the register
    void Wait()
    {
        moving = true;
    }
}
