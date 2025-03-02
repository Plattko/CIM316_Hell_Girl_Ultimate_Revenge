using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomIcon : MonoBehaviour
{
    private Image image;

    [Header("Start Room")]
    [SerializeField] private Sprite startUnvisited;
    [SerializeField] private Sprite startVisited;
    [SerializeField] private Sprite startCurrent;
    [Header("Combat Room")]
    [SerializeField] private Sprite combatUnvisited;
    [SerializeField] private Sprite combatVisited;
    [SerializeField] private Sprite combatCurrent;
    [Header("Item Room")]
    [SerializeField] private Sprite itemUnvisited;
    [SerializeField] private Sprite itemVisited;
    [SerializeField] private Sprite itemCurrent;
    [Header("Miniboss Room")]
    [SerializeField] private Sprite minibossUnvisited;
    [SerializeField] private Sprite minibossVisited;
    [SerializeField] private Sprite minibossCurrent;
    [Header("Boss Room")]
    [SerializeField] private Sprite bossUnvisited;
    [SerializeField] private Sprite bossVisited;
    [SerializeField] private Sprite bossCurrent;
    [Header("Ascension Room")]
    [SerializeField] private Sprite ascensionUnvisited;
    [SerializeField] private Sprite ascensionVisited;
    [SerializeField] private Sprite ascensionCurrent;
    private Sprite[] roomIcons;

    [HideInInspector] public bool isVisited;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetIcon(Room.RoomType roomType)
    {
        switch (roomType)
        {
            case Room.RoomType.Start:
                // Set the room icons array to the icons for that room type
                roomIcons = new Sprite[] { startUnvisited, startVisited, startCurrent };
                break;

            case Room.RoomType.Combat:
                roomIcons = new Sprite[] { combatUnvisited, combatVisited, combatCurrent };
                break;

            case Room.RoomType.Item:
                roomIcons = new Sprite[] { itemUnvisited, itemVisited, itemCurrent };
                break;

            case Room.RoomType.Miniboss:
                roomIcons = new Sprite[] { minibossUnvisited, minibossVisited, minibossCurrent };
                break;

            case Room.RoomType.Boss:
                roomIcons = new Sprite[] { bossUnvisited, bossVisited, bossCurrent };
                break;

            case Room.RoomType.Ascension:
                roomIcons = new Sprite[] { ascensionUnvisited, ascensionVisited, ascensionCurrent };
                break;

            default:
                break;
        }
        // Set the room icon's sprite to the unvisited sprite
        image.sprite = roomIcons[0];
    }

    public void UpdateIcon(bool isCurrent)
    {
        // If it's the current room, set it to the current sprite for that room type
        if (isCurrent) { image.sprite = roomIcons[2]; }
        // If the room has been visited, set it to the visited sprite for that room type
        else if (isVisited) { image.sprite = roomIcons[1]; }
    }
}
