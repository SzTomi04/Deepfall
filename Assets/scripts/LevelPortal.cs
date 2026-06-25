using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Ezt hozzáadtuk a New Input System miatt!

public class SceneTransition : MonoBehaviour
{
    [Header("A betöltendő jelenet pontos neve")]
    public string nextSceneName;

    private bool isPlayerAtDoor = false;
    private PlayerInventory playerInventory;

    private void Update()
    {
        // ÚJ INPUT SYSTEM: Ellenőrizzük, hogy van-e billentyűzet, és megnyomták-e az E betűt
        if (isPlayerAtDoor && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (playerInventory != null && playerInventory.hasKey)
            {
                Debug.Log("Kulcs behelyezve! Jelenet betöltése: " + nextSceneName);
                
                if (!string.IsNullOrWhiteSpace(nextSceneName))
                {
                    SceneManager.LoadScene(nextSceneName);
                }
                else
                {
                    Debug.LogError("Nincs megadva a következő jelenet neve a " + gameObject.name + " objektumon!");
                }
            }
            else
            {
                Debug.Log("Az ajtó zárva van! Keresd meg a kulcsot.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerAtDoor = true;
            playerInventory = collision.GetComponent<PlayerInventory>();
            Debug.Log("Játékos az ajtónál. Nyomj 'E'-t a nyitáshoz!");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerAtDoor = false;
            playerInventory = null; 
        }
    }
}