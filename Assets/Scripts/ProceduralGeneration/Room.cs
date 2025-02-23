using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data container for the rooms
public class Room
{
    // The room's position in the map grid
    public Vector2Int gridPos;

    // The type of room
    public enum RoomType
    {
        Start,
        Combat,
        Item,
        Miniboss,
        Boss,
    }
    public RoomType roomType;

    // Bools to say whether doors are in certain positions
    public bool doorUp, doorDown, doorLeft, doorRight;

    // Reference to the spawned room object
    public GameObject roomObj;

    // A constructor to pass information into an instance once created
    public Room(Vector2Int _gridPos, RoomType _roomType)
    {
        gridPos = _gridPos;
        roomType = _roomType;
    }
}
