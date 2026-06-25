using UnityEngine;
using UnityEngine.SceneManagement; // Ezt hozzá kell adni a SceneManager miatt!

public class PlayerInventory : MonoBehaviour
{
    // Ez a változó tárolja, hogy nálunk van-e a kulcs
    public bool hasKey = false; 

    // Amikor a szkript bekapcsol, feliratkozunk a jelenet-betöltés eseményre
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Amikor a szkript kikapcsol, leiratkozunk (fontos a memóriaszivárgás elkerülése végett!)
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Ez a függvény automatikusan lefut minden alkalommal, amikor egy új pálya betöltődik
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasKey = false;
        Debug.Log("Új pálya töltődött be: " + scene.name + ". A kulcs eltűnt az inventory-ból!");
    }
}