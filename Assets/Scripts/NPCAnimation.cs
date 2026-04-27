using UnityEngine;

public class NPCAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Animation anim;
    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("idle");
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.isPlaying == false)
        {
            anim.Play("idle");
        }
    }
}
