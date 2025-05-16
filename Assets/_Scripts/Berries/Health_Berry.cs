using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Berries
{
    /// <summary>
    /// Clase que representa una baya de vida que restaura salud al jugador cuando entra en contacto con ella.
    /// </summary>
    public class Berry : MonoBehaviour
    {
        [SerializeField]
        private int healAmount = 1;

        private PlayerHealth _health;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            _health = other.GetComponent<PlayerHealth>();

            if (!_health || _health.CurrentHealth <= 0)
                return;

            _health.Heal(healAmount);

            Destroy(gameObject);
        }
    }
}
