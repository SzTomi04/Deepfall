using UnityEngine;

public class KeyItem : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ha az az objektum, ami belénk ért, a "Player" taget viseli...
        if (collision.CompareTag("Player"))
        {
            // Elkérjük a játékos "hátizsákját" (inventory)
            PlayerInventory inv = collision.GetComponent<PlayerInventory>();
            
            if (inv != null)
            {
                inv.hasKey = true; // Belerakjuk a kulcsot
                Debug.Log("Kulcs felvéve!");
                gameObject.SetActive(false); // Eltüntetjük a kulcsot a pályáról
            }
        }
    }
}