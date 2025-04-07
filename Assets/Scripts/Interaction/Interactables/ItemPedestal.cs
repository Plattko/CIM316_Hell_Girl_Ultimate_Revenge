using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ItemPedestal : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject itemDisplay;
    private Item item;
    public event Action onItemChosen;

    public void Initialise(Item _item)
    {
        item = _item;
        // Set the item display's sprite to the item's icon
        itemDisplay.GetComponent<SpriteRenderer>().sprite = item.icon;
    }

    public void Interact(Interactor interactor)
    {
        Debug.Log("Interacted with " + name + ".");
        if (item is WeaponDataSO)
        {
            // Get a reference to the interactor's weapon manager script
            WeaponManager interactorWeaponManager = interactor.GetComponentInChildren<WeaponManager>();
            // Update the interactor's current weapon to this weapon
            //interactorWeaponManager.SwapWeapon((WeaponSO)item);
            Debug.Log("Picked up " + item.name + ".");
        }
        else if (item is Spell)
        {
            // Get a reference to the interactor's spell manager script
            SpellManager interactorSpellManager = interactor.GetComponentInChildren<SpellManager>();
            // Update the interactor's current spell to this spell
            interactorSpellManager.SwapSpell((Spell)item);
            Debug.Log("Picked up " + item.name + ".");
        }
        // Signal that the item was chosen
        onItemChosen?.Invoke();
    }

    public void Clear()
    {
        // If the item display exists, destroy it
        if (itemDisplay != null) { Destroy(itemDisplay); }
        // Destroy this script so the pedestal can't be interacted with
        Destroy(this);
    }
}
