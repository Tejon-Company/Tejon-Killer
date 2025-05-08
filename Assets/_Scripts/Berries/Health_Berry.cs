using UnityEngine;

public class Berry : MonoBehaviour
{
    [SerializeField]
    private int healAmount = 1;

    private PlayerHealth health;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            health = other.GetComponent<PlayerHealth>();
            if (health != null && health.CurrentHealth > 0)
            {
                health.Heal(healAmount);
                Destroy(gameObject);
            }
        }
    }
}
