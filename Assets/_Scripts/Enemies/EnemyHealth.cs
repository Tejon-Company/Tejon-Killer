using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 20;
    private int currentHealth;

    [SerializeField] private GameObject deathEffect;
    [SerializeField] private GameObject damageEffect;

    private IDamageableVisual damageableVisual;

    private void Start()
    {
        currentHealth = maxHealth;
        damageableVisual = GetComponent<IDamageableVisual>();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        damageableVisual?.FlashRed();

        if (damageEffect != null && currentHealth > 0)
        {
            Instantiate(damageEffect, transform.position, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            damageableVisual?.Die();

            if (deathEffect != null)
            {
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject); 
        }
    }
}