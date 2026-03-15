using UnityEngine;

public class followplayer : MonoBehaviour
{
    public Transform player; // Ide húzd be a Playert az Inspectorban
public Vector3 offset = new Vector3(0, 2, 0); // Mennyivel legyen a feje felett

void LateUpdate()
{
    if (player != null)
    {
        // Csak a pozíciót másolja, a forgást és a méretet (Scale) NEM!
        transform.position = player.position + offset;
    }
}
    
}
