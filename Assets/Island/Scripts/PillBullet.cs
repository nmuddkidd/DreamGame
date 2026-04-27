using UnityEngine;

public class PillBullet : MonoBehaviour
{
    public float damageAmount = 20f;

    void OnTriggerEnter(Collider other)
    {
        //check if enemy hit
        if (other.CompareTag("Enemy"))
        {
            Health enemyHealth = other.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damageAmount);
                Debug.Log("Pill hit the spider!");
            }

            //destroy pill after hit
            Destroy(gameObject);
        }
    }
}
