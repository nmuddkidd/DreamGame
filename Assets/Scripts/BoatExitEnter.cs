using UnityEngine;

public class BoatExitEnter : MonoBehaviour
{
    public GameObject player;
    public GameObject BoatCamera;
    public GameObject sittingVisual;
    public float clickDistance = 100f;

    private GameObject currentPlayer;
    private BoatController boatScript;
    private bool isPlayerInBoat = true; //dream starts with player "waking up" in boat
    private CameraFollow camFollow;
    void Start()
    {
        boatScript = GetComponent<BoatController>();

        BoatCamera.SetActive(true); //boat starts with child camera on
        boatScript.enabled = true; //boat starts with controller enabled

        if (sittingVisual != null) sittingVisual.SetActive(true); //sitting player in boat visual when dream starts
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleClick();
        }
    }

    void HandleClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //debugging 
            Debug.Log("Raycast hit: " + hit.collider.name + "Tag: " + hit.collider.tag + "Layer: " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            //exit boat
            if (isPlayerInBoat && hit.collider.CompareTag("Terrain"))
            {
                if (Vector3.Distance(transform.position, hit.point) <= clickDistance)
                {
                    ExitToLand(hit.point);
                }
                else
                {
                    Debug.Log("Too far from terrain. Distance: " + Vector3.Distance(transform.position, hit.point));
                }
            }
            //enter boat
            else if (!isPlayerInBoat && hit.collider.CompareTag("Boat"))
            {
                //find player in scene
                currentPlayer = GameObject.FindGameObjectWithTag("Player");

                if (currentPlayer != null && Vector3.Distance(currentPlayer.transform.position, transform.position) <= clickDistance)
                {
                    EnterBoat();
                }
                else
                {
                    Debug.Log("Too far fromm boat/player not found");
                }
            }
        }
        else
        {
            Debug.Log("Raycast not hitting anything");
        }
    } 
    void ExitToLand(Vector3 landPoint) //spawns player on land and turns boat off
    {
        
        GameObject newPlayer = Instantiate(player, landPoint + Vector3.up * 1.5f, Quaternion.identity);
        if (sittingVisual != null) sittingVisual.SetActive(false); //turns sitting visual off when on land
        //turn off boat camera and listener
        BoatCamera.SetActive(false); 
        if(BoatCamera.GetComponent<AudioListener>())
            BoatCamera.GetComponent<AudioListener>().enabled = false;

        //switch audio listener to main camera
        AudioListener playerEar = newPlayer.GetComponentInChildren<AudioListener>();
        if (playerEar != null) playerEar.enabled = true;

        boatScript.enabled = false;
        isPlayerInBoat = false;
    }

    void EnterBoat() //destroys roaming player and turns boat on
    {
        Destroy(currentPlayer);
        if (sittingVisual != null) sittingVisual.SetActive(true); //turns sitting visual on


        //turn on boat camera and listener
        BoatCamera.SetActive(true); 
        if (BoatCamera.GetComponent<AudioListener>())
            BoatCamera.GetComponent<AudioListener>().enabled = true;


        boatScript.enabled = true;
        isPlayerInBoat = true;
        Debug.Log("In Boat");
    }
}