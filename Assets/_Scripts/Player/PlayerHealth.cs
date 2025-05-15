using System.Collections;
using _Scripts.Managers;
using UnityEngine;

namespace _Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [SerializeField]
        private int maxHealth = 5;
        public int MaxHealth => maxHealth;

        [SerializeField]
        private float damageCooldown = 1.5f;

        public int CurrentHealth { get; private set; }

        private bool _isInvulnerable;

        private void Awake()
        {
            CurrentHealth = maxHealth;
        }

        private void Start()
        {
            NotifyHealthChanged();
        }

        public void Heal(int amount)
        {
            CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
            NotifyHealthChanged();
        }

        public void TakeDamage(int amount)
        {
            if (_isInvulnerable)
                return;

            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
            NotifyHealthChanged();

            StartCoroutine(DamageCooldownCoroutine());
        }

        private IEnumerator DamageCooldownCoroutine()
        {
            _isInvulnerable = true;
            EventManager.Instance?.damageCooldownEvent.Invoke(true);

            yield return new WaitForSeconds(damageCooldown);

            _isInvulnerable = false;
            EventManager.Instance?.damageCooldownEvent.Invoke(false);
        }

        private static void NotifyHealthChanged()
        {
            EventManager.Instance?.healthChangedEvent.Invoke();
        }
    }
}
