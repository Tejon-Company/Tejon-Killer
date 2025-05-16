using System;
using UnityEngine;
using UnityEngine.Events;

namespace _Scripts.Events
{
    [Serializable]
    public class Int2Event : UnityEvent<int, int> { }

    [Serializable]
    public class RageEvent : UnityEvent<float, float, float, float> { }

    
    /// <summary>
    /// Gestor centralizado de eventos del juego. Implementa el patrón Singleton para permitir
    /// la comunicación entre diferentes componentes sin acoplamiento directo.
    /// </summary>
    public class EventManager : MonoBehaviour
    {
        public static EventManager Instance;

        private void Awake()
        {
            if (Instance is not null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public UnityEvent enemyDiedEvent = new();
        public Int2Event updateBulletsEvent = new();
        public UnityEvent newGunEvent = new();
        public UnityEvent healthChangedEvent = new();
        public RageEvent rageBerryEvent = new();
        public UnityEvent<bool> damageCooldownEvent;
        public UnityEvent allEnemiesDefeated = new();
    }
}
