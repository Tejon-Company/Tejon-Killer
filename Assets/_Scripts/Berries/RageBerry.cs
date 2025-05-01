using UnityEngine;

public class RageBerry : MonoBehaviour
{
    [SerializeField] private float playerBaseSpeedMultiplier = 2f;
    [SerializeField] float playerJumpForceMultiplier=2f;
    [SerializeField] private float weaponFireRateMultiplier = 0.5f;
    [SerializeField] private float rageDuration = 7f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            EventManager.current.rageBerryEvent.Invoke(playerBaseSpeedMultiplier, playerJumpForceMultiplier, weaponFireRateMultiplier, rageDuration);
            Destroy(gameObject);
        }
    }

}
