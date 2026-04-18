using UnityEngine;

public class BoatController : MonoBehaviour
{
    public float speed = 100f;
    public float turnSpeed = 50f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //keys
        float moveVertical = Input.GetAxis("Vertical");
        float moveHorizontal = Input.GetAxis("Horizontal");

        //move according to player input
        rb.AddRelativeForce(Vector3.forward * moveVertical * speed);
        rb.AddRelativeTorque(Vector3.up * moveHorizontal * turnSpeed);
    }
}
