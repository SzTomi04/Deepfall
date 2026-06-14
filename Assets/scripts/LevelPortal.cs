using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelPortal : MonoBehaviour
{
    [Header("Beállítások")]
    [SerializeField] private string targetSceneName; // A jelenet pontos neve a Build Settings-ben
    [SerializeField] private int targetRoomID;      // Opcionális: ha a PlayerManager szoba-alapú rendszerét használod

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ellenőrizzük, hogy a játékos ért-e hozzá
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Átlépés a következő jelenetbe: " + targetSceneName);
            
            // Ha használod a PlayerManager-t a szobák követésére:
            if (PlayerManager.Instance != null)
            {
                // Itt frissítheted a PlayerManager belső állapotát, mielőtt váltasz
            }

            SceneManager.LoadScene(targetSceneName);
        }
    }
}