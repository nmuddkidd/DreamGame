using UnityEngine;

public class MonsterAnimator : MonoBehaviour
{
    [SerializeField] private float chaseSpeed = 3.6f;
    [SerializeField] private float catchDistance = 1.1f;

    private Animation anim;
    private Transform chaseTarget;
    private bool isChasing;

    void Start()
    {
        anim = GetComponent<Animation>();
        anim.Play("monsterIdle");
    }

    public void BeginChase(Transform target)
    {
        chaseTarget = target;
        isChasing = target != null;
        if (anim != null)
        {
            anim.Play("monsterWalk");
        }
    }

    public void StopChase()
    {
        isChasing = false;
        chaseTarget = null;
        if (anim != null)
        {
            anim.Play("monsterIdle");
        }
    }

    void Update()
    {
        if (!isChasing || chaseTarget == null)
        {
            if (anim != null && anim.IsPlaying("monsterWalk"))
            {
                anim.Play("monsterIdle");
            }
            return;
        }

        Vector3 chasePosition = new Vector3(chaseTarget.position.x, transform.position.y, chaseTarget.position.z);
        transform.position = Vector3.MoveTowards(transform.position, chasePosition, chaseSpeed * Time.deltaTime);

        Vector3 lookDirection = chasePosition - transform.position;
        if (lookDirection.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(lookDirection.normalized, Vector3.up);
        }

        if (anim != null && !anim.IsPlaying("monsterWalk"))
        {
            anim.Play("monsterWalk");
        }

        if (Vector3.Distance(transform.position, chaseTarget.position) <= catchDistance)
        {
            CatchPlayer();
        }
    }

    private void CatchPlayer()
    {
        isChasing = false;
        chaseTarget = null;
        TriggerWakeupSequence();
    }

    public void TriggerWakeupSequence()
    {
        GameObject logicObject = GameObject.FindGameObjectWithTag("Logic");
        if (logicObject == null)
        {
            return;
        }

        logic logicScript = logicObject.GetComponent<logic>();
        if (logicScript == null)
        {
            return;
        }

        logicScript.teleportPlayer(new Vector3(-14, 4, -7));
        logicScript.wakeup();
    }
}
