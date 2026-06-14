using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Door Settings")]
    [SerializeField] private bool isOpen = false;
    [SerializeField] private Vector3 openOffset = new Vector3(0, 3f, 0); // Irány és távolság (pl. x: 3 oldalra)
    [SerializeField] private float speed = 3f;

    [Header("Visuals (Optional)")]
    [SerializeField] private SpriteRenderer statusLight;
    [SerializeField] private Color lockedColor = Color.red;
    [SerializeField] private Color unlockedColor = Color.green;

    private Vector3 closedPosition;
    private Vector3 targetPosition;

    private void Start()
    {
        closedPosition = transform.position;
        UpdateTarget();
    }

    private void Update()
    {
        // Sima átmenet a célpozíció felé
        transform.position = Vector3.Lerp(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void UpdateTarget()
    {
        targetPosition = isOpen ? closedPosition + openOffset : closedPosition;
        
        if (statusLight != null)
        {
            statusLight.color = isOpen ? unlockedColor : lockedColor;
        }
    }

    // Automatikus nyitás, ha a játékos belép a trigger zónába
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = true;
            UpdateTarget();
        }
    }

    // Bezárás, ha elhagyja
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isOpen = false;
            UpdateTarget();
        }
    }
}