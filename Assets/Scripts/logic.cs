using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;

public class logic : MonoBehaviour
{
    [Header("Item Interaction")]
    public Text title;
    public Text description;
    public GameObject interactionUI;
    private string interactPrompt;
    private int dialogueIndex;
    private interactable interactableScript;

    [Header("Pih game")]
    public GameObject computerMenu;

    [Header("Calendar")]
    public Text dayCounter;
    private int days;
    private float textTimer;
    private int wakeupTextIndex = 99;
    private string wakeupText = "Today is day of the trial";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip alarm;

    [Header("Player")]
    public GameObject Player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(wakeupTextIndex<wakeupText.Length){
            textTimer+=Time.deltaTime;
            if (textTimer > .25)
            {
                advanceWakeupText();
                textTimer = 0;
            }
        }
        if(textTimer>0){
            textTimer-=Time.deltaTime;
            if (textTimer < 0)
            {
                advanceInteractText();
            }
        }
    }

    public void wakeup()
    {
        SceneManager.LoadScene("SampleScene");
        Player.transform.position = new Vector3(-24,100,0);
        days++;
        wakeupText = "Today is day "+days+" of the trial";
        audioSource.PlayOneShot(alarm,1);
        dayCounter.enabled = true;
        wakeupTextIndex = -17;
        textTimer = -14;
    }

    private void advanceWakeupText(){
        wakeupTextIndex++;
        string scrollDisplay = "";
        for(int i = wakeupTextIndex; i < wakeupTextIndex+17; i++){
            if(i<wakeupText.Length&&i>-1){
                scrollDisplay += wakeupText[i];
            }else{
                scrollDisplay += " ";
            }
        }
        dayCounter.text = scrollDisplay;
    }

    public void executepihh()
    {
        string projectRoot = Path.GetDirectoryName(Application.dataPath);
        
        // Combine to get the full path to the .exe
        string exePath = Path.Combine(projectRoot, "PIH-FPS-BUILD", "pih.exe");


        if (File.Exists(exePath))
        {
            ProcessStartInfo startInfo = new ProcessStartInfo(exePath);
            // Set working directory to the folder containing the exe
            startInfo.WorkingDirectory = Path.GetDirectoryName(exePath);
            
            Process.Start(startInfo);
            UnityEngine.Debug.Log("Executing: " + exePath);
        }
        else
        {
            UnityEngine.Debug.LogError("File not found at: " + exePath);
        }
    }

    public void interactText(interactable script){
        dialogueIndex = -1;
        interactableScript = script;
        title.text = interactableScript.title;
        advanceInteractText();
        switch (interactableScript.interaction)
        {
            case "dialogue":
                
            case "bed":
                //wakeup();
                SceneManager.LoadScene("Space");
                Player.transform.position = new Vector3(0,5,0);
                break;
            case "computer":
                computerMenu.SetActive(true);
                break;
            default:
                interactionUI.SetActive(true);
                break;
        }
    }

    public void advanceInteractText()
    {
        if (dialogueIndex < interactableScript.description.Length-1)
        {
            dialogueIndex++;
            description.text = interactableScript.description[dialogueIndex];
            textTimer = interactableScript.description[dialogueIndex].Length * .1f;
        }
        else
        {
            textTimer = -1;
            disableInteractionUI();
        }
    }

    public void disableInteractionUI(){
        interactionUI.SetActive(false);
        computerMenu.SetActive(false);
    }
}
