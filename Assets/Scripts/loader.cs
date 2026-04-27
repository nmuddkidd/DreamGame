using UnityEngine;
using UnityEngine.SceneManagement;

public class loader : MonoBehaviour
{
    
    public void startgame(){
        SceneManager.LoadScene("SampleScene");
    }

    public void exit(){
        Application.Quit();
    }
}
