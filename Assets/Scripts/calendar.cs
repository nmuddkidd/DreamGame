using UnityEngine;

public class calendar : MonoBehaviour
{
    public MeshRenderer meshRenderer;
    public Material[] days;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        logic logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<logic>();
        Material[] mats = meshRenderer.materials; 
        mats[0] = days[logic.days];
        meshRenderer.materials = mats;
        Debug.Log(logic.days);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
