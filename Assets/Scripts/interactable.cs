using UnityEngine;

public class interactable : MonoBehaviour
{  
    [Header("Inputs")]
    public string title;
    [Multiline]
    public string description;
    public string[] response;
    public string interaction;

    private Vector3 origin;
    private Quaternion rotorigin;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        origin = transform.position;
        rotorigin = transform.rotation;
    }

    public void reorigin(){
        transform.position = origin;
        transform.rotation = rotorigin;
    }
}
