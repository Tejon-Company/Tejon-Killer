using _Scripts.Events;
using UnityEngine;

namespace _Scripts.Enemies
{
    
    /// <summary>
    /// Gestiona la salud de los enemigos, procesando el daño recibido,
    /// mostrando efectos visuales y notificando cuando el enemigo muere.
    /// </summary>
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

            GetComponent<Enemy>()?.FlashRed();

            if (damageEffect && currentHealth > 0)
                Instantiate(damageEffect, transform.position, Quaternion.identity);

            if (currentHealth > 0)
                return;

            if (deathEffect)
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);

            if (EventManager.Instance && EventManager.Instance.enemyDiedEvent != null)
                EventManager.Instance.enemyDiedEvent.Invoke();
        }
    }
}
