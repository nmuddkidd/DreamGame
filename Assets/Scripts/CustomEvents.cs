using System;
using UnityEngine;

public class CustomEvents : MonoBehaviour
{
    public static CustomEvents current;


    public enum InputMode
    {
        Interact,
        Absent
    }

    public InputMode currentMode;

    private void Awake()
    {
        current = this;
        currentMode = InputMode.Absent;// defaults input mode to placement for ship placing 

    }

    public event Action<GameObject> PickUp;

    public void OnPickUp(GameObject other)
    {
        PickUp?.Invoke(other);
    }
}
