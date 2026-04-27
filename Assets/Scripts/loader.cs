using UnityEngine;
using UnityEngine.SceneManagement;

public class loader : MonoBehaviour
{
    public GameObject player;
    public void startgame(){
        player.SetActive(true);
        logic logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<logic>();
        logic.teleportPlayer(new Vector3(-14,4,-7));
        logic.wakeup();
        SceneManager.LoadScene("SampleScene");
    }

    public void exit(){
        Application.Quit();
    }
}
