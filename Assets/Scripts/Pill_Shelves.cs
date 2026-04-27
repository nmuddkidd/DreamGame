using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pill_Shelves : MonoBehaviour
{
    [SerializeField] private LayerMask layer;
    [SerializeField] private GameObject chips;
    [SerializeField] private GameObject evil;
    [SerializeField] private GameObject soda;
    [SerializeField] private GameObject gummi;
    [SerializeField] private GameObject crispy;
    [SerializeField] private GameObject greese;
    [SerializeField] private GameObject pills;


    private Vector3 box_pos = new Vector3((float)-9.5, 2, 8);
    private Vector3 box_dim = new Vector3(3, 2, 4);
    public Collider[] Shelves = { };
    private GameObject[] Items = { };
   // private bool flip;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        List<GameObject> validItems = new List<GameObject>();
        GameObject[] configuredItems = new GameObject[] { chips, evil, soda, gummi, crispy, greese };
        foreach (GameObject configuredItem in configuredItems)
        {
            if (configuredItem != null)
            {
                validItems.Add(configuredItem);
            }
        }

        Items = validItems.ToArray();
        if (Items.Length == 0)
        {
            Debug.LogError("Pill_Shelves: no item prefabs configured.");
            return;
        }

        Shelves = Physics.OverlapBox(box_pos, box_dim, Quaternion.Euler(Vector3.zero), layer);
        for (int i = 0; i < Shelves.Length; i++)
        { 
            if (Shelves[i] == null)
            {
                continue;
            }

            GameObject temp = Shelves[i].gameObject;
            int select = Random.Range(0, Items.Length);
            GameObject baby = Instantiate(Items[select], temp.gameObject.transform.position, temp.gameObject.transform.rotation);
            Transform parentTransform = temp.transform.parent != null ? temp.transform.parent : transform;
            baby.transform.SetParent(parentTransform, true);
            Vector3 pos = baby.transform.position;
            Vector3 rot = transform.rotation.eulerAngles;
          /*  if (temp.transform.parent.gameObject.name == "Aisle 2" || temp.transform.parent.gameObject.name == "Aisle 4")
            {
                flip = true;
            }
            else
            {
                flip = false;
            } */
            switch (select)
            {
                case 0:
                    rot.z = 90;
                    break;
                case 1:
                    rot.x = -180;
                    break;
                case 2:
                    pos.y = pos.y - (float)0.15;
                    rot = Vector3.zero;
                    break;
                case 3:
                    pos.y = pos.y - (float)0.1;
                    rot.z = -140;
                    break;
                case 4:
                    pos.y = pos.y - (float)0.05;
                    rot.x = 180;
                    rot.z = 90;
                    break;
                case 5:
                    pos.y = pos.y - (float)0.17;
                    break;
            }
            baby.transform.rotation = Quaternion.Euler(rot);
            baby.transform.position = pos;
            Shelves[i] = baby.GetComponent<Collider>();
            Destroy(temp.gameObject);
        }
        if (Restock.current != null)
        {
            Restock.current.PillsTaken += Restock_Shelves;
        }
        else
        {
            Debug.LogWarning("Pill_Shelves: Restock.current was null in Start; shelves will not auto-refill.");
        }
    }

    void Restock_Shelves(GameObject other)
    {
        if (other == null)
        {
            return;
        }

        for(int i = 0; i < Shelves.Length; i++)
        {
            if (Shelves[i] != null && other.name == Shelves[i].gameObject.name)
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
        /*if (Random.Range(1,101) < (Time.deltaTime / 2))
        {

        }   */  
        other.GetComponent<Collider>().enabled = true;
        other.GetComponent<MeshRenderer>().enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
                      
    }
}



