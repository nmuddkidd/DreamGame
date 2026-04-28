using UnityEngine;
using UnityEngine.SceneManagement;


public class persist : MonoBehaviour
{
    private Vector3 spawner;
    private GameObject player;
    private GameObject interaction;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = this.gameObject.transform.GetChild(0).gameObject;
        interaction = this.gameObject.transform.GetChild(2).gameObject;

        DontDestroyOnLoad(gameObject);
        ApplyLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += ApplyLoaded;
    }

    private void ApplyLoaded(Scene scene, LoadSceneMode mode)
    {
        //check for null
        if (player == null)
        {
            Debug.LogWarning("Player object is null, cannot activate");
            return;
        }
        else if (interaction == null)
        {
            Debug.LogWarning("Interaction screen object is null, cannot activate");
            return;
        }


        if (scene.name == "TitleScreen")
        {
            player.SetActive(false);
            interaction.SetActive(false);
        }
        else
        {
            player.SetActive(true);
            interaction.SetActive(true);
        }
        //set up unique spawners
        if (scene.name == "AnnaDream")
        {
            spawner = new Vector3((float)72, (float)17, (float)93);
            player.transform.localPosition = spawner;   
        }
        else if (scene.name == "SampleScene")
        {
            spawner = new Vector3((float)-16, (float)-0.4, (float)-7.5);
            player.transform.localPosition = spawner;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
