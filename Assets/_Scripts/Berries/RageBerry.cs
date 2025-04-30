using UnityEngine;

public class RageBerry : MonoBehaviour
{
    [SerializeField] private float playerSpeedMultiplier = 2f;  
    [SerializeField] private float weaponFireRateMultiplier = 0.5f; 
    [SerializeField] private float rageDuration = 7f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ApplyEffect(other);
            Destroy(gameObject);
        }
    }

    private void ApplyEffect(Collider player)
    {
        EventManager.current.rageBerryEvent.Invoke(playerSpeedMultiplier, weaponFireRateMultiplier, rageDuration);
    }
}
