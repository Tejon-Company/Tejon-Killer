using UnityEngine;

public class Berry : MonoBehaviour
{
    public enum BerryEffect { Heal, SpeedBoost, Shield }
    public BerryEffect effect = BerryEffect.Heal;

    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null && health.CurrentHealth > 0)
            {
                ApplyEffect(health);
                Destroy(gameObject);
            }
        }
    }

    private void ApplyEffect(PlayerHealth health)
    {
        switch (effect)
        {
            case BerryEffect.Heal:
                health.Heal(healAmount);
                break;
        }
    }
}
