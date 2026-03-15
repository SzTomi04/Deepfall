using UnityEngine;
using UnityEngine.UI; // Ezt ne felejtsd el!
using System.Collections;


public class HealthBar : MonoBehaviour
{
    public Slider slider;
    public float maxHealth = 100f;
    [SerializeField]private float currentHealth;
    private Animator anim;
    private bool isinanim = false; // Ez a változó fogja jelezni, hogy fut-e már a halál animáció
    [SerializeField]private GameObject healthBarUI; // Ez a változó fogja tárolni a HealthBar UI GameObjectjét
    

    void Start()
    {
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        
    }
    void Awake()
    {
        anim = GetComponent<Animator>();
        
    }

    public void TakeDamage(int damage)
    {
        AutoWalker walker = GetComponent<AutoWalker>(); // Megkeressük az AutoWalker komponenst, ha van
        currentHealth -= damage;
        slider.value = currentHealth; // Frissítjük a zöld csíkot

        if (currentHealth <= 0)
        {
            Die();
            walker.StopWalking();
        }
    }



    private void Die()
{
    Debug.Log("A karakter meghalt!");
    
    // Elindítjuk a folyamatot, ami várni fog
    StartCoroutine(DieRoutine());
}

private IEnumerator DieRoutine()
{
    // 1. Elindítjuk az animációt
    if (anim != null && !isinanim) // Csak akkor indítsuk el, ha még nem fut a halál animáció
    {
         // Állítsd le a karakter mozgását, ha van ilyen logika
        anim.SetTrigger("die");
        isinanim = true; // Jelezzük, hogy most már fut a halál animáció
        
        // 2. Várunk egy kicsit. 
        // Itt add meg másodpercben, milyen hosszú a halál animációd (pl. 1.5 másodperc)
        yield return new WaitForSeconds(5f); 
        gameObject.SetActive(false);
    }

    // 3. Csak most kapcsoljuk ki a karaktert
    healthBarUI.SetActive(false); // Kikapcsoljuk a HealthBar UI-t is
    
}
    
}