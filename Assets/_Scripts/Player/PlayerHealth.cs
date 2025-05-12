using _Scripts.Managers;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Scripts.Player
{
    public class PlayerHealth : MonoBehaviour
    {
        [FormerlySerializedAs("_maxHealth")]
        [SerializeField]
        private int maxHealth = 5;
        public int MaxHealth => maxHealth;

        public int CurrentHealth { get; private set; }

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
            CurrentHealth = Mathf.Max(CurrentHealth - amount, 0);
            NotifyHealthChanged();
        }

        private static void NotifyHealthChanged()
        {
            EventManager.Current?.healthChangedEvent.Invoke();
        }
    }
}
