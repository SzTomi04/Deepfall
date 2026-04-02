using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Serializable]
    public class RoomSpawnPoint
    {
        public int roomID;
        public Transform spawnPoint;
    }

    public static PlayerManager Instance { get; private set; }

    [Header("Player")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform defaultSpawnPoint;

    [Header("Rooms")]
    [SerializeField] private List<RoomSpawnPoint> roomSpawnPoints = new List<RoomSpawnPoint>();

    private GameObject currentPlayer;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public GameObject CreatePlayer()
    {
        if (currentPlayer != null)
        {
            return currentPlayer;
        }

        if (playerPrefab == null)
        {
            Debug.LogError("Player prefab nincs beallitva a PlayerManager-ben.");
            return null;
        }

        Transform spawnTransform = defaultSpawnPoint;
        if (spawnTransform == null && roomSpawnPoints.Count > 0)
        {
            spawnTransform = roomSpawnPoints[0].spawnPoint;
        }

        Vector3 spawnPosition = spawnTransform != null ? spawnTransform.position : Vector3.zero;
        Quaternion spawnRotation = spawnTransform != null ? spawnTransform.rotation : Quaternion.identity;

        currentPlayer = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        return currentPlayer;
    }

    public void MovePlayer(int roomID)
    {
        GameObject player = EnsurePlayerExists();
        if (player == null)
        {
            return;
        }

        RoomSpawnPoint room = roomSpawnPoints.Find(entry => entry.roomID == roomID && entry.spawnPoint != null);
        if (room == null)
        {
            Debug.LogWarning($"Nincs ilyen roomID-hez spawn pont: {roomID}");
            return;
        }

        player.transform.SetPositionAndRotation(room.spawnPoint.position, room.spawnPoint.rotation);

        Rigidbody2D body = player.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }

    public Vector3 GetPlayerLocation()
    {
        if (currentPlayer == null)
        {
            return defaultSpawnPoint != null ? defaultSpawnPoint.position : Vector3.zero;
        }

        return currentPlayer.transform.position;
    }

    private GameObject EnsurePlayerExists()
    {
        if (currentPlayer != null)
        {
            return currentPlayer;
        }

        return CreatePlayer();
    }
}