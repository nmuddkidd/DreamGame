using UnityEngine;

public class attractor : MonoBehaviour
{
    private GameObject player;
    public int Speed;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        transform.LookAt(player.transform);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = transform.position + Speed * Time.deltaTime * transform.forward;
    }
}
