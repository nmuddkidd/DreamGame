using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class interact_pt2 : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    private bool picked = false;
    public int barcode = 0;

    void Update()
    {
        //print(CustomEvents.current.currentMode);
        if (picked && Mouse.current.leftButton.wasPressedThisFrame)
        {
            //have to be able to reset when raycast is not on the object and is in hand
            CustomEvents.current.currentMode = CustomEvents.InputMode.Absent;
            CustomEvents.current.OnPickUp(this.gameObject);
            picked = false;
        }

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (CustomEvents.current.currentMode == CustomEvents.InputMode.Absent)
        {
            //switch the mode and start up the function 
            CustomEvents.current.currentMode = CustomEvents.InputMode.Interact;
            CustomEvents.current.OnPickUp(this.gameObject);
            picked = true;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {

        //light it up a little 
        if (eventData.reentered)
        {
            return;
        }
        else
        {
            this.gameObject.GetComponent<Renderer>().material.EnableKeyword("_EMISSION");
            this.gameObject.GetComponent<Renderer>().material.SetColor("_EmissionColor", Color.blue * 2f);
        }
        
    }


    public void OnPointerExit(PointerEventData eventData)
    {
        //turn off the light
        if (eventData.fullyExited)
        {
            this.GetComponent<Renderer>().material.DisableKeyword("_EMISSION");
        }
    }


}
