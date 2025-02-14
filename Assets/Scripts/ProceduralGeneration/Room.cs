using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Data container for the rooms
public class Room
{
    // The room's position in the map grid
    public Vector2 worldPos;

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

    // A constructor to pass information into an instance once created
    public Room(Vector2 _worldPos, RoomType _roomType)
    {
        worldPos = _worldPos;
        roomType = _roomType;
    }
}
