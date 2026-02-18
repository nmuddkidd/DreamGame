using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;

    public float Xsens = 1f;
    public float Ysens = 1f;

    public float jumpvelocity = 20f;
    public float horizdamping = 10f;

    private Rigidbody body;
    private float yaw = 0;
    private float pitch = 0;
    void Start()
    {
        body = GetComponent<Rigidbody>();
    }

    void Update()
    {
        movement();
    }

    void movement()
    {
        //rotation of playerbody
        pitch += Input.GetAxis("Mouse Y") * Ysens * -1;
        yaw += Input.GetAxis("Mouse X") * Xsens;
        transform.rotation = Quaternion.Euler(pitch,yaw,0);

        //getting rotation for controls
        float xrotation = transform.eulerAngles.y * Mathf.PI/180;

        //damping horizontal movement
        body.linearVelocity = new Vector3(body.linearVelocity.x - body.linearVelocity.x*Time.deltaTime / horizdamping ,body.linearVelocity.y,body.linearVelocity.z - body.linearVelocity.z * Time.deltaTime / horizdamping);

        //Controls
        if (Input.GetKey(KeyCode.W))
        {
            body.linearVelocity = new Vector3(moveSpeed * Mathf.Sin(xrotation), body.linearVelocity.y, moveSpeed * Mathf.Cos(xrotation));
        }
        else if (Input.GetKey(KeyCode.S))
        {
            body.linearVelocity = new Vector3(-1 * moveSpeed * Mathf.Sin(xrotation), body.linearVelocity.y, -1 * moveSpeed * Mathf.Cos(xrotation));
        }
        if (Input.GetKey(KeyCode.A))
        {
            body.linearVelocity = new Vector3(-1 * moveSpeed * Mathf.Cos(xrotation), body.linearVelocity.y, moveSpeed * Mathf.Sin(xrotation));
        }
        else if (Input.GetKey(KeyCode.D))
        {
            body.linearVelocity = new Vector3(moveSpeed * Mathf.Cos(xrotation), body.linearVelocity.y, -1 * moveSpeed * Mathf.Sin(xrotation));
        }

        if(Input.GetKey(KeyCode.Space)){//&&body.linearVelocity.y == 0){
            body.linearVelocity = new Vector3(body.linearVelocity.x, jumpvelocity , body.linearVelocity.z);
        }
    }
}