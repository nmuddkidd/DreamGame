using System;
using UnityEngine;

public class Restock : MonoBehaviour
{
    public static Restock current;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        current = this;
    }

    public event Action<GameObject> PillsTaken;
    public void OnPillsTaken(GameObject other)
    {
        PillsTaken?.Invoke(other);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
