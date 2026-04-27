using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isPlayer = false;
    public bool hasDeathAnimation = true;

    
    public string hitTriggerName = "Hit";

    private Animator anim;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;

       
        if (anim != null)
        {
            anim.SetTrigger(hitTriggerName);
        }

        if (currentHealth <= 0) Die();
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        if (isPlayer)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        else
        {
            if (CompareTag("Enemy"))
            {
                SpiderManager manager = Object.FindFirstObjectByType<SpiderManager>();
                if (manager != null) manager.NotifyDeath(transform.position);
            }

            if (hasDeathAnimation && anim != null)
            {
                anim.SetTrigger("Die");
                Destroy(gameObject, 2f);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public bool IsDead() { return isDead; }
}