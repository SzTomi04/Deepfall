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

            // KIZÁRÓLAG akkor csinál bármit, ha a játékos elég közel van
            if (distanceToPlayer <= detectionRange)
            {
                // 1. LÉPÉS: FOLYAMATOS FORGÁS A JÁTÉKOS FELÉ
                float directionX = player.position.x - transform.position.x;
                
                // Csak akkor próbálunk megfordulni, ha nem pont egy pixelnyire vannak egymáson
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

                // 2. LÉPÉS: CÉLZÁS ÉS LÖVÉS
                TryShootAtPlayer();
            }
        }
    }

    private void TryShootAtPlayer()
    {
        if (shootTimer < shootInterval)
            return;

        shootTimer = 0f;

        if (projectilePrefab == null || firePoint == null)
        {
            if (!warnedMissingProjectile && projectilePrefab == null)
            {
                warnedMissingProjectile = true;
                Debug.LogWarning(name + " has no projectilePrefab assigned.");
            }
            return;
        }

        if (player == null) return;

        Vector2 exactAimDirection = (player.position - firePoint.position).normalized;
        float sign = Mathf.Sign(player.position.x - transform.position.x);

        if (anim != null)
        {
            anim.SetTrigger("attack"); 
        }

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
            shotComp.SetDirection(sign, transform);
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

    // A sebződés most már csak a sebződéssel (és a halállal) foglalkozik, semmi mással!
    private void ProcessDamage(int damage, Vector3 sourcePosition)
    {
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