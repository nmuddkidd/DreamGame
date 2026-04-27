using UnityEngine;

public class Health : MonoBehaviour
{
    public float maxHealth = 100f;
    private float currentHealth;
    public bool isPlayer = false;
    private Animator anim;

    void Start()
    {
        currentHealth = maxHealth;
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void TakeDamage(float damage)

    {
        currentHealth -= damage;
        if (anim != null)
        {
            anim.SetTrigger("Take Damage");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        if (isPlayer)
        {
            Debug.Log("Player Died. Waking up in apartment..");

            UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
        }
        else
        {
            if (CompareTag("Enemy"))
            {
                SpiderManager manager = Object.FindFirstObjectByType<SpiderManager>();
                if (manager != null) manager.NotifyDeath(transform.position);

            }
            Destroy(gameObject);
        }
    }
}