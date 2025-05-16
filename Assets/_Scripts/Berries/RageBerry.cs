using _Scripts.Events;
using UnityEngine;

namespace _Scripts.Berries
{
    
    /// <summary>
    /// Representa una baya que otorga el estado de rabia al jugador cuando la recoge.
    /// </summary>
    public class RageBerry : MonoBehaviour
    {
        [SerializeField]
        private float playerBaseSpeedMultiplier = 2f;

        [SerializeField]
        private float playerJumpForceMultiplier = 2f;

        [SerializeField]
        private float weaponFireRateMultiplier = 0.5f;

        [SerializeField]
        private float rageDuration = 7f;

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player"))
                return;

            EventManager.Instance.rageBerryEvent.Invoke(
                playerBaseSpeedMultiplier,
                playerJumpForceMultiplier,
                weaponFireRateMultiplier,
                rageDuration
            );
            Destroy(gameObject);
        }
    }
}
