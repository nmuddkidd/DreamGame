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
    private int dialogueIndex;
    private interactable interactableScript;
    private float interactTimer;

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
    public sfxlogic sfxlogic;

    [Header("Player")]
    public GameObject Player;
    private GameObject trash;

    //timers
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
        if(interactTimer>0){
            interactTimer-=Time.deltaTime;
            if (interactTimer < 0)
            {
                advanceInteractText();
            }
        }
    }

    //UI STUFF
    //UI STUFF
    //UI STUFF

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
        if (script == null)
        {
            UnityEngine.Debug.LogWarning("logic.interactText called with null interactable.");
            return;
        }

        if (script.interaction == "cashregister" && CashRegisterQueue.ReleaseNextGrandma())
        {
            StartCoroutine(ShowCashRegisterCheckoutMessage());
            return;
        }

        dialogueIndex = -1;
        interactableScript = script;
        if (title != null)
        {
            title.text = interactableScript.title;
        }
        advanceInteractText();
        switch (interactableScript.interaction)
        {
            case "vehicle":
                break;
            case "cashregister":
                if (interactionUI != null)
                {
                    interactionUI.SetActive(true);
                }
                break;
            case "bed":
                dream();
                break;
            case "computer":
                if (computerMenu != null)
                {
                    computerMenu.SetActive(true);
                }
                break;
            default:
                if (interactionUI != null)
                {
                    interactionUI.SetActive(true);
                }
                break;
        }
    }

    public void advanceInteractText()
    {
        if (interactableScript == null || interactableScript.description == null || interactableScript.description.Length == 0)
        {
            interactTimer = -1;
            disableInteractionUI();
            return;
        }

        if (dialogueIndex < interactableScript.description.Length-1)
        {
            dialogueIndex++;
            if (description != null)
            {
                description.text = interactableScript.description[dialogueIndex];
            }
            interactTimer = interactableScript.description[dialogueIndex].Length * .1f;
        }
        else
        {
            interactTimer = -1;
            disableInteractionUI();
        }
    }

    public void disableInteractionUI(){
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
        if (computerMenu != null)
        {
            computerMenu.SetActive(false);
        }
    }

    IEnumerator ShowCashRegisterCheckoutMessage()
    {
        interactTimer = -1;

        if (title != null)
        {
            title.text = "Checked Out!";
        }

        if (description != null)
        {
            description.text = "Another customer served...";
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        disableInteractionUI();
    }

    //Gameplay logic
    //Gameplay logic
    //Gameplay logic

    public void wakeup()
    {
        SceneManager.LoadScene("SampleScene");
        days+=3;
        if(days>7){
            endgame();
        }
        Player.transform.position = new Vector3(-24,100,0);
        wakeupText = "Today is day "+days+" of the trial";
        audioSource.PlayOneShot(alarm,1);
        dayCounter.enabled = true;
        wakeupTextIndex = -17;
        textTimer = -14;
        if(days>4){
            sfxlogic.changeBackground("RealWorldSoft");
        }else{
            StartCoroutine(oneFrame());
            sfxlogic.changeBackground("RealWorld");
        }
    }

    IEnumerator oneFrame()
    {
        yield return null;
        GameObject.FindWithTag("trash").SetActive(false);
    }
    public void endgame(){

    }
    
    public void dream(){
        if(days>3){
            sfxlogic.changeBackground("Mirror");
        }else{
            sfxlogic.changeBackground("Reflection");
        }
        switch(Random.Range(1, 2)){
            case 0:
                handleSpaceDream();
                break;
            case 1:
                handleAnnaDream();
                break;
            case 2:
                handleGriffinDream();
                break;
            case 3:
                break;
            case 4:
                break;
        }
    }

    public void handleAnnaDream(){
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(2363,10,289));
        Player.GetComponent<FPSController>().setSpeed(50);
        SceneManager.LoadScene("AnnaDream");
    }

    public void handleGriffinDream(){
        SceneManager.LoadScene("GriffinDream");
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(0,0,0));
    }

    public void handleSpaceDream(){
        sfxlogic.changeBackground("Engine");
        SceneManager.LoadScene("Space");
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(0,0,0));
    }
}
