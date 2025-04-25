using UnityEngine;

public class Berry : MonoBehaviour
{
    public enum BerryEffect { Heal, SpeedBoost, Shield }
    public BerryEffect effect = BerryEffect.Heal;

    [SerializeField] private int healAmount = 1;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Colisioné con: " + other.name);
        if (other.CompareTag("Player"))
        {
            Debug.Log("¡El jugador tocó la baya!");
            PlayerHealth health = other.GetComponent<PlayerHealth>();
            if (health != null)
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
