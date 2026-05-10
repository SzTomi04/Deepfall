using UnityEngine;

public class AIEnemy : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;
    private bool movingRight = true;

    [Header("Combat")]
    public GameObject projectilePrefab; // should be the prefab with `shot` component
    public Transform firePoint;
    public float shootInterval = 1.5f;
    public float detectionRange = 8f;
    private float shootTimer = 0f;

    [Header("Health")]
    public int maxHealth = 50;
    public int currentHealth;

    private Transform player;
    private bool hasBeenHit = false;
    private Animator anim;
    private bool isPatrolling = true;
    private Rigidbody2D rb;

    private void Start()
    {
        anim = GetComponent<Animator>();
        currentHealth = maxHealth;

        // Ensure AI has a collider for grounding and shot detection
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
            Debug.Log("Added BoxCollider2D to " + name);
        }
        collider.isTrigger = false;

        // Ensure AI has a Rigidbody2D for physics-friendly patrol movement
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
            Debug.Log("Added Rigidbody2D to " + name);
        }
        // Use Dynamic body so gravity applies; keep rotation frozen
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;

        GameObject p = GameObject.FindWithTag("Player");
        if (p != null)
            player = p.transform;

        if (leftPoint == null || rightPoint == null)
        {
            // create minimal patrol points around start position
            leftPoint = new GameObject(name + "_left").transform;
            rightPoint = new GameObject(name + "_right").transform;
            leftPoint.position = transform.position + Vector3.left * 3f;
            rightPoint.position = transform.position + Vector3.right * 3f;
        }

        SnapToGround(collider);
    }

    private void Update()
    {
        shootTimer += Time.deltaTime;

        // Only shoot back if AI has been hit and player is in range
        if (hasBeenHit && player != null && Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            TryShootAtPlayer();
        }
    }

    private void FixedUpdate()
    {
        Patrol();
    }

    private void Patrol()
    {
        if (!isPatrolling) // do nothing if patrolling disabled
        {
            if (anim != null)
                anim.SetBool("isWalking", false);
            return;
        }

        // Play walk animation
        if (anim != null)
        {
            anim.SetBool("isWalking", true);
        }

        Transform target = movingRight ? rightPoint : leftPoint;
        float dir = movingRight ? 1f : -1f;

        if (rb != null)
        {
            // preserve vertical velocity (gravity) but set horizontal velocity
            Vector2 vel = rb.linearVelocity;
            vel.x = dir * speed;
            rb.linearVelocity = vel;
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }

        // check horizontal proximity to flip
        float dist = Mathf.Abs((rb != null ? rb.position.x : transform.position.x) - target.position.x);
        if (dist < 0.05f)
        {
            movingRight = !movingRight;
            Vector3 s = transform.localScale;
            s.x = Mathf.Sign(movingRight ? 1 : -1) * Mathf.Abs(s.x);
            transform.localScale = s;
        }
    }

    private void TryShootAtPlayer()
    {
        if (shootTimer < shootInterval)
            return;

        shootTimer = 0f;

        if (projectilePrefab == null || firePoint == null)
            return;

        Vector2 dir = (player.position - firePoint.position);
        float sign = Mathf.Sign(dir.x);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Debug.Log(name + " spawned projectile " + (proj != null ? proj.name : "null") + " with direction " + sign);
        var shotComp = proj.GetComponent<shot>();
        var box = proj.GetComponent<BoxCollider2D>();
        var rbody = proj.GetComponent<Rigidbody2D>();
        Debug.Log("projectile components: shot=" + (shotComp!=null) + " box=" + (box!=null) + " rb=" + (rbody!=null));
        if (shotComp != null)
        {
            shotComp.SetDirection(sign);
        }
        else
        {
            // If projectile uses different API, try to give it velocity
            if (rbody != null)
            {
                rbody.linearVelocity = new Vector2(sign * 10f, 0f);
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        hasBeenHit = true;
        Debug.Log(name + " HIT for " + damage + " damage! Health: " + currentHealth + "/" + maxHealth);

        if (anim != null)
        {
            anim.SetTrigger("hit");
        }

        // Stop horizontal movement in place when hit
        isPatrolling = false;
        if (rb != null)
        {
            // stop horizontal motion and lock X position so AI stays in place
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        // Now start shooting back at player
        shootTimer = shootInterval; // Reset to shoot immediately

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
        isPatrolling = false;
        Destroy(gameObject, 1f);
    }

    private void OnDrawGizmosSelected()
    {
        // draw patrol points and collider bounds for debugging in Scene view
        Gizmos.color = Color.cyan;
        if (leftPoint != null) Gizmos.DrawSphere(leftPoint.position, 0.1f);
        if (rightPoint != null) Gizmos.DrawSphere(rightPoint.position, 0.1f);

        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f);
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }

    private void SnapToGround(BoxCollider2D collider)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position + Vector3.up * 0.5f, Vector2.down, 20f, Physics2D.DefaultRaycastLayers);
        if (hitInfo.collider == null)
        {
            return;
        }

        float halfHeight = collider.bounds.extents.y;
        Vector3 position = transform.position;
        position.y = hitInfo.point.y + halfHeight;
        transform.position = position;

        if (rb != null)
        {
            rb.position = position;
        }
    }
}
