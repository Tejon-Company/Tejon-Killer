using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private int maxHealth = 20;
    private int currentHealth;

    [SerializeField]
    private GameObject deathEffect;

    [SerializeField]
    private GameObject damageEffect;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        GetComponent<Squirrel>()?.FlashRed(); 

        if (damageEffect != null && currentHealth != 0)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
