using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemRoom : MonoBehaviour
{
    [SerializeField] private List<Item> weaponPool = new List<Item>();
    [SerializeField] private List<Item> spellPool = new List<Item>();
    [SerializeField] private ItemPedestal[] itemPedestals;

    public enum ItemType
    {
        Weapons,
        Spells,
    }
    public ItemType itemType { get; private set; } = ItemType.Weapons;

    private void OnEnable()
    {
        // Connect every item pedestals's onItemChosen event to the ItemChosen function
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            pedestal.onItemChosen += OnItemChosen;
        }
    }

    private void OnDisable()
    {
        // Disconnect every item pedestals's onItemChosen event from the ItemChosen function
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            pedestal.onItemChosen -= OnItemChosen;
        }
    }

    private void Start()
    {
        // Alternate between spell rooms and weapon rooms
        itemType = GameManager.Instance.DoSpellRoom ? ItemType.Spells : ItemType.Weapons;
        GameManager.Instance.ToggleItemRoom();
        // Choose between using the weapon pool or spell pool based on the room's item type
        List <Item> availableItems = new List<Item>();
        if (itemType == ItemType.Weapons) { availableItems = weaponPool; }
        else if (itemType == ItemType.Spells) { availableItems = spellPool; }
        // Give each pedestal an item from the list of available items without repetition
        foreach (ItemPedestal itemPedestal in itemPedestals)
        {
            // Roll a random item from the available items list
            int roll = Random.Range(0, availableItems.Count);
            // Initialise the pedestal with the rolled item
            itemPedestal.Initialise(availableItems[roll]);
            // Remove the item from the available items list
            availableItems.RemoveAt(roll);
        }
    }

    private void OnItemChosen()
    {
        // Destroy each pedestal's item display
        foreach (ItemPedestal pedestal in itemPedestals)
        {
            // Unsubscribe from the pedestal's item chosen event
            pedestal.onItemChosen -= OnItemChosen;
            // Clear the pedestal so the item is no longer displayed and it can't be interacted with
            pedestal.Clear();
        }
    }
}
