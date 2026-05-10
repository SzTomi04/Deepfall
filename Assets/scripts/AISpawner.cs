using System.Collections.Generic;
using UnityEngine;

public class AISpawner : MonoBehaviour
{
    public GameObject aiPrefab; // prefab with AIEnemy component
    public List<Transform> spawnPoints = new List<Transform>();
    public int count = 1;

    [ContextMenu("Spawn All")]
    public void SpawnAll()
    {
        if (aiPrefab == null || spawnPoints == null || spawnPoints.Count == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            Transform sp = spawnPoints[i % spawnPoints.Count];
            Instantiate(aiPrefab, sp.position, sp.rotation);
        }
    }

    private void Start()
    {
        // optional: spawn on start
        SpawnAll();
    }
}
