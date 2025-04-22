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
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(currentHealth - amount, 0);
        NotifyHealthChanged();
    }

    private void NotifyHealthChanged()
    {
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.Invoke();
        }
    }
}
