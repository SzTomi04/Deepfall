using UnityEngine;

/// <summary>
/// Centralized player statistics and state.
/// Use this so PlayerManager, UI, and other systems query player data from one place.
/// </summary>
public class PlayerStats : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float jumpForce = 12f;
    [SerializeField] private int attackDamage = 20;
    [SerializeField] private float attackCooldown = 0.5f;
    private float lastAttackTime = 0f;

    public float GetMoveSpeed() => moveSpeed;
    public void SetMoveSpeed(float speed) => moveSpeed = speed;

    public float GetJumpForce() => jumpForce;
    public void SetJumpForce(float force) => jumpForce = force;

    public int GetAttackDamage() => attackDamage;
    public void SetAttackDamage(int damage) => attackDamage = damage;

    public float GetAttackCooldown() => attackCooldown;
    public void SetAttackCooldown(float cooldown) => attackCooldown = cooldown;

    public bool CanAttack()
    {
        return Time.time >= lastAttackTime + attackCooldown;
    }

    public void ResetAttackCooldown()
    {
        lastAttackTime = Time.time;
    }
}
