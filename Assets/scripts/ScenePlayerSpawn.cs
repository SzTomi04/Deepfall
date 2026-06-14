using System.Collections;
using UnityEngine;

public class ScenePlayerSpawn : MonoBehaviour
{
    [SerializeField] private Vector3 spawnOffset = Vector3.zero;
    [SerializeField] private bool resetVelocity = true;

    private void Start()
    {
        StartCoroutine(PlacePlayerWhenReady());
    }

    private IEnumerator PlacePlayerWhenReady()
    {
        GameObject player = null;

        for (int i = 0; i < 30; i++)
        {
            if (PlayerManager.Instance != null)
            {
                player = PlayerManager.Instance.GetCurrentPlayer();
                if (player != null)
                {
                    break;
                }
            }

            player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                break;
            }

            yield return null;
        }

        if (player == null)
        {
            yield break;
        }

        Vector3 targetPosition = transform.position + spawnOffset;
        player.transform.SetPositionAndRotation(targetPosition, transform.rotation);

        if (resetVelocity)
        {
            Rigidbody2D body = player.GetComponent<Rigidbody2D>();
            if (body != null)
            {
                body.linearVelocity = Vector2.zero;
                body.angularVelocity = 0f;
            }
        }
    }
}
