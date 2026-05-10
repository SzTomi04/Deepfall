using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Serializable]
    public class RoomDefinition
    {
        public int roomID;
        public string roomName;
        [TextArea(3, 8)] public string description;
        public bool isShelter;
    }

    [Serializable]
    public class RoomConnection
    {
        public int roomA;
        public int roomB;
    }

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
    [SerializeField] private List<RoomDefinition> roomDefinitions = new List<RoomDefinition>();
    [SerializeField] private List<RoomConnection> roomConnections = new List<RoomConnection>();
    [SerializeField] private List<RoomSpawnPoint> roomSpawnPoints = new List<RoomSpawnPoint>();
    [SerializeField] private int defaultRoomID = 0;
    [SerializeField] private int shelterRoomID = 1;
    [SerializeField] private float loseHeight = -25f;
    [SerializeField] private bool autoInitializeWorld = true;

    private GameObject currentPlayer;
    private readonly Dictionary<int, RoomDefinition> roomsById = new Dictionary<int, RoomDefinition>();
    private readonly Dictionary<int, HashSet<int>> connectionsByRoom = new Dictionary<int, HashSet<int>>();
    private int currentRoomID = -1;
    private bool worldInitialized;
    private bool gameOver;
    private bool gameWon;

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

    private void Start()
    {
        if (autoInitializeWorld)
        {
            InitWorld();
        }
    }

    private void Update()
    {
        GameLoop();
    }

    public void InitWorld()
    {
        LoadRooms();
        ParseConnections();

        if (roomsById.Count == 0)
        {
            gameOver = true;
            worldInitialized = true;
            Debug.LogWarning("Nincs betoltott szoba. A vilag inicializalasa nem sikerult.");
            return;
        }

        int startRoomID = roomsById.ContainsKey(defaultRoomID) ? defaultRoomID : GetFirstRoomID();
        worldInitialized = true;
        gameOver = false;
        gameWon = false;
        currentRoomID = startRoomID;

        CreatePlayer();
        EnterRoom(startRoomID);
    }

    public void LoadRooms()
    {
        roomsById.Clear();

        if (roomDefinitions.Count == 0)
        {
            roomsById.Add(0, new RoomDefinition
            {
                roomID = 0,
                roomName = "Indulo szoba",
                description = "A jatekos itt kezdi az utazast.",
                isShelter = false
            });

            roomsById.Add(1, new RoomDefinition
            {
                roomID = 1,
                roomName = "Menedek",
                description = "Ez a menedek, a celpont.",
                isShelter = true
            });

            shelterRoomID = 1;
            defaultRoomID = 0;
            return;
        }

        foreach (RoomDefinition roomDefinition in roomDefinitions)
        {
            if (roomDefinition == null)
            {
                continue;
            }

            if (roomsById.ContainsKey(roomDefinition.roomID))
            {
                Debug.LogWarning($"Duplikalt roomID talalhato: {roomDefinition.roomID}");
                continue;
            }

            roomsById.Add(roomDefinition.roomID, roomDefinition);

            if (roomDefinition.isShelter)
            {
                shelterRoomID = roomDefinition.roomID;
            }
        }
    }

    public void ParseConnections()
    {
        connectionsByRoom.Clear();

        foreach (int roomID in roomsById.Keys)
        {
            connectionsByRoom[roomID] = new HashSet<int>();
        }

        foreach (RoomConnection connection in roomConnections)
        {
            if (connection == null)
            {
                continue;
            }

            AddConnection(connection.roomA, connection.roomB);
        }
    }

    public bool EnterRoom(int roomID)
    {
        if (!roomsById.ContainsKey(roomID))
        {
            Debug.LogWarning($"Nem letezo szoba: {roomID}");
            return false;
        }

        currentRoomID = roomID;
        MovePlayer(roomID);
        Debug.Log(GetRoomDescription(roomID));
        return true;
    }

    public List<int> GetNeighbours(int roomID)
    {
        if (connectionsByRoom.TryGetValue(roomID, out HashSet<int> neighbours))
        {
            return new List<int>(neighbours);
        }

        return new List<int>();
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

    public GameObject GetCurrentPlayer()
    {
        return currentPlayer;
    }

    public void GameLoop()
    {
        if (!worldInitialized || gameOver || gameWon)
        {
            return;
        }

        if (CheckGameOver())
        {
            return;
        }

        CheckWinCondition();
    }

    public bool CheckGameOver()
    {
        if (currentPlayer == null)
        {
            return false;
        }

        if (currentPlayer.transform.position.y < loseHeight)
        {
            gameOver = true;
            Debug.Log("Jatek vege: a jatekos kikerult a palyarol.");
            return true;
        }

        return false;
    }

    public bool CheckWinCondition()
    {
        if (currentRoomID == shelterRoomID)
        {
            gameWon = true;
            Debug.Log("Győzelem: a jatekos elerte a menedeket.");
            return true;
        }

        if (roomsById.TryGetValue(currentRoomID, out RoomDefinition roomDefinition) && roomDefinition.isShelter)
        {
            gameWon = true;
            Debug.Log("Győzelem: a jatekos elerte a menedeket.");
            return true;
        }

        return false;
    }

    public bool AddRoom(int roomID, string roomName, string description, bool isShelter = false)
    {
        if (roomsById.ContainsKey(roomID))
        {
            return false;
        }

        RoomDefinition roomDefinition = new RoomDefinition
        {
            roomID = roomID,
            roomName = roomName,
            description = description,
            isShelter = isShelter
        };

        roomsById.Add(roomID, roomDefinition);
        if (!connectionsByRoom.ContainsKey(roomID))
        {
            connectionsByRoom.Add(roomID, new HashSet<int>());
        }

        if (isShelter)
        {
            shelterRoomID = roomID;
        }

        return true;
    }

    public bool AddConnection(int roomA, int roomB)
    {
        if (!roomsById.ContainsKey(roomA) || !roomsById.ContainsKey(roomB) || roomA == roomB)
        {
            return false;
        }

        if (!connectionsByRoom.ContainsKey(roomA))
        {
            connectionsByRoom[roomA] = new HashSet<int>();
        }

        if (!connectionsByRoom.ContainsKey(roomB))
        {
            connectionsByRoom[roomB] = new HashSet<int>();
        }

        connectionsByRoom[roomA].Add(roomB);
        connectionsByRoom[roomB].Add(roomA);
        return true;
    }

    public string GetRoomDescription(int roomID)
    {
        if (roomsById.TryGetValue(roomID, out RoomDefinition roomDefinition))
        {
            List<int> neighbours = GetNeighbours(roomID);
            string neighboursText = neighbours.Count > 0 ? string.Join(", ", neighbours) : "nincs kapcsolt szoba";

            return $"[{roomDefinition.roomName}] {roomDefinition.description}\nSzomszedok: {neighboursText}";
        }

        return $"Nincs informacio a(z) {roomID} azonositoju szobarol.";
    }

    public int GetCurrentRoomID()
    {
        return currentRoomID;
    }

    public bool IsGameOver()
    {
        return gameOver;
    }

    public bool HasWon()
    {
        return gameWon;
    }

    private GameObject EnsurePlayerExists()
    {
        if (currentPlayer != null)
        {
            return currentPlayer;
        }

        return CreatePlayer();
    }

    private int GetFirstRoomID()
    {
        foreach (int roomID in roomsById.Keys)
        {
            return roomID;
        }

        return defaultRoomID;
    }

    private Transform GetSpawnTransformForRoom(int roomID)
    {
        if (roomSpawnPoints.Count > 0)
        {
            RoomSpawnPoint room = roomSpawnPoints.Find(entry => entry.roomID == roomID && entry.spawnPoint != null);
            if (room != null)
            {
                return room.spawnPoint;
            }
        }

        return defaultSpawnPoint;
    }
}