using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;


public class FPSController : MonoBehaviour
{
    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultiplier = 2.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float jumpForce = 5.0f;
    [SerializeField] private float gravity = 9.81f;

    [Header("Look Sensitivity")]
    [SerializeField] private float mouseSensitivity = 2.0f;
    [SerializeField] private float upDownRange = 90.0f;
    //mouse reset parameters
    private Vector2 priorpos = Vector2.zero;
    private Vector2 center;




    private CharacterController characterController;
    private Camera mainCamera;
    private PlayInputHandler inputHandler;
    private Vector3 currentMovement;
    private float verticalRotation;

    //Inspection system
    private GameObject inspecItem;
    private bool inspecMode;
    private interactable interactionScript;


    private bool boatmode = false;
    [Header("Vehicle")]
    public float boatSpeed = 10f;
    public float exitDist = 100f;
    public float boatRotateSpeed = 2f;

    private logic logic;
    private float timer;

    [Header("Trash Cleanup")]
    [SerializeField] private GameObject dumpsterPointer;
    [SerializeField] private GameObject dumpsterPointerPrefab;
    [SerializeField] private float dumpsterPointerDistanceFromCamera = 1.5f;
    [SerializeField] private float dumpsterPointerVerticalOffset = -0.15f;
    [SerializeField] private float dumpsterDropDistance = 3f;
    private TrashBagDrag carriedTrashBag;
    private GameObject runtimeDumpsterPointer;
    private bool warnedMissingDumpsterPointer;

