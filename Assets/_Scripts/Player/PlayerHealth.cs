using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth = 5;
    public int MaxHealth
    {
        get => _maxHealth;
    }

    private int _currentHealth;
    public int CurrentHealth
    {
        get => _currentHealth;
    }

    private void Awake()
    {
        _currentHealth = _maxHealth;
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
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
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
