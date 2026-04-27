using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class NPC_Movement : MonoBehaviour
{
    [SerializeField] private GameObject target;
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
    [SerializeField] private float outsideMoveSpeed = 3f;
    [SerializeField] private float insideMoveSpeed = 1.5f;
    //Pills to find
    private GameObject Pills;
    //distance between NPC and the pills               
    private Vector2 pill_loc = new Vector2(0, 0);
    private Vector2 NPC_loc = new Vector2(0, 0);
    //get the desired rotation 
    private Quaternion target_rotation;
    private Quaternion cur_rotation;

    Animation anim;
    private float time = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        anim = GetComponent<Animation>();
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
        cur_rotation = transform.rotation;
        transform.LookAt(target.transform);
        target_rotation = transform.rotation;
        transform.rotation = cur_rotation;
        //transform.LookAt(target.transform);
    }

    void Searcher()
    {

        GameObject shelves = GameObject.Find("Pill Shelves");
        Pill_Shelves script = shelves.GetComponent<Pill_Shelves>();
        Pills = script.Shelves[Random.Range(0, (script.Shelves.Length))].gameObject;
    
        pill_loc.x = Pills.transform.position.x;
        pill_loc.y = Pills.transform.position.z;
        inside = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.isPlaying == false)
        {
            if (moving)
            {
                anim.Play("walkerWalk");
            }
            else
            {
                anim.Play("idle");
            }
        }
        time += Time.deltaTime;
        transform.rotation = Quaternion.Slerp(cur_rotation, target_rotation,  time * 3);
        if (target.gameObject.name == "Cube.013")
        {
            return;
        }
        {
            
        }
        //move along the path
        if (walking)
        {
            if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, GetMoveSpeed() * Time.deltaTime);
                anim.Play("walkerWalk");
            }
            if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z) && !(inside))
            {
                //find if the path has came to the doors 
                if (target.gameObject.name == "Paths End")
                {
                    target = GameObject.Find("Inside");
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
                }
                //if the target is inside of the station
                else if (target.gameObject.name == "Inside")
                {
                    target = target.transform.GetChild(Paths).gameObject;
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
                    Searcher();
                }
                else
                {
                    target = target.transform.GetChild(Paths).gameObject;
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
                    //Paths = Paths + 1;
                }
            }
            //once inside the store, switch up navigation if statements
            else if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z))
            {
                //if pills are a certain distance away, move to next aisle 
                if (Mathf.Abs(transform.position.z - Pills.transform.position.z) > 1.75)
                {
                    Paths = Paths + 1; 
                    target = target.transform.parent.gameObject.transform.GetChild(Paths).gameObject;
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
                }
                else
                {
                    target = target.transform.GetChild(0).gameObject;
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
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
                    //transform.LookAt(Pills.transform);
                    cur_rotation = transform.rotation;
                    transform.LookAt(Pills.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;

                    //Turn off Pill visibility
                    Pills.GetComponent<Collider>().enabled = false;
                    Pills.GetComponent<MeshRenderer>().enabled = false;
                    Restock.current.OnPillsTaken(Pills);
                    walking = false;
                    moving = false;
                    anim.Play("pickingUp");
                    Invoke("Wait",4);
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
                if (target.gameObject.name == "Cashier" && moving)
                {
                    //transform.LookAt(GameObject.Find("Cube.013").transform);
                    cur_rotation = transform.rotation;
                    transform.LookAt(GameObject.Find("Cube.013").transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;

                    moving = false;
                    anim.Play("idle");
                    Invoke("Waiter",5);
                     //target.transform.parent.gameObject;
                }
               /* else if (target.gameObject.name == "Inside")
                {
                    target = GameObject.Find("Paths End");
                    leaving = true;
                } */
                else if ((target.gameObject.name == "Aisle 2") | (target.gameObject.name == "Aisle 3"))
                {
                    target = GameObject.Find("Cashier");
                    cur_rotation = transform.rotation;
                    transform.LookAt(target.transform);
                    target_rotation = transform.rotation;
                    transform.rotation = cur_rotation;
                    time = 0;
                }
                if (moving)
                {
                   // transform.LookAt(target.transform);
                   
                    transform.position = Vector3.MoveTowards(transform.position, target.transform.position, GetMoveSpeed() * Time.deltaTime);
                }
            }
            else if (moving)
            {
               // transform.LookAt(target.transform);
               
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, GetMoveSpeed() * Time.deltaTime);
            }
        }
        else
        {
            //navigating up the parents

            if (moving)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, GetMoveSpeed() * Time.deltaTime);
                //transform.LookAt(target.transform);
               
            }

            if ((transform.position.x == target.transform.position.x) && (transform.position.z == target.transform.position.z))
            {
                if(target.gameObject.name == "Starting_1")
                {
                    Destroy(this.gameObject);
                }
                target = target.transform.parent.gameObject;
                cur_rotation = transform.rotation;
                transform.LookAt(target.transform);
                target_rotation = transform.rotation;
                transform.rotation = cur_rotation;
                time = 0;
            }
        }

    }
    void NextTargetPostPill()
    {
        if (target.gameObject.name == "Aisle End")
        {
            target = target.transform.parent.gameObject;
            cur_rotation = transform.rotation;
            transform.LookAt(target.transform);
            target_rotation = transform.rotation;
            transform.rotation = cur_rotation;
            time = 0;
        }
        else if (target.gameObject.name == "Aisle 1")
        {
            target = GameObject.Find("Cashier");
            cur_rotation = transform.rotation;
            transform.LookAt(target.transform);
            target_rotation = transform.rotation;
            transform.rotation = cur_rotation;
            time = 0;
        }
    }
    //pause at the register
    void Wait()
    {
        moving = true;
    }
    void Waiter()
    {
        anim.Play("dance");
        print("waiters script " + target.gameObject.name);
        target = GameObject.Find("Paths End");
        cur_rotation = transform.rotation;
        transform.LookAt(target.transform);
        target_rotation = transform.rotation;
        transform.rotation = cur_rotation;
        time = 0;
        leaving = true;
        moving = true;
    }

    float GetMoveSpeed()
    {
        return inside ? insideMoveSpeed : outsideMoveSpeed;
    }
}
