using UnityEngine;

public class PlantBedWatcher : MonoBehaviour
{
    public Transform bedTransform;
    public float bedLowerAmount = 5f;
    public float checkInterval = 0.5f;
    public bool lowerBedWhilePlantsExist = true;

    private Vector3 bedStartPosition;
    private bool bedIsLowered = false;
    private float nextCheckTime;

    void Start()
    {
        if (bedTransform == null)
        {
            Debug.LogWarning("PlantBedWatcher: Bed Transform is not assigned.");
            enabled = false;
            return;
        }

        bedStartPosition = bedTransform.position;
        UpdateBedState();
    }

    void Update()
    {
        if (Time.time < nextCheckTime)
        {
            return;
        }

        nextCheckTime = Time.time + checkInterval;
        UpdateBedState();
    }

    void UpdateBedState()
    {
        bool hasLivingPlants = Object.FindObjectsByType<PlantAi>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).Length > 0;

        if (hasLivingPlants)
        {
            if (lowerBedWhilePlantsExist)
            {
                LowerBed();
            }
        }
        else
        {
            RaiseBed();
        }
    }

    void LowerBed()
    {
        if (bedIsLowered)
        {
            return;
        }

        bedTransform.position = bedStartPosition + Vector3.down * bedLowerAmount;
        bedIsLowered = true;
    }

    void RaiseBed()
    {
        if (!bedIsLowered)
        {
            return;
        }

        bedTransform.position = bedStartPosition;
        bedIsLowered = false;
    }
}
