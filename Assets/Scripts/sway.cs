using UnityEngine;

public class sway : MonoBehaviour
{
    public float speed;
    public float maxSway;
    private Vector3 origin;
    private float timer;
    void Start()
    {
        origin = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer>10000){
            timer = 0;
        }
        Vector3 newpos = origin + new Vector3(maxSway*Mathf.Cos(speed*timer),maxSway*Mathf.Sin(speed*timer),0);
        transform.position = newpos;
    }
}
