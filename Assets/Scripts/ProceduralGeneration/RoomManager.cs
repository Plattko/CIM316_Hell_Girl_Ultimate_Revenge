using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject combatRoomPrefab;

    private Room[,] rooms;
    private RoomPaths[,] roomPaths;
    private Vector2 curRoom;

    [SerializeField] private GameObject playerPrefab;
    private GameObject player;

    public void SpawnRooms(int gridSizeX, int gridSizeY, Room[,] _rooms)
    {
        rooms = _rooms;
        // Initialise the room paths array at the correct size
        roomPaths = new RoomPaths[gridSizeX, gridSizeY];

        // A double for loop allows us to check every position in the rooms array
        for (int x = 0; x < (gridSizeX); x++)
        {
            for (int y = 0; y < (gridSizeY); y++)
            {
                // Continue to the next position if there is no room at that position
                if (rooms[x, y] == null)
                {
                    continue;
                }

                // Spawn the room
                GameObject room = SpawnRoom(rooms[x, y]);
                // Add its Room Paths script to the room paths array
                roomPaths[x, y] = room.GetComponent<RoomPaths>();
                // Subscribe to the room's onPathTriggered event
                room.GetComponent<RoomPaths>().onPathTriggered += TraverseRooms; // Not sure when to unsubscribe?
                // Set its paths
                room.GetComponent<RoomPaths>().SetPaths(rooms[x, y]);

                // If it's the starting room, set the current room to it
                if (rooms[x, y].roomType == Room.RoomType.Start)
                {
                    curRoom = new Vector2(x, y);
                }
            }
        }

        player = Instantiate(playerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity);
    }

    private GameObject SpawnRoom(Room roomData)
    {
        GameObject roomPrefab = null;

        switch (roomData.roomType)
        {
            case Room.RoomType.Start:
                roomPrefab = startingRoomPrefab;
                break;

            case Room.RoomType.Combat:
                roomPrefab = combatRoomPrefab;
                break;

            default:
                break;
        }

        GameObject room = Instantiate(roomPrefab, new Vector3(roomData.worldPos.x * 50f, 0f, roomData.worldPos.y * 50f), Quaternion.identity);
        return room;
    }

    private void TraverseRooms(int dir)
    {
        Vector2 newRoom = Vector2.zero;

        // 0 = UP, 1 = DOWN, 2 = LEFT, 3 = RIGHT
        if (dir == 0)
        {
            newRoom = curRoom + Vector2.up;

            // Teleport the player to the new room's bottom spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[1].position;
        }
        else if (dir == 1)
        {
            newRoom = curRoom + Vector2.down;

            // Teleport the player to the new room's top spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[0].position;
        }
        else if (dir == 2)
        {
            newRoom = curRoom + Vector2.left;

            // Teleport the player to the new room's right spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[3].position;
        }
        else if (dir == 3)
        {
            newRoom = curRoom + Vector2.right;

            // Teleport the player to the new room's left spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[2].position;
        }

        // Update the current room to the new room
        curRoom = newRoom;

        // TODO: Initialise the current room
    }
}
