using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayInputHandler : MonoBehaviour
{
    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Gameplay";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string look = "Look";
    [SerializeField] private string jump = "Jump";
    [SerializeField] private string sprint = "Sprint";
    [SerializeField] private string test = "Test";
    [SerializeField] private string click = "Click";
    [SerializeField] private string rotate = "Rotate";

    private InputAction moveAction;
    private InputAction lookAction;
    private InputAction jumpAction;
    private InputAction sprintAction;
    private InputAction testAction;
    private InputAction clickAction;
    private InputAction rotateAction;

    public Vector2 MoveInput { get; private set;}
    public Vector2 LookInput { get; private set;}
    public bool JumpTriggered { get; private set;}
    public float SprintValue {get; private set;}
    public bool TestTriggered { get; private set;}
    public bool ClickTriggered { get; private set;}
    public Vector2 RotateInput { get; private set;}

    public void ResetJump()
    {
        JumpTriggered = false;
    }

    public void ResetTest()
    {
        TestTriggered = false;
    }

    public void ResetClick()
    {
        ClickTriggered = false;
    }

    public static PlayInputHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        lookAction = playerControls.FindActionMap(actionMapName).FindAction(look);
        jumpAction = playerControls.FindActionMap(actionMapName).FindAction(jump);
        sprintAction = playerControls.FindActionMap(actionMapName).FindAction(sprint);
        testAction = playerControls.FindActionMap(actionMapName).FindAction(test);
        clickAction = playerControls.FindActionMap(actionMapName).FindAction(click);
        rotateAction = playerControls.FindActionMap(actionMapName).FindAction(rotate);
        RegisterInputActions();
    }

    void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;

        lookAction.performed += context => LookInput = context.ReadValue<Vector2>();
        lookAction.canceled += context => LookInput = Vector2.zero;

        jumpAction.performed += context => JumpTriggered = true;

        sprintAction.performed += context => SprintValue = context.ReadValue<float>();
        sprintAction.canceled += context => SprintValue = 0.0f;

        testAction.performed += context => TestTriggered = true;

        clickAction.performed += context => ClickTriggered = true;

        rotateAction.performed += context => RotateInput = context.ReadValue<Vector2>();
        rotateAction.canceled += context => RotateInput = Vector2.zero;
    }

    private void OnEnable()
    {
        playerControls.FindActionMap(actionMapName).Enable();
        playerControls.FindActionMap(actionMapName).FindAction(rotate).Disable();
    }

    private void OnDisable()
    {
        if (moveAction == null) return;
        playerControls.FindActionMap(actionMapName).Disable();
    }

    public void inspect(){
        playerControls.FindActionMap(actionMapName).Disable();
        playerControls.FindActionMap(actionMapName).FindAction(rotate).Enable();
        playerControls.FindActionMap(actionMapName).FindAction(click).Enable();
    }

    public void reset(){
        OnEnable();
    }

    public void boatmode(){
        OnDisable();
        playerControls.FindActionMap(actionMapName).FindAction(rotate).Enable();
        playerControls.FindActionMap(actionMapName).FindAction(move).Enable();
        playerControls.FindActionMap(actionMapName).FindAction(click).Enable();
    }
}
