using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // The physical size of the map
    [SerializeField] private Vector2Int mapSize = new Vector2Int(4, 4);

    // How many rooms the map grid can hold across the X and Y axis
    private int gridSizeX;
    private int gridSizeY;
    // The number of rooms to generate
    [SerializeField] private int roomCount = 20;

    // The starting and ending percentage chance a room will be replaced with a branch room
    [SerializeField] private float branchChanceStart = 0.5f;
    [SerializeField] private float branchChanceEnd = 0.01f;

    // 2D array for storing the rooms in a grid
    private Room[,] rooms;

    // List of the taken room positions to make it easier to check whether a position is taken
    private List<Vector2> takenPositions = new List<Vector2>();
    
    [SerializeField] private GameObject mapSpritePrefab;

    // TEMPORARY
    [SerializeField] private RoomManager roomManager;


    private void Start()
    {
        // Set the grid size directly to the map size because each map tile's dimensions is 1x1
        gridSizeX = mapSize.x;
        gridSizeY = mapSize.y;
        // Create the 2D room array at the size of the grid
        rooms = new Room[gridSizeX, gridSizeY];
        // If the room count is greater than the number of spaces in the grid, reduce it to the number of spaces
        roomCount = Mathf.Clamp(roomCount, 0, rooms.Length);

        PlaceRooms();
        PlaceRoomDoors();
        DrawMap();

        roomManager.SpawnRooms(rooms);
    }

    private void PlaceRooms()
    {
        // Spawn the starting room with a world position of (0, 0) so that it is shown in the centre of the scene when we draw the map
        rooms[Mathf.RoundToInt(gridSizeX / 2), Mathf.RoundToInt(gridSizeY / 2)] = new Room(Vector2.zero, Room.RoomType.Start);
        // Add the starting room's world position to the list of taken positions
        takenPositions.Insert(0, Vector2.zero);

        // Spawn the rest of the rooms using the room count minus 1 because the starting room has been placed
        for (int i = 0; i < roomCount - 1; i++)
        {
            // Get a valid room position
            Vector2 roomPos = GetValidRoomPos();
            // Calculate the chance that it is replaced with a branching room, lerping from the starting branch chance to the ending branch chance as more rooms are created
            float branchChance = Mathf.Lerp(branchChanceStart, branchChanceEnd, (float)i / (float)roomCount - 1.0f);
            // Roll to see if it is replaced with a branching room and do so if the current room has more than 1 neighbour
            if (GetNeighbourCount(roomPos) > 1 && Random.value < branchChance)
            {
                // Get a new branch room position
                roomPos = GetValidBranchRoomPos();
            }
            // Add a room at that position to the array, making sure to reverse the offset of the room's world position from its position in the array
            rooms[(int)roomPos.x + (gridSizeX / 2), (int)roomPos.y + (gridSizeY / 2)] = new Room(roomPos, Room.RoomType.Combat);
            // Add the room's world position to the taken positions list
            takenPositions.Insert(0, roomPos);
        }
    }

    private Vector2 GetValidRoomPos()
    {
        Vector2 validPos = Vector2.zero;
        do
        {
            // Randomly select a taken position
            Vector2 randTakenPos = takenPositions[Random.Range(0, takenPositions.Count)];
            // Randomly offset the chosen position by 1 in the direction up, down, left or right and set the valid position to the offset position
            int randOffset = Random.Range(0, 4);
            if (randOffset == 0) { validPos = randTakenPos + Vector2.right; } // Offset right
            else if (randOffset == 1) { validPos = randTakenPos + Vector2.left; } // Offset left
            else if (randOffset == 2) { validPos =  randTakenPos + Vector2.up; } // Offset upwards
            else if (randOffset == 3) { validPos =  randTakenPos + Vector2.down; } // Offset downwards
        }
        // Repeat if the chosen position is already taken or is out of bounds of the grid
        while (takenPositions.Contains(validPos) || validPos.x >= (gridSizeX / 2) || validPos.x < -(gridSizeX / 2) || validPos.y >= (gridSizeY / 2) || validPos.y < -(gridSizeY / 2));
        // Return the valid position
        return validPos;
    }

    private Vector2 GetValidBranchRoomPos()
    {
        Vector2 validPos = Vector2.zero;
        do
        {
            // Iteration counter to limit the number of tries to find a taken position with only 1 neighbour
            int iterations = 0;
            Vector2 randTakenPos = Vector2.zero;
            // Randomly select a taken position with only 1 neighbouring room
            do
            {
                randTakenPos = takenPositions[Random.Range(0, takenPositions.Count)];
                iterations++;
            }
            // Repeat if the chosen position has more than 1 neighbour or until the iteration count reaches 100
            while (GetNeighbourCount(randTakenPos) > 1 && iterations < 100);
            // Log a warning if it was unable to find a taken position with only 1 neighbour
            if (iterations >= 100) { Debug.LogWarning("Could not find taken position with fewer than " + GetNeighbourCount(randTakenPos) + " neighbours."); }
            // Randomly offset the chosen position by 1 in the direction up, down, left or right and set the valid position to the offset position
            int randOffset = Random.Range(0, 4);
            if (randOffset == 0) { validPos = randTakenPos + Vector2.right; } // Offset right
            else if (randOffset == 1) { validPos = randTakenPos + Vector2.left; } // Offset left
            else if (randOffset == 2) { validPos = randTakenPos + Vector2.up; } // Offset upwards
            else if (randOffset == 3) { validPos = randTakenPos + Vector2.down; } // Offset downwards
        }
        // Repeat if the chosen position is already taken or is out of bounds of the grid
        while (takenPositions.Contains(validPos) || validPos.x >= (gridSizeX / 2) || validPos.x < -(gridSizeX / 2) || validPos.y >= (gridSizeY / 2) || validPos.y < -(gridSizeY / 2));
        // Return the valid position
        return validPos;
    }

    private int GetNeighbourCount(Vector2 chosenPos)
    {
        int neighbourCount = 0;
        if (takenPositions.Contains(chosenPos + Vector2.up)) { neighbourCount++; }
        if (takenPositions.Contains(chosenPos + Vector2.down)) { neighbourCount++; }
        if (takenPositions.Contains(chosenPos + Vector2.right)) { neighbourCount++; }
        if (takenPositions.Contains(chosenPos + Vector2.left)) { neighbourCount++; }
        return neighbourCount;
    }

    private void PlaceRoomDoors()
    {
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

                // Set the bools for each room's doors based on its neighbouring rooms, leaving the bool false if the checked position is out of bounds
                if (y + 1 < gridSizeY && rooms[x, y + 1] != null)
                {
                    rooms[x, y].doorUp = true;
                }
                if (y - 1 >= 0 && rooms[x, y - 1] != null)
                {
                    rooms[x, y].doorDown = true;
                }
                if (x + 1 < gridSizeX && rooms[x + 1, y] != null)
                {
                    rooms[x, y].doorRight = true;
                }
                if (x - 1 >= 0 && rooms[x - 1, y] != null)
                {
                    rooms[x, y].doorLeft = true;
                }
            }
        }
    }

    private void DrawMap()
    {
        foreach (Room room in rooms)
        {
            // Continue to the next grid position if there is no room at that position
            if (room == null) { continue; }

            // Set the draw position to the room's world position
            Vector2 drawPos = room.worldPos;
            // Instantiate the map sprite at the draw position and get a reference to its Map Sprite Selector script
            MapSpriteSelector mapSpriteSelector = Instantiate(mapSpritePrefab, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
            // Set it to the correct map sprite
            mapSpriteSelector.SetSprite(room.doorUp, room.doorDown, room.doorLeft, room.doorRight, room.roomType);
        }
    }
}
