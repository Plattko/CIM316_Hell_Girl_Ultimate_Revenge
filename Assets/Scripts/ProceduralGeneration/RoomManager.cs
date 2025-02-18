using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject combatRoomPrefab;

    private Room[,] rooms;
    private RoomPaths[,] roomPaths;
    private Vector2 curRoom;

    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    public event Action<GameObject> onPlayerSpawned;

    // Room transition variables
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private float roomTransitionDuration = 0.1f;

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
                room.GetComponent<RoomPaths>().onPathTriggered += OnPathTriggered; // Not sure when to unsubscribe?
                // Set its paths
                room.GetComponent<RoomPaths>().SetPaths(rooms[x, y]);

                // If it's the starting room, set the current room to it
                if (rooms[x, y].roomType == Room.RoomType.Start)
                {
                    curRoom = new Vector2(x, y);
                }
                // Otherwise, disable the room so that only the current room is shown
                else
                {
                    room.SetActive(false);
                }
            }
        }

        // Spawn the player in the starting room
        player = Instantiate(playerPrefab, new Vector3(0f, 1f, 0f), Quaternion.identity);
        // Signal that the player has been spawned
        onPlayerSpawned?.Invoke(player);
    }

    private GameObject SpawnRoom(Room roomData)
    {
        GameObject roomPrefab = null;

        // Choose the room prefab that matches the room's type
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

        // Instantiate the room prefab at the correct position in the grid, multiplied by 50 to space the rooms out
        GameObject room = Instantiate(roomPrefab, new Vector3(roomData.worldPos.x * 50f, 0f, roomData.worldPos.y * 50f), Quaternion.identity);
        return room;
    }

    private IEnumerator TraverseRooms(int dir)
    {
        Vector2 newRoom = Vector2.zero;

        // Disable the player's movement
        player.GetComponent<PlayerController>().DisableMovement();

        // Fade to black
        yield return fadeToBlack.FadeOut();

        // Set the new room based on the path the player went through and teleport them to the correct entry point of the new room
        if (dir == 0) // Player went UP
        {
            // Set the new room to the room up
            newRoom = curRoom + Vector2.up;
            // Teleport the player to the new room's bottom spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[1].position;
        }
        else if (dir == 1) // Player went DOWN
        {
            // Set the new room to the room down
            newRoom = curRoom + Vector2.down;
            // Teleport the player to the new room's top spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[0].position;
        }
        else if (dir == 2) // Player went LEFT
        {
            // Set the new room to the room left
            newRoom = curRoom + Vector2.left;
            // Teleport the player to the new room's right spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[3].position;
        }
        else if (dir == 3) // Player went RIGHT
        {
            // Set the new room to the room right
            newRoom = curRoom + Vector2.right;
            // Teleport the player to the new room's left spawn point
            player.transform.position = roomPaths[(int)newRoom.x, (int)newRoom.y].entryPoints[2].position;
        }

        // Disable the previous room
        roomPaths[(int)curRoom.x, (int)curRoom.y].gameObject.SetActive(false);
        // Enable the new room
        roomPaths[(int)newRoom.x, (int)newRoom.y].gameObject.SetActive(true);
        // Make the new room the current room
        curRoom = newRoom;

        // Wait for the room transition duration
        yield return new WaitForSeconds(roomTransitionDuration);
        // Fade back in
        yield return fadeToBlack.FadeIn();

        // TODO: Initialise the current room

        // Re-enable the player's movement
        player.GetComponent<PlayerController>().EnableMovement();
    }

    private void OnPathTriggered(int dir)
    {
        StartCoroutine(TraverseRooms(dir));
    }
}
