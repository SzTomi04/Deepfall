using UnityEngine;

public class shot : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private LayerMask whatIsSolid; // Az Inspectorban állítsd be (pl. Ground, Enemy)
    
    private BoxCollider2D boxCollider;
    private bool hit;
    private Animator anim;
    private float direction;
    private float lifetime; // Mennyi idő után tűnik el a lövedék, ha nem talál el semmit
    public int damageAmount = 20;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }
    
    void Update()
{
    if (hit) return;

    float moveDistance = speed * Time.deltaTime;
    Vector2 moveDirection = transform.right * direction;

    // Ha a LayerMask nincs beállítva, akkor alapból minden réteget nézzen
    LayerMask maskToUse = whatIsSolid.value == 0 ? Physics2D.DefaultRaycastLayers : whatIsSolid;
    RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, moveDirection, moveDistance + 0.1f, maskToUse);

    // draw the ray so we can see it in the Scene view
    Debug.DrawRay(transform.position, moveDirection * (moveDistance + 0.1f), Color.red);

    if (direction == 0f)
    {
        Debug.LogWarning("shot: direction is 0 — SetDirection may not have been called on " + gameObject.name);
    }

    if (hitInfo.collider != null)
    {
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

    HealthBar playerHealth = hitInfo.collider.GetComponent<HealthBar>();
    if (playerHealth == null)
    {
        playerHealth = hitInfo.collider.GetComponentInParent<HealthBar>();
    }

    AIEnemy aiEnemy = hitInfo.collider.GetComponent<AIEnemy>();
    if (aiEnemy == null)
    {
        aiEnemy = hitInfo.collider.GetComponentInParent<AIEnemy>();
    }

    if (playerHealth != null)
    {
        // PLAYER TALÁLAT
        Debug.Log("Shot hit Player!");
        playerHealth.TakeDamage(damageAmount);
        transform.SetParent(hitInfo.collider.transform); // Rátapad a futó játékosra
        Explodeonplayer();
    }
    else if (aiEnemy != null)
    {
        // ENEMY TALÁLAT
        Debug.Log("Shot hit Enemy: " + aiEnemy.gameObject.name);
        aiEnemy.TakeDamage(damageAmount);
        Explode();
    }
    else
    {
        // FAL TALÁLAT
        Debug.Log("Shot hit Wall/Other: " + hitInfo.collider.name);
        Explode();
    }
}

// Az OnTriggerEnter2D-t töröld le teljesen!

    private void Explode()
{
    hit = true;
    boxCollider.enabled = false;

    // 1. Fizika leállítása (hogy ne fúródjon bele tovább a falba)
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null) 
    {
        rb.linearVelocity = Vector2.zero; // Megállítjuk a mozgást
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic-ra váltunk (az isKinematic helyett)
    }

    // 2. Animáció elindítása
    anim.SetTrigger("explode");

    // 3. Irány beállítása (ha a Sprite-odnak szüksége van a tükrözésre a falnál)
    transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * -direction, transform.localScale.y, transform.localScale.z);
}
     private void Explodeonplayer()
{
    hit = true;
    boxCollider.enabled = false;

    // 1. Fizika leállítása (hogy ne fúródjon bele tovább a falba)
    Rigidbody2D rb = GetComponent<Rigidbody2D>();
    if (rb != null) 
    {
        rb.linearVelocity = Vector2.zero; // Megállítjuk a mozgást
        rb.bodyType = RigidbodyType2D.Kinematic; // Kinematic-ra váltunk (az isKinematic helyett)
    }

    // 2. Animáció elindítása
    anim.SetTrigger("explodeonplayer");

    // 3. Irány beállítása (ha a Sprite-odnak szüksége van a tükrözésre a falnál)
    
}
    
    

    public void SetDirection(float _direction)
    {
    transform.SetParent(null); // Leválasztjuk a lövedéket, hogy ne örökölje a játékos mozgását
    lifetime = 0; // Újraindítjuk az élettartamot minden új lövésnél
    direction = _direction;
    gameObject.SetActive(true);
    hit = false;
    
    if (boxCollider != null) 
        boxCollider.enabled = true;

    // Kiszámoljuk az irányt: ha direction 1, akkor jobbra néz, ha -1, akkor balra.
    // Feltételezzük, hogy az eredeti sprite-od jobbra néz.
    float sX = Mathf.Abs(transform.localScale.x) * _direction;
    
    // Csak az X tengelyt bántjuk, a többi maradjon az eredeti (általában 1)
    transform.localScale = new Vector3(sX, transform.localScale.y, transform.localScale.z);
    
    // BIZTONSÁGI ELLENŐRZÉS: ha a Z tengely elmozdult volna korábban
    transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    private void Deactivate()
    {
        gameObject.SetActive(false);
    }

    
    

}