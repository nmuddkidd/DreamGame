using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Diagnostics;
using System.IO;
using System.Collections;

public class logic : MonoBehaviour
{
    private int days;
    private float timer;
    [Header("Item Interaction")]
    public Text title;
    public Text description;
    public GameObject interactionUI;
    private string interactPrompt;

    [Header("Pih game")]
    public GameObject computerMenu;
    public bool fastquit;

    [Header("TEMP calendar")]
    public GameObject calendarx; // Drag your Image prefab here in Inspector
    public Transform calendar; // Drag your Canvas or a Panel here


    
    public void SpawnImage(float x, float y)
    {
        // 1. Instantiate the prefab
        GameObject newImage = Instantiate(calendarx,calendar);

        // 2. Parent it to the Canvas (WorldPositionStays = false)
        // This ensures it uses the Canvas's local coordinate system correctly.
        newImage.transform.SetParent(calendar, false);
        RectTransform rect = newImage.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(x,y);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        timer+=Time.deltaTime;
        if (timer > 100000)
        {
            timer = 0;
        }
    }

    public void wakeup()
    {
        //SceneManager.LoadScene("SampleScene");
        days++;
        //calendar.SetActive(true);
        SpawnImage(-100+days*50,0);
    }

    void openwindows()
    {
        computerMenu.SetActive(true);
        
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

    public void interactText(string top, string body, string interaction){
        title.text = top;
        description.text = body;
        switch (interaction)
        {
            case "computer":
                computerMenu.SetActive(true);
                fastquit = false;
                break;
            default:
                interactionUI.SetActive(true);
                fastquit = true;
                break;
        }
    }

    public void disableInteractionUI(){
        interactionUI.SetActive(false);
        computerMenu.SetActive(false);
    }
}
