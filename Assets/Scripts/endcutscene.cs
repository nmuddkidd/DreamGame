using UnityEngine;
using UnityEngine.UI;

public class endcutscene : MonoBehaviour
{
    private float timer = 0;
    private int trigger = 0;
    public GameObject lights;
    public Text first;
    public Text second;
    public Text credits;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip scream;
    public AudioClip lightsound;
    public AudioClip crunch;
    public AudioClip reflection;
    void Start()
    {
        second.enabled = false;
        credits.enabled = false;
        first.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 5){
            switch(trigger){
                case 1:
                    first.enabled = false;
                    second.enabled = true;
                    break;
                case 2:
                    second.enabled = false;
                    break;
                case 3:
                    lights.SetActive(true);
                    audioSource.Play();
                    //audioSource.PlayOneShot(scream,1);
                    break;
                case 4:
                    audioSource.PlayOneShot(crunch,1);
                    break;
                case 5:
                    audioSource.Stop();
                    lights.SetActive(false);
                    audioSource.clip = reflection;
                    audioSource.Play();
                    break;
                case 7:
                    credits.enabled = true;
                    break;
            }
            trigger++;
            timer = 0;
        }
    }
}
