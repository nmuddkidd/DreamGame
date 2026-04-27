using UnityEngine;

public class PillShooter : MonoBehaviour
{
    [Header("Settings")]
    public GameObject pillPrefab;
    public float shootForce = 30f;
    public float fireRate = 0.5f;
    private float nextFireTime;

    [Header("References")]
    private Camera playerCamera;
    public Transform spawnPoint; 

    void Start()
    {
        // use player camera
        playerCamera = GetComponentInChildren<Camera>();

        if (playerCamera == null)
        {
            Debug.LogError("no camera found");
        }
    }

    void Update()
    {
        // shoot on right click
        if (Input.GetMouseButtonDown(1) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;

            Debug.Log("right click detected"); 
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (pillPrefab == null || spawnPoint == null) return;

        GameObject pill = Instantiate(pillPrefab, spawnPoint.position, spawnPoint.rotation);
        Rigidbody rb = pill.GetComponent<Rigidbody>();

        if (rb != null)
        {
            //shoot where camera looking
            rb.AddForce(playerCamera.transform.forward * shootForce, ForceMode.Impulse);
        }

        Destroy(pill, 5f); 
    }
}