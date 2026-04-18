using UnityEngine;

public class BoatExitEnter : MonoBehaviour
{
    public GameObject player;
    public GameObject BoatCamera;
    public float clickDistance = 10f;

    private GameObject currentPlayer;
    private BoatController boatScript;
    private bool isPlayerInBoat = true; //dream starts with player "waking up" in boat
    private CameraFollow camFollow;
    void Start()
    {
        boatScript = GetComponent<BoatController>();
        BoatCamera.SetActive(true); //boat starts with child camera on
        boatScript.enabled = true; //boat starts with controller enabled
        
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
            //exit boat
            if (isPlayerInBoat && hit.collider.CompareTag("Terrain"))
            {
                if (Vector3.Distance(transform.position, hit.point) <= clickDistance)
                {
                    ExitToLand(hit.point);
                }
            }
            //enter boat
            else if (!isPlayerInBoat && hit.collider.gameObject == gameObject)
            {
                //find player in scene
                currentPlayer = GameObject.FindGameObjectWithTag("Player");

                if (currentPlayer != null && Vector3.Distance(currentPlayer.transform.position, transform.position) <= clickDistance)
                {
                    EnterBoat();
                }
            }
        }
    } 
    void ExitToLand(Vector3 landPoint) //spawns player on land and turns boat off
    {
        
        GameObject newPlayer = Instantiate(player, landPoint + Vector3.up * 1.5f, Quaternion.identity);
        BoatCamera.SetActive(false); //turn off child camera
        boatScript.enabled = false;
        isPlayerInBoat = false;
    }

    void EnterBoat() //destroys roaming player and turns boat on
    {
        Destroy(currentPlayer);
        BoatCamera.SetActive(true); //turn on child camera
        boatScript.enabled = true;
        isPlayerInBoat = true;
        Debug.Log("In Boat");
    }
}