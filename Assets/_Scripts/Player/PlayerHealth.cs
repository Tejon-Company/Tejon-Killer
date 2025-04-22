using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 5;
    public int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth - 1;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Heal(1);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            TakeDamage(1);
        }
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        NotifyHealthChanged();
        Debug.Log("Curado, vida actual: " + currentHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        NotifyHealthChanged();
        Debug.Log("Daño recibido, vida actual: " + currentHealth);
    }

    private void NotifyHealthChanged()
    {
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.Invoke();
        }
        else
        {
            Debug.LogWarning("EventManager no encontrado al intentar notificar cambio de vida.");
        }
    }
}
