using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Új Input System!

public class RestartGame : MonoBehaviour
{
    [Header("Hova vigyen az R gomb?")]
    public string firstLevelName = "Level1"; // Ide írd be a kezdőpályád nevét!

    void Update()
    {
        // Figyeljük a billentyűzetet az R gomb lenyomására
        if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
        {
            Debug.Log("R megnyomva! Újraindítás...");
            SceneManager.LoadScene(firstLevelName);
        }
    }
}