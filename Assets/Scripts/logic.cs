using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;

public class logic : MonoBehaviour
{
    private const int DreamCount = 4;
    private static readonly bool[] dreamSeen = new bool[DreamCount];
    private static int seenDreamCount = 0;

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
    public int days;
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

    [Header("Misc UI")]
    public GameObject blind;
    private float blindTimer;

    private bool medicated;

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
        if(blindTimer>0){
            blindTimer -= Time.deltaTime;
            if(blindTimer < 0){
                teleportPlayer(new Vector3(-14,4,-7));
                wakeup();
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

        if (script.interaction == "trashpile")
        {
            TrashPileInteract pile = script.GetComponent<TrashPileInteract>();
            if (pile == null)
            {
                pile = script.GetComponentInParent<TrashPileInteract>();
            }
            if (pile != null && pile.SpawnTrashBag())
            {
                StartCoroutine(ShowTimedMessage("Trash Collected", "Bagged the mess. Take it to a dumpster.", 1.5f));
            }
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
            case "Pills":
                interactionUI.SetActive(true);
                medicated = true;
                UnityEngine.Debug.Log(medicated);
                break;
            case "vehicle":
                break;
            case "cashregister":
                if (interactionUI != null)
                {
                    interactionUI.SetActive(true);
                }
                break;
            case "bed":
                if(medicated){
                    medicated = false;
                    dream();
                }else{
                    interactionUI.SetActive(true);
                }
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
        yield return StartCoroutine(ShowTimedMessage("Checked Out!", "Another customer served...", 2f));
    }

    IEnumerator ShowTimedMessage(string messageTitle, string messageBody, float duration)
    {
        interactTimer = -1;

        if (title != null)
        {
            title.text = messageTitle;
        }

        if (description != null)
        {
            description.text = messageBody;
        }

        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }

        yield return new WaitForSeconds(duration);

        disableInteractionUI();
    }

    public void blindUI(){
        blindTimer = .2f;
        blind.SetActive(true);
    }

    //Gameplay logic
    //Gameplay logic
    //Gameplay logic

    public void wakeup()
    {
        Player.GetComponent<FPSController>().setSpeed(3);
        SceneManager.LoadScene("SampleScene");
        days++;
        if(days>6){
            endgame();
        }
        wakeupText = "Today is day "+days+" of the trial";
        audioSource.PlayOneShot(alarm,1);
        dayCounter.enabled = true;
        wakeupTextIndex = -17;
        textTimer = -14;
        resetBlind();
        if(days>4){
            sfxlogic.changeBackground("RealWorld");
        }else{
            StartCoroutine(oneFrame());
            sfxlogic.changeBackground("RealWorldSoft");
        }
    }

    IEnumerator resetBlind(){
        yield return null;
        blind.SetActive(false);
    }

    IEnumerator oneFrame()
    {
        yield return null;
        GameObject.FindWithTag("trash").SetActive(false);
        blind.SetActive(false);
    }

    public void endgame(){
        SceneManager.LoadScene("endgame");
        Destroy(Player);
    }
    
    public void dream(){
        dayCounter.text = "";
        if(days>3){
            sfxlogic.changeBackground("Mirror");
        }else{
            sfxlogic.changeBackground("Reflection");
        }

        int selectedDream = SelectDreamIndex();
        switch(selectedDream){
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
                handleSnowDream();
                break;
        }
    }

    int SelectDreamIndex()
    {
        if (seenDreamCount < DreamCount)
        {
            int unseenToSkip = Random.Range(0, DreamCount - seenDreamCount);
            for (int i = 0; i < DreamCount; i++)
            {
                if (dreamSeen[i])
                {
                    continue;
                }

                if (unseenToSkip == 0)
                {
                    dreamSeen[i] = true;
                    seenDreamCount++;
                    return i;
                }

                unseenToSkip--;
            }
        }

        return Random.Range(0, DreamCount);
    }

    public void handleAnnaDream(){
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(2363,10,289));
        Player.GetComponent<FPSController>().setSpeed(50);
        SceneManager.LoadScene("AnnaDream");
    }

    public void handleGriffinDream(){
        SceneManager.LoadScene("GriffinDream");
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(0,10,0));
    }

    public void handleSpaceDream(){
        sfxlogic.changeBackground("Engine");
        SceneManager.LoadScene("Space");
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(0,10,0));
    }

    public void handleSnowDream(){
        SceneManager.LoadScene("SnowDream");
        Player.GetComponent<FPSController>().teleportPlayer(new Vector3(1215,8,1890));
    }

    public void teleportPlayer(Vector3 newpos){
        Player.GetComponent<FPSController>().teleportPlayer(newpos);
    }
}
