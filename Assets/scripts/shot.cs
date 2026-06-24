using UnityEngine;

public class shot : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask whatIsSolid; // Az Inspectorban állítsd be (pl. Ground, Enemy)
    
    private BoxCollider2D boxCollider;
    private bool hit;
    private Animator anim;
    private float direction;
    private Transform ownerRoot;
    private float lifetime; // Mennyi idő után tűnik el a lövedék, ha nem talál el semmit
    public int damageAmount = 20;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();

        if (boxCollider == null)
        {
            Debug.LogWarning("shot prefab is missing a BoxCollider2D on " + gameObject.name);
        }

        if (anim == null)
        {
            Debug.LogWarning("shot prefab is missing an Animator on " + gameObject.name);
        }
    }
    
    void Update()
    {
        if (hit) return;

        float moveDistance = speed * Time.deltaTime;
        Vector2 moveDirection = transform.right * direction;

        // Always allow player/enemy layers to be detected, even if the inspector mask is incomplete.
        LayerMask maskToUse = whatIsSolid.value == 0 ? Physics2D.DefaultRaycastLayers : whatIsSolid;
        maskToUse |= LayerMask.GetMask("Player", "Enemy", "Robot");
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDirection, moveDistance + 0.1f, maskToUse);

        // draw the ray so we can see it in the Scene view
        Debug.DrawRay(transform.position, moveDirection * (moveDistance + 0.1f), Color.red);

        if (direction == 0f)
        {
            Debug.LogWarning("shot: direction is 0 — SetDirection may not have been called on " + gameObject.name);
        }

        if (hitInfo.collider != null)
        {
            if (ownerRoot != null && hitInfo.collider.transform.IsChildOf(ownerRoot))
            {
                transform.Translate(moveDistance * direction, 0, 0);
                lifetime += Time.deltaTime;
                if (lifetime >= 5f) gameObject.SetActive(false);
                return;
            }

            Debug.Log("shot: raycast hit " + hitInfo.collider.name + " (layer: " + LayerMask.LayerToName(hitInfo.collider.gameObject.layer) + ")");
            ProcessHit(hitInfo);
        }
        else
        {
            transform.Translate(moveDistance * direction, 0, 0);
        }

        // Élettartam
        lifetime += Time.deltaTime;
        if (lifetime >= 5f) gameObject.SetActive(false);
    }

    private void ProcessHit(RaycastHit2D hitInfo)
    {
        hit = true; // Megállítjuk az Update-et

        // Pontosan a falra/testre helyezzük
        transform.position = hitInfo.point;

        // Kinyerjük a meglőtt objektum rétegét (Layer)
        int hitLayer = hitInfo.collider.gameObject.layer;
        int playerLayer = LayerMask.NameToLayer("Player");
        int enemyLayer = LayerMask.NameToLayer("Enemy");

        // VIZSGÁLAT RÉTEGEK ALAPJÁN
        if (hitLayer == playerLayer)
        {
            // PLAYER TALÁLAT
            Debug.Log("Shot hit Player based on Layer!");
            
            // Megkeressük a HealthBar-t a játékoson és sebzünk
            HealthBar playerHealth = hitInfo.collider.GetComponentInParent<HealthBar>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            
            transform.SetParent(hitInfo.collider.transform); // Rátapad a futó játékosra
            Explodeonplayer();
        }
        else if (hitLayer == enemyLayer)
        {
            // ENEMY TALÁLAT
            Debug.Log("Shot hit Enemy based on Layer: " + hitInfo.collider.name);
            
            // Megkeressük az AIEnemy komponenst és sebzünk
            AIEnemy aiEnemy = hitInfo.collider.GetComponentInParent<AIEnemy>();
            if (aiEnemy != null)
            {
                aiEnemy.TakeDamage(damageAmount, transform.position); 
            }
            
            Explode();
        }
        else
        {
            // FAL TALÁLAT VAGY BÁRMI MÁS
            Debug.Log("Shot hit Wall/Other: " + hitInfo.collider.name);
            Explode();
        }
    }

    private void Explode()
    {
        hit = true;
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // 1. Fizika leállítása (hogy ne fúródjon bele tovább a falba)
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero; // Megállítjuk a mozgást
            rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic-ra váltunk
        }

        // 2. Animáció elindítása
        if (anim != null)
        {
            anim.SetTrigger("explode");
        }

        // 3. Irány beállítása (ha a Sprite-odnak szüksége van a tükrözésre a falnál)
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, transform.localScale.y, transform.localScale.z);
    }

    private void Explodeonplayer()
    {
        hit = true;
        if (boxCollider != null)
        {
            boxCollider.enabled = false;
        }

        // 1. Fizika leállítása
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null) 
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        // 2. Animáció elindítása
        if (anim != null)
        {
            anim.SetTrigger("explodeonplayer");
        }
    }

    public void SetDirection(float _direction)
    {
        SetDirection(_direction, null);
    }

    public void SetDirection(float _direction, Transform _ownerRoot)
    {
        transform.SetParent(null); // Leválasztjuk a lövedéket, hogy ne örökölje a fegyver mozgását
        lifetime = 0; // Újraindítjuk az élettartamot minden új lövésnél
        direction = _direction;
        ownerRoot = _ownerRoot;
        gameObject.SetActive(true);
        hit = false;
        
        if (boxCollider != null) 
            boxCollider.enabled = true;

        float sX = Mathf.Abs(transform.localScale.x) * _direction;
        transform.localScale = new Vector3(sX, transform.localScale.y, transform.localScale.z);
        
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}