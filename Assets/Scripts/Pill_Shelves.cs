using System.Collections;
using UnityEngine;

public class Pill_Shelves : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    private Vector3 box_pos = new Vector3((float)-9.5, 2, 8);
    private Vector3 box_dim = new Vector3(3, 2, 4);
    public Collider[] Shelves = { };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Shelves = Physics.OverlapBox(box_pos, box_dim, Quaternion.Euler(Vector3.zero), layer);
        Restock.current.PillsTaken += Restock_Shelves;
    }

    void Restock_Shelves(GameObject other)
    {
        for(int i = 0; i < Shelves.Length; i++)
        {
            if (other.name == Shelves[i].gameObject.name)
            {
                StartCoroutine(Refill(other));
               // Debug.Log(other.name);
            }
        }

    }

    //wait for the automatic restock, not immediate 
    IEnumerator Refill(GameObject other)
    {
        yield return new WaitForSeconds(15f);
        other.GetComponent<Collider>().enabled = true;
        other.GetComponent<MeshRenderer>().enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
                      
    }
}
