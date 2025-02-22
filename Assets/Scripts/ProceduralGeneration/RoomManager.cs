using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject combatRoomPrefab;

    private Dictionary<Vector2Int, Room> roomsDict = new Dictionary<Vector2Int, Room>();
    private Vector2Int curRoom;

    [SerializeField] private GameObject playerPrefab;
    private GameObject player;
    public event Action<GameObject> onPlayerSpawned;

    // Room transition variables
    [SerializeField] private FadeToBlack fadeToBlack;
    [SerializeField] private float roomTransitionDuration = 0.1f;
    [SerializeField] private float playerMoveDelay = 0.33f;

    public void SpawnRooms(Room[,] rooms)
    {
        // A double for loop allows us to check every position in the rooms array
        for (int x = 0; x < rooms.GetLength(0); x++)
        {
            for (int y = 0; y < rooms.GetLength(1); y++)
            {
                // Continue to the next position if there is no room at that position
                if (rooms[x, y] == null)
                {
                    continue;
                }

                // Get the grid position
                Vector2Int gridPos = new Vector2Int(x, y);
                // Create a new KVP in the rooms dict with the key as the grid position and the value as the corresponding room class
                roomsDict[gridPos] = rooms[x, y];

                // Spawn the room object
                GameObject roomObj = SpawnRoom(roomsDict[gridPos]);
                // Give the room class a reference to the room object
                roomsDict[gridPos].roomObj = roomObj;
                // Subscribe to the room objects's onPathTriggered event
                roomObj.GetComponent<RoomPaths>().onPathTriggered += OnPathTriggered; // Not sure when to unsubscribe?
                // Set its paths
                roomObj.GetComponent<RoomPaths>().SetPaths(roomsDict[gridPos]);

                // If it's the starting room, set the current room to it
                if (roomsDict[gridPos].roomType == Room.RoomType.Start)
                {
                    curRoom = new Vector2Int(x, y);
                }
                // Otherwise, disable the room so that only the current room is shown
                else
                {
                    roomObj.SetActive(false);
                }
            }
        }

        // Get the starting position of the player as the position of the current (starting) room + 1 on the y axis so the player isn't in the floor
        Vector3 startingPos = roomsDict[curRoom].roomObj.transform.position + Vector3.up;
        // Spawn the player in the starting room
        player = Instantiate(playerPrefab, startingPos, Quaternion.identity);
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
        GameObject roomObj = Instantiate(roomPrefab, new Vector3(roomData.worldPos.x * 50f, 0f, roomData.worldPos.y * 50f), Quaternion.identity);
        return roomObj;
    }

    private IEnumerator TraverseRooms(int dir)
    {
        Vector2Int newRoom = Vector2Int.zero;

        // Disable the player's movement
        player.GetComponent<PlayerController>().DisableMovement();

        // Fade to black
        yield return fadeToBlack.FadeOut();

        // Set the new room based on the path the player went through and teleport them to the correct entry point of the new room
        switch (dir)
        {
            // Player went UP
            case 0:
                // Set the new room to the room up
                newRoom = curRoom + Vector2Int.up;
                // Teleport the player to the new room's bottom spawn point
                player.transform.position = roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[1].position;
                break;

            // Player went DOWN
            case 1:
                // Set the new room to the room down
                newRoom = curRoom + Vector2Int.down;
                // Teleport the player to the new room's top spawn point
                player.transform.position = roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[0].position;
                break;

            // Player went LEFT
            case 2:
                // Set the new room to the room left
                newRoom = curRoom + Vector2Int.left;
                Debug.Log("Player position before TP: " + player.transform.position);
                // Teleport the player to the new room's right spawn point
                player.transform.position = roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[3].position;
                Debug.Log("Player position after TP: " + player.transform.position);
                break;

            // Player went RIGHT
            case 3:
                // Set the new room to the room right
                newRoom = curRoom + Vector2Int.right;
                // Teleport the player to the new room's left spawn point
                player.transform.position = roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[2].position;
                break;

            default:
                Debug.LogError("Direction not found.");
                break;
        }

        // Disable the previous room
        roomsDict[curRoom].roomObj.SetActive(false);
        Debug.Log("Is current room active: " + roomsDict[curRoom].roomObj.activeInHierarchy);
        // Enable the new room
        roomsDict[newRoom].roomObj.SetActive(true);
        Debug.Log("Is current room active: " + roomsDict[newRoom].roomObj.activeInHierarchy);
        // Make the new room the current room
        curRoom = newRoom;

        // Wait for the room transition duration
        yield return new WaitForSeconds(roomTransitionDuration);
        // Fade back in
        yield return fadeToBlack.FadeIn();

        // Initialise the room the player entered
        switch (roomsDict[curRoom].roomType)
        {
            case Room.RoomType.Combat:
                roomsDict[curRoom].roomObj.GetComponent<CombatRoom>().InitialiseRoom();
                break;

            case Room.RoomType.Item:
                break;

            case Room.RoomType.Miniboss:
                break;

            case Room.RoomType.Boss:
                break;

            default:
                break;
        }

        // Wait for the player move delay
        yield return new WaitForSeconds(playerMoveDelay);
        // Re-enable the player's movement
        player.GetComponent<PlayerController>().EnableMovement();
    }

    private void OnPathTriggered(int dir)
    {
        StartCoroutine(TraverseRooms(dir));
    }
}
