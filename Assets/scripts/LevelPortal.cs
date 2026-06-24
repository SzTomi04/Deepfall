using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [Header("A betöltendő jelenet pontos neve")]
    public string nextSceneName;

    // Ez a függvény automatikusan lefut, ha valami belép a trigger zónába
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Ellenőrizzük, hogy a játékos volt-e az (a Player taget használva)
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Játékos elérte a kijáratot. Jelenet betöltése: " + nextSceneName);
            
            // Ha a név nincs megadva, ne fagyjon ki a játék
            if (!string.IsNullOrWhiteSpace(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                Debug.LogError("Nincs megadva a következő jelenet neve a " + gameObject.name + " objektumon!");
            }
        }
    }
}