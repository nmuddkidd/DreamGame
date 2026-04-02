using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Example : MonoBehaviour
{
    [Header("Movement Control")]
    public float playerSpeed = 5.0f;
    public float jumpHeight = 1.5f;
    public float gravityValue = -9.81f;
    private Vector3 playerVelocity;
    private bool groundedPlayer;

    [Header("Camera Control")]
    public float Xsens = 10f;
    public float Ysens = 10f;

    [Header("Input Actions")]
    public InputActionReference moveAction;
    public InputActionReference jumpAction;
    public CharacterController controller;
    private Animator[] animcontrollers = new Animator[2];
    public bool animate = true;
    public GameObject mirror;

    [Header("Input Actions")]

    public String[] reqitems = {"block1","block2","block3"};
    private GameObject[] pickups;
    private bool[] pickedup = new bool[3];
    private float yaw;
    private float pitch;

    private GameObject maincamera;

    [Header("Music")]
    public AudioSource audioSource;
    public AudioClip[] snowfootsteps = new AudioClip[3];
    public AudioClip[] footsteps = new AudioClip[3];
    public int steptimer = 20;
    private float timer;

    private void OnEnable()
    {
        moveAction.action.Enable();
        jumpAction.action.Enable();
    }

    private void OnDisable()
    {
        moveAction.action.Disable();
        jumpAction.action.Disable();
    }

    void Start()
    {
        //animcontroller = GameObject.FindGameObjectWithTag("playermodel").GetComponent<Animator>();
        GameObject[] models = GameObject.FindGameObjectsWithTag("playermodel");
        if(animate){
            animcontrollers[0] = models[0].GetComponent<Animator>();
            animcontrollers[1] = models[1].GetComponent<Animator>();
        }
        maincamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Update()
    {
        movement();
        clickinteraction();
        timer += Time.deltaTime;
        if(timer > 100000)
        {
            timer=0;
        }
        if(Input.GetKeyDown(KeyCode.H)){
            mirror.SetActive(!mirror.activeSelf);
        }
        mirror.transform.position = maincamera.transform.position + maincamera.transform.forward*2;
    }

    void movement()
    {
        groundedPlayer = controller.isGrounded;

        if (groundedPlayer)
        {
            // Slight downward velocity to keep grounded stable
            if (playerVelocity.y < -2f)
                playerVelocity.y = -2f;
        }

        //rotation of playerbody
        pitch += Input.GetAxis("Mouse Y") * Ysens * -1;
        yaw += Input.GetAxis("Mouse X") * Xsens;
        maincamera.transform.rotation = Quaternion.Euler(pitch,yaw,0);
        transform.rotation = Quaternion.Euler(0,yaw,0);
        //(might want to make this only rotate the camera/ part of player the whole player model rotates as is)

        float xrotation = maincamera.transform.eulerAngles.y * Mathf.PI/180;

        // Jump using WasPressedThisFrame()
        if (groundedPlayer && jumpAction.action.WasPressedThisFrame())
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravityValue);
        }

        // Apply gravity
        playerVelocity.y += gravityValue * Time.deltaTime;
        
        //Control handling -- uses legacy input system -- could be updated to new input
        //when I programmed this only god and I knew how it worked, now only god knows
        Vector3 finalMove = new Vector3(0,playerVelocity.y,0);
        if (Input.GetKey(KeyCode.W)){
            finalMove = new Vector3(playerSpeed * Mathf.Sin(xrotation), playerVelocity.y, playerSpeed * Mathf.Cos(xrotation));
        }else if (Input.GetKey(KeyCode.S)){
            finalMove = new Vector3(-1 * playerSpeed * Mathf.Sin(xrotation), playerVelocity.y, -1 * playerSpeed * Mathf.Cos(xrotation));
        }if (Input.GetKey(KeyCode.A)){
            finalMove = new Vector3(-1 * playerSpeed * Mathf.Cos(xrotation), playerVelocity.y, playerSpeed * Mathf.Sin(xrotation));
        }else if (Input.GetKey(KeyCode.D)){
            finalMove = new Vector3(playerSpeed * Mathf.Cos(xrotation), playerVelocity.y, -1 * playerSpeed * Mathf.Sin(xrotation));
        }
        //submits adjusted final move command to movement controller
        controller.Move(finalMove * Time.deltaTime);

        if (Mathf.Sqrt(finalMove.x * finalMove.x + finalMove.z * finalMove.z) > 0 && (int)(timer*5)%steptimer==0){
            audioSource.clip = footsteps[UnityEngine.Random.Range(0,2)];
            audioSource.Play();
            //Debug.Log("footstep");
        }
        
        //Debug.Log(finalMove.magnitude);
        if(animate){
            animcontrollers[0].SetFloat("speed",Mathf.Sqrt(finalMove.x*finalMove.x+finalMove.z*finalMove.z));
            animcontrollers[1].SetFloat("speed",Mathf.Sqrt(finalMove.x*finalMove.x+finalMove.z*finalMove.z));
        }
    }

    void clickinteraction()
    {
        if(Input.GetMouseButtonDown(0))
        {
            //Debug.Log("not done yet :()");
        }
        pickups = GameObject.FindGameObjectsWithTag("pickup");
        foreach (GameObject pickup in pickups)
        {
            //Debug.Log(Vector3.Distance(transform.position, pickup.transform.position)+" "+Vector3.Angle(pickup.transform.position - transform.position, transform.forward));
            if(Input.GetMouseButtonDown(0)){
                if(Vector3.Distance(transform.position, pickup.transform.position)<5
                &&Vector3.Angle(pickup.transform.position - transform.position, transform.forward)<50)
                {
                    pickedup[Array.IndexOf(reqitems,pickup.name)] = true;
                    //Debug.Log(reqitems[Array.IndexOf(reqitems,pickup.name)]+" picked up");
                    Destroy(pickup);
                    audioSource.Play();
                }
            }
        }
    }

    bool allitems()
    {
        foreach(bool b in pickedup)
        {
            if(!b){
                return false;
            }
        }
        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (animate&&other.name == "Enemy")
        {
            animcontrollers[0].SetTrigger("die");
            animcontrollers[1].SetTrigger("die");

        }
    }
}