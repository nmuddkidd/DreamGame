using Unity.VisualScripting;
using UnityEngine;
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

    private CharacterController characterController;
    private Camera mainCamera;
    private PlayInputHandler inputHandler;
    private Vector3 currentMovement;
    private float verticalRotation;

    //Inspection system
    private GameObject inspecItem;
    private bool inspecMode;
    private interactable interactionScript;

    private logic logic;

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
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        if (inputHandler.ClickTriggered){
            ClickInteraction();
            inputHandler.ResetClick();
        }
        HandleRotation();
        Sleep();
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
            inspecItem.transform.Rotate(inputHandler.RotateInput.x,inputHandler.RotateInput.y,0);
        }
    }

    void Sleep()
    {
        /*if (inputHandler.TestTriggered)
        {
            Debug.Log("Test triggered");
            inputHandler.ResetTest();
            logic.wakeup();
        }*/
    }

    void ClickInteraction()
    {
        Debug.Log("Click triggered");
        inputHandler.ResetClick();
        if(!inspecMode){
            GameObject[] pickups = GameObject.FindGameObjectsWithTag("Interactable");
            foreach (GameObject pickup in pickups)
            {
                //Debug.Log(Vector3.Distance(transform.position, pickup.transform.position)+" "+Vector3.Angle(pickup.transform.position - transform.position, transform.forward));
                if(Vector3.Distance(transform.position, pickup.transform.position)<5
                &&Vector3.Angle(pickup.transform.position - transform.position, transform.forward)<50)
                {
                    inspecItem = pickup;
                    break;
                }
            }
            if(inspecItem!=null){
                interactionScript = inspecItem.GetComponent<interactable>();
                logic.interactText(interactionScript.title,interactionScript.description,interactionScript.interaction);
                if(interactionScript.setInspecMode){
                    inputHandler.inspect();
                    inspecMode=true;
                    Vector3 newpos = new Vector3(mainCamera.transform.position.x,mainCamera.transform.position.y+.25f,mainCamera.transform.position.z) + mainCamera.transform.forward;
                    inspecItem.transform.position = newpos;
                }
            }
        }else if(interactionScript.fastquit){
            exitInspecMode();
        }
    }

    public void exitInspecMode()
    {
        interactable script = inspecItem.GetComponent<interactable>();
        script.reorigin();
        inspecItem=null;
        inputHandler.reset();
        inspecMode=false;
        logic.disableInteractionUI();
    }
}
