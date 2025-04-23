using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomManager : MonoBehaviour
{
    [field: SerializeField] public Transform Map { get; private set; }

    [SerializeField] private GameObject startingRoomPrefab;
    [SerializeField] private GameObject[] combatRoomPrefabs;
    [SerializeField] private GameObject itemRoomPrefab;
    [SerializeField] private GameObject ascensionRoomPrefab;
    [SerializeField] private GameObject minibossRoomPrefab;
    [SerializeField] private GameObject bossRoomPrefab;
    [SerializeField] private int roomSpacing = 50;

    private Dictionary<Vector2Int, Room> roomsDict = new Dictionary<Vector2Int, Room>();
    private Vector2Int curRoom;

    [SerializeField] private GameObject playerPrefab;
    private GameObject player;


    // Room transition variables
    [SerializeField] private float roomTransitionDuration = 0.1f;
    [SerializeField] private float playerMoveDelay = 0.33f;

    // Onboarding variables
    [SerializeField] private GameObject onboardingRoomPrefab;

    private void OnDestroy()
    {
        if (player != null)
        {
            // Unsubscribe from the player's spell manager's spell dropped event
            player.GetComponentInChildren<SpellManager>().onSpellDropped -= ParentObjectToRoom;
        }
    }

    //-------------------------------------------------------------
    // SPAWNING ROOMS
    //-------------------------------------------------------------
    public void SpawnRooms(Room[,] rooms, Vector2Int gridCentre, bool spawnPlayer)
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
                GameObject roomObj = SpawnRoom(roomsDict[gridPos], gridCentre);
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
        if (spawnPlayer)
        {
            // Spawn the player in the starting room
            player = Instantiate(playerPrefab, startingPos, Quaternion.identity);
            // Subscribe to the player's spell manager's spell dropped event
            player.GetComponentInChildren<SpellManager>().onSpellDropped += ParentObjectToRoom;
        }
        else
        {
            TeleportPlayer(startingPos);
        }
        // Increase the rooms cleared stat to include the starting room
        StatsManager.Instance.IncreaseRoomsCleared();
        // Set the room to visited
        roomsDict[curRoom].IsVisited = true;
    }

    private GameObject SpawnRoom(Room roomData, Vector2Int gridCentre)
    {
        GameObject roomPrefab = null;

        // Choose the room prefab that matches the room's type
        switch (roomData.roomType)
        {
            case Room.RoomType.Start:
                if (!GameManager.Instance.HasOnboardedPlayer)
                {
                    roomPrefab = onboardingRoomPrefab;
                }
                else
                {
                    roomPrefab = startingRoomPrefab;
                }
                break;

            case Room.RoomType.Combat:
                roomPrefab = combatRoomPrefabs[UnityEngine.Random.Range(0, combatRoomPrefabs.Length)];
                break;

            case Room.RoomType.Item:
                roomPrefab = itemRoomPrefab;
                break;

            case Room.RoomType.Ascension:
                roomPrefab = ascensionRoomPrefab;
                break;

            case Room.RoomType.Miniboss:
                roomPrefab = minibossRoomPrefab;
                break;

            case Room.RoomType.Boss:
                roomPrefab = bossRoomPrefab;
                break;

            default:
                break;
        }

        // Set the room position to the room's grid position minus the grid's centre offset so the room positions are centred on (0, 0)
        Vector2Int roomPos = roomData.gridPos - gridCentre;
        // Instantiate the room prefab at the correct position in the grid multiplied by the room spacing value to space the rooms out
        GameObject roomObj = Instantiate(roomPrefab, new Vector3(roomPos.x * roomSpacing, 0f, roomPos.y * roomSpacing), Quaternion.identity);
        roomObj.transform.parent = Map;
        return roomObj;
    }

    public void ClearRooms()
    {
        // Destroy the room objects
        foreach (Transform room in Map)
        {
            Destroy(room.gameObject);
        }
        // Clear the rooms dictionary
        roomsDict.Clear();
    }

    //-------------------------------------------------------------
    // MOVING BETWEEN ROOMS
    //-------------------------------------------------------------
    private void OnPathTriggered(int newRoomDir)
    {
        StartCoroutine(RoomTransition(newRoomDir));
    }

    private IEnumerator RoomTransition(int newRoomDir)
    {
        // Disable the player's movement and attacks
        TogglePlayerInput(false);

        // Fade to black
        yield return UIManager.Instance.FadeOut(0.25f);

        // Set the new room based on the path the player went through and teleport them to the correct entry point of the new room
        Vector2Int newRoom = Vector2Int.zero;
        switch (newRoomDir)
        {
            // Player went UP
            case 0:
                // Set the new room to the room up
                newRoom = curRoom + Vector2Int.up;
                // Teleport the player to the new room's bottom spawn point
                TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[1].transform.position);
                break;

            // Player went DOWN
            case 1:
                // Set the new room to the room down
                newRoom = curRoom + Vector2Int.down;
                // Teleport the player to the new room's top spawn point
                TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[0].transform.position);
                break;

            // Player went LEFT
            case 2:
                // Set the new room to the room left
                newRoom = curRoom + Vector2Int.left;
                // Teleport the player to the new room's right spawn point
                TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[3].transform.position);
                break;

            // Player went RIGHT
            case 3:
                // Set the new room to the room right
                newRoom = curRoom + Vector2Int.right;
                // Teleport the player to the new room's left spawn point
                TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<RoomPaths>().entryPoints[2].transform.position);
                break;

            default:
                Debug.LogError("Direction not found.");
                break;
        }

        // TEMPORARY: Check if it's the boss room in order to execute end-demo behaviour
        var isBossRoom = roomsDict[newRoom].roomType == Room.RoomType.Boss;

        // TODO: Poorly planned, find better way to set player's position
        if (roomsDict[newRoom].roomObj.TryGetComponent(out MinibossRoom minibossRoom) && !minibossRoom.IsRoomCleared)
        {
            TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<MinibossRoom>().InitialEntryPoint.position);
        }
        else if (isBossRoom)
        {
            TeleportPlayer(roomsDict[newRoom].roomObj.GetComponent<BossRoom>().InitialEntryPoint.position);
        }

        // Disable the previous room
        roomsDict[curRoom].roomObj.SetActive(false);
        // Enable the new room
        roomsDict[newRoom].roomObj.SetActive(true);
        // Update the minimap
        UIManager.Instance.UpdateMap(newRoom, roomsDict[curRoom]);
        // Make the new room the current room
        curRoom = newRoom;

        // Wait for the room transition duration
        yield return new WaitForSeconds(roomTransitionDuration);

        // TEMPORARY: Ignore if boss room
        if (!isBossRoom)
        {
            // Fade back in
            yield return UIManager.Instance.FadeIn(0.1f);
        }

        // Initialise the room the player entered
        switch (roomsDict[curRoom].roomType)
        {
            case Room.RoomType.Start:
                break;

            case Room.RoomType.Combat:
                // Initialise the room
                roomsDict[curRoom].roomObj.GetComponent<CombatRoom>().InitialiseRoom();
                // Wait for the player move delay
                yield return new WaitForSeconds(playerMoveDelay);
                break;

            case Room.RoomType.Item:
                break;

            case Room.RoomType.Miniboss:
                // Initialise the room
                roomsDict[curRoom].roomObj.GetComponent<MinibossRoom>().InitialiseRoom();
                // Wait for the player move delay
                yield return new WaitForSeconds(playerMoveDelay);
                break;

            case Room.RoomType.Boss:
                // Initialise the room
                roomsDict[curRoom].roomObj.GetComponent<BossRoom>().InitialiseRoom();
                break;

            case Room.RoomType.Ascension:
                break;

            default:
                break;
        }

        // TODO: Messy solution, refactor code so that the Room class's IsVisited variable
        // and the CombatRoom, MinibossRoom and BossRoom's isRoomCleared variables don't create redundancy
        Room roomData = roomsDict[curRoom];
        if (!roomData.IsVisited)
        {
            // If the room isn't a combat room, miniboss room or boss room, increase the rooms cleared stat
            if (roomData.roomType != Room.RoomType.Combat && roomData.roomType != Room.RoomType.Miniboss && roomData.roomType != Room.RoomType.Boss)
            {
                StatsManager.Instance.IncreaseRoomsCleared();
            }

            // Set the room to visited
            roomData.IsVisited = true;
        }

        // TEMPORARY: Ignore if boss room
        if (!isBossRoom)
        {
            // Re-enable the player's movement and attacks
            TogglePlayerInput(true);
        }
    }

    public void TogglePlayerInput(bool enabled)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.ToggleMovement(enabled);
        playerController.ToggleAttacks(enabled);
    }

    private void TeleportPlayer(Vector3 targetPos)
    {
        // Set the player's position to the target position
        player.GetComponent<Rigidbody>().position = targetPos;
    }

    //-------------------------------------------------------------
    // PARENTING TO ROOMS
    //-------------------------------------------------------------
    private void ParentObjectToRoom(GameObject obj)
    {
        obj.transform.parent = roomsDict[curRoom].roomObj.transform;
    }
}
