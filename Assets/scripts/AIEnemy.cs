using System.Collections;
using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
   
    [Header("Combat")]
    public GameObject projectilePrefab; 
    public Transform firePoint;
    public float shootInterval = 1.5f;
    public float detectionRange = 8f; 
    public float bulletSpeed = 10f; 
    
    private float shootTimer = 0f;

    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth;

    private Transform player;
    private Animator anim;
    private Rigidbody2D rb;
    private bool warnedMissingProjectile;
    
    private bool isDead = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        collider.isTrigger = false;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

        if (firePoint == null)
        {
            GameObject firePointObject = new GameObject(name + "_firePoint");
            firePointObject.transform.SetParent(transform);
            firePointObject.transform.localPosition = new Vector3(0.5f, 0.2f, 0f);
            firePoint = firePointObject.transform;
        }

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        if (leftPoint == null || rightPoint == null)
        {
            leftPoint = new GameObject(name + "_left").transform;
            rightPoint = new GameObject(name + "_right").transform;
            leftPoint.position = transform.position + Vector3.left * 3f;
            rightPoint.position = transform.position + Vector3.right * 3f;
        }
    }

    private void Update()
    {
        if (isDead) return;

        shootTimer += Time.deltaTime;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null)
                player = p.transform;
        }

        if (player != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.position);

            if (distanceToPlayer <= detectionRange)
            {
                float directionX = player.position.x - transform.position.x;
                
                if (Mathf.Abs(directionX) > 0.1f)
                {
                    float sign = Mathf.Sign(directionX);
                    Vector3 scale = transform.localScale;
                    
                    if (Mathf.Sign(scale.x) != sign)
                    {
                        scale.x = Mathf.Abs(scale.x) * sign;
                        transform.localScale = scale;
                    }
                }

                TryShootAtPlayer();
            }
        }
    }

    private void TryShootAtPlayer()
    {
        if (isDead) return;

        if (shootTimer < shootInterval)
            return;

        shootTimer = 0f;

        if (projectilePrefab == null || firePoint == null || player == null)
            return;

        if (anim != null)
        {
            anim.SetTrigger("attack"); 
        }

        // 1. Kiszámoljuk a pontos célzási irányt a játékos felé
        Vector2 exactAimDirection = (player.position - firePoint.position).normalized;

        // 2. Kiszámoljuk a forgatási szöget és elforgatjuk a golyót
        float angle = Mathf.Atan2(exactAimDirection.y, exactAimDirection.x) * Mathf.Rad2Deg;
        Quaternion bulletRotation = Quaternion.Euler(new Vector3(0f, 0f, angle));

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, bulletRotation);
        if (proj == null)
            return;
        
        var shotComp = proj.GetComponent<shot>();
        var rbody = proj.GetComponent<Rigidbody2D>();
        
        if (rbody != null)
        {
            rbody.linearVelocity = exactAimDirection * bulletSpeed;
        }
        
        if (shotComp != null)
        {
            // A JAVÍTÁS ITT VAN: Mivel a golyót már ráirányítottuk a játékosra, 
            // a shot szkriptnek mindig 1-est (előre) adunk át, hogy abba az irányba induljon meg!
            shotComp.SetDirection(1f, transform);
        }
    }

    public void TakeDamage(int damage)
    {
        Vector3 source = player != null ? player.position : transform.position;
        ProcessDamage(damage, source);
    }

    public void TakeDamage(int damage, Vector3 damageSourcePosition)
    {
        ProcessDamage(damage, damageSourcePosition);
    }

    private void ProcessDamage(int damage, Vector3 sourcePosition)
    {
        if (isDead) return;

        currentHealth -= damage;
        Debug.Log(name + " HIT for " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        if (anim != null)
        {
            anim.SetTrigger("hit");
        }

        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        this.enabled = false;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        if (leftPoint != null) Gizmos.DrawSphere(leftPoint.position, 0.1f);
        if (rightPoint != null) Gizmos.DrawSphere(rightPoint.position, 0.1f);

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}