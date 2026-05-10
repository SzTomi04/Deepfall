using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public static WorldManager Instance { get; private set; }

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

    public void InitWorld()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.InitWorld();
        }
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

    public bool EnterRoom(int roomID)
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.EnterRoom(roomID);
    }

    public List<int> GetNeighbours(int roomID)
    {
        if (PlayerManager.Instance != null)
        {
            return PlayerManager.Instance.GetNeighbours(roomID);
        }

        return new List<int>();
    }

    public void GameLoop()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.GameLoop();
        }
    }

    public bool CheckGameOver()
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.CheckGameOver();
    }

    public bool CheckWinCondition()
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.CheckWinCondition();
    }

    public int GetCurrentRoomID()
    {
        return PlayerManager.Instance != null ? PlayerManager.Instance.GetCurrentRoomID() : -1;
    }

    public bool IsGameOver()
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.IsGameOver();
    }

    public bool HasWon()
    {
        return PlayerManager.Instance != null && PlayerManager.Instance.HasWon();
    }
}
