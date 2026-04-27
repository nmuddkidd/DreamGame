using UnityEngine;

public class PlantAi : MonoBehaviour
{
    public float attackRange = 8f;
    public float damage = 20f;
    public float attackRate = 2f;

    private Transform player;
    private Animator anim;
    private float nextAttackTime;

    void Start()
    {
        anim = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);
        if (distance < 15f) Debug.Log("Distance to player: " + distance);
        //attack when player near after cooldown
        if (distance <= attackRange && Time.time >= nextAttackTime)
        {
            AttackPlayer();
            nextAttackTime = Time.time + attackRate;
        }
    }

    void AttackPlayer()
    {
        if (anim != null) anim.SetTrigger("Attack");

        //health damage
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(damage);
            Debug.Log("Plant attacked the player!");
        }
    }
}
