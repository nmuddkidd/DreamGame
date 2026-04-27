using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Animation anim;
    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("monsterIdle");
    }

    // Update is called once per frame
    void Update()
    {
        if (anim.isPlaying == false)
        {
            anim.Play("monsterIdle");
        }
    }
}