    LayerMask layerMask;
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        layerMask = LayerMask.GetMask("Default"); 
        logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<logic>();

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        inputHandler = PlayInputHandler.Instance;
        //set cursor to center
        float width = Screen.width / 2;
        float height = Screen.height / 2;
        center = new Vector2(width, height);
        Mouse.current.WarpCursorPosition(center);
        ///make cursor invis
        ///this won't really happen in unity debugging unless you click the screen
        //Cursor.visible = false;
        CustomEvents.current.PickUp += RayHit;
    }

    private void OnDestroy()
    {
        Cursor.visible = true;
    }

    private void Update()
    {
        Sleep();
        if(!boatmode){
            HandleMovement();
        }else{
            HandleBoatMovement();
        }
        HandleLook();
        if (inputHandler.ClickTriggered){
            ClickInteraction();
            inputHandler.ResetClick();
        }
        if(inspecMode){
            HandleRotation();
        }
        HandleDumpsterPointer();
        testOOB();
        //reset mouse position if off center and still
        Vector2 mouse = Mouse.current.delta.ReadValue();
        if (mouse == priorpos)
        {
            //Mouse.current.WarpCursorPosition(center);
        }
        else
        {
            //priorpos = mouse;
        }
        //update the raycast to fit the mouse pointer
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
    }


    void HandleMovement()
    {
        float speed = walkSpeed * (inputHandler.SprintValue > 0 ? sprintMultiplier : 1f);

        Vector3 inputDirection = new Vector3(inputHandler.MoveInput.x, 0f, inputHandler.MoveInput.y);
        Vector3 worldDirection = transform.TransformDirection(inputDirection);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * speed;
        currentMovement.z = worldDirection.z * speed;

        HandleJumping();

        characterController.Move(currentMovement * Time.deltaTime);
    }

    void HandleBoatMovement(){
        timer += Time.deltaTime;
        if(timer > 10000){
            timer = 0;
        }
        inspecItem.transform.Rotate(0,inputHandler.MoveInput.x*boatRotateSpeed,0);
        Vector3 currentMovement = new Vector3(inputHandler.MoveInput.y * Mathf.Sin(inspecItem.transform.eulerAngles.y * Mathf.PI/180f) , 0 , inputHandler.MoveInput.y * Mathf.Cos(inspecItem.transform.eulerAngles.y * Mathf.PI/180f));
        //Debug.Log(transform.eulerAngles.y* Mathf.PI/180f);
        inspecItem.transform.position = new Vector3(transform.position.x,transform.position.y-2,transform.position.z);
        //inspecItem.transform.eulerAngles = new Vector3(0,transform.eulerAngles.y,0);
        characterController.Move(currentMovement * Time.deltaTime * boatSpeed);
    }

    void HandleJumping()
    {
        Ray jumpray = new Ray(mainCamera.transform.position, Vector3.down);
        RaycastHit jumphit;
        if( Physics.Raycast(jumpray, out jumphit, 2.2f, layerMask ))
        {
            currentMovement.y -= gravity * Time.deltaTime;

            if (inputHandler.JumpTriggered)
            {
                currentMovement.y = jumpForce;
            }
        }
        else 
        {
            currentMovement.y -= gravity * Time.deltaTime;
        }

        inputHandler.ResetJump();
    }

    void HandleLook()
    {
        float mouseXRotation = inputHandler.LookInput.x * mouseSensitivity;
        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= inputHandler.LookInput.y * mouseSensitivity;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownRange, upDownRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0,0);
    }

    void HandleRotation(){
        if(inspecMode&&inspecItem!=null){
            //Debug.Log(inputHandler.RotateInput);
            inspecItem.transform.Rotate(inputHandler.RotateInput.x,inputHandler.RotateInput.y,.1f);
        }
    }

    void Sleep()
    {
        if (inputHandler.TestTriggered)
        {
            Debug.Log("Test triggered");
            inputHandler.ResetTest();
            logic.wakeup();
        }
    }

    void RayHit(GameObject other)
    {
        inputHandler.ResetClick();
        inspecItem = other;
        interactionScript = inspecItem.GetComponent<interactable>();
        logic.interactText(interactionScript);

        if (interactionScript != null && interactionScript.interaction == "trashpile")
        {
            return;
        }

        //if the set interaction is to exit
        if (CustomEvents.current.currentMode == CustomEvents.InputMode.Absent)
        {
            exitInspecMode();
            return;
        }
        //if the set interaction is to inspect, also double check for NULL
        else if (inspecItem != null)
        {
            inputHandler.inspect();
            inspecMode = true;
            //set up the highlighted item
            Vector3 newpos = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + .25f, mainCamera.transform.position.z) + mainCamera.transform.forward;
            inspecItem.transform.position = newpos;
            return;
        }
    }
                              
    void ClickInteraction()
    {
        Debug.Log("Click triggered");
        inputHandler.ResetClick();
        //make sure the raycast function isn't the one being used
        if (CustomEvents.current.currentMode == CustomEvents.InputMode.Interact)
        {
            return;
        }

        if (!boatmode && IsCarryingTrashBag())
        {
            if (!TryDeliverTrashBag())
            {
                DropCarriedTrashBag();
            }
            return;
        }

        if(!inspecMode&&!boatmode){
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                NPC_Movement npcMovement = hit.collider.GetComponentInParent<NPC_Movement>();
                if (npcMovement != null && npcMovement.IsWaitingAtRegister())
                {
                    npcMovement.InteractAtRegister();
                    return;
                }

                interactable clickedInteractable = hit.collider.GetComponentInParent<interactable>();
                if (clickedInteractable != null)
                {
                    if (TryPickUpTrashBag(clickedInteractable))
                    {
                        return;
                    }

                    inspecItem = clickedInteractable.gameObject;
                    interactionScript = clickedInteractable;
                    logic.interactText(interactionScript);
                    if (interactionScript.interaction == "trashpile")
                    {
                        return;
                    }
                    if (interactionScript.grab)
                    {
                        inputHandler.inspect();
                        inspecMode = true;
                        Vector3 newpos = new Vector3(mainCamera.transform.position.x, mainCamera.transform.position.y + .25f, mainCamera.transform.position.z) + mainCamera.transform.forward;
                        inspecItem.transform.position = newpos;
                        return;
                    }
                    if (interactionScript.vehicle)
                    {
                        characterController.enabled = false;
                        gameObject.transform.position = new Vector3(inspecItem.transform.position.x, inspecItem.transform.position.y + 2, inspecItem.transform.position.z);
                        gameObject.transform.rotation = inspecItem.transform.rotation;
                        characterController.enabled = true;
                        currentMovement.y = 0;
                        boatmode = true;
                        return;
                    }
                }
            }

            inspecItem = null;
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (GameObject pickup in pickups)
            {
                //Debug.Log(Vector3.Distance(transform.position, pickup.transform.position)+" "+Vector3.Angle(pickup.transform.position - transform.position, transform.forward));
                if(Vector3.Distance(transform.position, pickup.transform.position)<5
                &&Vector3.Angle(pickup.transform.position - mainCamera.transform.position, mainCamera.transform.forward)<20)
                {
                    inspecItem = pickup;
                    break;
                }
            }
            if(inspecItem!=null){
                interactionScript = inspecItem.GetComponent<interactable>();
                if (interactionScript == null)
                {
                    Debug.LogWarning("Interactable-tagged object is missing interactable component: " + inspecItem.name);
                    return;
                }

                if (TryPickUpTrashBag(interactionScript))
                {
                    return;
                }

                logic.interactText(interactionScript);
                if (interactionScript.interaction == "trashpile")
                {
                    return;
                }
                if(interactionScript.grab){
                    inputHandler.inspect();
                    inspecMode=true;
                    Vector3 newpos = new Vector3(mainCamera.transform.position.x,mainCamera.transform.position.y+.25f,mainCamera.transform.position.z) + mainCamera.transform.forward;
                    inspecItem.transform.position = newpos;
                }else if(interactionScript.vehicle){
                    characterController.enabled = false;
                    gameObject.transform.position = new Vector3(inspecItem.transform.position.x,inspecItem.transform.position.y+2,inspecItem.transform.position.z);
                    gameObject.transform.rotation = inspecItem.transform.rotation;
                    characterController.enabled = true;
                    currentMovement.y = 0;
                    boatmode = true;
                }
            }
        }else if(inspecMode && interactionScript != null && interactionScript.fastquit){
            exitInspecMode();
        }else if(boatmode){
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //debugging 
                Debug.Log("Raycast hit: " + hit.collider.name + "Tag: " + hit.collider.tag + "Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

                //exit boat
                if (boatmode && hit.collider.CompareTag("Terrain"))
                {
                    if (Vector3.Distance(transform.position, hit.point) <= exitDist)
                    {
                        characterController.enabled = false;
                        gameObject.transform.position = hit.point + Vector3.up * 5f;
                        characterController.enabled = true;
                        Debug.Log(hit.point+" "+gameObject.transform.position);
                        boatmode=false;
                    }
                    else
                    {
                        Debug.Log("Too far from terrain. Distance: " + Vector3.Distance(transform.position, hit.point));
                    }
                }
            }else{
                Debug.Log("Raycast not hitting anything");
            }
        }
        
    }       
                   
    public void exitInspecMode()
    {
        if (inspecItem == null)
        {
            inputHandler.reset();
            inspecMode = false;
            logic.disableInteractionUI();
            return;
        }

        interactable script = inspecItem.GetComponent<interactable>();
        if (script != null)
        {
            script.reorigin();
        }
        inspecItem=null;
        interactionScript = null;
        inputHandler.reset();
        inspecMode=false;
        logic.disableInteractionUI();
    }                            

    void HandleDumpsterPointer()
    {
        GameObject activePointer = GetOrCreateDumpsterPointer();
        if (activePointer == null)
        {
            return;
        }

        if (!IsCarryingTrashBag())
        {
            activePointer.SetActive(false);
            return;
        }

        Transform nearestDumpster = FindClosestDumpster();
        if (nearestDumpster == null)
        {
            activePointer.SetActive(false);
            return;
        }

        activePointer.SetActive(true);
        Vector3 cameraForward = mainCamera.transform.forward;
        Vector3 cameraUp = mainCamera.transform.up;
        Vector3 pointerPosition = mainCamera.transform.position + (cameraForward * dumpsterPointerDistanceFromCamera) + (cameraUp * dumpsterPointerVerticalOffset);
        activePointer.transform.position = pointerPosition;

        Vector3 toDumpster = nearestDumpster.position - pointerPosition;
        if (toDumpster.sqrMagnitude > 0.0001f)
        {
            activePointer.transform.rotation = Quaternion.LookRotation(toDumpster.normalized, cameraUp);
        }
    }

    GameObject GetOrCreateDumpsterPointer()
    {
        if (dumpsterPointer != null)
        {
            return dumpsterPointer;
        }

        if (runtimeDumpsterPointer != null)
        {
            return runtimeDumpsterPointer;
        }

        if (dumpsterPointerPrefab != null)
        {
            runtimeDumpsterPointer = Instantiate(dumpsterPointerPrefab);
            runtimeDumpsterPointer.SetActive(false);
            return runtimeDumpsterPointer;
        }

        if (!warnedMissingDumpsterPointer)
        {
            Debug.LogWarning("FPSController: assign either Dumpster Pointer scene object or Dumpster Pointer Prefab.");
            warnedMissingDumpsterPointer = true;
        }

        return null;
    }

    bool IsCarryingTrashBag()
    {
        return carriedTrashBag != null && carriedTrashBag.IsBeingDragged;
    }

    Transform FindClosestDumpster()
    {
        GameObject[] dumpsters = GameObject.FindGameObjectsWithTag("Dumpster");
        Transform closest = null;
        float bestDistance = float.MaxValue;

        foreach (GameObject dumpster in dumpsters)
        {
            if (dumpster == null)
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, dumpster.transform.position);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                closest = dumpster.transform;
            }
        }

        return closest;
    }

    bool TryDeliverTrashBag()
    {
        if (!IsCarryingTrashBag())
        {
            return false;
        }

        Transform closestDumpster = FindClosestDumpster();
        if (closestDumpster == null)
        {
            return false;
        }

        if (Vector3.Distance(transform.position, closestDumpster.position) > dumpsterDropDistance)
        {
            return false;
        }

        carriedTrashBag.Consume();
        carriedTrashBag = null;
        logic.disableInteractionUI();
        return true;
    }

    void DropCarriedTrashBag()
    {
        if (carriedTrashBag == null)
        {
            return;
        }

        carriedTrashBag.EndDrag();
        carriedTrashBag = null;
        interactionScript = null;
        logic.disableInteractionUI();
    }

    bool TryPickUpTrashBag(interactable clickedInteractable)
    {
        if (clickedInteractable == null || clickedInteractable.interaction != "trashbag")
        {
            return false;
        }

        TrashBagDrag bag = clickedInteractable.GetComponentInParent<TrashBagDrag>();
        if (bag == null)
        {
            Debug.LogWarning("Trashbag interactable is missing TrashBagDrag: " + clickedInteractable.name);
            return true;
        }

        if (carriedTrashBag != null && carriedTrashBag != bag)
        {
            return true;
        }

        bag.BeginDrag(mainCamera.transform);
        carriedTrashBag = bag;
        interactionScript = clickedInteractable;
        logic.disableInteractionUI();
        return true;
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.name == "earth"){
            SceneManager.LoadScene("GriffinDream");
            teleportPlayer(new Vector3(0,90,0));
        }
        Debug.Log(hit.gameObject.name);
        if (hit.gameObject.tag == "wakeup")
        {
            logic.teleportPlayer(new Vector3(-14,4,-7));
            logic.wakeup();
        }
    }

    void testOOB()
    {
        if (transform.position.y < -10)
        {
            teleportPlayer(new Vector3(-24,100,0));
            logic.wakeup();
        }
    }

    void enterBoat(){
        inputHandler.boatmode();
    }

    public void teleportPlayer(Vector3 newpos){
        characterController.enabled = false;
        gameObject.transform.position = newpos;
        characterController.enabled = true;
    }

    void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Blind")) {
            logic.blindUI();
        }
    }

    public void setSpeed(float s){walkSpeed = s;}
}
