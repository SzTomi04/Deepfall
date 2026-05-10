using UnityEngine;

/// <summary>
/// Configurable template for different enemy types.
/// Attach to an AI enemy and tweak values; no code changes needed.
/// </summary>
[System.Serializable]
public class EnemySettings
{
    [Header("Movement")]
    public float speed = 2f;
    public float patrolDistance = 3f;

    [Header("Combat")]
    public int maxHealth = 50;
    public int attackDamage = 10;
    public float shootInterval = 1.5f;
    public float detectionRange = 8f;

    [Header("Behavior")]
    public bool canShoot = true;
    public bool canChase = true;
}

/// <summary>
/// Helper script to apply EnemySettings to an AIEnemy.
/// </summary>
public class EnemySettingsApplier : MonoBehaviour
{
    public EnemySettings settings = new EnemySettings();

    private void Start()
    {
        ApplySettings();
    }

    public void ApplySettings()
    {
        AIEnemy ai = GetComponent<AIEnemy>();
        if (ai != null)
        {
            ai.speed = settings.speed;
            ai.detectionRange = settings.detectionRange;
            ai.shootInterval = settings.shootInterval;
        }

        Health health = GetComponent<Health>();
        if (health != null)
        {
            // Health initialization via inspector is better, but you can log here
            Debug.Log($"Enemy {gameObject.name} configured with {settings.maxHealth} HP, {settings.attackDamage} DMG.");
        }
    }

    // Context menu for debugging
    [ContextMenu("Apply Settings")]
    public void ApplySettingsFromContext()
    {
        ApplySettings();
    }
}
