using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour
{
    [SerializeField] private int iconSize = 75;
    [SerializeField] private RectTransform mapArea;
    [SerializeField] private GameObject roomIconPrefab;

    private Dictionary<Vector2Int, RoomIcon> roomIcons = new Dictionary<Vector2Int, RoomIcon>();
    
    public void DrawMap(Room[,] rooms, Vector2Int gridCentre)
    {
        foreach (Room room in rooms)
        {
            // Continue to the next grid position if there is no room at that position
            if (room == null) { continue; }

            // Instantiate the room icon as a child of the map area
            RoomIcon roomIcon = Instantiate(roomIconPrefab, mapArea).GetComponent<RoomIcon>();
            // Create a new KVP in the room icons dictionary with the key as the grid position and the value as the corresponding room icon script
            roomIcons[room.gridPos] = roomIcon;

            // Set the draw position to the room's grid position minus the grid's centre offset so the map is centred on (0, 0)
            Vector2 drawPos = room.gridPos - gridCentre;
            // Set its position to the draw position multiplied by the icon size value to space the icons out
            roomIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(drawPos.x * iconSize, drawPos.y * iconSize);
            // Set it to the correct map sprite
            roomIcon.SetIcon(room.roomType);
            // Disable all room icons except for the starting room
            if (room.roomType != Room.RoomType.Start) { roomIcon.gameObject.SetActive(false); }
        }
        // Update the map
        UpdateMap(gridCentre, null);
    }

    public void UpdateMap(Vector2Int newRoomPos, Room prevRoom)
    {
        // Make the previous room visited
        if (prevRoom != null) { roomIcons[prevRoom.gridPos].isVisited = true; }

        // Update the new room's icon
        roomIcons[newRoomPos].UpdateIcon(true);
        // Createa list of the adjacent room icon positions
        List<Vector2Int> adjacentRoomIconPositions = new List<Vector2Int>();
        adjacentRoomIconPositions.Add(newRoomPos + Vector2Int.up);
        adjacentRoomIconPositions.Add(newRoomPos + Vector2Int.down);
        adjacentRoomIconPositions.Add(newRoomPos + Vector2Int.left);
        adjacentRoomIconPositions.Add(newRoomPos + Vector2Int.right);
        // Reveal each adjacent rooms' icon and update them appropriately
        foreach (Vector2Int pos in adjacentRoomIconPositions)
        {
            // Do nothing if there is no room icon at that position
            if (!roomIcons.ContainsKey(pos)) { continue; }
            // Enable the icon's game object if it's disabled
            if (!roomIcons[pos].gameObject.activeInHierarchy) { roomIcons[pos].gameObject.SetActive(true); }
            // Update the icon
            roomIcons[pos].UpdateIcon(false);
        }
        // Update the map area's position so the new room's icon is at the centre of the minimap
        if (prevRoom != null)
        {
            Vector2 newRoomVector = newRoomPos - prevRoom.gridPos;
            mapArea.anchoredPosition -= new Vector2(newRoomVector.x * iconSize, newRoomVector.y * iconSize);
        }
    }
}
