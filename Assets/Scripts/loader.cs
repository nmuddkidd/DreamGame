using UnityEngine;
using UnityEngine.SceneManagement;

public class loader : MonoBehaviour
{
    public GameObject player;
    public void startgame(){
        player.SetActive(true);
        SceneManager.LoadScene("SampleScene");
    }

    public void exit(){
        Application.Quit();
    }
}
