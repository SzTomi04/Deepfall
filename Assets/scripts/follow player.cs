using UnityEngine;

public class followplayer : MonoBehaviour
{
    public Transform player; // Ha nem adod meg kézzel, automatikusan megkeresi
    public Vector3 offset = new Vector3(0, 2, 0); // Mennyivel legyen a feje felett

    private void Start()
    {
        ResolvePlayer();
    }

    void LateUpdate()
    {
        if (player == null)
        {
            ResolvePlayer();
        }

        if (player != null)
        {
            // Csak a pozíciót másolja, a forgást és a méretet (Scale) NEM!
            transform.position = player.position + offset;
        }
    }

    private void ResolvePlayer()
    {
        if (player != null)
        {
            return;
        }

        if (PlayerManager.Instance != null && PlayerManager.Instance.GetCurrentPlayer() != null)
        {
            player = PlayerManager.Instance.GetCurrentPlayer().transform;
            return;
        }

        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }
    
}
