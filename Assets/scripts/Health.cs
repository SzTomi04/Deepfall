using UnityEngine;

/// <summary>
/// Base Health system for any damageble actor (Player, Enemy, etc).
/// Handles damage, death state, and broadcasts events.
/// </summary>
public class Health : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float currentHealth;

    // Events
    public delegate void HealthChanged(float newHealth, float maxHealth);
    public delegate void ActorDied();
    
    public event HealthChanged OnHealthChanged;
    public event ActorDied OnDeath;

    private bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        OnDeath?.Invoke();
        Debug.Log($"{gameObject.name} died.");
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}
