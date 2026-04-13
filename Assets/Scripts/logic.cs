using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;

public class logic : MonoBehaviour
{
    private int days;
    private float timer;
    [Header("Item Interaction")]
    public Text title;
    public Text description;
    public GameObject interractionUI;
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

    public void interactText(string top, string body){
        title.text = top;
        description.text = body;
    }

    public void interactionUI(bool option){
        interractionUI.SetActive(option);
    }
}
