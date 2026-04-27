using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    [Header("Trash Drop")]
    [SerializeField] private GameObject trashPilePrefab;
    [SerializeField] private float indoorTrashDropChancePerSecond = 0.03f;
    [SerializeField] private float indoorTrashDropCooldown = 3f;
    [SerializeField] private float indoorTrashDropMinDistance = 1.5f;
    [SerializeField] private float trashDropRaycastHeight = 2f;
    [SerializeField] private float trashDropRaycastDistance = 10f;
    [SerializeField] private float trashDropFloorOffset = 0.02f;
    [SerializeField] private float fixedTrashDropY = 2.341731f;
    [SerializeField] private string[] registerWaitObjectNames;
    private readonly List<GameObject> registerWaitObjects = new List<GameObject>();
    private bool waitingAtRegister = false;
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
    private float trashDropCooldownTimer = 0f;
    private Vector3 lastTrashDropPosition;

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
        lastTrashDropPosition = transform.position;
        CacheRegisterWaitObjects();
        SetRegisterWaitObjectsVisible(false);
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
        if (trashDropCooldownTimer > 0f)
        {
            trashDropCooldownTimer -= Time.deltaTime;
        }

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
                TryDropIndoorTrash();
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
                    if (TrySetTargetToChild(target.transform, Paths))
                    {
                        Searcher();
                    }
                }
                else
                {
                    TrySetTargetToChild(target.transform, Paths);
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
                    Transform parentTransform = target.transform.parent;
                    if (parentTransform != null && Paths < parentTransform.childCount)
                    {
                        TrySetTargetToChild(parentTransform, Paths);
                    }
                    else if (parentTransform != null && parentTransform.childCount > 0)
                    {
                        // Clamp to the last valid aisle node if the requested path index is out of range.
                        Paths = parentTransform.childCount - 1;
                        TrySetTargetToChild(parentTransform, Paths);
                    }
                }
                else
                {
                    Transform currentTargetTransform = target.transform;
                    if (currentTargetTransform.childCount > 0)
                    {
                        TrySetTargetToChild(currentTargetTransform, 0);
                    }
                    else
                    {
                        Transform parentTransform = currentTargetTransform.parent;
                        if (parentTransform != null)
                        {
                            int nextSiblingIndex = currentTargetTransform.GetSiblingIndex() + 1;
                            if (nextSiblingIndex < parentTransform.childCount)
                            {
                                Paths = nextSiblingIndex;
                                TrySetTargetToChild(parentTransform, nextSiblingIndex);
                            }
                            else
                            {
                                GameObject cashier = GameObject.Find("Cashier");
                                if (cashier != null)
                                {
                                    SetTargetGameObject(cashier);
                                }
                            }
                        }
                    }
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

                    waitingAtRegister = true;
                    CashRegisterQueue.Enqueue(this);
                    if (SurvivalGameManager.Instance != null)
                    {
                        SurvivalGameManager.Instance.OnCustomerWaitStarted(this);
                    }
                    SetRegisterWaitObjectsVisible(true);
                    moving = false;
                    anim.Play("idle");
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
        waitingAtRegister = false;
        if (SurvivalGameManager.Instance != null)
        {
            SurvivalGameManager.Instance.OnCustomerWaitEnded(this);
        }
        CashRegisterQueue.Remove(this);
        SetRegisterWaitObjectsVisible(false);
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

    public bool IsWaitingAtRegister()
    {
        return waitingAtRegister;
    }

    public void InteractAtRegister()
    {
        if (waitingAtRegister && !leaving && target != null && target.gameObject.name == "Cashier")
        {
            Waiter();
        }
    }

    float GetMoveSpeed()
    {
        return inside ? insideMoveSpeed : outsideMoveSpeed;
    }

    void SetRegisterWaitObjectsVisible(bool visible)
    {
        if (registerWaitObjects.Count == 0)
        {
            CacheRegisterWaitObjects();
        }

        foreach (GameObject registerWaitObject in registerWaitObjects)
        {
            if (registerWaitObject != null)
            {
                registerWaitObject.SetActive(visible);
            }
        }
    }

    void CacheRegisterWaitObjects()
    {
        registerWaitObjects.Clear();

        if (registerWaitObjectNames == null)
        {
            return;
        }

        foreach (string objectName in registerWaitObjectNames)
        {
            if (string.IsNullOrWhiteSpace(objectName))
            {
                continue;
            }

            GameObject sceneObject = FindSceneObjectByName(objectName);
            if (sceneObject != null)
            {
                registerWaitObjects.Add(sceneObject);
            }
        }
    }

    GameObject FindSceneObjectByName(string objectName)
    {
        GameObject foundObject = GameObject.Find(objectName);
        if (foundObject != null)
        {
            return foundObject;
        }

        GameObject[] allObjects = Resources.FindObjectsOfTypeAll<GameObject>();
        foreach (GameObject sceneObject in allObjects)
        {
            if (sceneObject.scene.IsValid() && sceneObject.name == objectName)
            {
                return sceneObject;
            }
        }

        return null;
    }

    void OnDisable()
    {
        if (SurvivalGameManager.Instance != null)
        {
            SurvivalGameManager.Instance.OnCustomerWaitEnded(this);
        }
        CashRegisterQueue.Remove(this);
    }

    void OnDestroy()
    {
        if (SurvivalGameManager.Instance != null)
        {
            SurvivalGameManager.Instance.OnCustomerWaitEnded(this);
        }
        CashRegisterQueue.Remove(this);
    }

    bool TrySetTargetToChild(Transform parentTransform, int childIndex)
    {
        if (parentTransform == null)
        {
            Debug.LogWarning("NPC_Movement: parent transform was null while selecting next path node.");
            return false;
        }

        if (childIndex < 0 || childIndex >= parentTransform.childCount)
        {
            Debug.LogWarning("NPC_Movement: attempted child index " + childIndex + " on " + parentTransform.name + " with childCount " + parentTransform.childCount + ".");
            return false;
        }

        target = parentTransform.GetChild(childIndex).gameObject;
        cur_rotation = transform.rotation;
        transform.LookAt(target.transform);
        target_rotation = transform.rotation;
        transform.rotation = cur_rotation;
        time = 0;
        return true;
    }

    void SetTargetGameObject(GameObject nextTarget)
    {
        if (nextTarget == null)
        {
            return;
        }

        target = nextTarget;
        cur_rotation = transform.rotation;
        transform.LookAt(target.transform);
        target_rotation = transform.rotation;
        transform.rotation = cur_rotation;
        time = 0;
    }

    void TryDropIndoorTrash()
    {
        if (!inside || leaving || trashPilePrefab == null)
        {
            return;
        }

        if (trashDropCooldownTimer > 0f)
        {
            return;
        }

        if (Vector3.Distance(transform.position, lastTrashDropPosition) < indoorTrashDropMinDistance)
        {
            return;
        }

        float chanceThisFrame = indoorTrashDropChancePerSecond * Time.deltaTime;
        if (Random.value <= chanceThisFrame)
        {
            Vector3 spawnPosition = transform.position;
            Vector3 rayOrigin = transform.position + Vector3.up * trashDropRaycastHeight;
            RaycastHit floorHit;

            if (Physics.Raycast(rayOrigin, Vector3.down, out floorHit, trashDropRaycastDistance))
            {
                spawnPosition = floorHit.point + Vector3.up * trashDropFloorOffset;
            }

            spawnPosition.y = fixedTrashDropY;

            GameObject trashPileInstance = Instantiate(trashPilePrefab, spawnPosition, Quaternion.Euler(-90f, 0f, 0f));
            int trashTaskId = SurvivalGameManager.Instance != null ? SurvivalGameManager.Instance.RegisterTrashDrop() : -1;
            if (trashTaskId >= 0)
            {
                TrashPileInteract pile = trashPileInstance.GetComponent<TrashPileInteract>();
                if (pile == null)
                {
                    pile = trashPileInstance.GetComponentInChildren<TrashPileInteract>();
                }
                if (pile != null)
                {
                    pile.SetTaskId(trashTaskId);
                }
            }
            lastTrashDropPosition = spawnPosition;
            trashDropCooldownTimer = indoorTrashDropCooldown;
        }
    }
}
