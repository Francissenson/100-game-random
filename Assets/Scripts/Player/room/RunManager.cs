using System.Collections.Generic;
using UnityEngine;

public sealed class RunManager : MonoBehaviour
{
    [SerializeField]
    private List<RoomDefinition> rooms =
        new();

    private int currentRoomIndex;

    public RoomDefinition CurrentRoom =>
        rooms.Count > currentRoomIndex
            ? rooms[currentRoomIndex]
            : null;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        Debug.Log(
            $"[RunManager] Loaded {rooms.Count} rooms.");
    }

    public bool HasNextRoom()
    {
        return currentRoomIndex + 1 < rooms.Count;
    }

    public RoomDefinition GetNextRoom()
    {
        if (!HasNextRoom())
        {
            return null;
        }

        return rooms[currentRoomIndex + 1];
    }

    public void AdvanceRoom()
    {
        currentRoomIndex++;

        Debug.Log(
            $"[RunManager] Advanced To Room {currentRoomIndex}");
    }
}