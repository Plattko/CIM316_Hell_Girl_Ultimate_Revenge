using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSpriteSelector : MonoBehaviour
{
    [SerializeField]
    private Sprite roomU, roomD, roomL, roomR,
        roomUD, roomLR, roomUL, roomUR, roomDL, roomDR,
        roomUDL, roomUDR, roomULR, roomDLR,
        roomUDLR;

    private int doorIndex = 0;
    private Room.RoomType roomType;

    [SerializeField] private Color startColour, combatColour, itemColour, ascensionColour;

    [SerializeField] private SpriteRenderer spriteRenderer;

    public void SetSprite(bool doorUp, bool doorDown, bool doorLeft, bool doorRight, Room.RoomType _roomType)
    {
        // Set the door index based on the doors the room has
        if (doorUp)
        {
            doorIndex += 1;
        }
        if (doorDown)
        {
            doorIndex += 2;
        }
        if (doorLeft)
        {
            doorIndex += 4;
        }
        if (doorRight)
        {
            doorIndex += 8;
        }
        // Set the room type
        roomType = _roomType;
        
        PickSprite();
        PickColour();
    }

    private void PickSprite()
    {
        // Up = 1, Down = 2, Left = 4, Right = 8

        if (doorIndex == 1) { spriteRenderer.sprite = roomU; }
        else if (doorIndex == 2) { spriteRenderer.sprite = roomD; }
        else if (doorIndex == 3) { spriteRenderer.sprite = roomUD; }
        else if (doorIndex == 4) { spriteRenderer.sprite = roomL; }
        else if (doorIndex == 5) { spriteRenderer.sprite = roomUL; }
        else if (doorIndex == 6) { spriteRenderer.sprite = roomDL; }
        else if (doorIndex == 7) { spriteRenderer.sprite = roomUDL; }
        else if (doorIndex == 8) { spriteRenderer.sprite = roomR; }
        else if (doorIndex == 9) { spriteRenderer.sprite = roomUR; }
        else if (doorIndex == 10) { spriteRenderer.sprite = roomDR; }
        else if (doorIndex == 11) { spriteRenderer.sprite = roomUDR; }
        else if (doorIndex == 12) { spriteRenderer.sprite = roomLR; }
        else if (doorIndex == 13) { spriteRenderer.sprite = roomULR; }
        else if (doorIndex == 14) { spriteRenderer.sprite = roomDLR; }
        else if (doorIndex == 15) { spriteRenderer.sprite = roomUDLR; }
    }

    private void PickColour()
    {
        switch (roomType)
        {
            case Room.RoomType.Start:
                spriteRenderer.color = startColour;
                break;

            case Room.RoomType.Combat:
                spriteRenderer.color = combatColour;
                break;

            case Room.RoomType.Item:
                spriteRenderer.color = itemColour;
                break;

            case Room.RoomType.Ascension:
                spriteRenderer.color = ascensionColour;
                break;

            default:
                break;
        }
    }
}
