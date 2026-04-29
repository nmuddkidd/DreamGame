using UnityEngine;

public class teleportspid : MonoBehaviour
{
    public GameObject Player;
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        if(Player!=null){
            Player.GetComponent<FPSController>().teleportPlayer(new Vector3(2427,25,1114));
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Player == null){
            Player = GameObject.FindGameObjectWithTag("Player");
            if(Player!=null){
                Player.GetComponent<FPSController>().teleportPlayer(new Vector3(2427,25,1114));
            }
        }
    }
}
