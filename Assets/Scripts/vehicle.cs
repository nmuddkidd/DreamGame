using UnityEngine;

public class vehicle : MonoBehaviour
{
    [Header("rocking")]
    public float maxSway;
    public float speed;

    private float timer;
    private Vector3 originalrot;
    private Vector3 protate;
    void Start()
    {
        originalrot = transform.eulerAngles;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > 10000){
            timer = 0;
        }
        Vector3 rotate = new Vector3(maxSway*Mathf.Cos(speed*timer),maxSway*Mathf.Sin(speed*timer),0);
        transform.eulerAngles = transform.eulerAngles + rotate - protate;
        protate = rotate;
    }
}
