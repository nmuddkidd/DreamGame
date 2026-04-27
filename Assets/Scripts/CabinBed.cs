using UnityEngine;

public class CabinBed : MonoBehaviour
{
    void OnMouseDown()
    {
        GameObject logicObject = GameObject.FindGameObjectWithTag("Logic");
        if (logicObject == null)
        {
            Debug.LogWarning("CabinBed: No object with tag 'Logic' found.");
            return;
        }

        logic logicScript = logicObject.GetComponent<logic>();
        if (logicScript == null)
        {
            Debug.LogWarning("CabinBed: Object tagged 'Logic' is missing the logic component.");
            return;
        }

        logicScript.teleportPlayer(new Vector3(-14, 4, -7));
        logicScript.wakeup();
    }
}
