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

        private PlayerHealth health;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            health = other.GetComponent<PlayerHealth>();

            if (!health || health.CurrentHealth <= 0)
                return;

            health.Heal(healAmount);

            Destroy(gameObject);
        }
    }
}
