using UnityEngine;

public class NewMonoBehaviourScript : MonoBehaviour
{
    void OnCollisionEnter(Collision collision) {
        // 'collision' contains data like contact points and impact velocity
        Debug.Log(collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            logic logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<logic>();
            logic.teleportPlayer(new Vector3(-14,4,-7));
            logic.wakeup();
        }
    }
}
