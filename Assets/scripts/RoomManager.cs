using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public static RoomManager Instance { get; private set; }

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

    public void LoadRooms()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.LoadRooms();
        }
    }

    public void ParseConnections()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.ParseConnections();
        }
    }

    public List<int> GetNeighbours(int roomID)
    {
        if (PlayerManager.Instance != null)
        {
            return PlayerManager.Instance.GetNeighbours(roomID);
        }

        return new List<int>();
    }

    public bool AddRoom(int roomID, string roomName, string description, bool isShelter = false)
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.AddRoom(roomID, roomName, description, isShelter);
    }

    public bool AddConnection(int roomA, int roomB)
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.AddConnection(roomA, roomB);
    }

    public string GetRoomDescription(int roomID)
    {
        if (PlayerManager.Instance != null)
        {
            return PlayerManager.Instance.GetRoomDescription(roomID);
        }

        return $"Nincs informacio a(z) {roomID} azonositoju szobarol.";
    }
}
