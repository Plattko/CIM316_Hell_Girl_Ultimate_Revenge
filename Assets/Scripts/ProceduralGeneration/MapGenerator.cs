using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    // How big the map grid is by the number of rooms
    [SerializeField] private Vector2 mapSize = new Vector2(4, 4);

    private int gridSizeX;
    private int gridSizeY;
    [SerializeField] private int roomCount = 20;

    // 2D array for storing the rooms in a grid
    private Room[,] rooms;

    // List of the taken room positions to make it easier to check whether a position is taken
    private List<Vector2> takenPositions = new List<Vector2>();
    
    [SerializeField] private GameObject mapSpritePrefab;


    private void Start()
    {
        if (roomCount >= (mapSize.x * 2) * (mapSize.y * 2))
        {
            roomCount = Mathf.RoundToInt((mapSize.x * 2) * (mapSize.y * 2));
            Debug.LogWarning("Too many rooms. Reducing room count to " + roomCount + ".");
        }
        gridSizeX = Mathf.RoundToInt(mapSize.x);
        gridSizeY = Mathf.RoundToInt(mapSize.y);

        CreateRooms();
        SetRoomDoors();
        DrawMap();
    }

    private void CreateRooms()
    {
        // Set the room array to the correct size
        rooms = new Room[gridSizeX * 2, gridSizeY * 2];
        // Spawn the starting room
        rooms[gridSizeX, gridSizeY] = new Room(Vector2.zero, Room.RoomType.Start);
        takenPositions.Insert(0, Vector2.zero);

        Vector2 checkPos = Vector2.zero;
        //float branchChance;
        //float branchChanceStart = 0.2f;
        //float branchChanceEnd = 0.01f;

        // Minus 1 because the starting room already exists
        for (int i = 0; i < roomCount - 1; i++)
        {
            //float randomPercentage = (float)i / (float)(roomCount - 1);
            //branchChance = Mathf.Lerp(branchChanceStart, branchChanceEnd, randomPercentage);
            
            checkPos = NewPosition();
            rooms[(int)checkPos.x + gridSizeX, (int)checkPos.y + gridSizeY] = new Room(checkPos, Room.RoomType.Combat);
            takenPositions.Insert(0, checkPos);
        }
    }

    private Vector2 NewPosition()
    {
        Vector2 chosenPos = Vector2.zero;
        int chosenPosX = 0;
        int chosenPosY = 0;
        do
        {
            // Choose a random taken position by selecting a random index in the taken positions list
            int index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
            // Set the chosen position's x and y to the same as the taken position
            chosenPosX = (int)takenPositions[index].x;
            chosenPosY = (int)takenPositions[index].y;
            // Pick the direction up, down, left or right and offset the chosen position by 1 in that direction
            bool isUpOrDown = (Random.value < 0.5f);
            bool isPositive = (Random.value < 0.5f);
            if (isUpOrDown)
            {
                // Upwards
                if (isPositive)
                {
                    chosenPosY++;
                }
                // Downwards
                else
                {
                    chosenPosY--;
                }
            }
            else
            {
                // Right
                if (isPositive)
                {
                    chosenPosX++;
                }
                // Left
                else
                {
                    chosenPosX--;
                }
            }
            chosenPos = new Vector2(chosenPosX, chosenPosY);
        }
        // Repeat if the chosen position is already taken or is out of bounds of the grid
        while (takenPositions.Contains(chosenPos) || chosenPosX >= gridSizeX || chosenPosX < -gridSizeX || chosenPosY >= gridSizeY || chosenPosY < -gridSizeY);
        // Return the chosen position
        return chosenPos;
    }

    //private Vector2 NewBranchPosition()
    //{
    //    Vector2 chosenPos = Vector2.zero;
    //    int chosenPosX = 0;
    //    int chosenPosY = 0;
    //    int iterations = 0; // Iteration counter to limit the number of tries to find a taken position with no neighbours
    //    do
    //    {
    //        int index = 0;
    //        // Chose a random taken position by selecting a random index in the taken positions list, repeating if its neighbour count is greater than 1
    //        do
    //        {
    //            index = Mathf.RoundToInt(Random.value * (takenPositions.Count - 1));
    //        }
    //        while (NeighbourCount(takenPositions[index]) > 1 && iterations < 100);
    //        // Set the chosen position's x and y to the same as the taken position
    //        chosenPosX = (int)takenPositions[index].x;
    //        chosenPosY = (int)takenPositions[index].y;
    //        // Pick the direction up, down, left or right and offset the chosen position by 1 in that direction
    //        bool isUpOrDown = (Random.value < 0.5f);
    //        bool isPositive = (Random.value < 0.5f);
    //        if (isUpOrDown)
    //        {
    //            // Upwards
    //            if (isPositive)
    //            {
    //                chosenPosY++;
    //            }
    //            // Downwards
    //            else
    //            {
    //                chosenPosY--;
    //            }
    //        }
    //        else
    //        {
    //            // Right
    //            if (isPositive)
    //            {
    //                chosenPosX++;
    //            }
    //            // Left
    //            else
    //            {
    //                chosenPosX--;
    //            }
    //        }
    //        chosenPos = new Vector2(chosenPosX, chosenPosY);
    //    }
    //    // Repeat if the chosen position is already taken or is out of bounds of the grid
    //    while (takenPositions.Contains(chosenPos) || chosenPosX >= gridSizeX || chosenPosX < -gridSizeX || chosenPosY >= gridSizeY || chosenPosY < -gridSizeY);
    //    if (iterations >= 100)
    //    {
    //        Debug.LogWarning("Couldn't find position with only 1 neighbour.");
    //    }
    //    // Return the chosen position
    //    return chosenPos;
    //}

    //private int NeighbourCount(Vector2 chosenPos)
    //{
    //    int neighbourCount = 0;
    //    if (takenPositions.Contains(chosenPos + Vector2.up)) { neighbourCount++; }
    //    if (takenPositions.Contains(chosenPos + Vector2.down)) { neighbourCount++; }
    //    if (takenPositions.Contains(chosenPos + Vector2.right)) { neighbourCount++; }
    //    if (takenPositions.Contains(chosenPos + Vector2.left)) { neighbourCount++; }
    //    return neighbourCount;
    //}

    private void SetRoomDoors()
    {
        // Double for loop allows us to check every position in the rooms array
        for (int x = 0; x < (gridSizeX * 2); x++)
        {
            for (int y = 0; y < (gridSizeY * 2); y++)
            {
                // Continue to the next position if there is no room at that position
                if (rooms[x, y] == null)
                {
                    continue;
                }

                // Set the bools for each room's doors based on its neighbouring rooms, automatically leaving the bool false if the checked position it out of bounds
                if (y + 1 < gridSizeY * 2 && rooms[x, y + 1] != null)
                {
                    rooms[x, y].doorUp = true;
                }
                if (y - 1 >= 0 && rooms[x, y - 1] != null)
                {
                    rooms[x, y].doorDown = true;
                }
                if (x + 1 < gridSizeX * 2 && rooms[x + 1, y] != null)
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

            Vector2 drawPos = room.gridPos;
            MapSpriteSelector mapSpriteSelector = Instantiate(mapSpritePrefab, drawPos, Quaternion.identity).GetComponent<MapSpriteSelector>();
            mapSpriteSelector.SetSprite(room.doorUp, room.doorDown, room.doorLeft, room.doorRight, room.roomType);
        }
    }
}
