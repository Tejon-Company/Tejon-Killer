using _Scripts.Managers;
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

        private int _currentHealth;

        private void Start() => _currentHealth = maxHealth;

        public void TakeDamage(int amount)
        {
            _currentHealth -= amount;

            GetComponent<Enemy>()?.FlashRed();

            if (damageEffect && _currentHealth > 0)
                Instantiate(damageEffect, transform.position, Quaternion.identity);

            if (_currentHealth > 0)
                return;

            if (deathEffect)
                Instantiate(deathEffect, transform.position, Quaternion.identity);
            Destroy(gameObject);

            if (EventManager.Instance != null && EventManager.Instance.enemyDiedEvent != null)
                EventManager.Instance.enemyDiedEvent.Invoke();
        }
    }
}
