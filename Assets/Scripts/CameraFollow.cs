using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 15.0f;
    public float height = 5.0f;
    public float smoothSpeed = 5f;

    public float mouseSensitivity = 1f;
    private float currentRotationAngle = 0f;
    private float currentHeightAngle = 20f;
    void LateUpdate()
    {
        if (target == null) return;

        currentRotationAngle += Input.GetAxis("Mouse X") * mouseSensitivity;
        currentHeightAngle -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        currentHeightAngle = Mathf.Clamp(currentHeightAngle, 5f, 60f);

        // calculate rotation
        Quaternion rotation = Quaternion.Euler(currentHeightAngle, currentRotationAngle, 0);

        // calculate position
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
        Vector3 position = rotation * negDistance + target.position + (Vector3.up * height);

        // smooth camera movement + look at target
        transform.position = Vector3.Lerp(transform.position, position, smoothSpeed * Time.deltaTime);
        transform.LookAt(target.position + Vector3.up * 2f);
    }

    // BoatExitEnter can call this to change camera target 
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }
}
