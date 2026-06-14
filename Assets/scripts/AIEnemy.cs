using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("Scene")]
    [SerializeField] private string nextSceneName = "lvl2";

    private Transform player;
    private bool hasBeenHit = false;
    private Animator anim;
    private Rigidbody2D rb;
    private bool warnedMissingProjectile;

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
        // Use Dynamic body so gravity applies; keep rotation frozen and X locked.
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
            // create minimal patrol points around start position
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
            {
                player = p.transform;
            }
        }

        // Once hit, keep trying to retaliate while the player exists.
        if (hasBeenHit && player != null)
        {
            TryShootAtPlayer();
        }
    }

    private void FixedUpdate()
    {
    }

    private void TryShootAtPlayer()
    {
        if (shootTimer < shootInterval)
            return;

        shootTimer = 0f;

        if (projectilePrefab == null || firePoint == null || player == null)
        {
            if (!warnedMissingProjectile && projectilePrefab == null)
            {
                warnedMissingProjectile = true;
                Debug.LogWarning(name + " has no projectilePrefab assigned.");
            }
            return;
        }

        Vector2 dir = (player.position - firePoint.position);
        float sign = Mathf.Sign(dir.x);

        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        if (proj == null)
            return;

        Debug.Log(name + " spawned projectile " + (proj != null ? proj.name : "null") + " with direction " + sign);
        var shotComp = proj.GetComponent<shot>();
        var box = proj.GetComponent<BoxCollider2D>();
        var rbody = proj.GetComponent<Rigidbody2D>();
        Debug.Log("projectile components: shot=" + (shotComp!=null) + " box=" + (box!=null) + " rb=" + (rbody!=null));
        if (shotComp != null)
        {
            shotComp.SetDirection(sign, transform);
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
        if (rb != null)
        {
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            // Csak akkor lő vissza, ha túlélte a találatot
            shootTimer = shootInterval; 
            TryShootAtPlayer();
        }
    }

    private void Die()
    {
        // Megállítjuk a lövöldözést és a hit logikát
        hasBeenHit = false;
        
        if (anim != null)
        {
            anim.SetTrigger("die");
        }

        // Kikapcsoljuk a fizikai jelenlétét, hogy a lövedékek ne találják el többet
        BoxCollider2D col = GetComponent<BoxCollider2D>();
        if (col != null) col.enabled = false;

        // Megállítjuk a fizikát, hogy ne essen le a pályáról a halál animáció alatt
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // Kikapcsoljuk az Update futását
        this.enabled = false;

        StartCoroutine(LoadNextSceneAfterDelay(1f));
    }

    private IEnumerator LoadNextSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning(name + " has no next scene name assigned.");
            yield break;
        }

        Debug.Log($"Jelenetváltás megkísérlése: '{nextSceneName}'...");

        if (Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError($"HIBA: A(z) '{nextSceneName}' jelenet nincs hozzáadva a Build Settings-hez!");
            // Ha hiba van, legalább adjuk vissza a vezérlést vagy tegyünk valamit, 
            // de a LoadScene hiba megállítja a folyamatot.
        }
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
