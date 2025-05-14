using UnityEngine;

namespace _Scripts.Enemies
{
    public class EnemyHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth = 20;

        [SerializeField]
        private GameObject deathEffect;

        [SerializeField]
        private GameObject damageEffect;

        private int currentHealth;

        private void Start() => currentHealth = maxHealth;

        public void TakeDamage(int amount)
        {
            currentHealth -= amount;
            Debug.Log($"{gameObject.name} ha recibido {amount} de daño. Vida restante: {currentHealth}");

            GetComponent<Enemy>()?.FlashRed();

            if (damageEffect && currentHealth > 0)
                Instantiate(damageEffect, transform.position, Quaternion.identity);

            if (currentHealth > 0)
                return;

            if (deathEffect)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}