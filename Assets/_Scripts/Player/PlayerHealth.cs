using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private int _maxHealth = 5;
    public int MaxHealth => _maxHealth;

    [SerializeField]
    private float damageCooldown = 1.5f; // tiempo de invulnerabilidad

    private int _currentHealth;
    public int CurrentHealth => _currentHealth;

    private bool isInvulnerable = false;
    public bool IsInvulnerable => isInvulnerable;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void Start()
    {
        NotifyHealthChanged();
    }

    public void Heal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, _maxHealth);
        NotifyHealthChanged();
    }

    public void TakeDamage(int amount)
    {
        if (isInvulnerable)
            return;

        _currentHealth = Mathf.Max(_currentHealth - amount, 0);
        NotifyHealthChanged();

        StartCoroutine(DamageCooldownCoroutine());
    }

    private IEnumerator DamageCooldownCoroutine()
    {
        isInvulnerable = true;
        if (EventManager.current != null)
            EventManager.current.damageCooldownEvent.Invoke(true);

        yield return new WaitForSeconds(damageCooldown);

        isInvulnerable = false;
        if (EventManager.current != null)
            EventManager.current.damageCooldownEvent.Invoke(false);
    }

    private void NotifyHealthChanged()
    {
        if (EventManager.current != null)
        {
            EventManager.current.healthChangedEvent.Invoke();
        }
    }
}
